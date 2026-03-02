using EMTTRACKER.Data;
using EMTTRACKER.Models;
using Microsoft.EntityFrameworkCore;

namespace EMTTRACKER.Repositories
{
    public class RepositoryIncidencias : IRepositoryIncidencias
    {
        EmtContext context;
        public RepositoryIncidencias(EmtContext context)
        {
            this.context = context;
        }

        public async Task<List<Incidencia>> GetIncidenciasAsync()
        {
            DateTime ahora = DateTime.Now;
            var consulta = from datos in this.context.Incidencias
                           where datos.FechaInicio >= ahora
                           select datos;
            return await consulta.ToListAsync();
        }
    }
}
