using ApiUnow.DTO;

namespace ApiUnow.Services.IServices
{
    public interface ICitaServices
    {
        Task<CitaDTO> PedirCita(CitaCreateDTO citaCreateDTO);
        Task<string> ConfirmarRechazarCita(int id, bool confirmar);
        Task<List<ClienteDTO>> ListClientesDia();
    }
}
