using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MinimalApi.Entidades.Veiculo
{
    public class Veiculo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id {get; set; } = default!;

        [Required]
        [StringLength(80)]
        public string Nome {get; set; } = default!;

        [StringLength(50)]
        public string Marca {get; set; }  = default!;

        [StringLength(20)]
        public string Modelo {get; set; } = default!;
        public int Ano {get; set;} = default!;
        
    }
}