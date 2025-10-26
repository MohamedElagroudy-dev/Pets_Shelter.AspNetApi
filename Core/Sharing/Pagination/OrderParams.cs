using Core.Entities.OrderAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Sharing.Pagination
{
    namespace Core.Sharing
    {
        public class OrderParams : PaginationParams
        {
            public string? BuyerEmail { get; set; }
            public OrderStatus? Status { get; set; }
            public OrderSort Sort { get; set; } = OrderSort.DateDesc;
        }
    }
}
