using Core.Constants;
using System.Text.Json.Serialization;

namespace Application.Common.Pagination
{
    public class AnimalApplicationParams : PaginationParams
    {
        public AdoptionApplicationSort Sort { get; set; } = AdoptionApplicationSort.SubmittedAtDesc;
        public ApplicationStatus? Status { get; set; }

        [JsonIgnore]
        public ApplicationType? ApplicationType { get; set; }
        public int? TotalCount { get; set; }
    }
}
