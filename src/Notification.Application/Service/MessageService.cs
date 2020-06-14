using System.Threading.Tasks;
using AutoMapper;
using Notification.Domain.Entities;
using Notification.Application.Service.Interface;
using Notification.Infrastructure.Repository.Interface;
using System.Collections.Generic;
using Notification.Dto.Model;
using System;

namespace Notification.Application.Service
{
    public class MessageService : IMessageService
    {

        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;

        public MessageService(IMessageRepository messageRepository, IMapper mapper)
        {
            _mapper = mapper;
            _messageRepository = messageRepository;
        }
        public async Task<List<MessageDto>> GetReceivedMessages()
        {
            var result = await _messageRepository.GetMessagesByNotInstance(Me.GetInstance().Id);
            return _mapper.Map<List<Message>, List<MessageDto>>(result);
        }

        public async Task<List<MessageDto>> GetSentMessages()
        {
            var result = await _messageRepository.GetMessagesByInstance(Me.GetInstance().Id);
            return _mapper.Map<List<Message>, List<MessageDto>>(result);
        }

        public async Task<MessageDto> InsertAsync(MessageDto messageDto)
        {
            var message = _mapper.Map<MessageDto, Message>(messageDto);
            var result = await _messageRepository.InsertAsync(message);
            return _mapper.Map<Message, MessageDto>(result);
        }
    }
}
