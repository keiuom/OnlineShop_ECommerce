using BuyNow.Core.Exceptions;
using Inventory.Common.DTOs;
using Inventory.Common.Models;
using Inventory.Core.Domain;
using Inventory.Data;
using Inventory.Data.Repositories;
using Inventory.Services.Products;
using System.Linq.Expressions;

namespace Inventory.Test.Services
{
    [TestFixture]
    public class ProductServiceTest
    {
        private AutoMock _autoMock;
        private Mock<IRepositoryWrapper> _repositoryMock;
        private Mock<IProductRepository> _productRepositoryMock;
        private IProductService _productService;

        [SetUp] 
        public void SetUp() 
        {
            _autoMock = AutoMock.GetLoose();
            _repositoryMock = _autoMock.Mock<IRepositoryWrapper>();
            _productRepositoryMock = _autoMock.Mock<IProductRepository>();
            _productService = _autoMock.Create<ProductService>();
        }

        [TearDown]
        public void TearDown() 
        {
            _autoMock?.Dispose();
        }

        [Test]
        public async Task GetProductsAsync_ReturnsListOfGetProductDto_WhenProductsExist()
        {
            // Arrange
            var products = GetProducts();

            _repositoryMock.Setup(r => r.ProductRepository)
                .Returns(_productRepositoryMock.Object);

            _repositoryMock.Setup(x => x.ProductRepository.GetAllAsync())
                .ReturnsAsync(products)
                .Verifiable();

            // Act
            var result = await _productService.GetProductsAsync();

            // Assert
            result.ShouldSatisfyAllConditions(
                result => _repositoryMock.Verify(),
                result => result.ShouldBeOfType<List<GetProductDto>>(),
                result => result[0].Id.ShouldBeEquivalentTo(products[0].Id),
                result => result[0].Name.ShouldBeEquivalentTo(products[0].Name),
                result => result[0].Quantity.ShouldBeEquivalentTo(products[0].Quantity)
                );
        }

        [Test]
        public async Task GetProductsAsync_ReturnsEmptyList_WhenNoProductsExist()
        {
            // Arrange
            var products = new List<Product>();
            var expectedTotalItemCount = 0;

            _repositoryMock.Setup(r => r.ProductRepository)
                .Returns(_productRepositoryMock.Object);

            _repositoryMock.Setup(x => x.ProductRepository.GetAllAsync())
                .ReturnsAsync(products)
                .Verifiable();

            // Act
            var result = await _productService.GetProductsAsync();

            // Assert
            result.ShouldSatisfyAllConditions(
                result => _repositoryMock.Verify(),
                result => result.ShouldBeOfType<List<GetProductDto>>(),
                result => result.Count.ShouldBeEquivalentTo(expectedTotalItemCount)
                );
        }

        [Test]
        public async Task GetProductByIdAsync_WithValidId_ReturnsCorrectDto()
        {
            // Arrange
            var productId = 1;
            var productEntity = GetProductEntity();

            _repositoryMock.Setup(r => r.ProductRepository)
                .Returns(_productRepositoryMock.Object);

            _repositoryMock.Setup(r => r.ProductRepository.GetByIdAsync(productId))
                .ReturnsAsync(productEntity)
                .Verifiable();

            // Act
            var result = await _productService.GetProductByIdAsync(productId);

            // Assert
            result.ShouldSatisfyAllConditions(
                result => _repositoryMock.Verify(),
                result => result.ShouldNotBeNull(),
                result => result.Id.ShouldBe(productId),
                result => result.Name.ShouldBe(productEntity.Name),
                result => result.Quantity.ShouldBe(productEntity.Quantity)
                );
        }

        [Test]
        public async Task GetProductByIdAsync_WithInvalidId_ThrowsNotFoundException()
        {
            // Arrange
            var productId = 100;
            Product? product = null;

            _repositoryMock.Setup(r => r.ProductRepository)
                .Returns(_productRepositoryMock.Object);

            _repositoryMock.Setup(r => r.ProductRepository.GetByIdAsync(productId))
                          .ReturnsAsync(product);

            // Act and Assert
            await Should.ThrowAsync<NotFoundException>(async () => await _productService.GetProductByIdAsync(productId));
        }

