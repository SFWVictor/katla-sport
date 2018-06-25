using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KatlaSport.DataAccess.ProductStoreHive;
using KatlaSport.Services.HiveManagement;
using Moq;
using Xunit;

namespace KatlaSport.Services.Tests.HiveManagement
{
    public class HiveServiceTests
    {
        public HiveServiceTests()
        {
            var x = MapperInitializer.Initialize();
        }

        [Fact]
        public void Ctor_HiveContextIsNull_ArgumentNullExceptionThrown()
        {
            var mockContext = new Mock<IProductStoreHiveContext>();
            var mockContextObject = mockContext.Object;

            var exception = Assert.Throws<ArgumentNullException>(() => new HiveService(mockContextObject, null));

            Assert.Equal(typeof(ArgumentNullException), exception.GetType());
        }

        [Fact]
        public void Ctor_UserContextIsNull_ArgumentNullExceptionThrown()
        {
            var mockContext = new Mock<IUserContext>();
            var mockContextObject = mockContext.Object;

            Assert.Throws<ArgumentNullException>(() => new HiveService(null, mockContextObject));
        }

        [Fact]
        public async Task GetHivesAsync_EmptyCollection_ReturnsEmptyCollection()
        {
            var mockUserContext = new Mock<IUserContext>();
            var mockHiveContext = new Mock<IProductStoreHiveContext>();
            mockHiveContext.Setup(c => c.Sections).ReturnsEntitySet(new List<StoreHiveSection>());
            mockHiveContext.Setup(c => c.Hives).ReturnsEntitySet(new List<StoreHive>());
            var service = new HiveService(mockHiveContext.Object, mockUserContext.Object);

            var list = await service.GetHivesAsync();

            Assert.Empty(list);
        }

        [Fact]
        public async Task GetHivesAsync_CollectionWithTwoElements_ReturnsCollectionWithTwoElements()
        {
            var mockUserContext = new Mock<IUserContext>();
            var mockHiveContext = new Mock<IProductStoreHiveContext>();
            List<StoreHive> hiveList = new List<StoreHive>()
            {
                new StoreHive() { Id = 1 },
                new StoreHive() { Id = 2 }
            };
            List<StoreHiveSection> sectionList = new List<StoreHiveSection>()
            {
                new StoreHiveSection() { StoreHiveId = 1 },
                new StoreHiveSection() { StoreHiveId = 2 }
            };
            mockHiveContext.Setup(c => c.Hives).ReturnsEntitySet(hiveList);
            mockHiveContext.Setup(c => c.Sections).ReturnsEntitySet(sectionList);
            var service = new HiveService(mockHiveContext.Object, mockUserContext.Object);

            var list = await service.GetHivesAsync();

            Assert.Equal(2, list.Count);
        }

        [Fact]
        public async Task GetHiveAsync_IdNotPresent_RequestedResourceNotFoundExceptionThrown()
        {
            var mockUserContext = new Mock<IUserContext>();
            var mockHiveContext = new Mock<IProductStoreHiveContext>();
            List<StoreHive> hiveList = new List<StoreHive>()
            {
                new StoreHive() { Id = 1 },
                new StoreHive() { Id = 2 }
            };
            mockHiveContext.Setup(c => c.Hives).ReturnsEntitySet(hiveList);
            var service = new HiveService(mockHiveContext.Object, mockUserContext.Object);

            Assert.ThrowsAsync<RequestedResourceNotFoundException>(() => service.GetHiveAsync(0));
        }

        [Fact]
        public async Task GetHiveAsync_IdPresent_RequestedHiveReturned()
        {
            var mockUserContext = new Mock<IUserContext>();
            var mockHiveContext = new Mock<IProductStoreHiveContext>();
            List<StoreHive> hiveList = new List<StoreHive>()
            {
                new StoreHive() { Id = 1 },
                new StoreHive() { Id = 2 }
            };
            mockHiveContext.Setup(c => c.Hives).ReturnsEntitySet(hiveList);
            var service = new HiveService(mockHiveContext.Object, mockUserContext.Object);

            var hive = await service.GetHiveAsync(1);

            Assert.Equal(1, hive.Id);
        }

