using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KatlaSport.DataAccess.ProductStoreHive;
using KatlaSport.Services.HiveManagement;
using Moq;
using Xunit;

namespace KatlaSport.Services.Tests.HiveManagement
{
    public class HiveSectionServiceTests
    {
        public HiveSectionServiceTests()
        {
            var x = MapperInitializer.Initialize();
        }

        [Fact]
        public void Ctor_HiveContextIsNull_ArgumentNullExceptionThrown()
        {
            var mockContext = new Mock<IProductStoreHiveContext>();
            var mockContextObject = mockContext.Object;

            var exception = Assert.Throws<ArgumentNullException>(() => new HiveSectionService(mockContextObject, null));

            Assert.Equal(typeof(ArgumentNullException), exception.GetType());
        }

        [Fact]
        public void Ctor_UserContextIsNull_ArgumentNullExceptionThrown()
        {
            var mockContext = new Mock<IUserContext>();
            var mockContextObject = mockContext.Object;

            Assert.Throws<ArgumentNullException>(() => new HiveSectionService(null, mockContextObject));
        }

        [Fact]
        public async Task GetHiveSectionsAsync_EmptyCollection_ReturnsEmptyCollection()
        {
            var mockUserContext = new Mock<IUserContext>();
            var mockHiveContext = new Mock<IProductStoreHiveContext>();
            mockHiveContext.Setup(c => c.Sections).ReturnsEntitySet(new List<StoreHiveSection>());
            mockHiveContext.Setup(c => c.Hives).ReturnsEntitySet(new List<StoreHive>());
            var service = new HiveSectionService(mockHiveContext.Object, mockUserContext.Object);

            var list = await service.GetHiveSectionsAsync();

            Assert.Empty(list);
        }

        [Fact]
        public async Task GetHiveSectionsAsync_CollectionWithTwoElements_ReturnsCollectionWithTwoElements()
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
            var service = new HiveSectionService(mockHiveContext.Object, mockUserContext.Object);

            var list = await service.GetHiveSectionsAsync();

            Assert.Equal(2, list.Count);
        }

        [Fact]
        public async Task GetHiveSectionAsync_IdNotPresent_RequestedResourceNotFoundExceptionThrown()
        {
            var mockUserContext = new Mock<IUserContext>();
            var mockHiveContext = new Mock<IProductStoreHiveContext>();
            List<StoreHiveSection> hiveSectionsList = new List<StoreHiveSection>()
            {
                new StoreHiveSection() { Id = 1 },
                new StoreHiveSection() { Id = 2 }
            };
            mockHiveContext.Setup(c => c.Sections).ReturnsEntitySet(hiveSectionsList);
            var service = new HiveSectionService(mockHiveContext.Object, mockUserContext.Object);

            Assert.ThrowsAsync<RequestedResourceNotFoundException>(() => service.GetHiveSectionAsync(0));
        }

        [Fact]
        public async Task GetHiveSectionAsync_IdPresent_RequestedHiveSectionReturned()
        {
            var mockUserContext = new Mock<IUserContext>();
            var mockHiveContext = new Mock<IProductStoreHiveContext>();
            List<StoreHiveSection> hiveSectionsList = new List<StoreHiveSection>()
            {
                new StoreHiveSection() { Id = 1 },
                new StoreHiveSection() { Id = 2 }
            };
            mockHiveContext.Setup(c => c.Sections).ReturnsEntitySet(hiveSectionsList);
            var service = new HiveSectionService(mockHiveContext.Object, mockUserContext.Object);

            var hiveSection = await service.GetHiveSectionAsync(1);

            Assert.Equal(1, hiveSection.Id);
        }

        [Fact]
        public async Task GetHiveSectionsAsync_IdNotPresent_ReturnsEmptyCollection()
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
            var service = new HiveSectionService(mockHiveContext.Object, mockUserContext.Object);

            var list = await service.GetHiveSectionsAsync(3);