        [Test]
        public async Task AddProductAsync_ShouldAddProductToRepository_AndReturnProductDto()
        {
            // Arrange
            var addProductModel = new AddProductModel
            {
                Name = "Test Product",
                Quantity = 10
            };

            var expectedProduct = new Product
            {
                Id = 1,
                Name = "Test Product",
                Quantity = 10,
                CreatedAt = DateTime.UtcNow,
                LastUpdatedAt = DateTime.UtcNow,
            };

            _repositoryMock.Setup(r => r.ProductRepository)
                .Returns(_productRepositoryMock.Object);

            _repositoryMock.Setup(x => x.ProductRepository.AddAsync(It.IsAny<Product>()))
                .Callback<Product>(p => p.Id = expectedProduct.Id)
                .Returns(Task.CompletedTask)
                .Verifiable();

            _repositoryMock.Setup(x => x.SaveAsync())
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Act
            var result = await _productService.AddProductAsync(addProductModel);

            // Assert
            result.ShouldSatisfyAllConditions(
                result => _repositoryMock.Verify(),
                result => result.ShouldNotBeNull(),
                result => result.Id.ShouldBe(expectedProduct.Id),
                result => result.Quantity.ShouldBe(expectedProduct.Quantity)
                );
        }

        [Test]
        public async Task AddProductAsync_ShouldThrowException_WhenInvalidInput()
        {
            // Arrange
            var addProductModel = new AddProductModel();

            _repositoryMock.Setup(r => r.ProductRepository)
                .Returns(_productRepositoryMock.Object);

            _repositoryMock.Setup(x => x.ProductRepository.AddAsync(It.IsAny<Product>()))
                .Throws<Exception>();

            // Act & Assert
            await Should.ThrowAsync<Exception>(async () => await _productService.AddProductAsync(addProductModel));
        }

        [Test]
        public async Task EditProductAsync_ProductExists_UpdatesProductAndReturnsGetProductDto()
        {
            // Arrange
            var existingProduct = new Product { Id = 1, Name = "Product 1", Quantity = 5 };
            var editProductModel = new EditProductModel { Id = 1, Quantity = 10 };

            _repositoryMock.Setup(r => r.ProductRepository)
                .Returns(_productRepositoryMock.Object);

            _repositoryMock.Setup(r => r.ProductRepository.GetByIdAsync(existingProduct.Id))
                .ReturnsAsync(existingProduct)
                .Verifiable();

            _repositoryMock.Setup(uow => uow.SaveAsync())
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Act
            var result = await _productService.EditProductAsync(editProductModel);

            // Assert
            result.ShouldSatisfyAllConditions(
                result => _repositoryMock.Verify(),
                result => result.ShouldNotBeNull(),
                result => result.Id.ShouldBe(existingProduct.Id),
                result => result.Name.ShouldBe(existingProduct.Name),
                result => result.Quantity.ShouldBe(existingProduct.Quantity)
                );
        }

        [Test]
        public void EditProductAsync_ProductDoesNotExist_ThrowsNotFoundException()
        {
            // Arrange
            var nonExistingProductId = 1;
            var editProductModel = new EditProductModel { Id = nonExistingProductId, Quantity = 10 };
            Product? expectedProduct = null;

            _repositoryMock.Setup(r => r.ProductRepository)
                .Returns(_productRepositoryMock.Object);

            _repositoryMock.Setup(r => r.ProductRepository.GetByIdAsync(nonExistingProductId))
                .ReturnsAsync(expectedProduct);

            // Act + Assert
            Should.ThrowAsync<NotFoundException>(async () => await _productService.EditProductAsync(editProductModel));
        }

        [Test]
        public async Task DeleteProductAsync_DeletesProduct_ReturnsSuccessResponse()
        {
            // Arrange
            var productId = 1;
            var product = new Product { Id = productId, Name = "Test Product", Quantity = 10 };

            _repositoryMock.Setup(r => r.ProductRepository)
                .Returns(_productRepositoryMock.Object);

            _repositoryMock.Setup(x => x.ProductRepository.GetByIdAsync(productId))
                .ReturnsAsync(product)
                .Verifiable();

            _repositoryMock.Setup(r => r.SaveAsync())
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Act
            var result = await _productService.DeleteProductAsync(productId);

            // Assert
            result.ShouldSatisfyAllConditions(
               result => _repositoryMock.Verify(),
               result => result.ShouldNotBeNull(),
               result => result.IsSuccess.ShouldBe(true),
               result => result.Message.ShouldBe("Product delete successfully!"),
               result => _repositoryMock.Verify(x => x.ProductRepository.Remove(product), Times.Once),
               result => _repositoryMock.Verify(x => x.SaveAsync(), Times.Once)
               );
        }

