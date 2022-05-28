using System.ComponentModel.DataAnnotations;

namespace ApiUnow.Models
{
    public class Cliente
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(maximumLength:50, MinimumLength = 2)]
        public string Name { get; set; }

        [Required]
        [StringLength(maximumLength:100, MinimumLength = 5)]
        public string Surname { get; set; }

        [StringLength(15)]
        public string? Phone { get; set; }

        [StringLength(50)]
        public string? Email { get; set; }
        public List<Cita> Citas { get; set; }
    }
}
