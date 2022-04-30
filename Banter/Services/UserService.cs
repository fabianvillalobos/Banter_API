using Banter.Entities;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Security;
using Banter.Models;
using Microsoft.Extensions.Options;
using Banter.Repositories;
using Microsoft.AspNetCore.Identity;
using Banter.Utilities;
using Microsoft.AspNetCore.Mvc;
using Banter.DTOs;

namespace Banter.Services
{
    public class UserService: IUserService
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _singinManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly JWTConfig _jwtConfig;

        public UserService(ApplicationDbContext applicationDbContext, UserManager<AppUser> userManager, SignInManager<AppUser> signinManager, RoleManager<IdentityRole> roleManager, IOptions<JWTConfig> jwtConfig)
        {
            db = applicationDbContext;
            _userManager = userManager;
            _singinManager = signinManager;
            _roleManager = roleManager;
            _jwtConfig = jwtConfig.Value;
        }

        public async Task<Respuesta> RegisterUser(User model)
        {
            Respuesta oRespuesta = new Respuesta();
            try
            {
                if (!await _roleManager.RoleExistsAsync(model.Role))
                {
                    oRespuesta.Exito = 0;
                    oRespuesta.Mensaje = "Rol no existe";
                    oRespuesta.Data = null;

                    return oRespuesta;
                }

                var user = new AppUser()
                {
                    FullName = model.FullName,
                    Email = model.Email,
                    UserName = model.Email,
                    DateCreated = DateTime.UtcNow,
                    DateModified = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    var tempUser = await _userManager.FindByEmailAsync(model.Email);
                    await _userManager.AddToRoleAsync(tempUser, model.Role);

                    oRespuesta.Exito = 1;
                    oRespuesta.Mensaje = "Usuario registrado";
                    oRespuesta.Data = null;

                    return oRespuesta;
                }
                else
                {
                    oRespuesta.Exito = 0;
                    oRespuesta.Mensaje = "Error";
                    oRespuesta.Data = result.Errors.Select(x => x.Description).ToArray();

                    return oRespuesta;
                }
            }
            catch (Exception ex)
            {
                oRespuesta.Exito = 0;
                oRespuesta.Mensaje = ex.Message;
                oRespuesta.Data = null;

                return oRespuesta;
            }
        }

        public async Task<Respuesta> AddRole(Rol model)
        {
            Respuesta oRespuesta = new Respuesta();
            try
            {
                if(model == null || model.Role  == "")
                {
                    oRespuesta.Exito = 0;
                    oRespuesta.Mensaje = "Sin parámetros";
                    oRespuesta.Data = null;

                    return oRespuesta;
                }
                if(await _roleManager.RoleExistsAsync(model.Role))
                {
                    oRespuesta.Exito = 0;
                    oRespuesta.Mensaje = "Rol ya existe";
                    oRespuesta.Data = null;

                    return oRespuesta;
                }

                var role = new IdentityRole();
                role.Name = model.Role;
                var result = await _roleManager.CreateAsync(role);

                if (result.Succeeded)
                {
                    oRespuesta.Exito = 1;
                    oRespuesta.Mensaje = "Rol creado exitosamente";
                    oRespuesta.Data = null;

                    return oRespuesta;
                }


                oRespuesta.Exito = 0;
                oRespuesta.Mensaje = "Algo fue mal";
                oRespuesta.Data = null;

                return oRespuesta;
            }
            catch (Exception ex)
            {
                oRespuesta.Exito = 0;
                oRespuesta.Mensaje = ex.Message;
                oRespuesta.Data = null;

                return oRespuesta;
            }
        }

        public async Task<Respuesta> Login(Login login)
        {
            Respuesta oRespuesta = new Respuesta();
            try
            {
                var appUser = await _userManager.FindByEmailAsync(login.Email);
                var role = (await _userManager.GetRolesAsync(appUser)).FirstOrDefault();
                var user = new UserDTO(appUser.FullName, appUser.Email, appUser.UserName, appUser.DateCreated, role);
                user.Token = GenerateToken(appUser, role);

                if(user.Token != "")
                {
                    oRespuesta.Exito = 1;
                    oRespuesta.Mensaje = "Acceso correcto";
                    oRespuesta.Data = user;

                    return oRespuesta;
                }

                var result = await _singinManager.PasswordSignInAsync(login.Email, login.Password, false, false);
                if (result.Succeeded)
                {
                    oRespuesta.Exito = 1;
                    oRespuesta.Mensaje = "Acceso correcto";
                    oRespuesta.Data = user;

                    return oRespuesta;
                }

                oRespuesta.Exito = 0;
                oRespuesta.Mensaje = "Acceso inválido";
                oRespuesta.Data = null;

                return oRespuesta;

            }
            catch (Exception ex)
            {
                oRespuesta.Exito = 0;
                oRespuesta.Mensaje = "Acceso inválido";
                oRespuesta.Data = null;

                return oRespuesta;
            }
        }

        public string GenerateToken(AppUser user, string role)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtConfig.Key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new System.Security.Claims.ClaimsIdentity(new[]
                {
                    new System.Security.Claims.Claim(JwtRegisteredClaimNames.NameId, user.Id),
                    new System.Security.Claims.Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new System.Security.Claims.Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new System.Security.Claims.Claim(ClaimTypes.Role, role)
                }),
                Expires = DateTime.UtcNow.AddHours(12),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            return jwtTokenHandler.WriteToken(token);
        }

        public async Task<Respuesta> GetUsers()
        {
            Respuesta oRespuesta = new Respuesta();

            try
            {
                List<UserDTO> usuarios = new List<UserDTO>();
                var users = _userManager.Users.ToList();
                if(users.Count > 0)
                {
                    foreach (var user in users)
                    {
                        var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
                        usuarios.Add(new UserDTO(
                            user.FullName, user.Email, user.UserName, user.DateCreated, role
                            ));
                    }
                    oRespuesta.Exito = 1;
                    oRespuesta.Mensaje = "";
                    oRespuesta.Data = usuarios;
                }
                else
                {
                    oRespuesta.Exito = 0;
                    oRespuesta.Mensaje = "Vacio";
                    oRespuesta.Data = null;
                }

                return oRespuesta;
                
            }
            catch (Exception ex)
            {
                oRespuesta.Exito = 0;
                oRespuesta.Mensaje = "Vacio";
                oRespuesta.Data = null;

                return oRespuesta;
            }
        }
    }
}
