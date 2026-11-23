using basic_ecommerce_platform.DTOs;
using basic_ecommerce_platform.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace basic_ecommerce_platform.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService) => _cartService = cartService;

        private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet]
        public async Task<ActionResult<CartResponseDto>> GetCart()
        {
            return Ok(await _cartService.GetCartAsync(UserId));
        }

        [HttpPost("items")]
        public async Task<ActionResult<CartResponseDto>> AddItem(AddCartItemDto dto)
        {
            return Ok(await _cartService.AddItemAsync(UserId, dto));
        }

        [HttpPut("items/{productId:int}")]
        public async Task<ActionResult<CartResponseDto>> UpdateItem(int productId, UpdateCartItemDto dto)
        {
            return Ok(await _cartService.UpdateItemAsync(UserId, productId, dto));
        }

        [HttpDelete("items/{productId:int}")]
        public async Task<IActionResult> RemoveItem(int productId)
        {
            await _cartService.RemoveItemAsync(UserId, productId);
            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> ClearCart()
        {
            await _cartService.ClearCartAsync(UserId);
            return NoContent();
        }
    }
}
