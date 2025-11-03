using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Product
{
    public class PetType : BaseEntity
    {
        public required string Name { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
