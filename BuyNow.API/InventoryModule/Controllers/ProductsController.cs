using BuyNow.Core.Common;
using Inventory.Common.DTOs;
using Inventory.Common.Models;
using Inventory.Services.Products;
using Microsoft.AspNetCore.Mvc;

namespace BuyNow.API.Inventory.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetProductDto>> GetProductAsync(int id)
        {
            return await _productService.GetProductByIdAsync(id);
        }

        [HttpPost]
        public async Task<ActionResult<GetProductDto>> AddProductAsync(AddProductModel productModel)
        {
            return await _productService.AddProductAsync(productModel);
        }

        [HttpPut]
        public async Task<ActionResult<GetProductDto>> EditProductAsync(EditProductModel productModel)
        {
            return await _productService.EditProductAsync(productModel);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Response>> DeleteProductAsync(int id)
        {
            return await _productService.DeleteProductAsync(id);
        }
    }
}