        [Test]
        public async Task DeleteProductAsync_ProductDoesNotExist_ThrowsNotFoundException()
        {
            // Arrange
            var productId = 1;
            var editProductModel = new EditProductModel { Id = 10, Quantity = 10 };
            Product? product = null;

            _repositoryMock.Setup(r => r.ProductRepository)
                .Returns(_productRepositoryMock.Object);

            _repositoryMock.Setup(x => x.ProductRepository.GetByIdAsync(productId))
                .ReturnsAsync(product);

            // Act + Assert
            await Should.ThrowAsync<NotFoundException>(async () => await _productService.EditProductAsync(editProductModel));
        }

        [Test]
        public async Task DeleteProductAsync_ErrorDeletingProduct_ThrowException()
        {
            // Arrange
            var productId = 1;
            var product = new Product { Id = productId, Name = "Test Product", Quantity = 10 };

            _repositoryMock.Setup(r => r.ProductRepository)
                .Returns(_productRepositoryMock.Object);

            _repositoryMock.Setup(x => x.ProductRepository.GetByIdAsync(productId))
                .ReturnsAsync(product);

            _repositoryMock.Setup(x => x.ProductRepository.Remove(It.IsAny<Product>()))
                .Throws<Exception>();

            // Act + Assert
            await Should.ThrowAsync<Exception>(async () => await _productService.DeleteProductAsync(productId));
        }

        [Test]
        public async Task CheckProductsAvailablityAsync_AllProductsAvailable_ReturnsSuccessResponse()
        {
            // Arrange
            var products = new List<ProductAvailablityModel>
            {
                new ProductAvailablityModel { Id = 1, Quantity = 2 },
                new ProductAvailablityModel { Id = 2, Quantity = 3 }
            };

            var productList = new List<Product>
            {
                new Product { Id = 1, Quantity = 5 },
                new Product { Id = 2, Quantity = 10 }
            };

            _repositoryMock.Setup(r => r.ProductRepository)
                .Returns(_productRepositoryMock.Object);

            _repositoryMock.Setup(r => r.ProductRepository.GetAsync(It.IsAny<Expression<Func<Product, bool>>>()))
                .ReturnsAsync(productList)
                .Verifiable();

            // Act
            var result = await _productService.CheckProductsAvailablityAsync(products);

            // Assert
            result.ShouldSatisfyAllConditions(
               result => _repositoryMock.Verify(),
               result => result.ShouldNotBeNull(),
               result => result.IsSuccess.ShouldBe(true),
               result => result.Message.ShouldBe("Products are available!"),
               result => result.StatusCode.ShouldBe(200)
               );
        }

        [Test]
        public async Task CheckProductsAvailablityAsync_InvalidProduct_ReturnsErrorResponse()
        {
            // Arrange
            var productList = new List<Product>
            {
                new Product { Id = 1, Name = "Product 1", Quantity = 10 },
                new Product { Id = 2, Name = "Product 2", Quantity = 5 },
            };

            var products = new List<ProductAvailablityModel>
            {
                new ProductAvailablityModel { Id = 1, Quantity = 5 },
                new ProductAvailablityModel { Id = 3, Quantity = 2 },
            };

            _repositoryMock.Setup(r => r.ProductRepository)
                .Returns(_productRepositoryMock.Object);

            _repositoryMock.Setup(r => r.ProductRepository.GetAsync(It.IsAny<Expression<Func<Product, bool>>>()))
                .ReturnsAsync(productList)
                .Verifiable();

            // Act
            var result = await _productService.CheckProductsAvailablityAsync(products);

            // Assert
            result.ShouldSatisfyAllConditions(
               result => _repositoryMock.Verify(),
               result => result.ShouldNotBeNull(),
               result => result.IsSuccess.ShouldBe(false),
               result => result.Message.ShouldBe("Invalid product found!"),
               result => result.StatusCode.ShouldBe(400)
               );
        }

