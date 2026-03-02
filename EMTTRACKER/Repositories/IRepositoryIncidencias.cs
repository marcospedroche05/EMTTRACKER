using EMTTRACKER.Models;

namespace EMTTRACKER.Repositories
{
    public interface IRepositoryIncidencias
    {
        public Task<List<Incidencia>> GetIncidenciasAsync();
    }
}
