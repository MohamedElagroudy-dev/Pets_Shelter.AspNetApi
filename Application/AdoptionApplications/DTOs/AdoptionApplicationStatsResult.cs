using Application.Common;
using Ecom.Application.AdoptionApplications.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.AdoptionApplications.DTOs
{
    public class AdoptionApplicationStatsResult
    {
        public PagedResult<AdoptionApplicationDto> PagedResult { get; set; } = null!;
        public int ApprovedRequestsCount { get; set; }
        public int RejectedRequestsCount { get; set; }
        public int PendingRequestsCount { get; set; }
        public double SuccessRate { get; set; }
    }
}
