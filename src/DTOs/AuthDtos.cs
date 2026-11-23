using System.ComponentModel.DataAnnotations;

namespace basic_ecommerce_platform.DTOs
{
    public record RegisterDto(
        [property: Required, MaxLength(100)] string FullName,
        [property: Required, EmailAddress, MaxLength(255)] string Email,
        [property: Required, MinLength(8)] string Password
        );

    public record LoginDto(
        [property: Required, EmailAddress] string Email,
        [property: Required] string Password
        );

    public record AuthResponseDto(
        string Token,
        DateTime ExpiresAt,
        string Email,
        IList<string> Roles
        );
}
