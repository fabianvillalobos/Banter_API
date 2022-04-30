using AutoMapper;
using Banter.DTOs;
using Banter.Entities;
using Banter.Services;
using Banter.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Banter.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoController : ControllerBase
    {
        private readonly IProductosService _ips;
        public ProductoController(IProductosService productosService)
        {
            _ips = productosService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ProductoDTO>>> Get()
        {
            var productos = await _ips.GetProductos();
            if(productos.Count > 0)
            {
                return Ok(productos);
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
