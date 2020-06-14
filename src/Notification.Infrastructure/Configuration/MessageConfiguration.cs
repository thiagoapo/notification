using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Notification.Domain.Entities;

namespace Notification.Infrastructure.Configuration
{
    public class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
           // builder.ToTable("message");
           // builder.HasKey(x => x.Id);

           // builder.Property(x => x.Id)
           //     .UseNpgsqlIdentityColumn<int>()
           //     .ValueGeneratedOnAdd()
           //      .ForNpgsqlUseSequenceHiLo("message_sequence")
           //      .HasColumnName("id");

           // builder.Property(x => x.Title)
           //      .IsRequired()
           //      .HasColumnType("varchar(100)")
           //      .HasMaxLength(100)
           //      .HasColumnName("title");

           //builder.Property(x => x.Content)
           //       .IsRequired()
           //       .HasColumnType("varchar(2000)")
           //       .HasMaxLength(2000)
           //       .HasColumnName("content");

           // builder.Property(x => x.SendError)
           //        .IsRequired()
           //        .HasColumnName("send_error");

           // builder.Property(x => x.ErrorMessage)
           //        .HasColumnType("varchar(1000)")
           //        .HasMaxLength(1000)
           //        .HasColumnName("error_message");

           // builder.Property(x => x.Active)
           //    .HasColumnName("active");
        }
    }
    
}
