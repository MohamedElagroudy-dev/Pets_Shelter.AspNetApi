using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.PetTypes.DTOs
{
    public record UpdatePetTypeDTO : AddPetTypeDTO
    {
        public int Id { get; set; }
    }
}
