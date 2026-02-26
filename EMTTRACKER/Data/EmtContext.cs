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
        public DbSet<VParadaInterurbana> ParadasInterurbanas { get; set; }
        public DbSet<Linea> Lineas { get; set; }
        public DbSet<VHorariosParadaUrbanos> VistaHorariosUrbanos { get; set; }
        public DbSet<VHorariosParadaInterurbanos> VistaHorariosInterurbanos { get; set; }
        public DbSet<Favorita> Favoritas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Favorita>(entity =>
            {
                entity.HasKey(f => new { f.IdUsuario, f.IdParada });
                entity.ToTable("FAVORITAS");
            });

            modelBuilder.Entity<VParadaUrbana>()
                .HasNoKey()
                .ToView("V_Paradas_Emt");

            modelBuilder.Entity<VParadaInterurbana>()
                .HasNoKey()
                .ToView("V_Paradas_Interurbano");

            modelBuilder.Entity<VHorariosParadaUrbanos>()
                .ToView("V_HORARIOS_RUTAPARADA_URBANO");

            modelBuilder.Entity<VHorariosParadaInterurbanos>()
                .ToView("V_HORARIOS_RUTAPARADA_INTERURBANO");
        }
    }
}