        [Fact]
        public async Task CreateHiveAsync_CodeAlreadyPresent_RequestedResourceHasConflictExceptionThrown()
        {
            var newHive = new UpdateHiveRequest() { Code = "aa" };
            var mockUserContext = new Mock<IUserContext>();
            var mockHiveContext = new Mock<IProductStoreHiveContext>();
            List<StoreHive> hiveList = new List<StoreHive>()
            {
                new StoreHive() { Id = 1, Code = "aa" },
                new StoreHive() { Id = 2, Code = "bb" }
            };
            mockHiveContext.Setup(c => c.Hives).ReturnsEntitySet(hiveList);
            var service = new HiveService(mockHiveContext.Object, mockUserContext.Object);

            Assert.ThrowsAsync<RequestedResourceHasConflictException>(() => service.CreateHiveAsync(newHive));
        }

        [Fact]
        public async Task CreateHiveAsync_UniqueCode_HiveWithSpecifiedCodeCreated()
        {
            const string newCode = "cc";
            var newHive = new UpdateHiveRequest() { Code = newCode };
            var mockUserContext = new Mock<IUserContext>();
            var mockHiveContext = new Mock<IProductStoreHiveContext>();
            List<StoreHive> hiveList = new List<StoreHive>()
            {
                new StoreHive() { Id = 1, Code = "aa" },
                new StoreHive() { Id = 2, Code = "bb" }
            };
            mockHiveContext.Setup(c => c.Hives).ReturnsEntitySet(hiveList);
            var service = new HiveService(mockHiveContext.Object, mockUserContext.Object);

            var hive = await service.CreateHiveAsync(newHive);

            Assert.Equal(newCode, hive.Code);
        }

        [Fact]
        public async Task UpdateHiveAsync_CodeAlreadyPresent_RequestedResourceHasConflictExceptionThrown()
        {
            var newHive = new UpdateHiveRequest() { Code = "bb" };
            var mockUserContext = new Mock<IUserContext>();
            var mockHiveContext = new Mock<IProductStoreHiveContext>();
            List<StoreHive> hiveList = new List<StoreHive>()
            {
                new StoreHive() { Id = 1, Code = "aa" },
                new StoreHive() { Id = 2, Code = "bb" }
            };
            mockHiveContext.Setup(c => c.Hives).ReturnsEntitySet(hiveList);
            var service = new HiveService(mockHiveContext.Object, mockUserContext.Object);

            Assert.ThrowsAsync<RequestedResourceHasConflictException>(() => service.UpdateHiveAsync(1, newHive));
        }

        [Fact]
        public async Task UpdateHiveAsync_IdNotPresent_RequestedResourceNotFoundExceptionThrown()
        {
            var newHive = new UpdateHiveRequest() { Code = "bb" };
            var mockUserContext = new Mock<IUserContext>();
            var mockHiveContext = new Mock<IProductStoreHiveContext>();
            List<StoreHive> hiveList = new List<StoreHive>()
            {
                new StoreHive() { Id = 1, Code = "aa" },
                new StoreHive() { Id = 2, Code = "bb" }
            };
            mockHiveContext.Setup(c => c.Hives).ReturnsEntitySet(hiveList);
            var service = new HiveService(mockHiveContext.Object, mockUserContext.Object);

            Assert.ThrowsAsync<RequestedResourceNotFoundException>(() => service.UpdateHiveAsync(3, newHive));
        }

        [Fact]
        public async Task UpdateHiveAsync_ValidRequest_HiveUpdated()
        {
            var newHive = new UpdateHiveRequest() { Code = "cc" };
            var mockUserContext = new Mock<IUserContext>();
            var mockHiveContext = new Mock<IProductStoreHiveContext>();
            List<StoreHive> hiveList = new List<StoreHive>()
            {
                new StoreHive() { Id = 1, Code = "aa" },
                new StoreHive() { Id = 2, Code = "bb" }
            };
            mockHiveContext.Setup(c => c.Hives).ReturnsEntitySet(hiveList);
            var service = new HiveService(mockHiveContext.Object, mockUserContext.Object);

            var updatedHive = await service.UpdateHiveAsync(1, newHive);

            Assert.Equal(newHive.Code, updatedHive.Code);
        }

