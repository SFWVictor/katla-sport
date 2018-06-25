using System;
using KatlaSport.DataAccess.ProductCatalogue;
using KatlaSport.Services.CatalogueManagement;
using Moq;
using Xunit;

namespace KatlaSport.Services.Tests.CatalogueManagement
{
    public class CatalogueManagementServiceTests
    {
        [Fact]
        public void Ctor_ContextIsNull_ThrowsArgumentNullException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new CatalogueManagementService(null));

            Assert.Equal(typeof(ArgumentNullException), ex.GetType());
        }

        [Fact]
        public void GetProductCategories_EmptyCollection_ReturnsEmptyCollection()
        {
            var context = new Mock<IProductCatalogueContext>();
            context.Setup(c => c.Categories).ReturnsEntitySet(new ProductCategory[0]);
            var service = new CatalogueManagementService(context.Object);

            var categories = service.GetProductCategories();

            Assert.Equal(0, categories.Count);
        }

        [Fact]
        public void GetProductCategories_CollectionWithOneElement_ReturnsCollectionWithOneElement()
        {
            var context = new Mock<IProductCatalogueContext>();
            int id = 1;
            var initialCategories = new ProductCategory[1] { new ProductCategory() { Id = id } };
            context.Setup(c => c.Categories).ReturnsEntitySet(initialCategories);
            var service = new CatalogueManagementService(context.Object);

            var categories = service.GetProductCategories();

            Assert.Equal(id, categories.Count);
            Assert.Contains(categories, c => c.Id == id);
        }

        [Fact]
        public void AddProductCategoryTest()
        {
            var context = new Mock<IProductCatalogueContext>();
            var service = new CatalogueManagementService(context.Object);

            service.AddProductCategory();
        }
    }
}
