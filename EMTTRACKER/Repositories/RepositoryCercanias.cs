using EMTTRACKER.Data;
using EMTTRACKER.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

#region VIEWS


//CREATE VIEW V_Lineas_En_Parada_Nombre
//AS
//	SELECT P.NOMBRE AS PARADA, L.IDLINEA, L.CODIGO AS LINEA, L.NOMBRE, L.TIPO FROM PARADAS P INNER JOIN RUTA_PARADA RP 
//	ON P.IDPARADA = RP.IDPARADA
//	INNER JOIN RUTAS R ON RP.IDRUTA = R.IDRUTA
//	INNER JOIN LINEAS L ON L.IDLINEA = R.IDLINEA 
//GO


//CREATE VIEW V_Paradas_Cercanias
//as
//	SELECT DISTINCT P.IDPARADA, P.NOMBRE FROM PARADAS P INNER JOIN RUTA_PARADA RP
//	ON P.IDPARADA = RP.IDPARADA
//	INNER JOIN RUTAS R ON RP.IDRUTA = R.IDRUTA
//	INNER JOIN LINEAS L ON L.IDLINEA = R.IDLINEA
//	WHERE L.TIPO = 'Cercanias'
//go


//CREATE VIEW V_HORARIOS_RUTAPARADA_CERCANIAS
//AS
//	SELECT 
//        ROW_NUMBER() OVER (ORDER BY L.CODIGO, H.HORASALIDA, R.DIRECCION) AS ID,
//        P.IDPARADA AS IDPARADA,
//        R.DIRECCION AS DIRECCION,
//        L.CODIGO AS LINEA,
//        CONVERT(VARCHAR(5), DATEADD(MINUTE, RP.TIEMPODESDEINICIO, H.HORASALIDA), 108) AS HORAESTIMADA 
//    FROM HORARIOS H 
//    INNER JOIN RUTAS R ON H.IDRUTA = R.IDRUTA
//    INNER JOIN RUTA_PARADA RP ON RP.IDRUTA = R.IDRUTA
//    INNER JOIN LINEAS L ON L.IDLINEA = R.IDLINEA
//    INNER JOIN PARADAS P ON RP.IDPARADA = P.IDPARADA
//    WHERE DATEADD(MINUTE, RP.TIEMPODESDEINICIO, H.HORASALIDA) > CONVERT(TIME, GETDATE())
//    AND L.TIPO = 'Cercanias'
//GO


//CREATE VIEW V_PARADAS_FAVORITAS_CERCANIAS
//AS
//	SELECT DISTINCT F.IDUSUARIO, F.IDPARADA, F.ALIAS FROM FAVORITAS F
//	INNER JOIN PARADAS P ON F.IDPARADA = P.IDPARADA
//	INNER JOIN RUTA_PARADA RP ON RP.IDPARADA = F.IDPARADA
//	INNER JOIN RUTAS R ON R.IDRUTA = RP.IDRUTA 
//	INNER JOIN LINEAS L ON L.IDLINEA = R.IDLINEA
//	WHERE L.TIPO = 'Cercanias'
//GO

#endregion


namespace EMTTRACKER.Repositories
{
    public class RepositoryCercanias : IRepositoryCercanias
    {
        private EmtContext context;
        SqlConnection cn;
        SqlCommand com;
        SqlDataReader reader;

        public RepositoryCercanias(EmtContext context)
        {
            this.context = context;
            string connectionString = @"Data Source=LOCALHOST\DEVELOPER;Initial Catalog=EMTTRACKER;User ID=SA;Password=Admin123;Trust Server Certificate=True";
            this.cn = new SqlConnection(connectionString);
            this.com = new SqlCommand();
            this.com.Connection = this.cn;
        }

        public async Task<Parada> GetParadaById(int idParada)
        {
            var consulta = from datos in this.context.Paradas
                           where datos.IdParada == idParada
                           select datos;
            return await consulta.FirstOrDefaultAsync();
        }

        public async Task<Parada> GetParadaByNombre(string nombre)
        {
            var consulta = from datos in this.context.Paradas
                           where datos.Nombre == nombre
                           select datos;
            return await consulta.FirstOrDefaultAsync();
        }

