using Core.Entities;

namespace Ecom.Core.Entities.Product
{
    public class Photo : BaseEntity
    {
        public required string ImageName { get; set; }

        public int ProductId { get; set; }

        
    }
}
