using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Notification.Infrastructure.Context;
using System;

namespace Notification.Test
{
    public static class InMemoryContextFactory
    {
        public static NotificationContext Create()
        {
            var options = new DbContextOptionsBuilder<NotificationContext>()
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            return new NotificationContext(options);
        }
    }
}
