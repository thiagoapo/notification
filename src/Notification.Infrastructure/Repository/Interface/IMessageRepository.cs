using Notification.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Notification.Infrastructure.Repository.Interface
{
    public interface IMessageRepository : IRepository<Message>
    {
        Task<List<Message>> GetMessagesByInstance(Guid instanceId);
        Task<List<Message>> GetMessagesByNotInstance(Guid instanceId);
    }
}
