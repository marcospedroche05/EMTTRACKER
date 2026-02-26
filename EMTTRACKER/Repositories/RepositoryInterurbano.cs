using EMTTRACKER.Data;
using EMTTRACKER.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

#region VIEWS

//VISTA PARA OBTENER TODAS LAS LINEAS QUE PASAN POR UNA PARADA

//CREATE VIEW V_Lineas_En_Parada
//AS
//	SELECT P.CODIGO, L.IDLINEA, L.CODIGO AS LINEA, L.NOMBRE, L.TIPO FROM PARADAS P INNER JOIN RUTA_PARADA RP 
//	ON P.IDPARADA = RP.IDPARADA
//	INNER JOIN RUTAS R ON RP.IDRUTA = R.IDRUTA
//	INNER JOIN LINEAS L ON L.IDLINEA = R.IDLINEA
//GO

// VISTA PARA OBTENER PARADAS DE BUSES INTERURBANOS (CON RUTAS DEFINIDAS)

//CREATE VIEW V_Paradas_Interurbano
//as
//	SELECT DISTINCT P.IDPARADA, P.CODIGO, P.NOMBRE FROM PARADAS P INNER JOIN RUTA_PARADA RP
//	ON P.IDPARADA = RP.IDPARADA
//	INNER JOIN RUTAS R ON RP.IDRUTA = R.IDRUTA
//	INNER JOIN LINEAS L ON L.IDLINEA = R.IDLINEA
//	WHERE L.TIPO = 'Interurbano'
//go

//VISTA PARA OBTENER TODOS LOS HORARIOS QUE ESTAN POR VENIR

//CREATE VIEW V_HORARIOS_RUTAPARADA_INTERURBANO
//AS
//	SELECT 
//        ROW_NUMBER() OVER (ORDER BY L.CODIGO, H.HORASALIDA, P.CODIGO) AS ID,
//        P.CODIGO AS PARADA,
//        L.CODIGO AS LINEA,
//        CONVERT(VARCHAR(5), DATEADD(MINUTE, RP.TIEMPODESDEINICIO, H.HORASALIDA), 108) AS HORAESTIMADA 
//    FROM HORARIOS H 
//    INNER JOIN RUTAS R ON H.IDRUTA = R.IDRUTA
//    INNER JOIN RUTA_PARADA RP ON RP.IDRUTA = R.IDRUTA
//    INNER JOIN LINEAS L ON L.IDLINEA = R.IDLINEA
//    INNER JOIN PARADAS P ON RP.IDPARADA = P.IDPARADA
//    WHERE DATEADD(MINUTE, RP.TIEMPODESDEINICIO, H.HORASALIDA) > CONVERT(TIME, GETDATE())
//    AND L.TIPO = 'Interurbano'
//GO


//OBTENER LAS PARADAS FAVORITAS QUE TIENE EL USUARIO REFERENTES A LA PESTAÑA DE INTERURBANOS

//CREATE VIEW V_PARADAS_FAVORITAS_INTERURBANO
//AS
//	SELECT DISTINCT F.IDUSUARIO, F.IDPARADA, P.CODIGO, F.ALIAS FROM FAVORITAS F
//	INNER JOIN PARADAS P ON F.IDPARADA = P.IDPARADA
//	INNER JOIN RUTA_PARADA RP ON RP.IDPARADA = F.IDPARADA
//	INNER JOIN RUTAS R ON R.IDRUTA = RP.IDRUTA 
//	INNER JOIN LINEAS L ON L.IDLINEA = R.IDLINEA
//	WHERE L.TIPO = 'Interurbano'
//GO

#endregion


namespace EMTTRACKER.Repositories
{
    public class RepositoryInterurbano: IRepositoryInterurbano
    {
        private EmtContext context;
        SqlConnection cn;
        SqlCommand com;
        SqlDataReader reader;

        public RepositoryInterurbano(EmtContext context)
        {
            this.context = context;
            string connectionString = @"Data Source=LOCALHOST\DEVELOPER;Initial Catalog=EMTTRACKER;User ID=SA;Password=Admin123;Trust Server Certificate=True";
            this.cn = new SqlConnection(connectionString);
            this.com = new SqlCommand();
            this.com.Connection = this.cn;
        }

        public async Task<Parada> GetParadaByCodigo(int codigo)
        {
            var consulta = from datos in this.context.Paradas
                           where datos.Codigo == codigo
                           select datos;
            return await consulta.FirstOrDefaultAsync();
        }

        public async Task<List<VParadaInterurbana>> GetAllParadasInterurbanas()
        {
            var consulta = from datos in this.context.ParadasInterurbanas
                           select datos;
            List<VParadaInterurbana> paradas = await consulta.ToListAsync();
            foreach(VParadaInterurbana parada in paradas)
            {
                parada.Lineas = await GetLineasByCodigoParadaAsync(parada.Codigo);
            }
            return paradas;
        }

        public async Task<VParadaInterurbana> FindParadaInterurbanoByCodigoAsync(int codigo)
        {
            var consulta = from datos in this.context.ParadasInterurbanas
                           where datos.Codigo == codigo
                           select datos;
            VParadaInterurbana parada = await consulta.FirstOrDefaultAsync();
            if (parada != null)
            {
                parada.Lineas = await this.GetLineasByCodigoParadaAsync(parada.Codigo);
            }
            return parada;
        }

        public async Task<List<VHorariosParadaInterurbanos>> GetHorariosParadaInterurbano(int codigo)
        {
            var consulta = from datos in this.context.VistaHorariosInterurbanos
                           where datos.Codigo == codigo
                           select datos;
            List<VHorariosParadaInterurbanos> horarios = await consulta.ToListAsync();
            horarios = horarios.OrderBy(x => x.HoraEstimada).ToList();
            return horarios;
        }

        public async Task<List<VParadaInterurbana>> GetFavoritasInterurbanasAsync(int idUsuario)
        {
            string sql = "SELECT * FROM V_PARADAS_FAVORITAS_INTERURBANO WHERE IDUSUARIO = @idusuario";
            this.com.Parameters.AddWithValue("@idusuario", idUsuario);
            this.com.CommandType = System.Data.CommandType.Text;
            this.com.CommandText = sql;

            await this.cn.OpenAsync();
            List<VParadaInterurbana> favoritasInterurbano = new List<VParadaInterurbana>();
            this.reader = await this.com.ExecuteReaderAsync();
            while (await this.reader.ReadAsync())
            {
                VParadaInterurbana favorita = new VParadaInterurbana
                {
                    IdParada = int.Parse(this.reader["IDPARADA"].ToString()),
                    Codigo = int.Parse(this.reader["CODIGO"].ToString()),
                    Nombre = this.reader["ALIAS"].ToString()
                };
                favoritasInterurbano.Add(favorita);
            }
            await this.reader.CloseAsync();
            await this.cn.CloseAsync();
            this.com.Parameters.Clear();

            foreach (VParadaInterurbana favorita in favoritasInterurbano)
            {
                favorita.Lineas = await this.GetLineasByCodigoParadaAsync(favorita.Codigo);
            }

            return favoritasInterurbano;
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

        public async Task AsignarAlias(int idUsuario, int codigo, string alias)
        {
            Favorita favorita = await this.FindFavoritaAsync(idUsuario, codigo);
            favorita.Alias = alias;
            await this.context.SaveChangesAsync();
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
            while (await this.reader.ReadAsync())
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
