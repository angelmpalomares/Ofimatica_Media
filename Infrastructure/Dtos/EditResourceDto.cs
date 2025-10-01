namespace Infrastructure.Dtos
{
    public class EditResourceDto
    {
        public string? ResourceType { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? Publication { get; set; }
        public string? Author { get; set; }
    }
}
