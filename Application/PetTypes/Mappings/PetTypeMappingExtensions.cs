using Application.PetTypes.DTOs;
using Core.Entities.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.PetTypes.Mappings
{
    public static class PetTypeMappingExtensions
    {
        public static PetTypeDTO ToDto(this PetType petType)
        {
            return new PetTypeDTO
            {
                Id = petType.Id,
                Name = petType.Name,
                Description = petType.Description
            };
        }
     
        public static PetType ToEntity(this AddPetTypeDTO dto)
        {
            return new PetType
            {
                Name = dto.Name,
                Description = dto.Description
            };
        }
        public static void UpdateEntity(this PetType petType, UpdatePetTypeDTO dto)
        {
            petType.Name = dto.Name;
            petType.Description = dto.Description;
        }
    }
}
