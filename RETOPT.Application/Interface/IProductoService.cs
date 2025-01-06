using RETOPT.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RETOPT.Application.Interface
{
    public interface IProductoService
    {
        //public Task<dynamic> getProdctos();
        Task<int> CreateProducto(int UsuarioId, string CodigoBarra, string Nombre, string Marca, string Categoria, decimal Precio);
        Task<IEnumerable<Producto>> GetProduto(int usuarioId);
        Task UpdateProducto(int usuarioId, int productoId, string CodigoBarra, string Nombre, string Marca, string Categoria, decimal Precio);
        Task<string> DeleteProducto(int productoId);
    }
}
