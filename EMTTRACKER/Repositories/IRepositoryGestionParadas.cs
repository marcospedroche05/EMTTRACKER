using EMTTRACKER.Models;

namespace EMTTRACKER.Repositories
{
    public interface IRepositoryGestionParadas
    {
        public Task<List<Parada>> GetAllParadasAsync();
        public Task<Parada> FindParadaAsync(int idparada);
        public Task InsertParadaAsync(int? codigo, string nombre);
        public Task UpdateParadaAsync(int idparada, int? codigo, string nombre);
        public Task DeleteParadaAsync(int idparada);
    }
}
