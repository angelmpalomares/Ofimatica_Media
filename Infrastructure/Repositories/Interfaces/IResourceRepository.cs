using Infrastructure.Dtos;
using Infrastructure.Models;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IResourceRepository
    {
        Task AddAsync(ResourceModel resource);
        void Update(ResourceModel resource);
        Task SaveChangesAsync();
        Task<ResourceModel?> GetByIdAsync(int ResourceId);
        Task DeleteAsync(ResourceModel resource);
        Task<(IEnumerable<ResourceModel>, int totalCount)> FilterResources(GetResourceFiltersDto filterResourcesDto);
    }
}
