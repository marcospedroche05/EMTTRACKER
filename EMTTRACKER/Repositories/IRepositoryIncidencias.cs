using EMTTRACKER.Models;

namespace EMTTRACKER.Repositories
{
    public interface IRepositoryIncidencias
    {
        public Task<List<Incidencia>> GetIncidenciasAsync();
        public Task<List<Incidencia>> GetAllIncidenciasAsync();
        public Task<Incidencia> FindIncidenciaAsync(int idincidencia);
        public Task InsertIncidenciaAsync(string titulo, string mensaje, DateTime fechainicio, DateTime fechafin);
        public Task UpdateIncidenciaAsync(int idincidencia, string titulo, string mensaje, DateTime fechainicio, DateTime fechafin);
        public Task DeleteIncidenciaAsync(int idincidencia);
    }
}
