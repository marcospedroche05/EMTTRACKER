using EMTTRACKER.Data;
using EMTTRACKER.Models;
using Microsoft.EntityFrameworkCore;

namespace EMTTRACKER.Repositories
{
    public class RepositoryGestionParadas : IRepositoryGestionParadas
    {
        EmtContext context;
        public RepositoryGestionParadas(EmtContext context)
        {
            this.context = context;
        }

        public async Task DeleteParadaAsync(int idparada)
        {
            Parada parada = await FindParadaAsync(idparada);
            this.context.Paradas.Remove(parada);
            await this.context.SaveChangesAsync();
        }

        public async Task<Parada> FindParadaAsync(int idparada)
        {
            var consulta = from datos in this.context.Paradas
                           where datos.IdParada == idparada
                           select datos;
            return await consulta.FirstOrDefaultAsync();
        }

        public async Task<List<Parada>> GetAllParadasAsync()
        {
            var consulta = from datos in this.context.Paradas
                           select datos;
            return await consulta.ToListAsync();
        }

        public async Task InsertParadaAsync(int? codigo, string nombre)
        {
            int siguienteId = await GetSiguienteIdParadaAsync();
            Parada parada = new Parada
            {
                IdParada = siguienteId,
                Codigo = codigo,
                Nombre = nombre
            };
            await this.context.Paradas.AddAsync(parada);
            await this.context.SaveChangesAsync();
        }

        public async Task UpdateParadaAsync(int idparada, int? codigo, string nombre)
        {
            Parada parada = await FindParadaAsync(idparada);
            if(codigo != null)
            {
                parada.Codigo = codigo;
            }
            parada.Nombre = nombre;
            await this.context.SaveChangesAsync();
        }

        private async Task<int> GetSiguienteIdParadaAsync()
        {
            int siguienteId = this.context.Paradas.AsEnumerable().MaxBy(x => x.IdParada).IdParada;
            return siguienteId + 1;
        }
    }
}
