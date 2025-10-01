using Application.Services.Implementations;
using Infrastructure.Dtos;
using Infrastructure.Models;
using Infrastructure.Repositories.Interfaces;
using Infrastructure.Enums;
using Moq;

namespace Tests.Unit
{
    public class ResourceServiceTests
    {
        private readonly Mock<IResourceRepository> _repoMock;
        private readonly ResourceService _service;

        public ResourceServiceTests()
        {
            _repoMock = new Mock<IResourceRepository>();
            _service = new ResourceService(_repoMock.Object);
        }

        [Fact]
        public async Task FilterResources_ReturnsMappedDtosAndTotal()
        {
            var models = new List<ResourceModel>
            {
                new ResourceModel { ResourceId = 1, Name = "El Padrino", Author = "Coppola", Description = "Película clásica", Publication = 1972, ResourceType = ResourceType.Pelicula },
                new ResourceModel { ResourceId = 2, Name = "Cien Años", Author = "GGM", Description = "Novela", Publication = 1967, ResourceType = ResourceType.Libro }
            };
            var tuple = ((IEnumerable<ResourceModel>)models, models.Count);
            _repoMock.Setup(r => r.FilterResources(It.IsAny<GetResourceFiltersDto>())).ReturnsAsync(tuple);
            var filters = new GetResourceFiltersDto { PageNumber = 1, PageSize = 10 };
            var result = await _service.FilterResources(filters);
            Assert.Equal(models.Count, result.Total);
            var list = result.Items.ToList();
            Assert.Equal(2, list.Count);
            Assert.Equal("El Padrino", list[0].Name);
            Assert.Equal("Pelicula", list[0].ResourceType.ToString());
        }
        [Fact]
        public async Task FilterResources_NoResults_ReturnsEmptyList()
        {
            var emptyResult = ((IEnumerable<ResourceModel>)new List<ResourceModel>(), 0);
            _repoMock.Setup(r => r.FilterResources(It.IsAny<GetResourceFiltersDto>()))
                     .ReturnsAsync(emptyResult);

            var filters = new GetResourceFiltersDto { PageNumber = 1, PageSize = 10 };
            var result = await _service.FilterResources(filters);
            Assert.Empty(result.Items);
            Assert.Equal(0, result.Total);
        }

        [Fact]
        public async Task FilterResources_MapsEnumToStringCorrectly()
        {
            var models = new List<ResourceModel>
            {
                new ResourceModel
                {
                    ResourceId = 5,
                    Name = "Matrix",
                    Author = "Wachowski",
                    Publication = 1999,
                    ResourceType = ResourceType.Pelicula,
                    Description = "Sci-fi"
                }
            };
            var tuple = ((IEnumerable<ResourceModel>)models, 1);

            _repoMock.Setup(r => r.FilterResources(It.IsAny<GetResourceFiltersDto>()))
                     .ReturnsAsync(tuple);
            var result = await _service.FilterResources(new GetResourceFiltersDto { PageNumber = 1, PageSize = 10});
            var resource = Assert.Single(result.Items);
            Assert.Equal("Pelicula", resource.ResourceType);
        }

        [Fact]
        public async Task FilterResources_PassesFiltersToRepository()
        {
            var filters = new GetResourceFiltersDto
            {
                Name = "Matrix",
                Author = "Wachowski",
                PageNumber = 2,
                PageSize = 5
            };

            _repoMock.Setup(r => r.FilterResources(filters))
                     .ReturnsAsync(((IEnumerable<ResourceModel>)new List<ResourceModel>(), 0));
            var result = await _service.FilterResources(filters);
            _repoMock.Verify(r => r.FilterResources(It.Is<GetResourceFiltersDto>(
                f => f.Name == "Matrix" && f.Author == "Wachowski" && f.PageNumber == 2 && f.PageSize == 5
            )), Times.Once);
        }
    }
}
