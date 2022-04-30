using Banter.Entities;
using Banter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Banter.Services
{
    public interface IUserService
    {
        Task<Respuesta> RegisterUser(User user);
        Task<Respuesta> AddRole(Rol rol);
        Task<Respuesta> Login(Login login);
        Task<Respuesta> GetUsers();
        string GenerateToken(AppUser user, string role);
    }
}
