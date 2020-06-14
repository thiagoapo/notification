using Notification.Dto.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Notification.Application.Service.Interface
{
    public interface IMessageService
    {
        Task<MessageDto> InsertAsync(MessageDto messageDto);
        Task<List<MessageDto>> GetReceivedMessages();
        Task<List<MessageDto>> GetSentMessages();
    }
}
