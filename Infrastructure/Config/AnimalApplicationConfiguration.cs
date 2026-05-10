using Core.Entities.AdoptionApp;
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
    public class AnimalApplicationConfiguration : IEntityTypeConfiguration<AdoptionApplication>
    {
        public void Configure(EntityTypeBuilder<AdoptionApplication> builder)
        {
            builder.ToTable("AnimalApplications");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.ApplicationType)
                   .IsRequired();

            builder.HasOne<BaseAnimal>(a => a.Animal)
                   .WithMany()
                   .HasForeignKey(a => a.AnimalId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.Applicant)
                   .WithMany()
                   .HasForeignKey(a => a.ApplicantId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
