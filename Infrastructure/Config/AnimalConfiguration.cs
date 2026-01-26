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
    public class AnimalConfiguration : IEntityTypeConfiguration<Animal>
    {
        public void Configure(EntityTypeBuilder<Animal> builder)
        {
            // Table name (optional but clean)
            builder.ToTable("Animals");

            // Primary Key
            builder.HasKey(a => a.Id);

            // Properties
            builder.Property(a => a.Name)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(a => a.Description)
                   .IsRequired();

            builder.Property(a => a.AgeYears)
                   .IsRequired();

            builder.Property(a => a.WeightKg)
                   .IsRequired();

            builder.Property(a => a.Size)
                   .IsRequired();

            builder.Property(a => a.Gender)
                   .IsRequired();

            builder.Property(a => a.CreatedAt)
                   .IsRequired();

            // Ignore computed property
            builder.Ignore(a => a.IsAdopted);

            // Relationships

            // Animal → PetType (Many-to-One)
            builder.HasOne(a => a.PetType)
                   .WithMany()
                   .HasForeignKey(a => a.PetTypeId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Animal → Adopter (User) (Many-to-One, nullable)
            builder.HasOne(a => a.Adopter)
                   .WithMany()
                   .HasForeignKey(a => a.AdopterId)
                   .OnDelete(DeleteBehavior.SetNull);

            // Animal → Photos (One-to-Many)
            builder.HasMany(a => a.Photos)
                   .WithOne(p => p.Animal)
                   .HasForeignKey(p => p.AnimalId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Owned Entity: AnimalTemperament
            builder.OwnsOne(a => a.Temperament, t =>
            {
                t.Property(x => x.AnimalsFriendlyLevel)
                 .IsRequired();

                t.Property(x => x.ChildrenFriendlyLevel)
                 .IsRequired();

                t.Property(x => x.HouseTrainedLevel)
                 .IsRequired();
            });
        }
    }
}
