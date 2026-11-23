using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using basic_ecommerce_platform.DTOs;
using basic_ecommerce_platform.Interfaces;

namespace basic_ecommerce_platform.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AddressesController : ControllerBase
    {
        private readonly IAddressService _addressService;

        public AddressesController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AddressResponseDto>>> GetMyAddresses()
        {
            var addresses = await _addressService.GetAddressesByUserIdAsync(CurrentUserId);
            return Ok(addresses);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<AddressResponseDto>> GetAddressById(int id)
        {
            var address = await _addressService.GetAddressByIdAsync(CurrentUserId, id);
            return Ok(address);
        }

        [HttpPost]
        public async Task<ActionResult<AddressResponseDto>> CreateAddress(AddressRequestDto dto)
        {
            var address = await _addressService.CreateAddressAsync(CurrentUserId, dto);
            return CreatedAtAction(nameof(GetAddressById), new { id = address.Id }, address);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<AddressResponseDto>> UpdateAddress(int id, AddressRequestDto dto)
        {
            var address = await _addressService.UpdateAddressAsync(CurrentUserId, id, dto);
            return Ok(address);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            await _addressService.DeleteAddressAsync(CurrentUserId, id);
            return NoContent();
        }
    }
}