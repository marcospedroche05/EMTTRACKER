using EMTTRACKER.Models;

namespace EMTTRACKER.Repositories
{
    public interface IRepositoryEmt
    {
        public Task<Parada> GetParadaByCodigo(int codigo);
        public Task<List<VParadaUrbana>> GetAllParadasUrbano();
        public Task<VParadaUrbana> FindParadaUrbanoByCodigoAsync(int codigo);
        public Task<List<VHorariosParadaUrbanos>> GetHorariosParadaUrbano(int codigo);
        public Task<Favorita> FindFavoritaAsync(int idUsuario, int codigo);
        public Task InsertFavoritaAsync(int idUsuario, int codigo, string nombre);
        public Task DeleteFavoritaAsync(int idUsuario, int codigo);
        public Task<List<VParadaUrbana>> GetFavoritasUrbanasAsync(int idUsuario);
        public Task AsignarAlias(int idUsuario, int codigo, string alias);
    }
}
