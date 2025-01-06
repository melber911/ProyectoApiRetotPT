using RETOPT.Domain.Entity;
using RETOPT.Infrastructure.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RETOPT.Infrastructure.Interface
{
    public interface IUsuarioRepository
    {
        //public Task<dynamic> getUsuarios();
        Task<int> CreateUser(string username, string passwordHash,string email);
        Task<Usuario> GetUserByUsername(string username);
        Task<Usuario> GetUserByID(int userID);
    }
}
