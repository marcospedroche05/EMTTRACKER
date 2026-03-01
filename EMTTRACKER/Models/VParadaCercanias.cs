using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMTTRACKER.Models
{
    [Table("V_Paradas_Cercanias")]
    public class VParadaCercanias
    {
        [Key]
        [Column("IDPARADA")]
        public int IdParada { get; set; }
        [Column("NOMBRE")]
        public string Nombre { get; set; }

        public List<Linea> Lineas { get; set; }

        public VParadaCercanias()
        {
            this.Lineas = new List<Linea>();
        }
    }
}
