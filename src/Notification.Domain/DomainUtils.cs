using System;
using System.Collections.Generic;
using System.Text;

namespace Notification.Domain
{
    public static class DomainUtils
    {
        public static DateTime GetLocalDate()
        {
            try
            {
                DateTime timeUtc = DateTime.UtcNow;
                var brasilia = TimeZoneInfo.FindSystemTimeZoneById("Brazil/East");
                return TimeZoneInfo.ConvertTimeFromUtc(timeUtc, brasilia);

            }
            catch (System.Exception)
            {

                return DateTime.Now;
            }


        }
    }
}
