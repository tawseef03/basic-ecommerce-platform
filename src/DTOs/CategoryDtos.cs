using System.ComponentModel.DataAnnotations;

namespace basic_ecommerce_platform.DTOs
{
    public record CategoryRequestDto
    (
        [Required, MaxLength(100)] string Name
    );

    public record CategoryResponseDto
    (
        int Id,
        string Name
    );
}