        [Fact]
        public async Task DeleteHiveAsync_IdNotPresent_RequestedResourceNotFoundExceptionThrown()
        {
            var mockUserContext = new Mock<IUserContext>();
            var mockHiveContext = new Mock<IProductStoreHiveContext>();
            List<StoreHive> hiveList = new List<StoreHive>()
            {
                new StoreHive() { Id = 1 },
                new StoreHive() { Id = 2 }
            };
            mockHiveContext.Setup(c => c.Hives).ReturnsEntitySet(hiveList);
            var service = new HiveService(mockHiveContext.Object, mockUserContext.Object);

            Assert.ThrowsAsync<RequestedResourceNotFoundException>(() => service.DeleteHiveAsync(3));
        }

        [Fact]
        public async Task DeleteHiveAsync_StatusIsNotDeleted_RequestedResourceHasConflictThrown()
        {
            var mockUserContext = new Mock<IUserContext>();
            var mockHiveContext = new Mock<IProductStoreHiveContext>();
            List<StoreHive> hiveList = new List<StoreHive>()
            {
                new StoreHive() { Id = 1, IsDeleted = false },
                new StoreHive() { Id = 2 }
            };
            mockHiveContext.Setup(c => c.Hives).ReturnsEntitySet(hiveList);
            var service = new HiveService(mockHiveContext.Object, mockUserContext.Object);

            Assert.ThrowsAsync<RequestedResourceHasConflictException>(() => service.DeleteHiveAsync(1));
        }

        [Fact]
        public async Task DeleteHiveAsync_IdPresent_RequestedHiveDeleted()
        {
            var mockUserContext = new Mock<IUserContext>();
            var mockHiveContext = new Mock<IProductStoreHiveContext>();
            List<StoreHive> hiveList = new List<StoreHive>()
            {
                new StoreHive() { Id = 1, IsDeleted = true },
                new StoreHive() { Id = 2 }
            };
            List<StoreHiveSection> sectionList = new List<StoreHiveSection>()
            {
                new StoreHiveSection() { StoreHiveId = 1 },
                new StoreHiveSection() { StoreHiveId = 2 }
            };
            mockHiveContext.Setup(c => c.Hives).ReturnsEntitySet(hiveList);
            mockHiveContext.Setup(c => c.Sections).ReturnsEntitySet(sectionList);
            var service = new HiveService(mockHiveContext.Object, mockUserContext.Object);

            await service.DeleteHiveAsync(1);
            var hives = await service.GetHivesAsync();

            Assert.DoesNotContain(hives, h => h.Id == 1);
        }

        [Fact]
        public async Task SetStatusAsync_IdNotPresent_RequestedResourceNotFoundExceptionThrown()
        {
            var mockUserContext = new Mock<IUserContext>();
            var mockHiveContext = new Mock<IProductStoreHiveContext>();
            List<StoreHive> hiveList = new List<StoreHive>()
            {
                new StoreHive() { Id = 1 },
                new StoreHive() { Id = 2 }
            };
            mockHiveContext.Setup(c => c.Hives).ReturnsEntitySet(hiveList);
            var service = new HiveService(mockHiveContext.Object, mockUserContext.Object);

            Assert.ThrowsAsync<RequestedResourceNotFoundException>(() => service.SetStatusAsync(3, false));
        }

        [Fact]
        public async Task SetStatusAsync_IdPresent_RequestedHiveStatusChanged()
        {
            var mockUserContext = new Mock<IUserContext>();
            var mockHiveContext = new Mock<IProductStoreHiveContext>();
            List<StoreHive> hiveList = new List<StoreHive>()
            {
                new StoreHive() { Id = 1, IsDeleted = true },
                new StoreHive() { Id = 2 }
            };
            mockHiveContext.Setup(c => c.Hives).ReturnsEntitySet(hiveList);
            var service = new HiveService(mockHiveContext.Object, mockUserContext.Object);

            await service.SetStatusAsync(1, false);
            var hive = await service.GetHiveAsync(1);

            Assert.False(hive.IsDeleted);
        }
    }
}
