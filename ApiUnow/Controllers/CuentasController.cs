using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ApiUnow.Services;
using Microsoft.AspNetCore.DataProtection;
using ApiUnow.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;

namespace ApiUnow.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CuentasController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly IConfiguration configuration;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly HashServices hashService;
        private readonly IDataProtector dataProtector;

        public CuentasController(UserManager<IdentityUser> userManager, IConfiguration configuration,
                                SignInManager<IdentityUser> signInManager, IDataProtectionProvider dataProtectionProvider,
                                HashServices hashService)
        {
            this.userManager = userManager;
            this.configuration = configuration;
            this.signInManager = signInManager;
            this.hashService = hashService;
            dataProtector = dataProtectionProvider.CreateProtector("valor_unico_y_secreto");
        }

        [HttpPost("registrar", Name = "crearUsuario")] // api/cuentas/registrar
        public async Task<ActionResult<RespuestaAutenticacion>> Registrar(CredencialesUsuario credencialesUsuario)
        {
            var usuario = new IdentityUser()
            {
                UserName = credencialesUsuario.Email,
                Email = credencialesUsuario.Email
            };
            var resultado = await userManager.CreateAsync(usuario, credencialesUsuario.Password);

            if (resultado.Succeeded)
            {
                return await ConstruirToken(credencialesUsuario);
            }
            else
            {
                return BadRequest(resultado.Errors);
            }
        }

        [HttpPost("login", Name = "loginUsuario")]
        public async Task<ActionResult<RespuestaAutenticacion>> Login(CredencialesUsuario credencialesUsuario)
        {
            var resultado = await signInManager.PasswordSignInAsync(
                                                                credencialesUsuario.Email,
                                                                credencialesUsuario.Password,
                                                                isPersistent: false,
                                                                lockoutOnFailure: false);
            if (resultado.Succeeded)
            {
                return await ConstruirToken(credencialesUsuario);
            }
            else
            {
                return BadRequest("Login incorrecto");
            }
        }

        private async Task<RespuestaAutenticacion> ConstruirToken(CredencialesUsuario credencialesUsuario)
        {
            var claims = new List<Claim>()
            {
                new Claim("email", credencialesUsuario.Email)
            };

            var usuario = await userManager.FindByEmailAsync(credencialesUsuario.Email);
            var claimsBD = await userManager.GetClaimsAsync(usuario);
            claims.AddRange(claimsBD);

            var llave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["llaveJWT"]));
            var creds = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);
            var expiracion = DateTime.UtcNow.AddYears(1);

            var securityToken = new JwtSecurityToken(claims: claims, expires: expiracion, signingCredentials: creds);
            return new RespuestaAutenticacion()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                Expiracion = expiracion
            };
        }

        [HttpPost("DarPermisosCliente", Name = "darPermisosCliente")]
        public async Task<ActionResult> DarPermisosCliente(string email)
        {
            var usuario = await userManager.FindByEmailAsync(email);
            await userManager.AddClaimAsync(usuario, new Claim("esCliente", "1"));
            return NoContent();
        }

        [HttpPost("QuitarPermisosCliente", Name = "quitarPermisosCliente")]
        public async Task<ActionResult> QuitarPermisosCliente(string email)
        {
            var usuario = await userManager.FindByEmailAsync(email);
            await userManager.RemoveClaimAsync(usuario, new Claim("esCliente", "1"));
            return NoContent();
        }

        [HttpPost("DarPermisosTaller", Name = "darPermisosTaller")]
        public async Task<ActionResult> DarPermisosTaller(string email)
        {
            var usuario = await userManager.FindByEmailAsync(email);
            await userManager.AddClaimAsync(usuario, new Claim("esTaller", "1"));
            return NoContent();
        }

        [HttpPost("QuitarPermisosTaller", Name = "quitarPermisosTaller")]
        public async Task<ActionResult> QuitarPermisosTaller(string email)
        {
            var usuario = await userManager.FindByEmailAsync(email);
            await userManager.RemoveClaimAsync(usuario, new Claim("esTaller", "1"));
            return NoContent();
        }
    }
}
