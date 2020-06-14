
using System;

namespace Notification.Dto.Model
{
    public class MessageDto: BaseDto
    {
        public Guid RequestId { get; set; }
        public Guid InstanceId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public bool SendError { get; set; }
        public string ErrorMessage { get; set; }
    }
}
