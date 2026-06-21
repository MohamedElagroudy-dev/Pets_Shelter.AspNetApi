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
    public class DonationConfiguration : IEntityTypeConfiguration<Donation>
    {
        public void Configure(EntityTypeBuilder<Donation> builder)
        {
            builder.ToTable("Donations");

            builder.HasKey(d => d.Id);

            builder.Property(d => d.Amount)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            builder.Property(d => d.Message)
                   .HasMaxLength(500);

            builder.Property(d => d.DonatedAt)
                   .IsRequired();

            builder.HasOne(d => d.Donor)
                   .WithMany()
                   .HasForeignKey(d => d.DonorId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(d => d.DonationAnimalId);
            builder.HasIndex(d => d.DonorId);
        }
    }
}
