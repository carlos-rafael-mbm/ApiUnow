using ApiUnow.DAL;
using ApiUnow.DTO;
using ApiUnow.Models;
using ApiUnow.Services.IServices;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ApiUnow.Services
{
    public class CitaServices : ICitaServices
    {
        private readonly ApiUnowDBContext context;
        private readonly IMapper autoMapper;

        public CitaServices(ApiUnowDBContext context, IMapper autoMapper)
        {
            this.context = context;
            this.autoMapper = autoMapper;
        }

        public async Task<string> ConfirmarRechazarCita(int id, bool confirmar)
        {
            var cita = await context.Cita.FindAsync(id);
#pragma warning disable CS8602 // Desreferencia de una referencia posiblemente NULL.
            cita.Estado = confirmar ? 2 : 3;
#pragma warning restore CS8602 // Desreferencia de una referencia posiblemente NULL.
            context.Entry(cita).State = EntityState.Modified;
            try
            {
                await context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return "Error al " + (confirmar ? "confirmar" : "rechazar") + " cita";
            }
            return confirmar ? "Cita confirmada" : "Cita rechazada";
        }

        public async Task<List<ClienteDTO>> ListClientesDia()
        {
#pragma warning disable CS8629 // Un tipo que acepta valores NULL puede ser nulo.
            return await (from cita in context.Cita
                          join cliente in context.Cliente
                          on cita.ClienteId equals cliente.Id
                          where cita.Fecha.Value.Date == DateTime.Now.Date
                          select new ClienteDTO
                          {
                              Id = cliente.Id,
                              Name = cliente.Name,
                              Surname = cliente.Surname,
                              Phone = cliente.Phone,
                              Email = cliente.Email
                          }).Distinct().ToListAsync();
#pragma warning restore CS8629 // Un tipo que acepta valores NULL puede ser nulo.
        }

        public async Task<CitaDTO> PedirCita(CitaCreateDTO citaCreateDTO)
        {
            var cliente = await context.Cliente.FindAsync(citaCreateDTO.ClienteId);
            var taller = await context.Taller.FindAsync(citaCreateDTO.TallerId);
            Cita cita = new Cita
            {
                Id = 0,
                ClienteId = cliente != null ? cliente.Id : 0,
                TallerId = taller != null ? taller.Id : 0,
                Estado = 1,
                Fecha = citaCreateDTO.Fecha,
                Cliente = cliente != null ? cliente : new Cliente(),
                Taller = taller != null ? taller : new Taller()
            };
            context.Cita.Add(cita);
            await context.SaveChangesAsync();
            return autoMapper.Map<CitaDTO>(cita);
        }
    }
}
