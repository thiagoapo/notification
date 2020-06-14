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
using Hangfire;
using Notification.Infrastructure.MQSettings;

namespace Notification.Application.Service
{
    public class ConsumeSendMessageMQService : BackgroundService
    {
        private readonly ILogger _logger;
        private IConnection _connection;
        private IModel _model;
        private readonly IConfiguration _configuration;
        private readonly ConnectionFactory _factory;
        private readonly RabbitMQSettings _rabbitMQSettings;

        public const int SecondsToSend = 5000; //5 seconds 

        public ConsumeSendMessageMQService(IServiceProvider services, ILoggerFactory loggerFactory, IConfiguration configuration, RabbitMQSettings rabbitMQSettings)
        {
            Services = services;
            _configuration = configuration;
            this._logger = loggerFactory.CreateLogger<ConsumeSendMessageMQService>();

            _rabbitMQSettings = rabbitMQSettings;

            _factory = new ConnectionFactory
            {
                HostName = _rabbitMQSettings.HostName,
                Port = int.Parse(_rabbitMQSettings.Port),
                UserName = _rabbitMQSettings.UserName,
                Password = _rabbitMQSettings.Password
            };

            _factory.AutomaticRecoveryEnabled = true;
        }

        public IServiceProvider Services { get; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await DoWork(stoppingToken);
        }

        private async Task DoWork(CancellationToken stoppingToken)
        {
            int messageCount = 0;

            await Task.Run(() =>
            {
                while (true)
                {
                    messageCount++;
                    SendMessageDoWork(stoppingToken, messageCount).Wait();
                    Thread.Sleep(SecondsToSend);
                }
            });
        }

        private async Task SendMessageDoWork(CancellationToken stoppingToken, int messageCount)
        {
            using (var scope = Services.CreateScope())
            {
                var scopedProcessingService = scope.ServiceProvider.GetRequiredService<IMessageService>();
                if (!stoppingToken.IsCancellationRequested)
                {
                    MessageDto message = new MessageDto()
                    {
                        Title = $"Hello World! ({messageCount})",
                        Content = $"Hello World! ({messageCount})"
                    };

                    SendMessageNotification(message);
                }
            }
        }

        private void SendMessageNotification(MessageDto message)
        {
            var queueName = _configuration["RabbitQueueName:Notification"];
            var routingKey = _configuration["RabbitQueueName:RoutingKeyNotification"];
            while (!Send(queueName, routingKey, message)) ;
        }

        public bool Send(string queueName, string routingKey, MessageDto message)
        {
            try
            {
                lock (_factory)
                {
                    using (_connection = _factory.CreateConnection())
                    {
                        using (_model = _connection.CreateModel())
                        {
                            _model.ExchangeDeclare(_rabbitMQSettings.ExchangeName, _rabbitMQSettings.Type, durable: true);

                            _model.QueueDeclare(queueName, true, false, false, null);
                            _model.QueueBind(queueName, _rabbitMQSettings.ExchangeName, routingKey);

                            message.InstanceId = Me.GetInstance().Id;
                            message.RequestId = Guid.NewGuid();
                            message.CreateAt = DomainUtils.GetLocalDate();

                            byte[] objsend = ObjectSerialize.Serialize(message);

                            _model.BasicPublish(_rabbitMQSettings.ExchangeName, routingKey, null, objsend);
                        }
                    }
                }

                using (var scope = Services.CreateScope())
                {
                    var scopedProcessingService = scope.ServiceProvider.GetRequiredService<IMessageService>();
                    scopedProcessingService.InsertAsync(message).Wait();
                }

                return true;
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine(ex.ToString());
                _model.Dispose();
                _connection.Dispose();
                return false;
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }
    }
}
