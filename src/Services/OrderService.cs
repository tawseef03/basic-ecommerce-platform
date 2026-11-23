using basic_ecommerce_platform.Exceptions;
using basic_ecommerce_platform.Interfaces;
using basic_ecommerce_platform.Models;
using basic_ecommerce_platform.Data;
using basic_ecommerce_platform.DTOs;
using Microsoft.EntityFrameworkCore;
using basic_ecommerce_platform.Payment;

namespace basic_ecommerce_platform.Services
{
    public class OrderService : IOrderService
    {
        private const int pageSize = 10;
        private readonly AppDbContext _context;
        private readonly IPaymentGateway _gateway;
        public OrderService(AppDbContext context, IPaymentGateway gateway)
        {
            _context = context;
            _gateway = gateway;
        }
    
        public async Task<IEnumerable<OrderSummaryDto>> GetAllOrdersAsync(int userId, int page)
        {
            page = Math.Max(page, 1);

            return await _context.Orders
                .AsNoTracking()
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.CreatedAt)
                .ThenByDescending(a => a.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(a => new OrderSummaryDto(a.Id, a.Status, a.CreatedAt, a.Total, a.Items.Count))
                .ToListAsync();
        }
        public async Task<OrderDetailDto> GetOrderByIdAsync(int userId, int id)
        {
            var order = await _context.Orders
                .AsNoTracking()
                .Where(a => a.UserId == userId && a.Id == id)
                .Select(a => new OrderDetailDto(
                    a.Id,
                    a.Status,
                    a.CreatedAt,
                    a.Total,
                    new ShippingAddressDto(
                        a.Address.Line1,
                        a.Address.Line2,
                        a.Address.City,
                        a.Address.PostalCode
                        ),
                    a.Items.Select(i => new OrderItemResponseDto(
                        i.ProductId,
                        i.ProductName,
                        i.UnitPrice,
                        i.Quantity,
                        i.UnitPrice * i.Quantity)).ToList()
                    ))
                .FirstOrDefaultAsync();
            return order ?? throw new NotFoundException("Order not found.");
        }
        public async Task<IEnumerable<AdminOrderSummaryDto>> GetAllOrdersAsAdminAsync(AdminOrderQuery query)
        {
            if (query.From is DateOnly from && query.To is DateOnly to && from > to)
                throw new BadRequestException(["From must not be after To."]);

            var orders = _context.Orders
                .AsNoTracking()
                .IgnoreQueryFilters()
                .AsQueryable();

            if (query.Status is OrderStatus status)
                orders = orders.Where(o => o.Status == status);

            if (query.UserId is int userId)
                orders = orders.Where(o => o.UserId == userId);

            if (query.CategoryId is int categoryId)
                orders = orders.Where(o => o.Items.Any(i => i.Product.CategoryId == categoryId));

            if (query.From is DateOnly f)
                orders = orders.Where(o => o.CreatedAt >= f.ToDateTime(TimeOnly.MinValue));

            if (query.To is DateOnly t)
                orders = orders.Where(o => o.CreatedAt < t.AddDays(1).ToDateTime(TimeOnly.MinValue));

            return await orders
                .OrderByDescending(o => o.CreatedAt)
                .ThenByDescending(o => o.Id)
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(o => new AdminOrderSummaryDto(
                    o.Id,
                    o.UserId,
                    o.User.Email!,
                    o.Status,
                    o.CreatedAt,
                    o.Total,
                    o.Items.Count))
                .ToListAsync();
        }
        public async Task<AdminOrderDetailDto> GetOrderByIdAsAdminAsync(int id)
        {
            var order = await _context.Orders
            .AsNoTracking()
            .Where(o => o.Id == id)
            .Select(o => new AdminOrderDetailDto(
                o.Id,
                o.UserId,
                o.User.Email!,
                o.Status,
                o.CreatedAt,
                o.Total,
                new ShippingAddressDto(
                    o.Address.Line1,
                    o.Address.Line2,
                    o.Address.City,
                    o.Address.PostalCode),
                o.Items.Select(i => new OrderItemResponseDto(
                    i.ProductId,
                    i.ProductName,
                    i.UnitPrice,
                    i.Quantity,
                    i.UnitPrice * i.Quantity)).ToList()))
            .FirstOrDefaultAsync();

            return order ?? throw new NotFoundException("Order not found.");
        }
        public async Task<OrderDetailDto> CreateOrderAsync(int userId, CreateOrderDto dto)
        {
            var cart = await _context.Carts
                .IgnoreQueryFilters()
                .Include(c => c.Items)
                .ThenInclude(c => c.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart is null || cart.Items.Count == 0)
                throw new BadRequestException(["Cart is empty."]);
            if (!await _context.Addresses.AnyAsync(a => a.Id == dto.AddressId && a.UserId == userId))
                throw new NotFoundException("Address not found.");
            if (cart.Items.Any(i => !i.Product.IsActive))
                throw new BadRequestException(["One or more products in the cart are unavailable."]);
            if (cart.Items.Any(i => i.Product.StockQuantity < i.Quantity))
                throw new BadRequestException(["Insufficient stock for one or more products."]);

            var shippingAddress = await _context.Addresses
                .AsNoTracking()
                .Where(a => a.Id == dto.AddressId && a.UserId == userId)
                .Select(a => new ShippingAddress
                {
                    Line1 = a.Line1,
                    Line2 = a.Line2,
                    City = a.City,
                    PostalCode = a.PostalCode
                })
                .FirstOrDefaultAsync();

            var order = new Order
            {
                UserId = userId,
                Address = shippingAddress,
                Status = OrderStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                Items = cart.Items.Select(i => new OrderItem
                {
                    ProductId = i.ProductId,
                    ProductName = i.Product.Name,
                    UnitPrice = i.Product.UnitPrice,
                    Quantity = i.Quantity
                }).ToList(),
                Total = cart.Items.Sum(i => i.Quantity * i.Product.UnitPrice)
            };

            _context.Orders.Add(order);
            _context.CartItems.RemoveRange(cart.Items);
            await _context.SaveChangesAsync();

            return new OrderDetailDto(
                order.Id,
                order.Status,
                order.CreatedAt,
                order.Total,
                new ShippingAddressDto(
                    order.Address.Line1,
                    order.Address.Line2,
                    order.Address.City,
                    order.Address.PostalCode),
                order.Items.Select(i => new OrderItemResponseDto(
                    i.ProductId,
                    i.ProductName,
                    i.UnitPrice,
                    i.Quantity,
                    i.UnitPrice * i.Quantity)).ToList());
        }
        public async Task<OrderDetailDto> PayOrderAsync(int userId, int orderId, PayOrderDto dto)
        {
            var order = await _context.Orders
                .AsNoTracking()
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId)
                ?? throw new NotFoundException("Order not found.");

