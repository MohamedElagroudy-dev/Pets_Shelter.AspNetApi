using Core.Constants;
using Core.Entities;
using Core.Entities.Product;
using Ecom.Core.Entities.Product;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities.Animal
{
    public abstract class BaseAnimal : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public double AgeYears { get; set; }
        public AnimalSize Size { get; set; }
        public double WeightKg { get; set; }
        public Gender Gender { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<AnimalPhoto> Photos { get; set; } = new List<AnimalPhoto>();
        public int PetTypeId { get; set; }
        [ForeignKey(nameof(PetTypeId))]
        public virtual PetType PetType { get; set; } = null!;
        public AnimalTemperament Temperament { get; set; } = null!;
    }
}
