using EMTTRACKER.Models;
using Microsoft.EntityFrameworkCore;

namespace EMTTRACKER.Data
{
    public class EmtContext: DbContext
    {
        public EmtContext(DbContextOptions<EmtContext> options): base(options) 
        {}

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Parada> Paradas { get; set; }
        public DbSet<VParadaUrbana> ParadasUrbanas { get; set; }
        public DbSet<Linea> Lineas { get; set; }
    }
}
