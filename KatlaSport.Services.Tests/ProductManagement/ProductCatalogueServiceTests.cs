using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KatlaSport.DataAccess.ProductCatalogue;
using KatlaSport.Services.ProductManagement;
using Moq;
using Xunit;
using DbCategory = KatlaSport.DataAccess.ProductCatalogue.ProductCategory;

namespace KatlaSport.Services.Tests.ProductManagement
{
    public class ProductCatalogueServiceTests
    {
        public ProductCatalogueServiceTests()
        {
            var x = MapperInitializer.Initialize();
        }

        [Fact]
        public void Ctor_ProductContextIsNull_ArgumentNullExceptionThrown()
        {
            var mockContext = new Mock<IProductCatalogueContext>();
            var mockContextObject = mockContext.Object;

            var exception = Assert.Throws<ArgumentNullException>(() => new ProductCatalogueService(mockContextObject, null));

            Assert.Equal(typeof(ArgumentNullException), exception.GetType());
        }

        [Fact]
        public void Ctor_UserContextIsNull_ArgumentNullExceptionThrown()
        {
            var mockContext = new Mock<IUserContext>();
            var mockContextObject = mockContext.Object;

            Assert.Throws<ArgumentNullException>(() => new ProductCatalogueService(null, mockContextObject));
        }

        [Fact]
        public async Task GetProductsAsync_EmptyCollectionRequested_ReturnsEmptyCollection()
        {
            var mockUserContext = new Mock<IUserContext>();
            var mockProductContext = new Mock<IProductCatalogueContext>();
            mockProductContext.Setup(c => c.Products).ReturnsEntitySet(new List<CatalogueProduct>());
            var service = new ProductCatalogueService(mockProductContext.Object, mockUserContext.Object);

            var list = await service.GetProductsAsync(0, 0);

            Assert.Empty(list);
        }

        [Fact]
        public async Task GetProductsAsync_TwoElementsRequested_ReturnsCollectionWithTwoElements()
        {
            var mockUserContext = new Mock<IUserContext>();
            var mockProductContext = new Mock<IProductCatalogueContext>();
            List<CatalogueProduct> productList = new List<CatalogueProduct>()
            {
                new CatalogueProduct() { Id = 1 },
                new CatalogueProduct() { Id = 2 }
            };
            mockProductContext.Setup(c => c.Products).ReturnsEntitySet(productList);
            var service = new ProductCatalogueService(mockProductContext.Object, mockUserContext.Object);

            var list = await service.GetProductsAsync(0, 2);

            Assert.Equal(2, list.Count);
        }

        [Fact]
        public async Task GetCategoryProductsAsync_IdNotPresent_RequestedResourceNotFoundExceptionThrown()
        {
            var mockUserContext = new Mock<IUserContext>();
            var mockProductContext = new Mock<IProductCatalogueContext>();
            List<CatalogueProduct> productList = new List<CatalogueProduct>()
            {
                new CatalogueProduct() { Id = 1, CategoryId = 1 },
                new CatalogueProduct() { Id = 2, CategoryId = 2 }
            };
            List<DbCategory> categoryList = new List<DbCategory>()
            {
                new DbCategory() { Id = 1 },
                new DbCategory() { Id = 2 }
            };
            mockProductContext.Setup(c => c.Products).ReturnsEntitySet(productList);
            mockProductContext.Setup(c => c.Categories).ReturnsEntitySet(categoryList);
            var service = new ProductCatalogueService(mockProductContext.Object, mockUserContext.Object);

            Assert.ThrowsAsync<RequestedResourceNotFoundException>(() => service.GetCategoryProductsAsync(0));
        }

        [Fact]
        public async Task GetCategoryProductsAsync_IdPresent_ReturnsAppropriateProducts()
        {
            var mockUserContext = new Mock<IUserContext>();
            var mockProductContext = new Mock<IProductCatalogueContext>();
            List<CatalogueProduct> productList = new List<CatalogueProduct>()
            {
                new CatalogueProduct() { Id = 1, CategoryId = 1 },
                new CatalogueProduct() { Id = 2, CategoryId = 2 }
            };
            List<DbCategory> categoryList = new List<DbCategory>()
            {
                new DbCategory() { Id = 1 },
                new DbCategory() { Id = 2 }
            };
            mockProductContext.Setup(c => c.Products).ReturnsEntitySet(productList);
            mockProductContext.Setup(c => c.Categories).ReturnsEntitySet(categoryList);
            var service = new ProductCatalogueService(mockProductContext.Object, mockUserContext.Object);

            var list = await service.GetCategoryProductsAsync(1);

            Assert.Single(list);
        }

