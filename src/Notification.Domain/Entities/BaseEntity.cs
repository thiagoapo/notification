using System;
using System.ComponentModel.DataAnnotations;

namespace Notification.Domain.Entities
{
    public class BaseEntity
    {
        [Key]
        public int Id { get; set; }

        private DateTime? _createAt;

        public DateTime? CreateAt
        {
            get { return _createAt; }
            set { _createAt = (value == null ? DomainUtils.GetLocalDate() : value); }
        }

        public bool? Active { get; set; }
    }
}
