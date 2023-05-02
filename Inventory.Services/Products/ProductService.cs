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
            var products = (await _repository.ProductRepository.GetAllAsync())
                            .ToList();

            var getProductDtos = products.Select(p =>
            {
                return new GetProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Quantity = p.Quantity
                };
            }).ToList();

            return getProductDtos;
        }

        public async Task<GetProductDto> GetProductByIdAsync(int productId)
        {
            var product = await GetProductEntityAsync(productId);

            if (product is null)
                throw new NotFoundException(nameof(Product));

            return new GetProductDto { 
                Id = product.Id,
                Name = product.Name,
                Quantity = product.Quantity,
            };
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
            await _repository.SaveAsync();

            return new GetProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Quantity = product.Quantity,
            };
        }

        public async Task<GetProductDto> EditProductAsync(EditProductModel productModel)
        {
            var existingProduct = await GetProductEntityAsync(productModel.Id);

            existingProduct.Quantity = productModel.Quantity;
            existingProduct.LastUpdatedAt = DateTime.UtcNow;

            _repository.ProductRepository.Edit(existingProduct);
            await _repository.SaveAsync();

            return new GetProductDto
            {
                Id = existingProduct.Id,
                Name = existingProduct.Name,
                Quantity = existingProduct.Quantity,
            };
        }

        public async Task<Response> DeleteProductAsync(int productId)
        {
            var product = await GetProductEntityAsync(productId);

            _repository.ProductRepository.Remove(product);
            await _repository.SaveAsync();

            return new Response { IsSuccess = true, Message = "Product delete successfully!" };
        }

        public async Task<Response> CheckProductsAvailablityAsync(List<ProductAvailablityModel> products)
        {
            var productList = await GetProductsByIds(products);

            foreach (var product in products) 
            {
                var existingProduct = productList.FirstOrDefault(p => p.Id == product.Id);

                if (existingProduct is null)
                    return new Response { IsSuccess = false, Message = "Invalid product found!", StatusCode = 400 };

                if(existingProduct.Quantity < product.Quantity)
                    return new Response { IsSuccess = false, Message = "Product is not available!", StatusCode = 400 };
            }

            return new Response { IsSuccess = true, Message = "Products are available!", StatusCode = 200 };
        }

        public async Task UpdateProductsQuantityAsync(List<ProductAvailablityModel> products)
        {
            var productList = await GetProductsByIds(products);

            foreach (var product in products)
            {
                var existingProduct = productList.FirstOrDefault(p => p.Id == product.Id);

                if (existingProduct is not null && existingProduct.Quantity >= product.Quantity)
                {
                    existingProduct.Quantity = existingProduct.Quantity - product.Quantity;
                    existingProduct.LastUpdatedAt = DateTime.UtcNow;
                    _repository.ProductRepository.Edit(existingProduct);
                }
            }

            await _repository.SaveAsync();
        }

        private async Task<IList<Product>> GetProductsByIds(List<ProductAvailablityModel> products)
        {
            var productIds = products.Select(p => p.Id);

            return await _repository.ProductRepository
                    .GetAsync(p => productIds.Contains(p.Id));
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
