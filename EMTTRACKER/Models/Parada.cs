using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMTTRACKER.Models
{
    [Table("PARADAS")]
    public class Parada
    {
        [Key]
        [Column("IDPARADA")]
        public int IdParada { get; set; }
        [Column("CODIGO")]
        public int Codigo { get; set; }
        [Column("NOMBRE")]
        public string Nombre { get; set; }
    }
}
