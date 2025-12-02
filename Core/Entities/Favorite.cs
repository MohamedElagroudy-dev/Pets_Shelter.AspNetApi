using Core.Entities.Product;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Favorite : BaseEntity
    {
        public required string AppUserId { get; set; }
        public int ProductId { get; set; }

        public DateTime DateAdded { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(AppUserId))]
        public AppUser AppUser { get; set; } = null!;

        [ForeignKey(nameof(ProductId))]
        public Core.Entities.Product.Product Product { get; set; } = null!;

        [SetsRequiredMembers]
        public Favorite(string appUserId, int productId)
        {
            AppUserId = appUserId;
            ProductId = productId;
        }

        public Favorite() { }
    }
}
