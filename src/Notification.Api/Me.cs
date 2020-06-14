using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Notification.Api
{
    /// <summary>
    /// MS
    /// </summary>
    public class Me
    {
        private Guid _Id { get; set; }
        /// <summary>
        /// Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public Me()
        {
            _Id = Guid.NewGuid();
        }

        /// <summary>
        /// Instance MS
        /// </summary>
        public static Me Instance { get; set; }
    }
}
