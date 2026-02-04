using Core.Entities.AdoptionApp;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Config
{
    public class AdoptionApplicationConfiguration
        : IEntityTypeConfiguration<AdoptionApplication>
    {
        public void Configure(EntityTypeBuilder<AdoptionApplication> builder)
        {
            // Table
            builder.ToTable("AdoptionApplications");

            // Primary Key
            builder.HasKey(x => x.Id);

            // Relationships
            builder.HasOne(x => x.Animal)
                   .WithMany()
                   .HasForeignKey(x => x.AnimalId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Applicant)
                   .WithMany()
                   .HasForeignKey(x => x.ApplicantId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Status
            builder.Property(x => x.Status)
                   .IsRequired();

            builder.Property(x => x.SubmittedAt)
                   .IsRequired();

            // -----------------------------
            // Owned Types
            // -----------------------------

            builder.OwnsOne(x => x.ApplicantInfo, ai =>
            {
                ai.Property(p => p.FirstName).IsRequired().HasMaxLength(100);
                ai.Property(p => p.LastName).IsRequired().HasMaxLength(100);
                ai.Property(p => p.PhoneNumber).IsRequired().HasMaxLength(30);
                ai.Property(p => p.Email).IsRequired().HasMaxLength(150);
            });

            builder.OwnsOne(x => x.AddressInfo, ad =>
            {
                ad.Property(p => p.Country).IsRequired().HasMaxLength(100);
                ad.Property(p => p.City).IsRequired().HasMaxLength(100);
                ad.Property(p => p.ZipCode).IsRequired().HasMaxLength(20);
                ad.Property(p => p.Address).IsRequired().HasMaxLength(300);
            });

            builder.OwnsOne(x => x.HouseholdInfo, hh =>
            {
                hh.Property(p => p.Details)
                  .IsRequired()
                  .HasMaxLength(5000);
            });

            builder.OwnsOne(x => x.PetCareInfo, pc =>
            {
                pc.Property(p => p.ResponsiblePerson).IsRequired().HasMaxLength(2000);
                pc.Property(p => p.AdoptionReason).IsRequired().HasMaxLength(10000);
                pc.Property(p => p.AloneTimeDetails).IsRequired().HasMaxLength(5000);
                pc.Property(p => p.LivingEnvironment).IsRequired().HasMaxLength(3000);
            });

            builder.OwnsOne(x => x.Preferences, pr =>
            {
                pr.Property(p => p.Dog);
                pr.Property(p => p.Cat);
                pr.Property(p => p.Bird);
                pr.Property(p => p.Lizard);
                pr.Property(p => p.Rabbit);
                pr.Property(p => p.Other);

                pr.Property(p => p.HouseTrained);
                pr.Property(p => p.Declawed);
                pr.Property(p => p.Young);
                pr.Property(p => p.MultiplePets);
                pr.Property(p => p.SpecialConsiderations);
            });

            builder.OwnsOne(x => x.Agreement, ag =>
            {
                ag.Property(p => p.Accepted)
                  .IsRequired();
            });
        }
    }
}
