using Core.Constants;

namespace Application.Common.Pagination
{
    public class AnimalParams : PaginationParams
    {
        public AnimalSort? Sort { get; set; } = AnimalSort.CreatedAtDesc;
        public int? PetTypeId { get; set; }
        public Core.Constants.Gender? Gender { get; set; }
        public double? AgeFromYears { get; set; }
        public double? AgeToYears { get; set; }
        public int? TotalCount { get; set; }
    }
}
