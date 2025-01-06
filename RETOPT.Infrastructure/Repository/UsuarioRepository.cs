using Dapper;
using Microsoft.Extensions.Configuration;
using RETOPT.Domain.Entity;
using RETOPT.Infrastructure.Data;
using RETOPT.Infrastructure.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace RETOPT.Infrastructure.Repository
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly string _connectionString;

        public UsuarioRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConnectionSQL");
        }
        public async Task<int> CreateUser(string username, string passwordHash, string email)
        {
            try
            {
                // Conexión y comando en bloques using para liberar recursos
                await using var conn = new SqlConnection(_connectionString);
                await using var cmd = new SqlCommand("sp_CrearUsuarioSeguridad", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@PassHash", passwordHash);
                cmd.Parameters.AddWithValue("@Email", email);
                await conn.OpenAsync();
                var result = await cmd.ExecuteScalarAsync();

                if (result == null || result == DBNull.Value)
                    throw new InvalidOperationException("El procedimiento almacenado no devolvió un valor válido.");

                return Convert.ToInt32(result);
            }
            catch (SqlException ex)
            {
                throw new InvalidOperationException("Ocurrió un error al interactuar con la base de datos.", ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Ocurrió un error inesperado al crear el usuario.", ex);
            }
        }

        public async Task<Usuario> GetUserByUsername(string username)
        {
            try
            {
                // Conexión y comando en bloques using para liberar recursos
                await using var conn = new SqlConnection(_connectionString);
                await using var cmd = new SqlCommand("sp_ObtenerUsuarioXNombre", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@Username", username);
                await conn.OpenAsync();
                await using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new Usuario
                    {
                        UsuarioID = reader.GetInt32(reader.GetOrdinal("UsuarioID")),
                        Username = reader.GetString(reader.GetOrdinal("Username")),
                        PassHash = reader.GetString(reader.GetOrdinal("PassHash")),
                        Email = reader.GetString(reader.GetOrdinal("Email")),
                        FechaCrea = reader.GetDateTime(reader.GetOrdinal("FechaCrea"))
                    };
                }
                return null;
            }
            catch (SqlException ex)
            {
                throw new InvalidOperationException("Ocurrió un error al interactuar con la base de datos.", ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Ocurrió un error inesperado al obtener el usuario.", ex);
            }
        }
        public async Task<Usuario> GetUserByID(int userID)
        {
            try
            {
                // Conexión y comando en bloques using para liberar recursos
                await using var conn = new SqlConnection(_connectionString);
                await using var cmd = new SqlCommand("sp_ObtenerUsuarioXID", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@UsuarioID", userID);
                await conn.OpenAsync();
                await using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new Usuario
                    {
                        UsuarioID = reader.GetInt32(reader.GetOrdinal("UsuarioID")),
                        Username = reader.GetString(reader.GetOrdinal("Username")),
                        PassHash = reader.GetString(reader.GetOrdinal("PassHash")),
                        Email = reader.GetString(reader.GetOrdinal("Email")),
                        FechaCrea = reader.GetDateTime(reader.GetOrdinal("FechaCrea"))
                    };
                }
                return null;
            }
            catch (SqlException ex)
            {
                throw new InvalidOperationException("Ocurrió un error al interactuar con la base de datos.", ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Ocurrió un error inesperado al obtener el usuario.", ex);
            }
        }

    }
}
