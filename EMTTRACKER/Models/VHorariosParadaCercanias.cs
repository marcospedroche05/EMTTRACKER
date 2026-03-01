using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMTTRACKER.Models
{
    [Table("V_HORARIOS_RUTAPARADA_CERCANIAS")]
    public class VHorariosParadaCercanias
    {
        [Key]
        [Column("ID")]
        public long Id { get; set; }
        [Column("IDPARADA")]
        public int IdParada { get; set; }
        [Column("DIRECCION")]
        public string Direccion { get; set; }
        [Column("LINEA")]
        public string Linea { get; set; }
        [Column("HORAESTIMADA")]
        public string HoraEstimada { get; set; }
    }
}
