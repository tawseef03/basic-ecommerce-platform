using basic_ecommerce_platform.DTOs;

namespace basic_ecommerce_platform.Interfaces
{
    public interface ICartService
    {
        Task<CartResponseDto> GetCartAsync(int userId);
        Task<CartResponseDto> AddItemAsync(int userId, AddCartItemDto dto);
        Task<CartResponseDto> UpdateItemAsync(int userId, int productId, UpdateCartItemDto dto);
        Task RemoveItemAsync(int userId, int productId);
        Task ClearCartAsync(int userId);
    }
}