        public async Task<List<VParadaCercanias>> GetAllParadasCercanias()
        {
            var consulta = from datos in this.context.ParadasCercanias
                           select datos;
            List<VParadaCercanias> paradas = await consulta.ToListAsync();
            foreach (VParadaCercanias parada in paradas)
            {
                parada.Lineas = await GetLineasByNombreParadaAsync(parada.Nombre);
            }
            return paradas;
        }

        public async Task<VParadaCercanias> FindParadaCercaniasByNombreAsync(string nombre)
        {
            var consulta = from datos in this.context.ParadasCercanias
                           where datos.Nombre == nombre
                           select datos;
            VParadaCercanias parada = await consulta.FirstOrDefaultAsync();
            if (parada != null)
            {
                parada.Lineas = await this.GetLineasByNombreParadaAsync(parada.Nombre);
            }
            return parada;
        }

        public async Task<List<VHorariosParadaCercanias>> GetHorariosParadaCercanias(int idParada)
        {
            var consulta = from datos in this.context.VistaHorariosCercanias
                           where datos.IdParada == idParada
                           select datos;
            List<VHorariosParadaCercanias> horarios = await consulta.ToListAsync();
            horarios = horarios.OrderBy(x => x.HoraEstimada).ToList();
            return horarios;
        }

        public async Task<List<VParadaCercanias>> GetFavoritasCercaniasAsync(int idUsuario)
        {
            string sql = "SELECT * FROM V_PARADAS_FAVORITAS_CERCANIAS WHERE IDUSUARIO = @idusuario";
            this.com.Parameters.AddWithValue("@idusuario", idUsuario);
            this.com.CommandType = System.Data.CommandType.Text;
            this.com.CommandText = sql;

            await this.cn.OpenAsync();
            List<VParadaCercanias> favoritasCercanias = new List<VParadaCercanias>();
            this.reader = await this.com.ExecuteReaderAsync();
            while (await this.reader.ReadAsync())
            {
                VParadaCercanias favorita = new VParadaCercanias
                {
                    IdParada = int.Parse(this.reader["IDPARADA"].ToString()),
                    Nombre = this.reader["ALIAS"].ToString()
                };
                favoritasCercanias.Add(favorita);
            }
            await this.reader.CloseAsync();
            await this.cn.CloseAsync();
            this.com.Parameters.Clear();

            foreach (VParadaCercanias favorita in favoritasCercanias)
            {
                favorita.Lineas = await this.GetLineasByNombreParadaAsync(favorita.Nombre);
            }

            return favoritasCercanias;
        }

        public async Task<Favorita> FindFavoritaAsync(int idUsuario, int idParada)
        {
            var consulta = from datos in this.context.Favoritas
                           where datos.IdUsuario == idUsuario &&
                           datos.IdParada == idParada
                           select datos;
            return await consulta.FirstOrDefaultAsync();
        }

        public async Task InsertFavoritaAsync(int idUsuario, int idParada, string nombre)
        {
            Favorita favorita = new Favorita
            {
                IdUsuario = idUsuario,
                IdParada = idParada,
                Alias = nombre
            };
            await this.context.Favoritas.AddAsync(favorita);
            await this.context.SaveChangesAsync();
        }

        public async Task DeleteFavoritaAsync(int idUsuario, int idParada)
        {
            Favorita favorita = await this.FindFavoritaAsync(idUsuario, idParada);
            this.context.Favoritas.Remove(favorita);
            await this.context.SaveChangesAsync();
        }

        public async Task AsignarAlias(int idUsuario, int idParada, string alias)
        {
            Favorita favorita = await this.FindFavoritaAsync(idUsuario, idParada);
            favorita.Alias = alias;
            await this.context.SaveChangesAsync();
        }


        private async Task<List<Linea>> GetLineasByNombreParadaAsync(string nombre)
        {
            string sql = "SELECT * FROM V_Lineas_En_Parada_Nombre WHERE PARADA = @nombre";
            this.com.Parameters.AddWithValue("@nombre", nombre);
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
