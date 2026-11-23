using Microsoft.AspNetCore.Identity;
using basic_ecommerce_platform.Data;
using basic_ecommerce_platform.DTOs;
using basic_ecommerce_platform.Exceptions;
using basic_ecommerce_platform.Interfaces;
using basic_ecommerce_platform.Models;

namespace basic_ecommerce_platform.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;
        private readonly AppDbContext _context;

        public AuthService(UserManager<User> userManager, ITokenService tokenService, AppDbContext context)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _context = context;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            var user = new User
            {
                UserName = dto.Email,
                Email = dto.Email,
                FullName = dto.FullName
            };

            await using var transaction = await _context.Database.BeginTransactionAsync();

            var createResult = await _userManager.CreateAsync(user, dto.Password);
            if (!createResult.Succeeded)
            {
                throw new BadRequestException(createResult.Errors.Select(e => e.Description));
            }

            var roleResult = await _userManager.AddToRoleAsync(user, "Customer");
            if (!roleResult.Succeeded)
            {
                throw new BadRequestException(roleResult.Errors.Select(e => e.Description));
            }

            await transaction.CommitAsync();

            return await BuildResponseAsync(user);
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user is null || !await _userManager.CheckPasswordAsync(user, dto.Password))
            {
                throw new UnauthorizedException("Invalid email or password.");
            }

            return await BuildResponseAsync(user);
        }

        private async Task<AuthResponseDto> BuildResponseAsync(User user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var (token, expiresAt) = _tokenService.CreateToken(user, roles);
            return new AuthResponseDto(token, expiresAt, user.Email!, roles);
        }
    }
}
