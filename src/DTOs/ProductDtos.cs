using System.ComponentModel.DataAnnotations;

namespace basic_ecommerce_platform.DTOs
{
    public record ProductQueryDto(
        int? CategoryId = null,
        decimal? MinPrice = null,
        decimal? MaxPrice = null,
        string? Search = null,
        int Page = 1,
        int PageSize = 20
    );
    public record ProductRequestDto
    (
        [Required, MaxLength(100)] string Name,
        [Required, MaxLength(1023)] string Description,
        [Range(0.01, 99999999.99)] decimal Price,
        [Range(0, int.MaxValue)] int StockQuantity,
        [Range(1, int.MaxValue)] int CategoryId
    );
    public record ProductResponseDto
    (
        int Id,
        string Name,
        string Description,
        decimal Price,
        int StockQuantity,
        int CategoryId,
        string CategoryName,
        bool IsActive
    );
}
