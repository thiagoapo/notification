
using Notification.Domain.Entities;
using Notification.Dto.Model;

namespace Notification.Infrastructure.Mapper
{
    public class MapperRepository : AutoMapper.Profile
    {
        public MapperRepository()
        {
            CreateMap<MessageDto, Message>();
            CreateMap<Message, MessageDto>();
        }
    }
}
