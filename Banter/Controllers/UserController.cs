using Banter.Models;
using Banter.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Banter.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _ius;
        public UserController(IUserService userService)
        {
            _ius = userService;
        }

        [HttpPost]
        [Route("RegistrarUsuario")]
        public async Task<ActionResult> RegistrarUsuario([FromBody] User model)
        {
            var registrar = await _ius.RegisterUser(model);

            if(registrar.Exito == 1)
            {
                return Ok(registrar);
            }
            else
            {
                return BadRequest(registrar);
            }
        }

        [HttpPost]
        //[Authorize(Roles = "Admin")]
        [Route("AgregarRol")]
        public async Task<ActionResult> AgregarRol([FromBody] Rol model)
        {
            var registrar = await _ius.AddRole(model);

            if (registrar.Exito == 1)
            {
                return Ok(registrar);
            }
            else
            {
                return BadRequest(registrar);
            }
        }

        [HttpPost]
        [Route("Login")]
        public async Task<ActionResult> Login([FromBody] Login model)
        {
            Respuesta registrar = new Respuesta();

            if (ModelState.IsValid)
            {
               registrar  = await _ius.Login(model);
               if(registrar.Exito == 1)
                {
                    return Ok(registrar);
                }
                else
                {
                    return BadRequest(registrar);
                }
            }
            else
            {
                return BadRequest();
            }
            
        }

        [HttpGet]
        [Route("ObtUsuarios")]
        public async Task<ActionResult> ObtUsuarios()
        {
            var usuarios = await _ius.GetUsers();

            if(usuarios.Exito == 1)
            {
                return Ok(usuarios);
            }
            else
            {
                return BadRequest(usuarios);
            }
        }
    }
}
