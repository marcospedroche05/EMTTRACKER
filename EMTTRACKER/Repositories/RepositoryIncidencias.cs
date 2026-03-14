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

        public async Task<List<Incidencia>> GetAllIncidenciasAsync()
        {
            var consulta = from datos in this.context.Incidencias
                           select datos;
            return await consulta.ToListAsync();
        }

        public async Task DeleteIncidenciaAsync(int idincidencia)
        {
            Incidencia incidencia = await FindIncidenciaAsync(idincidencia);
            this.context.Incidencias.Remove(incidencia);
            await this.context.SaveChangesAsync();
        }

        public async Task<Incidencia> FindIncidenciaAsync(int idincidencia)
        {
            var consulta = from datos in this.context.Incidencias
                           where datos.IdIncidencia == idincidencia
                           select datos;
            return await consulta.FirstOrDefaultAsync();
        }

        public async Task<List<Incidencia>> GetIncidenciasAsync()
        {
            DateTime ahora = DateTime.Now;
            var consulta = from datos in this.context.Incidencias
                           where datos.FechaInicio >= ahora
                           select datos;
            return await consulta.ToListAsync();
        }

        public async Task InsertIncidenciaAsync(string titulo, string mensaje, DateTime fechainicio, DateTime fechafin)
        {
            int siguienteId = await GetSiguienteIdIncidencia();
            Incidencia incidencia = new Incidencia
            {
                IdIncidencia = siguienteId,
                Titulo = titulo,
                Mensaje = mensaje,
                FechaInicio = fechainicio,
                FechaFin = fechafin
            };
            await this.context.Incidencias.AddAsync(incidencia);
            await this.context.SaveChangesAsync();
        }

        public async Task UpdateIncidenciaAsync(int idincidencia, string titulo, string mensaje, DateTime fechainicio, DateTime fechafin)
        {
            Incidencia incidencia = await FindIncidenciaAsync(idincidencia);
            incidencia.Titulo = titulo;
            incidencia.Mensaje = mensaje;
            incidencia.FechaInicio = fechainicio;
            incidencia.FechaFin = fechafin;
            await this.context.SaveChangesAsync();
        }

        private async Task<int> GetSiguienteIdIncidencia()
        {
            int ultimoId = this.context.Incidencias.AsEnumerable().MaxBy(x => x.IdIncidencia).IdIncidencia;
            return ultimoId + 1;
        }
    }
}
