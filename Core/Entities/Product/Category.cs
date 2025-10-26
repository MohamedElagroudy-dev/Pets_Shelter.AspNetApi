using Core.Entities;

namespace Ecom.Core.Entities.Product
{
    public class Category : BaseEntity
    {
        public required string Name { get; set; }
        public  string Description { get; set; } = string.Empty;

        // Initialize collection to avoid null warnings
        //public virtual ICollection<Product> Products { get; set; } = new HashSet<Product>();
    }
}
