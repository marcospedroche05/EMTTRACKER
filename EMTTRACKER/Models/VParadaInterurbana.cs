using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMTTRACKER.Models
{
    [Table("V_Paradas_Interurbano")]
    public class VParadaInterurbana
    {
        [Key]
        [Column("IDPARADA")]
        public int IdParada { get; set; }
        [Column("CODIGO")]
        public int Codigo { get; set; }
        [Column("NOMBRE")]
        public string Nombre { get; set; }

        public List<Linea> Lineas { get; set; }

        public VParadaInterurbana()
        {
            this.Lineas = new List<Linea>();
        }
    }
}
