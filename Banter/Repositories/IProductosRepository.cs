using Banter.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Banter.Repositories
{
    public interface IProductosRepository
    {
        Task<List<ProductoDTO>> GetProductos();
    }
}