            Assert.Empty(list);
        }

        [Fact]
        public async Task GetHiveSectionsAsync_IdPresent_ReturnsAppropriateItems()
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
            var service = new HiveSectionService(mockHiveContext.Object, mockUserContext.Object);

            var list = await service.GetHiveSectionsAsync(1);

            Assert.Single(list);
        }

        [Fact]
        public async Task CreateHiveSectionAsync_CodeAlreadyPresent_RequestedResourceHasConflictExceptionThrown()
        {
            var newHiveSection = new UpdateHiveSectionRequest() { Code = "aa" };
            var mockUserContext = new Mock<IUserContext>();
            var mockHiveContext = new Mock<IProductStoreHiveContext>();
            List<StoreHiveSection> hiveSectionsList = new List<StoreHiveSection>()
            {
                new StoreHiveSection() { Id = 1, Code = "aa" },
                new StoreHiveSection() { Id = 2, Code = "bb" }
            };
            mockHiveContext.Setup(c => c.Sections).ReturnsEntitySet(hiveSectionsList);
            var service = new HiveSectionService(mockHiveContext.Object, mockUserContext.Object);

            Assert.ThrowsAsync<RequestedResourceHasConflictException>(() => service.CreateHiveSectionAsync(1, newHiveSection));
        }

        [Fact]
        public async Task CreateHiveSectionAsync_UniqueCode_HiveSectionWithSpecifiedCodeCreated()
        {
            const string newCode = "cc";
            var newHiveSection = new UpdateHiveSectionRequest() { Code = newCode };
            var mockUserContext = new Mock<IUserContext>();
            var mockHiveContext = new Mock<IProductStoreHiveContext>();
            List<StoreHiveSection> hiveSectionsList = new List<StoreHiveSection>()
            {
                new StoreHiveSection() { Id = 1, Code = "aa" },
                new StoreHiveSection() { Id = 2, Code = "bb" }
            };
            mockHiveContext.Setup(c => c.Sections).ReturnsEntitySet(hiveSectionsList);
            var service = new HiveSectionService(mockHiveContext.Object, mockUserContext.Object);

            var hive = await service.CreateHiveSectionAsync(1, newHiveSection);

            Assert.Equal(newCode, hive.Code);
        }

        [Fact]
        public async Task UpdateHiveSectionAsync_CodeAlreadyPresent_RequestedResourceHasConflictExceptionThrown()
        {
            var newHiveSection = new UpdateHiveSectionRequest() { Code = "bb" };
            var mockUserContext = new Mock<IUserContext>();
            var mockHiveContext = new Mock<IProductStoreHiveContext>();
            List<StoreHiveSection> hiveSectionsList = new List<StoreHiveSection>()
            {
                new StoreHiveSection() { Id = 1, Code = "aa" },
                new StoreHiveSection() { Id = 2, Code = "bb" }
            };
            mockHiveContext.Setup(c => c.Sections).ReturnsEntitySet(hiveSectionsList);
            var service = new HiveSectionService(mockHiveContext.Object, mockUserContext.Object);

            Assert.ThrowsAsync<RequestedResourceHasConflictException>(() => service.UpdateHiveSectionAsync(1, newHiveSection));
        }

        [Fact]
        public async Task UpdateHiveSectionAsync_IdNotPresent_RequestedResourceNotFoundExceptionThrown()
        {
            var newHiveSection = new UpdateHiveSectionRequest() { Code = "aa" };
            var mockUserContext = new Mock<IUserContext>();
            var mockHiveContext = new Mock<IProductStoreHiveContext>();
            List<StoreHiveSection> hiveSectionsList = new List<StoreHiveSection>()
            {
                new StoreHiveSection() { Id = 1, Code = "aa" },
                new StoreHiveSection() { Id = 2, Code = "bb" }
            };
            mockHiveContext.Setup(c => c.Sections).ReturnsEntitySet(hiveSectionsList);
            var service = new HiveSectionService(mockHiveContext.Object, mockUserContext.Object);

            Assert.ThrowsAsync<RequestedResourceNotFoundException>(() => service.UpdateHiveSectionAsync(3, newHiveSection));
        }

        [Fact]
        public async Task UpdateHiveSectionAsync_ValidRequest_HiveUpdated()
        {
            var newHiveSection = new UpdateHiveSectionRequest() { Code = "cc" };
            var mockUserContext = new Mock<IUserContext>();
            var mockHiveContext = new Mock<IProductStoreHiveContext>();
            List<StoreHiveSection> hiveSectionsList = new List<StoreHiveSection>()
            {
                new StoreHiveSection() { Id = 1, Code = "aa" },
                new StoreHiveSection() { Id = 2, Code = "bb" }
            };
            mockHiveContext.Setup(c => c.Sections).ReturnsEntitySet(hiveSectionsList);
            var service = new HiveSectionService(mockHiveContext.Object, mockUserContext.Object);

            var updatedHiveSection = await service.UpdateHiveSectionAsync(1, newHiveSection);

            Assert.Equal(newHiveSection.Code, updatedHiveSection.Code);
        }

        [Fact]
        public async Task DeleteHiveSectionAsync_IdNotPresent_RequestedResourceNotFoundExceptionThrown()
        {
            var mockUserContext = new Mock<IUserContext>();
            var mockHiveContext = new Mock<IProductStoreHiveContext>();
            List<StoreHiveSection> hiveSectionsList = new List<StoreHiveSection>()
            {
                new StoreHiveSection() { Id = 1, Code = "aa" },
                new StoreHiveSection() { Id = 2, Code = "bb" }
            };
            mockHiveContext.Setup(c => c.Sections).ReturnsEntitySet(hiveSectionsList);
            var service = new HiveSectionService(mockHiveContext.Object, mockUserContext.Object);

            Assert.ThrowsAsync<RequestedResourceNotFoundException>(() => service.DeleteHiveSectionAsync(3));
        }

        [Fact]
        public async Task DeleteHiveSectionAsync_StatusIsNotDeleted_RequestedResourceHasConflictThrown()
        {
            var mockUserContext = new Mock<IUserContext>();
            var mockHiveContext = new Mock<IProductStoreHiveContext>();
            List<StoreHiveSection> hiveSectionsList = new List<StoreHiveSection>()
            {
                new StoreHiveSection() { Id = 1, IsDeleted = false, Code = "aa" },
                new StoreHiveSection() { Id = 2, Code = "bb" }
            };
            mockHiveContext.Setup(c => c.Sections).ReturnsEntitySet(hiveSectionsList);
            var service = new HiveSectionService(mockHiveContext.Object, mockUserContext.Object);

            Assert.ThrowsAsync<RequestedResourceHasConflictException>(() => service.DeleteHiveSectionAsync(1));
        }

        [Fact]
        public async Task DeleteHiveSectionAsync_IdPresent_RequestedHiveDeleted()
        {
            var mockUserContext = new Mock<IUserContext>();
            var mockHiveContext = new Mock<IProductStoreHiveContext>();
            List<StoreHiveSection> sectionList = new List<StoreHiveSection>()
            {
                new StoreHiveSection() { Id = 1, IsDeleted = true },
                new StoreHiveSection() { Id = 2 }
            };
            mockHiveContext.Setup(c => c.Sections).ReturnsEntitySet(sectionList);
            var service = new HiveSectionService(mockHiveContext.Object, mockUserContext.Object);

            await service.DeleteHiveSectionAsync(1);
            var hiveSections = await service.GetHiveSectionsAsync();

            Assert.DoesNotContain(hiveSections, h => h.Id == 1);
        }

        [Fact]
        public async Task SetStatusAsync_IdNotPresent_RequestedResourceNotFoundExceptionThrown()
        {
            var mockUserContext = new Mock<IUserContext>();
            var mockHiveContext = new Mock<IProductStoreHiveContext>();
            List<StoreHiveSection> hiveSectionsList = new List<StoreHiveSection>()
            {
                new StoreHiveSection() { Id = 1, Code = "aa" },
                new StoreHiveSection() { Id = 2, Code = "bb" }
            };
            mockHiveContext.Setup(c => c.Sections).ReturnsEntitySet(hiveSectionsList);
            var service = new HiveSectionService(mockHiveContext.Object, mockUserContext.Object);

            Assert.ThrowsAsync<RequestedResourceNotFoundException>(() => service.SetStatusAsync(3, false));
        }

        [Fact]
        public async Task SetStatusAsync_IdPresent_RequestedHiveStatusChanged()
        {
            var mockUserContext = new Mock<IUserContext>();
            var mockHiveContext = new Mock<IProductStoreHiveContext>();
            List<StoreHiveSection> hiveSectionsList = new List<StoreHiveSection>()
            {
                new StoreHiveSection() { Id = 1, Code = "aa" },
                new StoreHiveSection() { Id = 2, Code = "bb" }
            };
            mockHiveContext.Setup(c => c.Sections).ReturnsEntitySet(hiveSectionsList);
            var service = new HiveSectionService(mockHiveContext.Object, mockUserContext.Object);

            await service.SetStatusAsync(1, false);
            var hive = await service.GetHiveSectionAsync(1);

            Assert.False(hive.IsDeleted);
        }
    }
}
