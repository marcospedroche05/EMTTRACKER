using EMTTRACKER.Models;

namespace EMTTRACKER.Repositories
{
    public interface IRepositoryCercanias
    {
        public Task<Parada> GetParadaByNombre(string nombre);
        public Task<Parada> GetParadaById(int idParada);
        public Task<List<VParadaCercanias>> GetAllParadasCercanias();
        public Task<VParadaCercanias> FindParadaCercaniasByNombreAsync(string nombre);
        public Task<List<VHorariosParadaCercanias>> GetHorariosParadaCercanias(int idParada);
        public Task<Favorita> FindFavoritaAsync(int idUsuario, int idParada);
        public Task InsertFavoritaAsync(int idUsuario, int idParada, string nombre);
        public Task DeleteFavoritaAsync(int idUsuario, int idParada);
        public Task<List<VParadaCercanias>> GetFavoritasCercaniasAsync(int idUsuario);
        public Task AsignarAlias(int idUsuario, int idParada, string alias);
    }
}
