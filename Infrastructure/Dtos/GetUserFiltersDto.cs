namespace Infrastructure.Dtos
{
    public class GetUserFiltersDto
    {
        public string? Name { get; set; }
        public string? Username { get; set; }
        public bool? IsActive { get; set; }
        public required int PageNumber { get; set; }
        public required int PageSize { get; set; }
    }
}
