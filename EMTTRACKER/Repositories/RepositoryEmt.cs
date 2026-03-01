using EMTTRACKER.Data;
using EMTTRACKER.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using static System.Runtime.InteropServices.JavaScript.JSType;

#region VIEWS

// VISTA PARA OBTENER PARADAS DE BUSES URBANOS (CON RUTAS DEFINIDAS)

//CREATE VIEW V_Paradas_Emt
//as
//	SELECT DISTINCT P.IDPARADA, P.CODIGO, P.NOMBRE FROM PARADAS P INNER JOIN RUTA_PARADA RP
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


//VISTA PARA OBTENER TODOS LOS HORARIOS QUE ESTAN POR VENIR

//CREATE VIEW V_HORARIOS_RUTAPARADA_URBANO
//AS
//	SELECT 
//        ROW_NUMBER() OVER (ORDER BY L.CODIGO, H.HORASALIDA, P.CODIGO, R.DIRECCION) AS ID,
//        P.CODIGO AS PARADA,
//        R.DIRECCION,
//        L.CODIGO AS LINEA,
//        CONVERT(VARCHAR(5), DATEADD(MINUTE, RP.TIEMPODESDEINICIO, H.HORASALIDA), 108) AS HORAESTIMADA 
//    FROM HORARIOS H 
//    INNER JOIN RUTAS R ON H.IDRUTA = R.IDRUTA
//    INNER JOIN RUTA_PARADA RP ON RP.IDRUTA = R.IDRUTA
//    INNER JOIN LINEAS L ON L.IDLINEA = R.IDLINEA
//    INNER JOIN PARADAS P ON RP.IDPARADA = P.IDPARADA
//    WHERE DATEADD(MINUTE, RP.TIEMPODESDEINICIO, H.HORASALIDA) > CONVERT(TIME, GETDATE())
//    AND L.TIPO = 'Urbano'
//GO



//OBTENER LAS PARADAS FAVORITAS QUE TIENE EL USUARIO REFERENTES A LA PESTAÑA DE EMT

//CREATE VIEW V_PARADAS_FAVORITAS_URBANO
//AS
//	SELECT DISTINCT F.IDUSUARIO, F.IDPARADA, P.CODIGO, F.ALIAS FROM FAVORITAS F
//	INNER JOIN PARADAS P ON F.IDPARADA = P.IDPARADA
//	INNER JOIN RUTA_PARADA RP ON RP.IDPARADA = F.IDPARADA
//	INNER JOIN RUTAS R ON R.IDRUTA = RP.IDRUTA 
//	INNER JOIN LINEAS L ON L.IDLINEA = R.IDLINEA
//	WHERE L.TIPO = 'Urbano'
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

        public async Task<List<VParadaUrbana>> GetAllParadasUrbano()
        {
            var consulta = from datos in this.context.ParadasUrbanas
                           select datos;
            List<VParadaUrbana> paradas = await consulta.ToListAsync();

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

        public async Task<List<VHorariosParadaUrbanos>> GetHorariosParadaUrbano(int codigo)
        {
            var consulta = from datos in this.context.VistaHorariosUrbanos
                           where datos.Codigo == codigo
                           select datos;
            List<VHorariosParadaUrbanos> horarios = await consulta.ToListAsync();
            horarios = horarios.OrderBy(x => x.HoraEstimada).ToList();
            return horarios;
        }

        public async Task<Favorita> FindFavoritaAsync(int idUsuario, int codigo)
        {
            var consulta = from datos in this.context.Favoritas
                           where datos.IdUsuario == idUsuario &&
                           datos.IdParada == codigo
                           select datos;
            return await consulta.FirstOrDefaultAsync();
        }

        public async Task InsertFavoritaAsync(int idUsuario, int codigo, string nombre)
        {
            Favorita favorita = new Favorita
            {
                IdUsuario = idUsuario,
                IdParada = codigo,
                Alias = nombre
            };
            await this.context.Favoritas.AddAsync(favorita);
            await this.context.SaveChangesAsync();
        }

        public async Task DeleteFavoritaAsync(int idUsuario, int codigo)
        {
            Favorita favorita = await this.FindFavoritaAsync(idUsuario, codigo);
            this.context.Favoritas.Remove(favorita);
            await this.context.SaveChangesAsync();
        }

        public async Task<Parada> GetParadaByCodigo(int codigo)
        {
            var consulta = from datos in this.context.Paradas
                           where datos.Codigo == codigo
                           select datos;
            return await consulta.FirstOrDefaultAsync();
        }

        public async Task<List<VParadaUrbana>> GetFavoritasUrbanasAsync(int idUsuario)
        {
            string sql = "SELECT * FROM V_PARADAS_FAVORITAS_URBANO WHERE IDUSUARIO = @idusuario";
            this.com.Parameters.AddWithValue("@idusuario", idUsuario);
            this.com.CommandType = System.Data.CommandType.Text;
            this.com.CommandText = sql;

            await this.cn.OpenAsync();
            List<VParadaUrbana> favoritasUrbano = new List<VParadaUrbana>();
            this.reader = await this.com.ExecuteReaderAsync();
            while (await this.reader.ReadAsync())
            {
                VParadaUrbana favorita = new VParadaUrbana
                {
                    IdParada = int.Parse(this.reader["IDPARADA"].ToString()),
                    Codigo = int.Parse(this.reader["CODIGO"].ToString()),
                    Nombre = this.reader["ALIAS"].ToString()
                };
                favoritasUrbano.Add(favorita);
            }
            await this.reader.CloseAsync();
            await this.cn.CloseAsync();
            this.com.Parameters.Clear();

            foreach(VParadaUrbana favorita in favoritasUrbano)
            {
                favorita.Lineas = await this.GetLineasByCodigoParadaAsync(favorita.Codigo);
            }

            return favoritasUrbano;
        }

        public async Task AsignarAlias(int idUsuario, int codigo, string alias)
        {
            Favorita favorita = await this.FindFavoritaAsync(idUsuario, codigo);
            favorita.Alias = alias;
            await this.context.SaveChangesAsync();
        }
    }
}
