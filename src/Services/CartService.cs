using basic_ecommerce_platform.Data;
using basic_ecommerce_platform.DTOs;
using basic_ecommerce_platform.Exceptions;
using basic_ecommerce_platform.Interfaces;
using basic_ecommerce_platform.Models;
using Microsoft.EntityFrameworkCore;

namespace basic_ecommerce_platform.Services
{
    public class CartService : ICartService
    {
        private readonly AppDbContext _context;
        public CartService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<CartResponseDto> GetCartAsync(int userId)
        {
            var items = await _context.CartItems
                .AsNoTracking()
                .Where(i => i.Cart.UserId == userId)
                .OrderBy(i => i.Id)
                .Select(i => new CartItemResponseDto(
                    i.ProductId,
                    i.Product.Name,
                    i.Product.UnitPrice,
                    i.Quantity,
                    i.Quantity * i.Product.UnitPrice
                ))
                .ToListAsync();
            return new CartResponseDto(
                items,
                items.Sum(i => i.LineTotal)
            );
        }
        public async Task<CartResponseDto> AddItemAsync(int userId, AddCartItemDto dto)
        {
            var product = await _context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == dto.ProductId)
                ?? throw new NotFoundException($"Product with ID {dto.ProductId} not found.");

            var cart = await GetOrCreateCartAsync(userId);

            var item = await _context.CartItems
                .FirstOrDefaultAsync(i => i.CartId == cart.Id && i.ProductId == dto.ProductId);

            var newQuantity = (item?.Quantity ?? 0) + dto.Quantity;
            if (newQuantity > product.StockQuantity)
                throw new BadRequestException([$"Only {product.StockQuantity} in stock."]);

            if (item is null)
                _context.CartItems.Add(new CartItem
                {
                    CartId = cart.Id,
                    ProductId = dto.ProductId,
                    Quantity = newQuantity
                });
            else
                item.Quantity = newQuantity;

            await _context.SaveChangesAsync();
            return await GetCartAsync(userId);
        }
        public async Task<CartResponseDto> UpdateItemAsync(int userId, int productId, UpdateCartItemDto dto)
        {
            var item = await _context.CartItems
                .Include(i => i.Product)
                .FirstOrDefaultAsync(i => i.Cart.UserId == userId && i.ProductId == productId)
                ?? throw new NotFoundException("Item not in cart.");

            if (dto.Quantity > item.Product.StockQuantity)
                throw new BadRequestException([$"Only {item.Product.StockQuantity} in stock."]);

            item.Quantity = dto.Quantity;
            await _context.SaveChangesAsync();
            return await GetCartAsync(userId);
        }
        public async Task RemoveItemAsync(int userId, int productId)
        {
            var deleted = await _context.CartItems
                .Where(i => i.Cart.UserId == userId && i.ProductId == productId)
                .ExecuteDeleteAsync();

            if (deleted == 0)
                throw new NotFoundException("Item not in cart.");
        }
        public async Task ClearCartAsync(int userId)
        {
            await _context.CartItems
                .Where(i => i.Cart.UserId == userId)
                .ExecuteDeleteAsync();
        }

        private async Task<Cart> GetOrCreateCartAsync(int userId)
        {
            var cart = await _context.Carts 
                .FirstOrDefaultAsync(c => c.UserId == userId);
            if (cart is null)
            {
                cart = new Cart { UserId = userId };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }
            return cart;
        }
    }
}
