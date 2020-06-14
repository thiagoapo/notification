using System;

namespace Notification.Dto.Model
{
    public class BaseDto
    {
        public int Id { get; set; }

        public DateTime? CreateAt { get; set; }

        public bool? Active { get; set; }
    }
}
