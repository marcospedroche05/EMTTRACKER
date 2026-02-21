using EMTTRACKER.Data;
using EMTTRACKER.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

#region VIEWS

// VISTA PARA OBTENER PARADAS DE BUSES URBANOS (CON RUTAS DEFINIDAS)

//CREATE VIEW V_Paradas_Emt
//as
//	SELECT P.IDPARADA, P.CODIGO, P.NOMBRE FROM PARADAS P INNER JOIN RUTA_PARADA RP
//	ON P.IDPARADA = RP.IDPARADA
//	INNER JOIN RUTAS R ON RP.IDRUTA = R.IDRUTA
//	INNER JOIN LINEAS L ON L.IDLINEA = R.IDLINEA
//	WHERE L.TIPO = 'Urbano'
//go


//VISTA PARA OBTENER TODAS LAS LINEAS QUE PASAN POR UNA PARADA

//CREATE VIEW V_Lineas_En_Parada
//AS
//	SELECT P.CODIGO, L.IDLINEA, L.CODIGO AS LINEA, L.NOMBRE, L.TIPO FROM PARADAS P INNER JOIN RUTA_PARADA RP 
//	ON P.IDPARADA = RP.IDPARADA
//	INNER JOIN RUTAS R ON RP.IDRUTA = R.IDRUTA
//	INNER JOIN LINEAS L ON L.IDLINEA = R.IDLINEA
//GO

#endregion

namespace EMTTRACKER.Repositories
{
    public class RepositoryEmt : IRepositoryEmt
    {
        EmtContext context;
        SqlConnection cn;
        SqlCommand com;
        SqlDataReader reader;

        public RepositoryEmt(EmtContext context)
        {
            this.context = context;
            string connectionString = @"Data Source=LOCALHOST\DEVELOPER;Initial Catalog=EMTTRACKER;User ID=SA;Password=Admin123;Trust Server Certificate=True";
            this.cn = new SqlConnection(connectionString);
            this.com = new SqlCommand();
            this.com.Connection = this.cn;
        }

        public async Task<List<Parada>> GetAllParadasAsync()
        {
            var consulta = from datos in this.context.Paradas
                           select datos;
            return await consulta.ToListAsync();
        }

        public async Task<Parada> FindParadaByCodigoAsync(int codigo)
        {
            var consulta = from datos in this.context.Paradas
                           where datos.Codigo == codigo
                           select datos;
            return await consulta.FirstOrDefaultAsync();
        }

        public async Task<Parada> FindParadaByIdAsync(int id)
        {
            var consulta = from datos in this.context.Paradas
                           where datos.IdParada == id
                           select datos;
            return await consulta.FirstOrDefaultAsync();
        }

        public async Task<List<VParadaUrbana>> GetAllParadasUrbano()
        {
            var consulta = from datos in this.context.ParadasUrbanas
                           select datos;
            List<VParadaUrbana> paradas = new List<VParadaUrbana>();
            paradas = await consulta.ToListAsync();

            foreach (VParadaUrbana parada in paradas)
            {
                List<Linea> lineas = await this.GetLineasByCodigoParadaAsync(parada.Codigo);
                parada.Lineas = lineas;
            }
            return paradas;
        }

        public async Task<VParadaUrbana> FindParadaUrbanoByCodigoAsync(int codigo)
        {
            var consulta = from datos in this.context.ParadasUrbanas
                           where datos.Codigo == codigo
                           select datos;
            VParadaUrbana parada = await consulta.FirstOrDefaultAsync();
            if(parada != null)
            {
                parada.Lineas = await this.GetLineasByCodigoParadaAsync(parada.Codigo);
            }
            return parada;
        }

        private async Task<List<Linea>> GetLineasByCodigoParadaAsync(int codigo)
        {
            string sql = "SELECT * FROM V_Lineas_En_Parada WHERE CODIGO = @codigo";
            this.com.Parameters.AddWithValue("@codigo", codigo);
            this.com.CommandType = System.Data.CommandType.Text;
            this.com.CommandText = sql;

            await this.cn.OpenAsync();
            List<Linea> lineas = new List<Linea>();
            this.reader = await this.com.ExecuteReaderAsync();
            while(await this.reader.ReadAsync())
            {
                Linea linea = new Linea
                {
                    IdLinea = int.Parse(this.reader["IDLINEA"].ToString()),
                    Codigo = this.reader["LINEA"].ToString(),
                    Nombre = this.reader["NOMBRE"].ToString(),
                    Tipo = this.reader["TIPO"].ToString()
                };
                lineas.Add(linea);
            }
            await this.reader.CloseAsync();
            await this.cn.CloseAsync();
            this.com.Parameters.Clear();

            return lineas;
        }
    }
}
