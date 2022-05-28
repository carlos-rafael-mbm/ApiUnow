using System.ComponentModel.DataAnnotations;

namespace ApiUnow.Models
{
    public class Cita
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ClienteId { get; set; }

        [Required]
        public int TallerId { get; set; }

        public int Estado { get; set; }

        public DateTime? Fecha { get; set; }

        public Cliente Cliente { get; set; }

        public Taller Taller { get; set; }
    }
}