        [Test]
        public async Task CheckProductsAvailablityAsync_ProductQuantityNotAvailable_ReturnsErrorResponse()
        {
            // Arrange
            var productList = new List<Product>
            {
                new Product { Id = 1, Name = "Product 1", Quantity = 0 },
                new Product { Id = 2, Name = "Product 2", Quantity = 5 },
            };

            var products = new List<ProductAvailablityModel>
            {
                new ProductAvailablityModel { Id = 1, Quantity = 5 },
                new ProductAvailablityModel { Id = 2, Quantity = 2 },
            };

            _repositoryMock.Setup(r => r.ProductRepository)
                .Returns(_productRepositoryMock.Object);

            _repositoryMock.Setup(r => r.ProductRepository.GetAsync(It.IsAny<Expression<Func<Product, bool>>>()))
                .ReturnsAsync(productList)
                .Verifiable();

            // Act
            var result = await _productService.CheckProductsAvailablityAsync(products);

            // Assert
            result.ShouldSatisfyAllConditions(
               result => _repositoryMock.Verify(),
               result => result.ShouldNotBeNull(),
               result => result.IsSuccess.ShouldBe(false),
               result => result.Message.ShouldBe("Product is not available!"),
               result => result.StatusCode.ShouldBe(400)
               );
        }

        [Test]
        public async Task UpdateProductsQuantityAsync_ProductsExist_UpdatesProductQuantities()
        {
            // Arrange
            var products = new List<ProductAvailablityModel>
            {
                new ProductAvailablityModel { Id = 1, Quantity = 5 },
                new ProductAvailablityModel { Id = 2, Quantity = 3 },
            };

            var productList = new List<Product>
            {
                new Product { Id = 1, Name = "Product 1", Quantity = 10 },
                new Product { Id = 2, Name = "Product 2", Quantity = 5 },
            };

            _repositoryMock.Setup(r => r.ProductRepository)
                .Returns(_productRepositoryMock.Object);

            _repositoryMock.Setup(r => r.ProductRepository.GetAsync(It.IsAny<Expression<Func<Product, bool>>>()))
                .ReturnsAsync(productList)
                .Verifiable();

            _repositoryMock.Setup(r => r.ProductRepository.Edit(It.IsAny<Product>()))
                .Verifiable();

            // Act
            await _productService.UpdateProductsQuantityAsync(products);

            // Assert
            _repositoryMock.Verify(r => r.ProductRepository.Edit(It.IsAny<Product>()), Times.Exactly(2));
            _repositoryMock.Verify(r => r.SaveAsync(), Times.Once);
            _repositoryMock.Verify();
        }

        [Test]
        public async Task UpdateProductsQuantityAsync_ProductDoesNotExist_DoesNotUpdateProductQuantity()
        {
            // Arrange
            var products = new List<ProductAvailablityModel>
            {
                new ProductAvailablityModel { Id = 1, Quantity = 5 },
            };

            var productList = new List<Product>
            {
                new Product { Id = 2, Name = "Product 2", Quantity = 5 },
            };

            _repositoryMock.Setup(r => r.ProductRepository)
                .Returns(_productRepositoryMock.Object);

            _repositoryMock.Setup(r => r.ProductRepository.GetAsync(It.IsAny<Expression<Func<Product, bool>>>()))
                .ReturnsAsync(productList)
                .Verifiable();

            // Act
            await _productService.UpdateProductsQuantityAsync(products);

            // Assert
            _repositoryMock.Verify(r => r.ProductRepository.Edit(It.IsAny<Product>()), Times.Never);
            _repositoryMock.Verify(r => r.SaveAsync(), Times.Once);
        }

        [Test]
        public async Task UpdateProductsQuantityAsync_ProductQuantityIsGreaterThanAvailableQuantity_DoesNotUpdateProductQuantity()
        {
            // Arrange
            var products = new List<ProductAvailablityModel>
            {
                new ProductAvailablityModel { Id = 1, Quantity = 15 },
            };

            var productList = new List<Product>
            {
                new Product { Id = 1, Name = "Product 1", Quantity = 10 },
            };

            _repositoryMock.Setup(r => r.ProductRepository)
                .Returns(_productRepositoryMock.Object);

            _repositoryMock.Setup(r => r.ProductRepository.GetAsync(It.IsAny<Expression<Func<Product, bool>>>()))
                .ReturnsAsync(productList)
                .Verifiable();

            // Act
            await _productService.UpdateProductsQuantityAsync(products);

            // Assert
            _repositoryMock.Verify(r => r.ProductRepository.Edit(It.IsAny<Product>()), Times.Never);
            _repositoryMock.Verify(r => r.SaveAsync(), Times.Once);
            _repositoryMock.Verify();
        }

        private Product GetProductEntity()
        {
            return new Product { Id = 1, Name = "Product 1", Quantity = 10 };
        }

        private List<Product> GetProducts()
        {
            return new List<Product>
            {
                new Product { Id = 1, Name = "Product 1", Quantity = 10 },
                new Product { Id = 2, Name = "Product 2", Quantity = 20 },
            };
        }
    }
}
