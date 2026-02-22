using EMTTRACKER.Models;

namespace EMTTRACKER.Repositories
{
    public interface IRepositoryEmt
    {
        public Task<List<VParadaUrbana>> GetAllParadasUrbano();
        public Task<VParadaUrbana> FindParadaUrbanoByCodigoAsync(int codigo);
        public Task<List<VHorariosParadaUrbanos>> GetHorariosParadaUrbano(int codigo);
    }
}
