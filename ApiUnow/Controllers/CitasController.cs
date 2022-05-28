using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiUnow.DAL;
using ApiUnow.Models;
using ApiUnow.DTO;
using AutoMapper;
using ApiUnow.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace ApiUnow.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CitasController : ControllerBase
    {
        private readonly ApiUnowDBContext _context;
        private readonly IMapper _mapper;
        private readonly ICitaServices _citaServices;

        public CitasController(ApiUnowDBContext context, IMapper mapper, ICitaServices citaServices)
        {
            _context = context;
            this._mapper = mapper;
            this._citaServices = citaServices;
        }

        [HttpGet("ListClientesDia")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsTaller")]
        public async Task<List<ClienteDTO>> ListClientesDia()
        {
            return await _citaServices.ListClientesDia();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CitaDTO>> GetCita(int id)
        {
            if (_context.Cita == null)
            {
                return NotFound();
            }
            var cita = await _context.Cita.FindAsync(id);

            if (cita == null)
            {
                return NotFound();
            }

            return _mapper.Map<CitaDTO>(cita);
        }

        [HttpPut("ConfirmarRechazarCita/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsTaller")]
        public async Task<ActionResult> ConfirmarRechazarCita(int id, bool confirmar)
        {
            if (!_context.Cita.Any(x => x.Id == id))
            {
                return BadRequest("No se encontró cita");
            }

            string mensaje = await _citaServices.ConfirmarRechazarCita(id, confirmar);

            return Ok(mensaje);
        }

        [HttpPost("PedirCita")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsCliente")]
        public async Task<ActionResult> PedirCita(CitaCreateDTO citaCreateDTO)
        {
            var cliente = await _context.Cliente.FindAsync(citaCreateDTO.ClienteId);
            if (cliente == null)
                return BadRequest("Cliente no existe");

            var taller = await _context.Taller.FindAsync(citaCreateDTO.TallerId);
            if (taller == null)
                return BadRequest("Taller no existe");

            CitaDTO citaDTO = await _citaServices.PedirCita(citaCreateDTO);

            return CreatedAtAction("GetCita", new { id = citaDTO.Id }, citaDTO);
        }
    }
}
