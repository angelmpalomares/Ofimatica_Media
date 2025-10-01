using Application.Services.Interfaces;
using Infrastructure.Dtos;
using Infrastructure.Enums;
using Infrastructure.Models;
using Infrastructure.Repositories.Interfaces;
using Infrastructure.Validation;
using System.ComponentModel.DataAnnotations;
using ValidationResult = Infrastructure.Validation.ValidationResult;

namespace Application.Services.Implementations
{
    public class ResourceService(IResourceRepository resourceRepository) : IResourceService
    {
        private readonly IResourceRepository _resourceRepository = resourceRepository;

        public async Task CreateResource(CreateResourceDto createResourceDto)
        {
            try
            {
                await ValidateResourceData(createResourceDto);
            }
            catch (ValidationException ex)
            {
                throw new ValidationException(ex.Message);
            }
            ResourceType resourceType = (ResourceType)Enum.Parse(typeof(ResourceType), createResourceDto.ResourceType);
            var newResource = new ResourceModel
            {
                Author = createResourceDto.Author,
                Description = createResourceDto.Description,
                Publication = createResourceDto.Publication,
                Name = createResourceDto.Name,
                ResourceType = resourceType
            };
            await _resourceRepository.AddAsync(newResource);
            await _resourceRepository.SaveChangesAsync();
        }
        public async Task<GetResourceDto> GetById(int resourceId)
        {
            var resource = await _resourceRepository.GetByIdAsync(resourceId) ?? throw new KeyNotFoundException("ErrorMessage.ResourceNotFound");
            return MapToDto(resource);
        }
        public async Task EditResource(int resourceId, EditResourceDto editResourceDto)
        {
            var resource = await _resourceRepository.GetByIdAsync(resourceId)
                ?? throw new KeyNotFoundException("ErrorMessage.ResourceNotFound");

            var allErrors = new List<string>();

            var nameValidation = ResourceValidator.ValidateName(editResourceDto.Name, true);
            if (!nameValidation.IsValid)
                allErrors.AddRange(nameValidation.Errors);

            var descriptionValidation = ResourceValidator.ValidateDescription(editResourceDto.Description, true);
            if (!descriptionValidation.IsValid)
                allErrors.AddRange(descriptionValidation.Errors);

            var authorValidation = ResourceValidator.ValidateAuthor(editResourceDto.Author, true);
            if (!authorValidation.IsValid)
                allErrors.AddRange(authorValidation.Errors);

            var yearValidation = ResourceValidator.ValidateYear(editResourceDto.Publication, true);
            if (!yearValidation.IsValid)
                allErrors.AddRange(yearValidation.Errors);

            if (allErrors.Any())
            {
                throw new ValidationException(string.Join(", ", allErrors));
            }

            if (!string.IsNullOrWhiteSpace(editResourceDto.Name))
                resource.Name = editResourceDto.Name;

            if (!string.IsNullOrWhiteSpace(editResourceDto.Description))
                resource.Description = editResourceDto.Description;

            if (editResourceDto.Publication.HasValue)
                resource.Publication = editResourceDto.Publication.Value;

            if (!string.IsNullOrWhiteSpace(editResourceDto.Author))
                resource.Author = editResourceDto.Author;

            _resourceRepository.Update(resource);
            await _resourceRepository.SaveChangesAsync();
        }
        public async Task<PagedCollectionResponse<GetResourceDto>> FilterResources(GetResourceFiltersDto filterResourcesDto)
        {
            var (resources, totalCount) = await _resourceRepository.FilterResources(filterResourcesDto);
            var resourcesDtos = resources.Select(_ => MapToDto(_)).ToList() ?? [];
            return new PagedCollectionResponse<GetResourceDto>() { Items = resourcesDtos, Total = totalCount };
        }
        private static GetResourceDto MapToDto(ResourceModel resource)
        {
            return new GetResourceDto
            { 
                ResourceId = resource.ResourceId,
                ResourceType = resource.ResourceType.ToString(),
                Author = resource.Author, 
                Name = resource.Name, 
                Description = resource.Description, 
                Publication = resource.Publication 
            };
        }
        public async Task DeleteResource(int resourceId)
        {
            var resource = await _resourceRepository.GetByIdAsync(resourceId) ?? throw new KeyNotFoundException("ErrorMessage.ResourceNotFound");
            await _resourceRepository.DeleteAsync(resource);
            await _resourceRepository.SaveChangesAsync();
        }
        #region Helpers
        private static Task ValidateResourceData(CreateResourceDto resourceDto, bool isUpdate = false)
        {
            var errors = new List<string>();
            var nameValidation = ResourceValidator.ValidateName(resourceDto.Name, isUpdate);
            var descriptionValidation = ResourceValidator.ValidateDescription(resourceDto.Description, isUpdate);
            var authorValidation = ResourceValidator.ValidateAuthor(resourceDto.Author, isUpdate);
            var yearValidation = ResourceValidator.ValidateYear(resourceDto.Publication, isUpdate);
            if(!nameValidation.IsValid) errors.AddRange(nameValidation.Errors);
            if (!descriptionValidation.IsValid) errors.AddRange(nameValidation.Errors);
            if (!authorValidation.IsValid) errors.AddRange(nameValidation.Errors);
            if (!yearValidation.IsValid) errors.AddRange(nameValidation.Errors);
            if (errors.Count != 0)
            {
                throw new ValidationException(string.Join(", ", errors));
            }
            return Task.CompletedTask;
        }
        #endregion
    }
}
