using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Plans
{
    public class PlanFeature
    {
        public int Id { get; set; }

        public string FeatureText { get; set; } = null!; // "3 months unlimited profiles"

        public int SubscriptionPlanId { get; set; }
        public SubscriptionPlan? SubscriptionPlan { get; set; }
    }
}
