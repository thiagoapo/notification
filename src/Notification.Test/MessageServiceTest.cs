using AutoMapper;
using Moq;
using Notification.Application.Service;
using Notification.Application.Service.Interface;
using Notification.Dto.Model;
using Notification.Infrastructure.Context;
using Notification.Infrastructure.Mapper;
using Notification.Infrastructure.Repository;
using Notification.Infrastructure.Repository.Interface;
using System;
using System.Collections.Generic;
using Xunit;

namespace Notification.Test
{
    public class MessageServiceTest
    {
        private readonly IMapper _mapper;
        private readonly NotificationContext _context;
        private readonly MessageService _service;
        private readonly IMessageRepository _repository;

        private readonly MessageService _moqService;
        private readonly Mock<IMessageRepository> _moqRepository;

        public MessageServiceTest()
        {
            var mapperConfig = new MapperConfiguration(config =>
            {
                config.AddProfile<MapperRepository>();
            });
            _mapper = mapperConfig.CreateMapper();
            _context = InMemoryContextFactory.Create();
            this._moqRepository = new Mock<IMessageRepository>();
            this._moqService = new MessageService(_moqRepository.Object, _mapper);

            this._repository = new MessageRepository(_context, _mapper);
            this._service = new MessageService(_repository, _mapper);
        }

        public void Setup()
        {
            List<Domain.Entities.Message> myList = new List<Domain.Entities.Message>() {
                 new Domain.Entities.Message(){
                     InstanceId = Me.GetInstance().Id,
                     RequestId = Guid.NewGuid(),
                     Title = "Hello World! (Test)",
                     Content = "Hello World! (Test)"
                 },
                 new Domain.Entities.Message(){
                     InstanceId = Me.GetInstance().Id,
                     RequestId = Guid.NewGuid(),
                     Title = "Hello World! (Test 2)",
                     Content = "Hello World! (Test 2)"
                 }
            };

            List<Domain.Entities.Message> otherList = new List<Domain.Entities.Message>() {
                new Domain.Entities.Message(){
                     InstanceId = Guid.NewGuid(),
                     RequestId = Guid.NewGuid(),
                     Title = "Hello World! (Test Other)",
                     Content = "Hello World! (Test Other)"
                 }
            };

            _moqRepository.Setup(p => p.GetMessagesByInstance(It.IsIn<Guid>(Me.GetInstance().Id))).ReturnsAsync(() => myList);
            _moqRepository.Setup(p => p.GetMessagesByInstance(It.IsNotIn<Guid>(Me.GetInstance().Id))).ReturnsAsync(() => new List<Domain.Entities.Message>());
            _moqRepository.Setup(p => p.GetMessagesByNotInstance(It.IsIn<Guid>(Me.GetInstance().Id))).ReturnsAsync(() => otherList);
            _moqRepository.Setup(p => p.GetMessagesByNotInstance(It.IsNotIn<Guid>(Me.GetInstance().Id))).ReturnsAsync(() => new List<Domain.Entities.Message>());
        }

        [Fact]
        public void ShouldInsert()
        {
            MessageDto dto = new MessageDto();

            dto.InstanceId = Me.GetInstance().Id;
            dto.RequestId = Guid.NewGuid();
            dto.Title = "Hello World! (Test)";
            dto.Content = "Hello World! (Test)";

            var result = _service.InsertAsync(dto).Result;

            Assert.NotNull(result);
        }

        [Fact]
        public void GetReceivedMessages()
        {
            Setup();

            var result = _moqService.GetReceivedMessages().Result;

            Assert.NotNull(result);
            Assert.Equal(1, result.Count);
        }

        [Fact]
        public void GetSentMessages()
        {
            Setup();

            var result = _moqService.GetSentMessages().Result;

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }
    }
}
