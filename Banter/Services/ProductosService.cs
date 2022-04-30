using Banter.DTOs;
using Banter.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Banter.Services
{
    public class ProductosService: IProductosService
    {
        private IProductosRepository _ipr;

        public ProductosService(IProductosRepository productosRepository)
        {
            _ipr = productosRepository;
        }

        public async Task<List<ProductoDTO>> GetProductos()
        {
            var productos = await _ipr.GetProductos();

            return productos;
        }
    }
}
