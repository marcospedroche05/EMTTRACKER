using EMTTRACKER.Models;
using Microsoft.EntityFrameworkCore;

namespace EMTTRACKER.Data
{
    public class EmtContext: DbContext
    {
        public EmtContext(DbContextOptions<EmtContext> options): base(options) 
        {}

        public DbSet<Usuario> Usuarios { get; set; }
    }
}
