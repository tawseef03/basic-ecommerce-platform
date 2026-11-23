using System.ComponentModel.DataAnnotations;

namespace basic_ecommerce_platform.DTOs
{
    public record AddressRequestDto
    (
        [property: Required, MaxLength(255)] string Line1,
        [property: MaxLength(255)] string? Line2,
        [property: Required, MaxLength(100)] string City,
        [property: Required, MaxLength(20)] string PostalCode
    );
    public record AddressResponseDto
    (
        int Id,
        string Line1,
        string? Line2,
        string City,
        string PostalCode
    );
}
