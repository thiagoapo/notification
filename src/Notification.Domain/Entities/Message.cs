
using System;

namespace Notification.Domain.Entities
{
    public class Message : BaseEntity
    {
        public Guid RequestId { get; set; }
        public Guid InstanceId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public bool SendError { get; set; }
        public string ErrorMessage { get; set; }
    }
}
