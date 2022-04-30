using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Banter.Entities
{
    public class Producto
    {
        [Key]
        public int Id { get; set; }
        
        public string Nombre { get; set; }
        public string Categoria { get; set; }
        public int Medida { get; set; }
    }
}
