using basic_ecommerce_platform.Data;
using basic_ecommerce_platform.DTOs;
using basic_ecommerce_platform.Exceptions;
using basic_ecommerce_platform.Interfaces;
using basic_ecommerce_platform.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace basic_ecommerce_platform.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;
        private const int MinPageSize = 10;
        private const int MaxPageSize = 50;
        public ProductService(AppDbContext context)
        {
            _context = context;
        }

        private async Task<List<ProductResponseDto>> QueryProductsAsync(ProductQueryDto q, bool includeInactive)
        {
            if (q.MinPrice > q.MaxPrice)
                throw new BadRequestException(new[] { "MinPrice cannot be greater than MaxPrice." });
            if (q.CategoryId is not null && !await _context.Categories.AnyAsync(c => c.Id == q.CategoryId))
                throw new NotFoundException($"Category with ID {q.CategoryId} not found.");

            var query = _context.Products.AsNoTracking();
            if (includeInactive) query = query.IgnoreQueryFilters();

            if (q.CategoryId is not null) query = query.Where(p => p.CategoryId == q.CategoryId);
            if (q.MinPrice is not null) query = query.Where(p => p.UnitPrice >= q.MinPrice);
            if (q.MaxPrice is not null) query = query.Where(p => p.UnitPrice <= q.MaxPrice);
            if (!string.IsNullOrWhiteSpace(q.Search)) query = query.Where(p => p.Name.Contains(q.Search));

            var page = Math.Clamp(q.Page, 1, 10_000);
            var pageSize = Math.Clamp(q.PageSize, MinPageSize, MaxPageSize);

            return await query
                .OrderBy(p => p.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(ToDto)
                .ToListAsync();
        }

        public async Task<IEnumerable<ProductResponseDto>> GetProductsAsync(ProductQueryDto q)
        {
            return await QueryProductsAsync(q, false);
        }
        public async Task<ProductResponseDto> GetProductByIdAsync(int id)
        {
            var product = await _context.Products
               .AsNoTracking()
               .Where(p => p.Id == id)
               .Select(ToDto)
               .FirstOrDefaultAsync();

            return product ?? throw new NotFoundException($"Product with ID {id} not found.");
        }

        public async Task<IEnumerable<ProductResponseDto>> GetProductsAsAdminAsync(ProductQueryDto q)
        {
            return await QueryProductsAsync(q, true);
        }

        public async Task<ProductResponseDto> GetProductByIdAsAdminAsync(int id)
        {
            var product = await _context.Products
               .IgnoreQueryFilters()
               .AsNoTracking()
               .Where(p => p.Id == id)
               .Select(ToDto)
               .FirstOrDefaultAsync();
            return product ?? throw new NotFoundException($"Product with ID {id} not found.");
        }
        public async Task<ProductResponseDto> CreateProductAsync(ProductRequestDto dto)
        {
            if (!await _context.Categories.AnyAsync(c => c.Id == dto.CategoryId))
                throw new NotFoundException($"Category with ID {dto.CategoryId} not found.");
            if (await _context.Products.IgnoreQueryFilters().AnyAsync(p => p.Name == dto.Name))
                throw new ConflictException($"Product '{dto.Name}' already exists.");

            var product = new Product { 
                Name = dto.Name, Description = dto.Description, UnitPrice = dto.Price,
                StockQuantity = dto.StockQuantity, CategoryId = dto.CategoryId };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return await GetProductByIdAsAdminAsync(product.Id);
        }
        public async Task<ProductResponseDto> UpdateProductAsync(int id, ProductRequestDto dto)
        {
            var product = await _context.Products
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(p => p.Id == id)
                ?? throw new NotFoundException($"Product with ID {id} not found.");

            if (!await _context.Categories.AnyAsync(c => c.Id == dto.CategoryId))
                throw new NotFoundException($"Category with ID {dto.CategoryId} not found.");
            if (await _context.Products.IgnoreQueryFilters().AnyAsync(p => p.Name == dto.Name && p.Id != id))
                throw new ConflictException($"Product '{dto.Name}' already exists.");

            product.Name = dto.Name;
            product.Description = dto.Description;
            product.UnitPrice = dto.Price;
            product.StockQuantity = dto.StockQuantity;
            product.CategoryId = dto.CategoryId;
            await _context.SaveChangesAsync();

            return await GetProductByIdAsAdminAsync(id);
        }
        public async Task DeleteProductAsync(int id)
        {
            var product = await _context.Products
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(p => p.Id == id)
                ?? throw new NotFoundException($"Product with ID {id} not found.");

            product.IsActive = false;
            await _context.SaveChangesAsync();
        }

        private static readonly Expression<Func<Product, ProductResponseDto>> ToDto = p =>
            new ProductResponseDto(
                p.Id, p.Name, p.Description, p.UnitPrice,
                p.StockQuantity, p.CategoryId, p.Category.Name, p.IsActive);
    }
}
