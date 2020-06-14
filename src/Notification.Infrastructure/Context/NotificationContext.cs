
using Microsoft.EntityFrameworkCore;
using Notification.Domain.Entities;
using Notification.Infrastructure.Configuration;

namespace Notification.Infrastructure.Context
{
    public class NotificationContext : DbContext
    {

        public DbSet<Message> Messages { get; set; }

        public NotificationContext(DbContextOptions<NotificationContext> options) : base(options) {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //modelBuilder.HasSequence("message_sequence", x => x.StartsAt(1));

            modelBuilder.ApplyConfiguration(new MessageConfiguration());
        }
    }
}
