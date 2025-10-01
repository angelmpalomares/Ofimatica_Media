
namespace Infrastructure.Dtos
{
    public class GetResourceFiltersDto
    {
        public string? Name { get; set; }
        public string? ResourceType { get; set; }
        public int? Year { get; set; }
        public string? Author { get; set; }
        public required int PageNumber { get; set; }
        public required int PageSize { get; set; }
    }
}
