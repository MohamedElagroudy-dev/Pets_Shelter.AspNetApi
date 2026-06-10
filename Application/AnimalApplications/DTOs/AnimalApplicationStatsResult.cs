using Application.Common;
using Ecom.Application.AnimalApplications.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.AnimalApplications.DTOs
{
    public class AnimalApplicationStatsResult
    {
        public PagedResult<AnimalApplicationDto> PagedResult { get; set; } = null!;
        public int ApprovedRequestsCount { get; set; }
        public int RejectedRequestsCount { get; set; }
        public int PendingRequestsCount { get; set; }
        public double SuccessRate { get; set; }
    }
}
