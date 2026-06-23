using Core.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Pagination
{
    public class DonationAnimalParams
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Search { get; set; }
        public int? PetTypeId { get; set; }
        public Gender? Gender { get; set; }
        public double? AgeFromYears { get; set; }
        public double? AgeToYears { get; set; }
        public DonationStatus? Status { get; set; }
        public AnimalSort? Sort { get; set; }
    }
}
