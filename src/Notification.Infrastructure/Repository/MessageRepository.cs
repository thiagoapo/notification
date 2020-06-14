using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Notification.Domain.Entities;
using Notification.Infrastructure.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Notification.Infrastructure.Repository
{
    public class MessageRepository : Repository<Message>, IMessageRepository
    {
        private readonly IMapper _mapper;

        public MessageRepository(Context.NotificationContext context, IMapper mapper) : base(context)
        {
            _mapper = mapper;
        }

        public async Task<List<Message>> GetMessagesByInstance(Guid instanceId)
        {
            return await _GetMessagesByInstance(instanceId);
        }

        public async Task<List<Message>> GetMessagesByNotInstance(Guid instanceId)
        {
            return await _GetMessagesByInstance(instanceId, notEqual: true);
        }

        private async Task<List<Message>> _GetMessagesByInstance(Guid instanceId, bool notEqual = false)
        {
            var query =   _context.Messages.Where(p => p.InstanceId == instanceId);

            if(notEqual)
                query = _context.Messages.Where(p => p.InstanceId != instanceId);

            return await query.ToListAsync();
        }
    }
}
