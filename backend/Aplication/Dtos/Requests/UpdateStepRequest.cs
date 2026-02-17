using System.ComponentModel.DataAnnotations;

namespace Aplication.Dtos.Requests
{
    public class UpdateStepRequest
    {
        [Required(ErrorMessage = "El id es obligatorio")]
        [Range(0, long.MaxValue, ErrorMessage = "El status debe ser un numero positivo.")]
        public long Id { get; set; }

        [Required(ErrorMessage = "El usuario es obligatorio")]
        [Range(0, int.MaxValue, ErrorMessage = "El usuario debe ser un numero positivo.")]
        public int User { get; set; }

        [Required(ErrorMessage = "El status es obligatorio")]
        [Range(0, int.MaxValue, ErrorMessage = "El status debe ser un numero positivo.")]
        public int Status { get; set; }

        [Required(ErrorMessage = "La observación obligatoria")]
        [StringLength(255, ErrorMessage = "La observación no puede tener más de 255 caracteres.")]
        public string Observation { get; set; }
    }
}
