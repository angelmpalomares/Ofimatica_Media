using Infrastructure.Database;
using Infrastructure.Dtos;
using Infrastructure.Enums;
using Infrastructure.Models;
using Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Implementations
{
    public class ResourceRepository(InfraDbContext context) : BaseRepository<ResourceModel>(context), IResourceRepository
    {
        public async Task<(IEnumerable<ResourceModel>, int totalCount)> FilterResources(GetResourceFiltersDto filterResourcesDto)
        {
            int pageNumber = filterResourcesDto.PageNumber - 1;
            IQueryable<ResourceModel?> query = _context.Resources;
            if (!string.IsNullOrWhiteSpace(filterResourcesDto.Name))
            {
                var searchTerm = filterResourcesDto.Name.Trim().ToLower();
                query = query.Where(a => a.Name.ToLower().Contains(searchTerm));
            }
            if (!string.IsNullOrWhiteSpace(filterResourcesDto.ResourceType))
            {
                ResourceType type = (ResourceType)Enum.Parse(typeof(ResourceType), filterResourcesDto.ResourceType);
                query = query.Where(a => a.ResourceType == type);
            }
            if (filterResourcesDto.Year.HasValue)
            {
                query = query.Where(a => a.Publication == filterResourcesDto.Year);
            }
            if (!string.IsNullOrWhiteSpace(filterResourcesDto.Author))
            {
                var authorSearch = filterResourcesDto.Author.ToLower();
                query = query.Where(a => a.Author.ToLower().Contains(authorSearch));
            }
            int totalCount = await query.CountAsync();
            var resources = await query
                .Skip(filterResourcesDto.PageSize * pageNumber)
                .Take(filterResourcesDto.PageSize)
                .ToListAsync();
            return (resources, totalCount);
        }
    }
}
