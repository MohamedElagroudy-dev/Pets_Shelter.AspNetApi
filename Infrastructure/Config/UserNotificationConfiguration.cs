using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Config
{
    public class UserNotificationConfiguration : IEntityTypeConfiguration<UserNotification>
    {
        public void Configure(EntityTypeBuilder<UserNotification> builder)
        {
            builder.HasKey(n => n.Id);

            builder.Property(n => n.Type)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(n => n.Message)
                   .HasMaxLength(1000)
                   .IsRequired();

            builder.Property(n => n.Title)
                   .HasMaxLength(200);

            builder.Property(n => n.DataJson)
                   .HasColumnType("nvarchar(max)");

            // Fast lookup: all undelivered notifications for a user
            builder.HasIndex(n => new { n.UserId, n.IsDelivered });

            builder.HasOne(n => n.User)
                   .WithMany()
                   .HasForeignKey(n => n.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}