using RETOPT.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RETOPT.Infrastructure.Interface
{
    public interface IProductoRepository
    {
        //public Task<dynamic> getProductos();
        Task<int> CreateProducto(int UsuarioId, string CodigoBarra, string Nombre, string Marca, string Categoria, decimal Precio);
        Task<IEnumerable<Producto>> GetProductoXUsuario(int usuarioId);
        Task UpdateProducto(int productoId, string CodigoBarra, string Nombre, string Marca, string Categoria, decimal Precio);
        Task<string> DeleteProducto(int productoId);
    }
}
