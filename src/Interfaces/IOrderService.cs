using basic_ecommerce_platform.DTOs;

namespace basic_ecommerce_platform.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderSummaryDto>> GetAllOrdersAsync(int userId, int page);
        Task<OrderDetailDto> GetOrderByIdAsync(int userId, int id);
        Task<IEnumerable<AdminOrderSummaryDto>> GetAllOrdersAsAdminAsync(AdminOrderQuery query);
        Task<AdminOrderDetailDto> GetOrderByIdAsAdminAsync(int id);
        Task<OrderDetailDto> CreateOrderAsync(int userId, CreateOrderDto dto);
        Task<OrderDetailDto> PayOrderAsync(int userId, int orderId, PayOrderDto dto);
    }
}
