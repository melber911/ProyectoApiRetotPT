using Dapper;
using Microsoft.Extensions.Configuration;
using RETOPT.Domain.Entity;
using RETOPT.Infrastructure.Data;
using RETOPT.Infrastructure.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RETOPT.Infrastructure.Repository
{
    public class ProductoRepository : IProductoRepository
    {
        
        private readonly string _connectionString;

        public ProductoRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConnectionSQL");
        }

        [Obsolete]
        public async Task<int> CreateProducto(int UsuarioId, string CodigoBarra, string Nombre, string Marca, string Categoria, decimal Precio)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand("sp_CrearProducto", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@UsuarioId", UsuarioId);
                    cmd.Parameters.AddWithValue("@CodigoBarra", CodigoBarra);
                    cmd.Parameters.AddWithValue("@Nombre", Nombre ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Marca", Marca ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Categoria", Categoria ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Precio", Precio);
                    await conn.OpenAsync();

                    var result = await cmd.ExecuteScalarAsync();
                    return Convert.ToInt32(result);
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Error al ejecutar el procedimiento almacenado.", ex);
            }
            catch (InvalidCastException ex)
            {
                throw new Exception("El procedimiento almacenado devolvió un resultado inesperado.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un error al crear el producto.", ex);
            }
        }

        [Obsolete]
        public async Task<IEnumerable<Producto>> GetProductoXUsuario(int usuarioId)
        {
            var producto = new List<Producto>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_ObtenerProductoxUsuario", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UsuarioId", usuarioId);

                await conn.OpenAsync();
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        producto.Add(new Producto
                        {
                            ProductoId = reader.GetInt32(reader.GetOrdinal("ProductoId")),
                            UsuarioId = usuarioId,
                            CodigoBarra = reader.GetString(reader.GetOrdinal("CodigoBarra")),
                            Nombre = reader.IsDBNull(reader.GetOrdinal("Nombre")) ? null : reader.GetString(reader.GetOrdinal("Nombre")),
                            Marca = reader.GetString(reader.GetOrdinal("Marca")),
                            Categoria = reader.GetString(reader.GetOrdinal("Categoria")),
                            Precio = reader.GetDecimal(reader.GetOrdinal("Precio")),
                            FechaCrea = reader.GetDateTime(reader.GetOrdinal("FechaCrea")),
                            FechaActualiza = reader.GetDateTime(reader.GetOrdinal("FechaActualiza"))
                        });
                    }
                }
            }
            return producto;
        }
        [Obsolete]
        public async Task UpdateProducto(int productoId, string CodigoBarra, string Nombre, string Marca, string Categoria, decimal Precio)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_ActualizarProducto", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ProductoID", productoId);
                cmd.Parameters.AddWithValue("@CodigoBarra", CodigoBarra);
                cmd.Parameters.AddWithValue("@Nombre", Nombre ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Marca", Marca);
                cmd.Parameters.AddWithValue("@Categoria", Categoria);
                cmd.Parameters.AddWithValue("@Precio", Precio);

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }
        [Obsolete]
        public async Task<string> DeleteProducto(int productoId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand("sp_EliminarProducto", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ProductoID", productoId);

                    await conn.OpenAsync();
                    var result = await cmd.ExecuteScalarAsync();

                    return result?.ToString() ?? "Error desconocido al eliminar el producto.";
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Error al intentar eliminar el producto en la base de datos.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un error al eliminar el producto.", ex);
            }
        }
    }
}
