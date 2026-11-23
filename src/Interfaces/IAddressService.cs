using basic_ecommerce_platform.DTOs;

namespace basic_ecommerce_platform.Interfaces
{
    public interface IAddressService
    {
        Task<IEnumerable<AddressResponseDto>> GetAddressesByUserIdAsync(int userId);
        Task<AddressResponseDto> GetAddressByIdAsync(int userId, int id);
        Task<AddressResponseDto> CreateAddressAsync(int userId, AddressRequestDto dto);
        Task<AddressResponseDto> UpdateAddressAsync(int userId, int id, AddressRequestDto dto);
        Task DeleteAddressAsync(int userId, int id);
    }
}
