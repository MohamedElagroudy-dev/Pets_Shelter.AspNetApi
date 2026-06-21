using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.donationAnimal.DTOs
{
    public record PhotoDTO
    {
        public int Id { get; init; }
        public int AnimalId { get; init; }
        public string ImageUrl { get; init; } = string.Empty;
    }
}