        [Fact]
        public async Task GetProductAsync_IdNotPresent_RequestedResourceNotFoundExceptionThrown()
        {
            var mockUserContext = new Mock<IUserContext>();
            var mockProductContext = new Mock<IProductCatalogueContext>();
            List<CatalogueProduct> productList = new List<CatalogueProduct>()
            {
                new CatalogueProduct() { Id = 1, CategoryId = 1 },
                new CatalogueProduct() { Id = 2, CategoryId = 2 }
            };
            List<DbCategory> categoryList = new List<DbCategory>()
            {
                new DbCategory() { Id = 1 },
                new DbCategory() { Id = 2 }
            };
            mockProductContext.Setup(c => c.Products).ReturnsEntitySet(productList);
            mockProductContext.Setup(c => c.Categories).ReturnsEntitySet(categoryList);
            var service = new ProductCatalogueService(mockProductContext.Object, mockUserContext.Object);

            Assert.ThrowsAsync<RequestedResourceNotFoundException>(() => service.GetProductAsync(0));
        }

        [Fact]
        public async Task GetProductAsync_IdPresent_RequestedProductReturned()
        {
            var mockUserContext = new Mock<IUserContext>();
            var mockProductContext = new Mock<IProductCatalogueContext>();
            List<CatalogueProduct> productList = new List<CatalogueProduct>()
            {
                new CatalogueProduct() { Id = 1, CategoryId = 1 },
                new CatalogueProduct() { Id = 2, CategoryId = 2 }
            };
            List<DbCategory> categoryList = new List<DbCategory>()
            {
                new DbCategory() { Id = 1 },
                new DbCategory() { Id = 2 }
            };
            mockProductContext.Setup(c => c.Products).ReturnsEntitySet(productList);
            mockProductContext.Setup(c => c.Categories).ReturnsEntitySet(categoryList);
            var service = new ProductCatalogueService(mockProductContext.Object, mockUserContext.Object);

            var product = await service.GetProductAsync(1);

            Assert.Equal(1, product.Id);
        }

        [Fact]
        public async Task CreateProductAsync_CodeAlreadyPresent_RequestedResourceHasConflictExceptionThrown()
        {
            var newProduct = new UpdateProductRequest() { Code = "aa" };
            var mockUserContext = new Mock<IUserContext>();
            var mockProductContext = new Mock<IProductCatalogueContext>();
            List<CatalogueProduct> productList = new List<CatalogueProduct>()
            {
                new CatalogueProduct() { Id = 1, Code = "aa" },
                new CatalogueProduct() { Id = 2, Code = "bb" }
            };
            mockProductContext.Setup(c => c.Products).ReturnsEntitySet(productList);
            var service = new ProductCatalogueService(mockProductContext.Object, mockUserContext.Object);

            Assert.ThrowsAsync<RequestedResourceHasConflictException>(() => service.CreateProductAsync(newProduct));
        }

        [Fact]
        public async Task CreateProductAsync_UniqueCode_ProductWithSpecifiedCodeCreated()
        {
            const string newCode = "cc";
            var newProduct = new UpdateProductRequest() { Code = newCode };
            var mockUserContext = new Mock<IUserContext>();
            var mockProductContext = new Mock<IProductCatalogueContext>();
            List<CatalogueProduct> productList = new List<CatalogueProduct>()
            {
                new CatalogueProduct() { Id = 1, Code = "aa" },
                new CatalogueProduct() { Id = 2, Code = "bb" }
            };
            mockUserContext.Setup(c => c.UserId).Returns(1);
            mockProductContext.Setup(c => c.Products).ReturnsEntitySet(productList);
            var service = new ProductCatalogueService(mockProductContext.Object, mockUserContext.Object);

            var product = await service.CreateProductAsync(newProduct);

            Assert.Equal(newCode, product.Code);
        }

        [Fact]
        public async Task UpdateProductAsync_CodeAlreadyPresent_RequestedResourceHasConflictExceptionThrown()
        {
            var newProduct = new UpdateProductRequest() { Code = "bb" };
            var mockUserContext = new Mock<IUserContext>();
            var mockProductContext = new Mock<IProductCatalogueContext>();
            List<CatalogueProduct> productList = new List<CatalogueProduct>()
            {
                new CatalogueProduct() { Id = 1, Code = "aa" },
                new CatalogueProduct() { Id = 2, Code = "bb" }
            };
            mockProductContext.Setup(c => c.Products).ReturnsEntitySet(productList);
            var service = new ProductCatalogueService(mockProductContext.Object, mockUserContext.Object);

            Assert.ThrowsAsync<RequestedResourceHasConflictException>(() => service.UpdateProductAsync(1, newProduct));
        }

        [Fact]
        public async Task UpdateProductAsync_IdNotPresent_RequestedResourceNotFoundExceptionThrown()
        {
            var newProduct = new UpdateProductRequest() { Code = "bb" };
            var mockUserContext = new Mock<IUserContext>();
            var mockProductContext = new Mock<IProductCatalogueContext>();
            List<CatalogueProduct> productList = new List<CatalogueProduct>()
            {
                new CatalogueProduct() { Id = 1, Code = "aa" },
                new CatalogueProduct() { Id = 2, Code = "bb" }
            };
            mockProductContext.Setup(c => c.Products).ReturnsEntitySet(productList);
            var service = new ProductCatalogueService(mockProductContext.Object, mockUserContext.Object);

            Assert.ThrowsAsync<RequestedResourceNotFoundException>(() => service.UpdateProductAsync(3, newProduct));
        }

