using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMTTRACKER.Models
{
    [Table("LINEAS")]
    public class Linea
    {
        [Key]
        [Column("IDLINEA")]
        public int IdLinea { get; set; }
        [Column("CODIGO")]
        public string Codigo { get; set; }
        [Column("NOMBRE")]
        public string Nombre { get; set; }
        [Column("TIPO")]
        public string Tipo { get; set; }
    }
}
