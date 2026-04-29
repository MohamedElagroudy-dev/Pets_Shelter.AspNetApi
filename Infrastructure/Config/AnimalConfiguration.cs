using Core.Entities.Animal;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Config
{
    public class AnimalConfiguration : IEntityTypeConfiguration<BaseAnimal>
    {
        public void Configure(EntityTypeBuilder<BaseAnimal> builder)
        {
            // Table name
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

            // Discriminator for TPH
            builder.HasDiscriminator<string>("AnimalRole")
                .HasValue<AdoptionAnimal>("Adoption")
                .HasValue<FosterAnimal>("Foster");

            // Shared relationships
            builder.HasOne(a => a.PetType)
                .WithMany()
                .HasForeignKey(a => a.PetTypeId)
                .OnDelete(DeleteBehavior.Restrict);

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

    public class AdoptionAnimalConfiguration : IEntityTypeConfiguration<AdoptionAnimal>
    {
        public void Configure(EntityTypeBuilder<AdoptionAnimal> builder)
        {
            // Ignore computed property
            builder.Ignore(a => a.IsAdopted);

            builder.HasOne(a => a.Adopter)
                .WithMany()
                .HasForeignKey(a => a.AdopterId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }

    public class FosterAnimalConfiguration : IEntityTypeConfiguration<FosterAnimal>
    {
        public void Configure(EntityTypeBuilder<FosterAnimal> builder)
        {
            // Ignore computed property
            builder.Ignore(a => a.IsFostered);

            // Foster specific configuration can be added here if needed
        }
    }
}
