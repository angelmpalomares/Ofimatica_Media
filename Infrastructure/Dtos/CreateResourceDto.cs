
namespace Infrastructure.Dtos
{
    public class CreateResourceDto
    {
        public required string ResourceType { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required int Publication { get; set; }
        public required string Author { get; set; }
    }
}
