using basic_ecommerce_platform.Data;
using basic_ecommerce_platform.DTOs;
using basic_ecommerce_platform.Exceptions;
using basic_ecommerce_platform.Interfaces;
using basic_ecommerce_platform.Models;
using Microsoft.EntityFrameworkCore;

namespace basic_ecommerce_platform.Services
{
    public class AddressService : IAddressService
    {
        private readonly AppDbContext _context;
        public AddressService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<AddressResponseDto>> GetAddressesByUserIdAsync(int userId)
        {
            return await _context.Addresses
                .AsNoTracking()
                .Where(a => a.UserId == userId)
                .Select(a => new AddressResponseDto(a.Id, a.Line1, a.Line2, a.City, a.PostalCode))
                .ToListAsync();
        }

        public async Task<AddressResponseDto> GetAddressByIdAsync(int userId, int id)
        {
            var address = await _context.Addresses
                .AsNoTracking()
                .Where(a => a.Id == id && a.UserId == userId)
                .Select(a => new AddressResponseDto(a.Id, a.Line1, a.Line2, a.City, a.PostalCode))
                .FirstOrDefaultAsync();

            return address ?? throw new NotFoundException("Address not found.");
        }

        public async Task<AddressResponseDto> CreateAddressAsync(int userId, AddressRequestDto dto)
        {
            if (await _context.Addresses.CountAsync(a => a.UserId == userId) >= 10)
                throw new ConflictException("Address limit reached.");

            var address = new Address
            {
                Line1 = dto.Line1.Trim(),
                Line2 = dto.Line2?.Trim(),
                City = dto.City.Trim(),
                PostalCode = dto.PostalCode.Trim(),
                UserId = userId
            };
            _context.Addresses.Add(address);
            await _context.SaveChangesAsync();
            return new AddressResponseDto(address.Id, address.Line1, address.Line2, address.City, address.PostalCode);
        }

        public async Task<AddressResponseDto> UpdateAddressAsync(int userId, int id, AddressRequestDto dto)
        {
            var address = await _context.Addresses
                .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId)
                ?? throw new NotFoundException($"Address with ID {id} not found.");
            address.Line1 = dto.Line1.Trim();
            address.Line2 = dto.Line2?.Trim();
            address.City = dto.City.Trim();
            address.PostalCode = dto.PostalCode.Trim();
            await _context.SaveChangesAsync();
            return new AddressResponseDto(address.Id, address.Line1, address.Line2, address.City, address.PostalCode);
        }

        public async Task DeleteAddressAsync(int userId, int id)
        {
            var address = await _context.Addresses
                .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId)
                ?? throw new NotFoundException($"Address with ID {id} not found.");
            _context.Addresses.Remove(address);
            await _context.SaveChangesAsync();
        }
    }
}
