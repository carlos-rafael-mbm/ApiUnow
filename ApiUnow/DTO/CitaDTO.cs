using ApiUnow.Models;

namespace ApiUnow.DTO
{
    public class CitaDTO
    {
        public int Id { get; set; }
        public int Estado { get; set; }
        public DateTime? Fecha { get; set; }
        public ClienteDTO Cliente { get; set; }
        public TallerDTO Taller { get; set; }
    }
}
