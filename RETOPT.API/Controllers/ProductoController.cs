using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RETOPT.Application.DTOs;
using RETOPT.Application.Interface;
using RETOPT.Application.Service;
using RETOPT.Domain.Entity;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text.RegularExpressions;

namespace RETOPT.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoController : ControllerBase
    {
        private readonly IProductoService _productoService;
        public ProductoController(IProductoService productoService)
        {
            _productoService = productoService;
        }
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] CreateTaskDto dto)
        {
            try
            {
                //var UsuarioId = GetUserId() == null ? 1 : dto.usuarioId;
                var UsuarioId = dto.usuarioId;
                var productoId = await _productoService.CreateProducto(UsuarioId, dto.CodigoBarra, dto.Nombre, dto.Marca, dto.Categoria, dto.Precio);
                return CreatedAtAction(nameof(GetByUser), new { usuarioId = UsuarioId }, new { Message = "Producto creado exitosamente.", ProductoId = productoId });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error al crear el producto.", Details = ex.Message });
            }
        }
        // GET: api/Producto/{usuarioId}
        [HttpGet("Obtener")]
        public async Task<IActionResult> GetByUser(int usuarioId)
        {
            try
            {
                var productos = await _productoService.GetProduto(usuarioId);
                if (!productos.Any())
                    return NotFound(new { Message = $"No se encontraron productos para el usuario con ID {usuarioId}." });

                return Ok(productos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error al obtener los productos.", Details = ex.Message });
            }
        }
        // PUT: api/Producto/{productoId}
        [HttpPut("Update{UsuarioId},{ProductoId}")]
        public async Task<IActionResult> Update(int UsuarioId, 
            int ProductoId, 
            string CodigoBarra = "",
            string Nombre ="", 
            string Marca = "", 
            string Categoria = "", 
            decimal Precio=0)
        {
            try
            {
                await _productoService.UpdateProducto(UsuarioId, ProductoId, CodigoBarra, Nombre,Marca, Categoria, Precio);
                return Ok(new { Message = "Producto actualizado exitosamente." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error al actualizar el producto.", Details = ex.Message });
            }
        }

        // DELETE: api/Producto/{productoId}
        [HttpDelete("{productoId}")]
        public async Task<IActionResult> Delete(int productoId)
        {
            try
            {
                await _productoService.DeleteProducto(productoId);
                return Ok(new { Message = "Producto eliminado exitosamente." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error al eliminar el producto.", Details = ex.Message });
            }
        }
        private int GetUserId()
        {
            //var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //return int.Parse(userIdClaim);

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var id = User.FindFirst(ClaimTypes.Sid)?.Value;
            if (int.TryParse(id, out var userId))
            {
                return Convert.ToInt32(id);
            }
            else
            {
                // Opcional: Loggear el error para diagnóstico 
                throw new InvalidOperationException("Invalid user identifier.");
            }
        }
    }
}

