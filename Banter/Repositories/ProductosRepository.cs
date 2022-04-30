using AutoMapper;
using Banter.DTOs;
using Banter.Utilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Banter.Repositories
{
    public class ProductosRepository: IProductosRepository
    {
        private readonly ApplicationDbContext db;
        private IMapper mapper;

        public ProductosRepository(ApplicationDbContext applicationDbContext, IMapper mapper)
        {
            db = applicationDbContext;
            this.mapper = mapper;
        }

        public async Task<List<ProductoDTO>> GetProductos()
        {
            var productos = await db.Producto.Select(a => new ProductoDTO
            {
                Id = a.Id,
                Nombre = a.Nombre,
                Categoria = a.Categoria,
                Medida = a.Medida
            }).ToListAsync();

            return productos;
        }
    }
}
