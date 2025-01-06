using RETOPT.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RETOPT.Application.Interface
{
    public interface IUsuarioService
    {
        //public Task<dynamic> getUsuarios();
        Task<int> CreateUser(string username, string password, string email);
        Task<Usuario> AuthenticateUser(string username, string password);
    }
}
