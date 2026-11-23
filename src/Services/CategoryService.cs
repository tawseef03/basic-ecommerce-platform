using basic_ecommerce_platform.Data;
using basic_ecommerce_platform.DTOs;
using basic_ecommerce_platform.Exceptions;
using basic_ecommerce_platform.Interfaces;
using basic_ecommerce_platform.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace basic_ecommerce_platform.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _context;
        public CategoryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CategoryResponseDto>> GetAllCategoriesAsync()
        {
            return await _context.Categories
                .AsNoTracking()
                .Select(c => new CategoryResponseDto(c.Id, c.Name))
                .ToListAsync();
        }

        public async Task<CategoryResponseDto> GetCategoryByIdAsync(int id)
        {
            var category = await _context.Categories
               .AsNoTracking()
               .Where(c => c.Id == id)
               .Select(c => new CategoryResponseDto(c.Id, c.Name))
               .FirstOrDefaultAsync();

            return category ?? throw new NotFoundException($"Category with ID {id} not found.");
        }

        public async Task<CategoryResponseDto> CreateCategoryAsync(CategoryRequestDto dto)
        {
            if (await _context.Categories.AnyAsync(c => c.Name == dto.Name))
                throw new ConflictException($"Category '{dto.Name}' already exists.");
            var category = new Category{ Name = dto.Name };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return new CategoryResponseDto(category.Id, category.Name);
        }

        public async Task<CategoryResponseDto> UpdateCategoryAsync(int id, CategoryRequestDto dto)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id)
                ?? throw new NotFoundException($"Category with ID {id} not found.");
            if (await _context.Categories.AnyAsync(c => c.Name == dto.Name && c.Id != id))
                throw new ConflictException($"Category '{dto.Name}' already exists.");
            category.Name = dto.Name;
            await _context.SaveChangesAsync();
            return new CategoryResponseDto(category.Id, category.Name);
        }

        public async Task DeleteCategoryAsync(int id)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id)
                ?? throw new NotFoundException($"Category with ID {id} not found.");
            if (await _context.Products.IgnoreQueryFilters().AnyAsync(p => p.CategoryId == id))
                throw new ConflictException("Category still has products.");
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }
    }
}
