using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Infrastructure.Enums;

namespace Infrastructure.Models
{
    public class ResourceModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ResourceId { get; set; }
        public required ResourceType ResourceType { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required int Publication {  get; set; }
        public required string Author {  get; set; }
    }
}
