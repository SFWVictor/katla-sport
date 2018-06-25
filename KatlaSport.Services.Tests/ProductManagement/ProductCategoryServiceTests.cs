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
    public class ProductCategoryServiceTests
    {
        public ProductCategoryServiceTests()
        {
            var x = MapperInitializer.Initialize();
        }

        [Fact]
        public void Ctor_ProductContextIsNull_ArgumentNullExceptionThrown()
        {
            var mockContext = new Mock<IProductCatalogueContext>();
            var mockContextObject = mockContext.Object;

            var exception = Assert.Throws<ArgumentNullException>(() => new ProductCategoryService(mockContextObject, null));

            Assert.Equal(typeof(ArgumentNullException), exception.GetType());
        }

        [Fact]
        public void Ctor_UserContextIsNull_ArgumentNullExceptionThrown()
        {
            var mockContext = new Mock<IUserContext>();
            var mockContextObject = mockContext.Object;

            Assert.Throws<ArgumentNullException>(() => new ProductCategoryService(null, mockContextObject));
        }

        [Fact]
        public async Task GetCategoriesAsync_EmptyCollectionRequested_ReturnsEmptyCollection()
        {
            var mockUserContext = new Mock<IUserContext>();
            var mockProductContext = new Mock<IProductCatalogueContext>();
            mockProductContext.Setup(c => c.Products).ReturnsEntitySet(new List<CatalogueProduct>());
            mockProductContext.Setup(c => c.Categories).ReturnsEntitySet(new List<DbCategory>());
            var service = new ProductCategoryService(mockProductContext.Object, mockUserContext.Object);

            var list = await service.GetCategoriesAsync(0, 0);

            Assert.Empty(list);
        }

        [Fact]
        public async Task GetCategoriesAsync_TwoElementsRequested_ReturnsCollectionWithTwoElements()
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
            var service = new ProductCategoryService(mockProductContext.Object, mockUserContext.Object);

            var list = await service.GetCategoriesAsync(0, 2);

            Assert.Equal(2, list.Count);
        }

        [Fact]
        public async Task GetCategoryAsync_IdNotPresent_RequestedResourceNotFoundExceptionThrown()
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
            var service = new ProductCategoryService(mockProductContext.Object, mockUserContext.Object);

            Assert.ThrowsAsync<RequestedResourceNotFoundException>(() => service.GetCategoryAsync(0));
        }

        [Fact]
        public async Task GetCategoryAsync_IdPresent_RequestedCategoryReturned()
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
            var service = new ProductCategoryService(mockProductContext.Object, mockUserContext.Object);

            var category = await service.GetCategoryAsync(1);

            Assert.Equal(1, category.Id);
        }

        [Fact]
        public async Task CreateCategoryAsync_CodeAlreadyPresent_RequestedResourceHasConflictExceptionThrown()
        {
            var newCategory = new UpdateProductCategoryRequest() { Code = "aa" };
            var mockUserContext = new Mock<IUserContext>();
            var mockProductContext = new Mock<IProductCatalogueContext>();
            List<DbCategory> categoryList = new List<DbCategory>()
            {
                new DbCategory() { Id = 1 },
                new DbCategory() { Id = 2 }
            };
            mockProductContext.Setup(c => c.Categories).ReturnsEntitySet(categoryList);
            var service = new ProductCategoryService(mockProductContext.Object, mockUserContext.Object);

            Assert.ThrowsAsync<RequestedResourceHasConflictException>(() => service.CreateCategoryAsync(newCategory));
        }

        [Fact]
        public async Task CreateCategoryAsync_UniqueCode_ProductWithSpecifiedCodeCreated()
        {
            const string newCode = "cc";
            var newCategory = new UpdateProductCategoryRequest() { Code = newCode };
            var mockUserContext = new Mock<IUserContext>();
            var mockProductContext = new Mock<IProductCatalogueContext>();
            List<DbCategory> categoryList = new List<DbCategory>()
            {
                new DbCategory() { Id = 1 },
                new DbCategory() { Id = 2 }
            };
            mockProductContext.Setup(c => c.Categories).ReturnsEntitySet(categoryList);
            mockUserContext.Setup(c => c.UserId).Returns(1);
            var service = new ProductCategoryService(mockProductContext.Object, mockUserContext.Object);

            var product = await service.CreateCategoryAsync(newCategory);

            Assert.Equal(newCode, product.Code);
        }

        [Fact]
        public async Task UpdateCategoryAsync_CodeAlreadyPresent_RequestedResourceHasConflictExceptionThrown()
        {
            var newCategory = new UpdateProductCategoryRequest() { Code = "bb" };
            var mockUserContext = new Mock<IUserContext>();
            var mockProductContext = new Mock<IProductCatalogueContext>();
            List<DbCategory> categoryList = new List<DbCategory>()
            {
                new DbCategory() { Id = 1 },
                new DbCategory() { Id = 2 }
            };
            mockProductContext.Setup(c => c.Categories).ReturnsEntitySet(categoryList);
            var service = new ProductCategoryService(mockProductContext.Object, mockUserContext.Object);

            Assert.ThrowsAsync<RequestedResourceHasConflictException>(() => service.UpdateCategoryAsync(1, newCategory));
        }

        [Fact]
        public async Task UpdateCategoryAsync_IdNotPresent_RequestedResourceNotFoundExceptionThrown()
        {
            var newCategory = new UpdateProductCategoryRequest() { Code = "bb" };
            var mockUserContext = new Mock<IUserContext>();
            var mockProductContext = new Mock<IProductCatalogueContext>();
            List<DbCategory> categoryList = new List<DbCategory>()
            {
                new DbCategory() { Id = 1 },
                new DbCategory() { Id = 2 }
            };
            mockProductContext.Setup(c => c.Categories).ReturnsEntitySet(categoryList);
            var service = new ProductCategoryService(mockProductContext.Object, mockUserContext.Object);

            Assert.ThrowsAsync<RequestedResourceNotFoundException>(() => service.UpdateCategoryAsync(3, newCategory));
        }

        [Fact]
        public async Task UpdateCategoryAsync_ValidRequest_ProductUpdated()
        {
            var newCategory = new UpdateProductCategoryRequest() { Code = "cc" };
            var mockUserContext = new Mock<IUserContext>();
            var mockProductContext = new Mock<IProductCatalogueContext>();
            List<DbCategory> categoryList = new List<DbCategory>()
            {
                new DbCategory() { Id = 1 },
                new DbCategory() { Id = 2 }
            };
            mockProductContext.Setup(c => c.Categories).ReturnsEntitySet(categoryList);
            mockUserContext.Setup(c => c.UserId).Returns(1);
            var service = new ProductCategoryService(mockProductContext.Object, mockUserContext.Object);

            var updatedProduct = await service.UpdateCategoryAsync(1, newCategory);

            Assert.Equal(newCategory.Code, updatedProduct.Code);
        }

        [Fact]
        public async Task DeleteCategoryAsync_IdNotPresent_RequestedResourceNotFoundExceptionThrown()
        {
            var mockUserContext = new Mock<IUserContext>();
            var mockProductContext = new Mock<IProductCatalogueContext>();
            List<DbCategory> categoryList = new List<DbCategory>()
            {
                new DbCategory() { Id = 1 },
                new DbCategory() { Id = 2 }
            };
            mockProductContext.Setup(c => c.Categories).ReturnsEntitySet(categoryList);
            var service = new ProductCategoryService(mockProductContext.Object, mockUserContext.Object);

            Assert.ThrowsAsync<RequestedResourceNotFoundException>(() => service.DeleteCategoryAsync(3));
        }

        [Fact]
        public async Task DeleteCategoryAsync_StatusIsNotDeleted_RequestedResourceHasConflictThrown()
        {
            var mockUserContext = new Mock<IUserContext>();
            var mockProductContext = new Mock<IProductCatalogueContext>();
            List<DbCategory> categoryList = new List<DbCategory>()
            {
                new DbCategory() { Id = 1, IsDeleted = false },
                new DbCategory() { Id = 2 }
            };
            mockProductContext.Setup(c => c.Categories).ReturnsEntitySet(categoryList);
            var service = new ProductCategoryService(mockProductContext.Object, mockUserContext.Object);

            Assert.ThrowsAsync<RequestedResourceHasConflictException>(() => service.DeleteCategoryAsync(1));
        }

        [Fact]
        public async Task DeleteCategoryAsync_IdPresent_RequestedProductDeleted()
        {
            var mockUserContext = new Mock<IUserContext>();
            var mockProductContext = new Mock<IProductCatalogueContext>();
            List<DbCategory> categoryList = new List<DbCategory>()
            {
                new DbCategory() { Id = 1, IsDeleted = true },
                new DbCategory() { Id = 2 }
            };
            mockProductContext.Setup(c => c.Categories).ReturnsEntitySet(categoryList);
            mockProductContext.Setup(c => c.Products).ReturnsEntitySet(new List<CatalogueProduct>());
            var service = new ProductCategoryService(mockProductContext.Object, mockUserContext.Object);

            await service.DeleteCategoryAsync(1);
            var categories = await service.GetCategoriesAsync(0, categoryList.Count);

            Assert.DoesNotContain(categories, h => h.Id == 1);
        }

        [Fact]
        public async Task SetStatusAsync_IdNotPresent_RequestedResourceNotFoundExceptionThrown()
        {
            var mockUserContext = new Mock<IUserContext>();
            var mockProductContext = new Mock<IProductCatalogueContext>();
            List<DbCategory> categoryList = new List<DbCategory>()
            {
                new DbCategory() { Id = 1 },
                new DbCategory() { Id = 2 }
            };
            mockProductContext.Setup(c => c.Categories).ReturnsEntitySet(categoryList);
            var service = new ProductCategoryService(mockProductContext.Object, mockUserContext.Object);

            Assert.ThrowsAsync<RequestedResourceNotFoundException>(() => service.SetStatusAsync(3, false));
        }

        [Fact]
        public async Task SetStatusAsync_IdPresent_RequestedProductStatusChanged()
        {
            var mockUserContext = new Mock<IUserContext>();
            var mockProductContext = new Mock<IProductCatalogueContext>();
            List<DbCategory> categoryList = new List<DbCategory>()
            {
                new DbCategory() { Id = 1 },
                new DbCategory() { Id = 2 }
            };
            mockProductContext.Setup(c => c.Categories).ReturnsEntitySet(categoryList);
            mockUserContext.Setup(c => c.UserId).Returns(1);
            var service = new ProductCategoryService(mockProductContext.Object, mockUserContext.Object);

            await service.SetStatusAsync(1, false);
            var product = await service.GetCategoryAsync(1);

            Assert.False(product.IsDeleted);
        }
    }
}
