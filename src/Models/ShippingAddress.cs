using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace basic_ecommerce_platform.Models
{
    [ComplexType]
    public class ShippingAddress
    {
        [Required, MaxLength(255)]
        public string Line1 { get; init; } = string.Empty;

        public string? Line2 { get; init; }

        [Required, MaxLength(100)]
        public string City { get; init; } = string.Empty;

        [Required, MaxLength(100)]
        public string PostalCode { get; init; } = string.Empty;
    }
}
