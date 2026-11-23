using System.ComponentModel.DataAnnotations;

namespace basic_ecommerce_platform.DTOs
{
    public record AddCartItemDto(
        [property: Range(1, int.MaxValue)] int ProductId,
        [property: Range(1, 100)] int Quantity
        );
    public record UpdateCartItemDto(
        [property: Range(1, 100)] int Quantity
        );
    public record CartItemResponseDto(
        int ProductId,
        string ProductName,
        decimal UnitPrice,
        int Quantity,
        decimal LineTotal
        );
    public record CartResponseDto(
        IEnumerable<CartItemResponseDto> Items,
        decimal Total
        );
}
