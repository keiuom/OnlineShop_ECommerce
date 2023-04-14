using BuyNow.Core.Common;
using BuyNow.Core.Exceptions;
using Inventory.Common.DTOs;
using Inventory.Common.Models;
using Inventory.Core.Domain;
using Inventory.Data;

namespace Inventory.Services.Products
{
    public class ProductService : IProductService
    {
        private readonly IRepositoryWrapper _repository;

        public ProductService(IRepositoryWrapper repository)
        {
            _repository = repository;
        }

        public async Task<List<GetProductDto>> GetProductsAsync()
        {
            //var products = await _productRepository.GetAllAsync();
            throw new NotImplementedException();
        }

        public async Task<GetProductDto> GetProductByIdAsync(int productId)
        {
            var product = await GetProductEntityAsync(productId);

            return new();
        }

        public async Task<GetProductDto> AddProductAsync(AddProductModel productModel)
        {
            var dateTime = DateTime.UtcNow;

            var product = new Product
            {
                Name = productModel.Name,
                Quantity = productModel.Quantity,
                CreatedAt = dateTime,
                LastUpdatedAt = dateTime,
            };

            await _repository.ProductRepository.AddAsync(product);

            return new();
        }

        public async Task<GetProductDto> EditProductAsync(EditProductModel productModel)
        {
            var existingProduct = await GetProductEntityAsync(productModel.Id);

            existingProduct.Quantity = productModel.Quantity;
            existingProduct.LastUpdatedAt = DateTime.UtcNow;

            _repository.ProductRepository.Edit(existingProduct);
        }

        public async Task<Response> DeleteProductAsync(int productId)
        {
            var product = await GetProductEntityAsync(productId);

            _repository.ProductRepository.Remove(product);

            return new Response { IsSuccess = true, Message = "Product delete successfully!" };
        }

        private async Task<Product> GetProductEntityAsync(int productId)
        {
            var product = await _repository.ProductRepository.GetByIdAsync(productId);

            if (product == null)
                throw new NotFoundException(nameof(Product));

            return product;
        }
    }
}
