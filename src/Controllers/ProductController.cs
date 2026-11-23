using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using basic_ecommerce_platform.DTOs;
using basic_ecommerce_platform.Interfaces;

namespace basic_ecommerce_platform.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductResponseDto>>> GetProducts([FromQuery] ProductQueryDto query)
        {
            var products = await _productService.GetProductsAsync(query);
            return Ok(products);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductResponseDto>> GetProductById(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            return Ok(product);
        }

        [HttpGet("~/api/admin/product")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<ProductResponseDto>>> GetProductsAsAdmin([FromQuery] ProductQueryDto query)
        {
            var products = await _productService.GetProductsAsAdminAsync(query);
            return Ok(products);
        }

        [HttpGet("~/api/admin/product/{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ProductResponseDto>> GetProductByIdAsAdmin(int id)
        {
            var product = await _productService.GetProductByIdAsAdminAsync(id);
            return Ok(product);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ProductResponseDto>> CreateProduct(ProductRequestDto dto)
        {
            var product = await _productService.CreateProductAsync(dto);
            return CreatedAtAction(nameof(GetProductByIdAsAdmin), new { id = product.Id }, product);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ProductResponseDto>> UpdateProduct(int id, ProductRequestDto dto)
        {
            var product = await _productService.UpdateProductAsync(id, dto);
            return Ok(product);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            await _productService.DeleteProductAsync(id);
            return NoContent();
        }
    }
}