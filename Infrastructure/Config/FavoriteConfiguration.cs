using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Config
{
    public class FavoriteConfiguration : IEntityTypeConfiguration<Favorite>
    {
        public void Configure(EntityTypeBuilder<Favorite> builder)
        {
            builder.HasIndex(f => new { f.AppUserId, f.ProductId })
          .IsUnique();

            builder.HasOne(f => f.AppUser)
          .WithMany()
          .HasForeignKey(f => f.AppUserId);
        }
    }
}
