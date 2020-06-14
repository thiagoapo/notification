using System;

namespace Notification.Application.Service
{
    /// <summary>
    /// MS
    /// </summary>
    public class Me
    {
        /// <summary>
        /// Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public Me()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Instance MS
        /// </summary>
        private static Me Instance { get; set; }

        public static Me GetInstance()
        {
            if (Instance == null)
                Instance = new Me();
            return Instance;
        }
    }
}
