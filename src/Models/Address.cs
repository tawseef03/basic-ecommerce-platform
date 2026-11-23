using System.ComponentModel.DataAnnotations;

namespace basic_ecommerce_platform.Models
{

    // This DB saves multiples addresses for all users. The user can choose
    // their delivery address on checkout.
    public class Address
    {
        public int Id { get; set; }

        [Required, MaxLength(255)]
        public string Line1 { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? Line2 { get; set; }

        [Required, MaxLength(100)]
        public string City { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        public string PostalCode { get; set; } = string.Empty;

        public int UserId { get; set; }
        public User User { get; set; } = null!;
    }
}
