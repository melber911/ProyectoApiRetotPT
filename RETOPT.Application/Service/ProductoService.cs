using RETOPT.Application.Interface;
using RETOPT.Domain.Entity;
using RETOPT.Infrastructure.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RETOPT.Application.Service
{
    public class ProductoService : IProductoService
    {
        private readonly IProductoRepository _productoRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        public ProductoService(IProductoRepository productorepository, IUsuarioRepository usuariorepository)
        {
            _productoRepository = productorepository;
            _usuarioRepository = usuariorepository;
        }
        public async Task<int> CreateProducto(int UsuarioId, string CodigoBarra, string Nombre, string Marca, string Categoria, decimal Precio)
        {
            // Validar parámetros
            if (string.IsNullOrWhiteSpace(CodigoBarra))
                throw new ArgumentException("El código de barra es obligatorio.", nameof(CodigoBarra));
            if (string.IsNullOrWhiteSpace(Nombre))
                throw new ArgumentException("El nombre es obligatorio.", nameof(Nombre));
            if (Precio <= 0)
                throw new ArgumentException("El precio debe ser mayor a cero.", nameof(Precio));
            // Verificar si el usuario ya existe
            var existingUser = await _usuarioRepository.GetUserByID(UsuarioId);
            if (existingUser == null)
                throw new InvalidOperationException("El Usuario no existe");

            return await _productoRepository.CreateProducto(UsuarioId, CodigoBarra, Nombre, Marca, Categoria, Precio);
        }

        public async Task<IEnumerable<Producto>> GetProduto(int usuarioId)
        {
            return await _productoRepository.GetProductoXUsuario(usuarioId);
        }

        public async Task UpdateProducto(int usuarioId, int productoId, string CodigoBarra, string Nombre, string Marca, string Categoria, decimal Precio)
        {
            // Validar parámetros
            if (string.IsNullOrWhiteSpace(CodigoBarra))
                throw new ArgumentException("El código de barra es obligatorio.", nameof(CodigoBarra));
            if (string.IsNullOrWhiteSpace(Nombre))
                throw new ArgumentException("El nombre es obligatorio.", nameof(Nombre));
            if (Precio <= 0)
                throw new ArgumentException("El precio debe ser mayor a cero.", nameof(Precio));
            // verificar si el producto pertenece  al usuario
            var tasks = await _productoRepository.GetProductoXUsuario(usuarioId);
            if (!tasks.Any(t => t.ProductoId == productoId))
                throw new UnauthorizedAccessException("El producto no  pertenece a este usuario.");

            await _productoRepository.UpdateProducto(productoId, CodigoBarra, Nombre, Marca, Categoria, Precio);
        }

        public async Task<string> DeleteProducto(int productoId)
        {
            if (productoId <= 0)
                throw new ArgumentException("El ProductoID debe ser un valor positivo.", nameof(productoId));

            // Delegar a la capa de datos
            return await _productoRepository.DeleteProducto(productoId);
        }
    }
}
