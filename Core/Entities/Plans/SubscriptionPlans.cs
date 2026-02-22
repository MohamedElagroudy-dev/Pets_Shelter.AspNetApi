using Core.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Plans
{
    public class SubscriptionPlan
    {
        public required int Id { get; set; }

        public required string Name { get; set; }          // Bronze Paws
        public decimal Price { get; set; }        // 300
        public int DurationInMonths { get; set; } // 3

        public string? Description { get; set; }  // optional

        public required PlanType PlanType { get; set; }    // Adoption / Foster / Sponsor

        public ICollection<PlanFeature>? Features { get; set; }
    }
}
