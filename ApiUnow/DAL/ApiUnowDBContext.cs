using ApiUnow.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ApiUnow.DAL
{
    public class ApiUnowDBContext : IdentityDbContext
    {
        public ApiUnowDBContext(DbContextOptions<ApiUnowDBContext> options) : base(options)
        {
        }

        public DbSet<Cliente> Cliente { get; set; }
        public DbSet<Taller> Taller { get; set; }
        public DbSet<Cita> Cita { get; set; }
    }
}
