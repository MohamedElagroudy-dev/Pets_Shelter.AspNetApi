using Core.Entities.Animal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Config
{
    public class DonationAnimalConfiguration : IEntityTypeConfiguration<DonationAnimal>
    {
        public void Configure(EntityTypeBuilder<DonationAnimal> builder)
        {
            builder.Property(d => d.GoalAmount)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            builder.Property(d => d.CollectedAmount)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            // Ignore computed properties
            builder.Ignore(d => d.RemainingAmount);
            builder.Ignore(d => d.ProgressPercentage);

            builder.Property(d => d.DonationStatus)
                   .IsRequired();

            builder.HasMany(d => d.Donations)
                   .WithOne(don => don.DonationAnimal)
                   .HasForeignKey(don => don.DonationAnimalId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
