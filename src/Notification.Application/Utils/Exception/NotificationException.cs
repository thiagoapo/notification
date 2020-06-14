using System;
using System.Runtime.Serialization;

namespace Notification.Application.Utils.Exception
{
    [Serializable]
    public class NotificationException : System.Exception
    {
        public NotificationException()
        {
        }

        public NotificationException(string message) : base(message)
        {
        }

        public NotificationException(string message, System.Exception innerException) : base(message, innerException)
        {
        }

        protected NotificationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
