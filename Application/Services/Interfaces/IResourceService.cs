using Application.Services.Implementations;
using Infrastructure.Dtos;

namespace Application.Services.Interfaces
{
    public interface IResourceService
    {
        Task CreateResource(CreateResourceDto createResourceDto);
        Task EditResource(int resourceId, EditResourceDto editResourceDto);
        Task DeleteResource(int resourceId);
        Task<GetResourceDto> GetById(int resourceId);
        Task<PagedCollectionResponse<GetResourceDto>> FilterResources(GetResourceFiltersDto filterResourcesDto);
    }
}
