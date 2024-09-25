using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MinimalApi.Entidades.Adm
{
    public class Administrador
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id {get; set; } = default!;

        [Required]
        [StringLength(255)]
        public string Email {get; set; } = default!;

        [StringLength(50)]
        public string Senha {get; set; }  = default!;

        [StringLength(20)]
        public string Perfil {get; set; } = default!;
        
    }
}