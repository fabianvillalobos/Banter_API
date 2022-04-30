using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Banter.DTOs
{
    public class ProductoDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Categoria { get; set; }
        public int Medida { get; set; }
    }
}
