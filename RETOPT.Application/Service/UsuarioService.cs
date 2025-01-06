using RETOPT.Application.Interface;
using RETOPT.Domain.Entity;
using RETOPT.Domain.Interface;
using RETOPT.Infrastructure.Interface;
using RETOPT.Infrastructure.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RETOPT.Application.Service
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _repository;
        private readonly IPasswordHasher _passwordHasher;
        public UsuarioService(IUsuarioRepository repository, IPasswordHasher passwordHasher)
        {
            _repository = repository;
            _passwordHasher = passwordHasher;
        }
        //public async Task<dynamic> getUsuarios()
        //{
        //    return await _repository.getUsuarios();
        //}
        public async Task<int> CreateUser(string username, string password,string email)
        {
            // Validaciones básicas
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("El userName no puede ser vacio");
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("El PassWord no puede ser vacio");

            // Verificar si el usuario ya existe
            var existingUser = await _repository.GetUserByUsername(username);
            if (existingUser != null)
                throw new InvalidOperationException("El Usuario ya Existe");

            // Hashear la contraseña
            var passwordHash = _passwordHasher.HashPassword(password);

            // Crear usuario
            return await _repository.CreateUser(username, passwordHash,email);
        }

        public async Task<Usuario> AuthenticateUser(string username, string password)
        {
            var user = await _repository.GetUserByUsername(username);
            if (user == null)
                return null;

            if (_passwordHasher.VerifyPassword(password, user.PassHash))
                return user;

            return null;
        }
    }
}
