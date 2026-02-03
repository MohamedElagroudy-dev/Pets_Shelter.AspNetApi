using Core.Constants;

namespace Application.Common.Pagination
{
    public class AdoptionApplicationParams : PaginationParams
    {
        public AdoptionApplicationSort Sort { get; set; } = AdoptionApplicationSort.SubmittedAtDesc;
        public ApplicationStatus? Status { get; set; }
        public int? TotalCount { get; set; }
    }
}
