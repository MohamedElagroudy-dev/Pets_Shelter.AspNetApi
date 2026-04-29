using Core.Entities;
using Core.Entities.Product;
using Ecom.Core.Entities.Product;
using System;
using System.Collections.Generic;

namespace Core.Entities.Animal
{
    public class AdoptionAnimal : BaseAnimal
    {
        public bool IsAdopted => AdopterId != null;
        public string? AdopterId { get; set; }
        public AppUser? Adopter { get; set; }
    }
}
