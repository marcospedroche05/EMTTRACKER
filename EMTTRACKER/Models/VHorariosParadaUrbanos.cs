using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMTTRACKER.Models
{
    [Table("V_HORARIOS_RUTAPARADA_URBANO")]
    public class VHorariosParadaUrbanos
    {
        [Key]
        [Column("ID")]
        public long Id { get; set; }
        [Column("PARADA")]
        public int Codigo { get; set; }
        [Column("LINEA")]
        public string Linea { get; set; }
        [Column("HORAESTIMADA")]
        public string HoraEstimada { get; set; }
    }
}
