using EMTTRACKER.Models;

namespace EMTTRACKER.Repositories
{
    public interface IRepositoryInterurbano
    {
        public Task<Parada> GetParadaByCodigo(int codigo);
        public Task<List<VParadaInterurbana>> GetAllParadasInterurbanas();
        public Task<VParadaInterurbana> FindParadaInterurbanoByCodigoAsync(int codigo);
        public Task<List<VHorariosParadaInterurbanos>> GetHorariosParadaInterurbano(int codigo);
        public Task<Favorita> FindFavoritaAsync(int idUsuario, int codigo);
        public Task InsertFavoritaAsync(int idUsuario, int codigo, string nombre);
        public Task DeleteFavoritaAsync(int idUsuario, int codigo);
        public Task<List<VParadaInterurbana>> GetFavoritasInterurbanasAsync(int idUsuario);
        public Task AsignarAlias(int idUsuario, int codigo, string alias);
    }
}
