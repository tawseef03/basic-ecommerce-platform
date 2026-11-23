using basic_ecommerce_platform.Models;
using System.ComponentModel.DataAnnotations;

namespace basic_ecommerce_platform.DTOs
{
    public record CreateOrderDto(
        [property: Range(1, int.MaxValue)] int AddressId
        );

    public record AdminOrderQuery(
        [property: Range(1, int.MaxValue)] int Page = 1,
        [property: Range(1, 50)] int PageSize = 10,
        OrderStatus? Status = null,
        int? UserId = null,
        int? CategoryId = null,
        DateOnly? From = null,
        DateOnly? To = null
        );

    public record PayOrderDto(
        [property: Required, MaxLength(50)] string PaymentToken
        );

    public record ShippingAddressDto(
        string Line1,
        string? Line2,
        string City,
        string PostalCode
        );

    public record OrderItemResponseDto(
        int ProductId,
        string ProductName,
        decimal UnitPrice,
        int Quantity,
        decimal LineTotal
        );

    public record OrderSummaryDto(
        int Id,
        OrderStatus Status,
        DateTime CreatedAt,
        decimal Total,
        int ItemCount
        );

    public record OrderDetailDto(
        int Id,
        OrderStatus Status,
        DateTime CreatedAt,
        decimal Total,
        ShippingAddressDto ShippingAddress,
        IEnumerable<OrderItemResponseDto> Items
        );

    public record AdminOrderSummaryDto(
        int Id,
        int UserId,
        string UserEmail,
        OrderStatus Status,
        DateTime CreatedAt,
        decimal Total,
        int ItemCount
        );

    public record AdminOrderDetailDto(
        int Id,
        int UserId,
        string UserEmail,
        OrderStatus Status,
        DateTime CreatedAt,
        decimal Total,
        ShippingAddressDto ShippingAddress,
        IEnumerable<OrderItemResponseDto> Items
        );
}