            await using (var tx = await _context.Database.BeginTransactionAsync())
            {
                var claimed = await _context.Orders
                    .Where(o => o.Id == orderId && o.Status == OrderStatus.Pending)
                    .ExecuteUpdateAsync(s => s
                        .SetProperty(o => o.Status, OrderStatus.Paid));

                if (claimed == 0)
                    throw new ConflictException("Order is not payable");

                foreach (var item in order.Items.OrderBy(i => i.ProductId))
                {
                    var rows = await _context.Products
                        .Where(p => p.Id == item.ProductId && p.StockQuantity >= item.Quantity)
                        .ExecuteUpdateAsync(s => s.SetProperty(
                            p => p.StockQuantity,
                            p => p.StockQuantity - item.Quantity));

                    if (rows == 0)
                    {
                        await tx.RollbackAsync();
                        await CancelOrderAsync(orderId);
                        throw new ConflictException($"{item.ProductName} is out of stock. Order cancelled");
                    }
                }

                var result = await _gateway.ChargeAsync(order.Total, dto.PaymentToken);

                if (!result.Success)
                {
                    await tx.RollbackAsync();
                    throw new BadRequestException([result.FailureReason ?? "Payment failed."]);
                }

                await tx.CommitAsync();

            }
            
            return await GetOrderByIdAsync(userId, orderId);
        }

        private Task CancelOrderAsync(int orderId) =>
        _context.Orders
            .Where(o => o.Id == orderId && o.Status == OrderStatus.Pending)
            .ExecuteUpdateAsync(s => s.SetProperty(o => o.Status, OrderStatus.Cancelled));
    }
}
