using AutoMapper;
using Notification.Application.Service;
using Notification.Dto.Model;
using Notification.Infrastructure.Context;
using Notification.Infrastructure.Mapper;
using Notification.Infrastructure.Repository;
using Notification.Infrastructure.Repository.Interface;
using Xunit;

namespace Notification.Test
{
    public class MessageServiceTest
    {
        private readonly IMapper _mapper;
        private readonly NotificationContext _context;
        private readonly MessageService _service;
        private readonly IMessageRepository _repository;

        public MessageServiceTest()
        {
            var mapperConfig = new MapperConfiguration(config =>
            {
                config.AddProfile<MapperRepository>();
            });
            _mapper = mapperConfig.CreateMapper();
            _context = InMemoryContextFactory.Create();
            this._repository = new MessageRepository(_context, _mapper);
            this._service = new MessageService(_repository, _mapper);
                       
        }        

        [Fact]
        public void ShouldInsert()
        {
            MessageDto dto = new MessageDto();

            var result = _service.InsertAsync(dto).Result;

            Assert.NotNull(result);
        }
    }
}
