using basic_ecommerce_platform.DTOs;

namespace basic_ecommerce_platform.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryResponseDto>> GetAllCategoriesAsync();
        Task<CategoryResponseDto> GetCategoryByIdAsync(int id);
        Task<CategoryResponseDto> CreateCategoryAsync(CategoryRequestDto dto);
        Task<CategoryResponseDto> UpdateCategoryAsync(int id, CategoryRequestDto dto);
        Task DeleteCategoryAsync(int id);
    }
}
