using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Animal
{
    public class AnimalPhoto : BaseEntity
    {
        public int AnimalId { get; set; }
        public BaseAnimal Animal { get; set; } = null!;

        public string ImageUrl { get; set; } = null!;
    }
}
