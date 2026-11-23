using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace basic_ecommerce_platform.Models
{
    public class User : IdentityUser<int>
    {
        [Required, MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public Cart? Cart { get; set; }
        public ICollection<Address> Addresses { get; set; } = new List<Address>();
    }
}
