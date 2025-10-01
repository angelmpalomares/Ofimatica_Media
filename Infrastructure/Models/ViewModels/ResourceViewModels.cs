using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Models.ViewModels
{
    public class CreateResourceVm
    {
        [Display(Name = "Tipo de recurso")]
        [Required(ErrorMessage = "El tipo es obligatorio")]
        public string ResourceType { get; set; } = string.Empty;
        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Name { get; set; } = string.Empty;
        [Display(Name = "Descripcion")]
        [Required(ErrorMessage = "La descripcion es obligatoria")]
        public string Description { get; set; } = string.Empty;
        [Display(Name = "Año de publicación")]
        [Required(ErrorMessage = "El año es obligatorio")]
        public int Publication { get; set; }
        [Display(Name = "Autor")]
        [Required(ErrorMessage = "El autor es obligatorio")]
        public string Author { get; set; } = string.Empty;
    }
    public class EditResourceVm
    {
        public int ResourceId { get; set; }
        public string? ResourceType { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? Publication { get; set; }
        public string? Author { get; set; }
    }
    public class ResourceFilterVm
    {
        public string? Name { get; set; }
        public string? ResourceType { get; set; }
        public int? Year { get; set; }
        public string? Author { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
