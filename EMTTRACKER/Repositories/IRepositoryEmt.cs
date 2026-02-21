using EMTTRACKER.Models;

namespace EMTTRACKER.Repositories
{
    public interface IRepositoryEmt
    {
        public Task<List<Parada>> GetAllParadasAsync();
        public Task<Parada> FindParadaByCodigoAsync(int codigo);
        public Task<Parada> FindParadaByIdAsync(int id);
        public Task<List<VParadaUrbana>> GetAllParadasUrbano();
        public Task<VParadaUrbana> FindParadaUrbanoByCodigoAsync(int codigo);
    }
}
