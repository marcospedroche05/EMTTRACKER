using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMTTRACKER.Models
{
    [Table("INCIDENCIAS")]
    public class Incidencia
    {
        [Key]
        [Column("IDINCIDENCIA")]
        public int IdIncidencia { get; set; }
        [Column("TITULO")]
        public string Titulo { get; set; }
        [Column("MENSAJE")]
        public string Mensaje { get; set; }
        [Column("FECHAINICIO")]
        public DateTime FechaInicio { get; set; }
        [Column("FECHAFIN")]
        public DateTime FechaFin { get; set; }
    }
}
