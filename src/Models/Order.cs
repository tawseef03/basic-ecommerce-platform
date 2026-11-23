using System.ComponentModel.DataAnnotations;

namespace basic_ecommerce_platform.Models
{
    public class Order
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public ShippingAddress Address { get; set; } = new();

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();

        public decimal Total { get; set; }
    }

    public enum OrderStatus
    {
        Pending,
        Paid,
        Cancelled
    }
}
