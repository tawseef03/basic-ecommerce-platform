using basic_ecommerce_platform.DTOs;

namespace basic_ecommerce_platform.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductResponseDto>> GetProductsAsync(ProductQueryDto query);
        Task<ProductResponseDto> GetProductByIdAsync(int id);
        
        Task<IEnumerable<ProductResponseDto>> GetProductsAsAdminAsync(ProductQueryDto query);
        Task<ProductResponseDto> GetProductByIdAsAdminAsync(int id);
        Task<ProductResponseDto> CreateProductAsync(ProductRequestDto dto);
        Task<ProductResponseDto> UpdateProductAsync(int productId, ProductRequestDto dto);
        Task DeleteProductAsync(int id);
    }
}
