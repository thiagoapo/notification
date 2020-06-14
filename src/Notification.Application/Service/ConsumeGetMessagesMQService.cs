using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Notification.Infrastructure.Mapper;
using Notification.Application.Utils;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using FirebaseAdmin.Messaging;
using Notification.Application.Service.Interface;
using System.Linq;
using Notification.Domain;
using Notification.Dto.Model;
using Notification.Infrastructure.MQSettings;

namespace Notification.Application.Service
{
    public class ConsumeGetMessagesMQService : BackgroundService
    {
        private readonly ILogger _logger;
        private IConnection _connection;
        private IModel _channel;
        private readonly IConfiguration _configuration;

        private string _queueName = string.Empty;

        private string _exchangeName;
        private string _routingKey;

        private ConnectionFactory _factory;
        private readonly RabbitMQSettings _rabbitMQSettings;

        public ConsumeGetMessagesMQService(IServiceProvider services, ILoggerFactory loggerFactory, IConfiguration configuration, RabbitMQSettings rabbitMQSettings)
        {
            Services = services;
            _configuration = configuration;
            this._logger = loggerFactory.CreateLogger<ConsumeGetMessagesMQService>();


            _rabbitMQSettings = rabbitMQSettings;
        }

        public IServiceProvider Services { get; }

        private void InitRabbitMQ()
        {
            _factory = new ConnectionFactory
            {
                HostName = _rabbitMQSettings.HostName,
                Port = int.Parse(_rabbitMQSettings.Port),
                UserName = _rabbitMQSettings.UserName,
                Password = _rabbitMQSettings.Password
            };

            //_factory.AutomaticRecoveryEnabled = true;

            _connection = _factory.CreateConnection();

            _channel = _connection.CreateModel();

            _exchangeName = _configuration["RabbitMQSettings:ExchangeName"];

            _queueName = _configuration["RabbitQueueName:Notification"];
            _routingKey = _configuration["RabbitQueueName:RoutingKeyNotification"];

            _channel.ExchangeDeclare(_exchangeName, ExchangeType.Topic, durable: true);
            _channel.QueueDeclare(_queueName, true, false, false, null);
            _channel.QueueBind(_queueName, _exchangeName, _queueName, null);
            _channel.BasicQos(0, 1, false);

            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            if (_connection == null)
            {
                InitRabbitMQ();
            }

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ch, ea) =>
            {
                ReceivedMessage(ea, stoppingToken);
            };

            consumer.Shutdown += OnConsumerShutdown;
            consumer.Registered += OnConsumerRegistered;
            consumer.Unregistered += OnConsumerUnregistered;
            consumer.ConsumerCancelled += OnConsumerConsumerCancelled;

            _channel.BasicConsume(_queueName, true, consumer);
            return Task.CompletedTask;
        }

        private void ReceivedMessage(BasicDeliverEventArgs ea, CancellationToken stoppingToken)
        {
            Guid? receivedInstance = null;

            try
            {
                using (var scope = Services.CreateScope())
                {
                    var scopedProcessingService = scope.ServiceProvider.GetRequiredService<IMessageService>();

                    var content = Encoding.UTF8.GetString(ea.Body);

                    var obj = ObjectSerialize.TryParseJson(content, out MessageDto result);

                    if (obj)
                    {
                        receivedInstance = result.InstanceId;

                        if (result.InstanceId != Me.GetInstance().Id)
                        {
                            _logger.LogInformation("processing consumer");
                            scopedProcessingService.InsertAsync(result);
                            _logger.LogInformation($"consumer received {content}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _channel.BasicPublish(_exchangeName, _routingKey, null, ea.Body);
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                if (receivedInstance.HasValue && receivedInstance.Value != Me.GetInstance().Id)
                {
                   _channel.BasicAck(ea.DeliveryTag, false);
                }
            }
        }

        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            _logger.LogInformation($"connection shut down {e.ReplyText}");

        }

        private void OnConsumerConsumerCancelled(object sender, ConsumerEventArgs e)
        {
            _logger.LogInformation($"consumer cancelled {e.ConsumerTag}");
        }

        private void OnConsumerUnregistered(object sender, ConsumerEventArgs e)
        {
            _logger.LogInformation($"consumer unregistered {e.ConsumerTag}");
        }

        private void OnConsumerRegistered(object sender, ConsumerEventArgs e)
        {
            _logger.LogInformation($"consumer registered {e.ConsumerTag}");
        }

        private void OnConsumerShutdown(object sender, ShutdownEventArgs e)
        {
            _logger.LogInformation($"consumer shutdown {e.ReplyText}");
        }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }
    }
}
