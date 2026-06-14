using Core.Entities.Chat;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Config
{
    public class ChatRoomConfiguration : IEntityTypeConfiguration<ChatRoom>
    {
        public void Configure(EntityTypeBuilder<ChatRoom> builder)
        {
            builder.HasKey(r => r.Id);

            // One customer -> one room
            builder.HasIndex(r => r.CustomerId)
                   .IsUnique();

            builder.HasOne(r => r.Customer)
                   .WithOne(u => u.ChatRoom)
                   .HasForeignKey<ChatRoom>(r => r.CustomerId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(r => r.Messages)
                   .WithOne(m => m.ChatRoom)
                   .HasForeignKey(m => m.ChatRoomId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