        [Fact]
        public async Task UpdateProductAsync_ValidRequest_ProductUpdated()
        {
            var newProduct = new UpdateProductRequest() { Code = "cc" };
            var mockUserContext = new Mock<IUserContext>();
            var mockProductContext = new Mock<IProductCatalogueContext>();
            List<CatalogueProduct> productList = new List<CatalogueProduct>()
            {
                new CatalogueProduct() { Id = 1, Code = "aa" },
                new CatalogueProduct() { Id = 2, Code = "bb" }
            };
            mockUserContext.Setup(c => c.UserId).Returns(1);
            mockProductContext.Setup(c => c.Products).ReturnsEntitySet(productList);
            var service = new ProductCatalogueService(mockProductContext.Object, mockUserContext.Object);

            var updatedProduct = await service.UpdateProductAsync(1, newProduct);

            Assert.Equal(newProduct.Code, updatedProduct.Code);
        }

        [Fact]
        public async Task DeleteProductAsync_IdNotPresent_RequestedResourceNotFoundExceptionThrown()
        {
            var mockUserContext = new Mock<IUserContext>();
            var mockProductContext = new Mock<IProductCatalogueContext>();
            List<CatalogueProduct> productList = new List<CatalogueProduct>()
            {
                new CatalogueProduct() { Id = 1 },
                new CatalogueProduct() { Id = 2 }
            };
            mockProductContext.Setup(c => c.Products).ReturnsEntitySet(productList);
            var service = new ProductCatalogueService(mockProductContext.Object, mockUserContext.Object);

            Assert.ThrowsAsync<RequestedResourceNotFoundException>(() => service.DeleteProductAsync(3));
        }

        [Fact]
        public async Task DeleteProductAsync_StatusIsNotDeleted_RequestedResourceHasConflictThrown()
        {
            var mockUserContext = new Mock<IUserContext>();
            var mockProductContext = new Mock<IProductCatalogueContext>();
            List<CatalogueProduct> productList = new List<CatalogueProduct>()
            {
                new CatalogueProduct() { Id = 1, IsDeleted = false },
                new CatalogueProduct() { Id = 2 }
            };
            mockProductContext.Setup(c => c.Products).ReturnsEntitySet(productList);
            var service = new ProductCatalogueService(mockProductContext.Object, mockUserContext.Object);

            Assert.ThrowsAsync<RequestedResourceHasConflictException>(() => service.DeleteProductAsync(1));
        }

        [Fact]
        public async Task DeleteProductAsync_IdPresent_RequestedProductDeleted()
        {
            var mockUserContext = new Mock<IUserContext>();
            var mockProductContext = new Mock<IProductCatalogueContext>();
            List<CatalogueProduct> productList = new List<CatalogueProduct>()
            {
                new CatalogueProduct() { Id = 1, IsDeleted = true },
                new CatalogueProduct() { Id = 2 }
            };
            mockProductContext.Setup(c => c.Products).ReturnsEntitySet(productList);
            var service = new ProductCatalogueService(mockProductContext.Object, mockUserContext.Object);

            await service.DeleteProductAsync(1);
            var products = await service.GetProductsAsync(0, productList.Count);

            Assert.DoesNotContain(products, h => h.Id == 1);
        }

        [Fact]
        public async Task SetStatusAsync_IdNotPresent_RequestedResourceNotFoundExceptionThrown()
        {
            var mockUserContext = new Mock<IUserContext>();
            var mockProductContext = new Mock<IProductCatalogueContext>();
            List<CatalogueProduct> productList = new List<CatalogueProduct>()
            {
                new CatalogueProduct() { Id = 1 },
                new CatalogueProduct() { Id = 2 }
            };
            mockProductContext.Setup(c => c.Products).ReturnsEntitySet(productList);
            var service = new ProductCatalogueService(mockProductContext.Object, mockUserContext.Object);

            Assert.ThrowsAsync<RequestedResourceNotFoundException>(() => service.SetStatusAsync(3, false));
        }

        [Fact]
        public async Task SetStatusAsync_IdPresent_RequestedProductStatusChanged()
        {
            var mockUserContext = new Mock<IUserContext>();
            var mockProductContext = new Mock<IProductCatalogueContext>();
            List<CatalogueProduct> productList = new List<CatalogueProduct>()
            {
                new CatalogueProduct() { Id = 1, IsDeleted = true },
                new CatalogueProduct() { Id = 2 }
            };
            mockUserContext.Setup(c => c.UserId).Returns(1);
            mockProductContext.Setup(c => c.Products).ReturnsEntitySet(productList);
            var service = new ProductCatalogueService(mockProductContext.Object, mockUserContext.Object);

            await service.SetStatusAsync(1, false);
            var product = await service.GetProductAsync(1);

            Assert.False(product.IsDeleted);
        }
    }
}
