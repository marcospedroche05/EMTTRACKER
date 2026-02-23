using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMTTRACKER.Models
{
    [Table("FAVORITAS")]
    [PrimaryKey(nameof(IdUsuario), nameof(IdParada))]
    public class Favorita
    {
        [Column("IDUSUARIO")]
        public int IdUsuario { get; set; }
        [Column("IDPARADA")]
        public int IdParada { get; set; }
        [Column("ALIAS")]
        public string Alias { get; set; }
    }
}
