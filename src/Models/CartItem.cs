using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace basic_ecommerce_platform.Models
{
    [Index(nameof(CartId), nameof(ProductId), IsUnique = true)]
    public class CartItem
    {
        public int Id { get; set; }

        public int CartId { get; set; }
        public Cart Cart { get; set; } = null!;

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; } = 1;
    }
}
