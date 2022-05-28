using System.ComponentModel.DataAnnotations;

namespace ApiUnow.Models
{
    public class Taller
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [StringLength(200)]
        public string? Direccion { get; set; }

        public List<Cita> Citas { get; set; }
    }
}
