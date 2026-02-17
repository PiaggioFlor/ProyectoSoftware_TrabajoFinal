using System.ComponentModel.DataAnnotations;

namespace Aplication.Dtos.Requests
{
    public class CreateProjectRequest
    {
        [Required(ErrorMessage = "El título es obligatorio.")]
        [StringLength(100, ErrorMessage = "El título no puede tener más de 100 caracteres.")]
        public string? title { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        [StringLength(100, ErrorMessage = "La descripción no puede tener más de 100 caracteres.")]
        public string? description { get; set; }

        [Required(ErrorMessage = "El valor estimado es obligatorio.")]
        [Range(0, double.MaxValue, ErrorMessage = "El monto estimado debe ser un valor positivo.")]
        public decimal? amount { get; set; }

        [Required(ErrorMessage = "La duración estimada es obligatoria.")]
        [Range(0, int.MaxValue, ErrorMessage = "La duración estimada debe ser un número positivo.")]
        public int? duration { get; set; }

        [Required(ErrorMessage = "El area es obligatoria")]
        [Range(0, int.MaxValue, ErrorMessage = "El area debe ser un numero positivo.")]
        public int? area { get; set; }

        [Required(ErrorMessage = "El tipo es obligatorio")]
        [Range(0, int.MaxValue, ErrorMessage = "El tipo debe ser un numero positivo.")]
        public int? type { get; set; }

        [Required(ErrorMessage = "El usuario es obligatorio")]
        [Range(0, int.MaxValue, ErrorMessage = "El usuario debe ser un numero positivo.")]
        public int? user { get; set; }
    }
}
