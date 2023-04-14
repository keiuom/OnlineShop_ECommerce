using BuyNow.Core.Common;
using Inventory.Common.DTOs;
using Inventory.Common.Models;

namespace Inventory.Services.Products
{
    public interface IProductService
    {
        Task<List<GetProductDto>> GetProductsAsync();

        Task<GetProductDto> GetProductByIdAsync(int productId);

        Task<GetProductDto> AddProductAsync(AddProductModel productModel);

        Task<GetProductDto> EditProductAsync(EditProductModel productModel);

        Task<Response> DeleteProductAsync(int productId);
    }
}
