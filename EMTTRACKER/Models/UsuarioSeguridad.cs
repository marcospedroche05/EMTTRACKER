using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMTTRACKER.Models
{
    [Table("USERS_SECURITY")]
    public class UsuarioSeguridad
    {
        [Key]
        [Column("IDUSUARIO")]
        public int IdUsuario { get; set; }
        
        [Column("SALT")]
        public string Salt { get; set; }
        
        [Column("PASS")]
        public byte[] Pass { get; set; }
    }
}
