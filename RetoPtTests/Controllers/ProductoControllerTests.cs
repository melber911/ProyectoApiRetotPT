using Moq;
using RETOPT.API.Controllers;
using RETOPT.Application.Interface;
using RETOPT.Application.Service;
using RETOPT.Application.DTOs;
using RETOPT.Domain.Entity;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace RetoPtTests.Controllers
{
    public class ProductoControllerTests
    {
        private readonly ProductoController _controller;
        private readonly Mock<IProductoService> _mockProductoService;

        public ProductoControllerTests()
        {
            _mockProductoService = new Mock<IProductoService>();
            _controller = new ProductoController(_mockProductoService.Object);
        }

        [Fact]
        public async Task Create_ShouldReturnCreatedAtAction_WhenProductCreatedSuccessfully()
        {
            // Arrange
            int usuarioId = 1;
            int productoId = 100;
            string codigoBarra = "12345";
            string nombre = "Producto Test";
            string marca = "Marca Test";
            string categoria = "Categoria Test";
            decimal precio = 10.5m;

            var product = new CreateTaskDto()
            {
                //= 1,
                CodigoBarra = "12345",
                Nombre = "Producto Test",
                Marca = "Marca Test",
                Categoria = "Categoria Test",
                Precio = 10.5m
            };

            _mockProductoService.Setup(service => service.CreateProducto(usuarioId, codigoBarra, nombre, marca, categoria, precio))
                .ReturnsAsync(productoId);

            // Act
            var result = await _controller.Create(product);

            // Assert
            // En tu prueba
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            var jsonResponse = JObject.FromObject(createdAtActionResult.Value);

            var message = jsonResponse["Message"].ToString();
            Assert.Equal("Producto creado exitosamente.", message);

            productoId = (int)jsonResponse["ProductoId"];
            Assert.Equal(productoId, productoId);
        }

        [Fact]
        public async Task Create_ShouldReturnBadRequest_WhenArgumentExceptionIsThrown()
        {
            // Arrange

            var product = new CreateTaskDto()
            {
                //= 1,
                CodigoBarra = "12345",
                Nombre = "Producto Test",
                Marca = "Marca Test",
                Categoria = "Categoria Test",
                Precio = 10.5m,
                usuarioId = 1
            };

            int usuarioId = 1;
            string codigoBarra = "12345";
            string nombre = "Producto Test";
            string marca = "Marca Test";
            string categoria = "Categoria Test";
            decimal precio = 10.5m;

            _mockProductoService.Setup(service => service.CreateProducto(usuarioId, codigoBarra, nombre, marca, categoria, precio))
                .ThrowsAsync(new ArgumentException("Datos inválidos"));

            // Act
            var result = await _controller.Create(product);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var jsonResponse = JObject.FromObject(badRequestResult.Value);
            var message = jsonResponse["Message"].ToString();
            Assert.Equal("Datos inválidos", message);
        }

        [Fact]
        public async Task Create_ShouldReturnInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            var product = new CreateTaskDto()
            {
                //= 1,
                CodigoBarra = "12345",
                Nombre = "Producto Test",
                Marca = "Marca Test",
                Categoria = "Categoria Test",
                Precio = 10.5m
            };

            // Arrange
            int usuarioId = 1;
            int productoId = 100;
            string codigoBarra = "12345";
            string nombre = "Producto Test";
            string marca = "Marca Test";
            string categoria = "Categoria Test";
            decimal precio = 10.5m;

            _mockProductoService.Setup(service => service.CreateProducto(usuarioId, codigoBarra, nombre, marca, categoria, precio))
                .ThrowsAsync(new Exception("Error inesperado"));

            // Act
            var result = await _controller.Create(product);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            var jsonResponse = JObject.FromObject(statusCodeResult.Value);
            var message = jsonResponse["Message"].ToString();
            Assert.Equal("Error al crear el producto.", message);
        }

        [Fact]
        public async Task GetByUser_ShouldReturnOk_WhenProductsFound()
        {
            // Arrange
            int usuarioId = 1;
            var productos = new List<Producto>
    {
        new Producto { ProductoId = 1, Nombre = "Producto 1" },
        new Producto { ProductoId = 2, Nombre = "Producto 2" }
    };

            _mockProductoService.Setup(service => service.GetProduto(usuarioId))
                .ReturnsAsync(productos);

            // Act
            var result = await _controller.GetByUser(usuarioId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = okResult.Value as List<Producto>;
            Assert.Equal(2, value.Count);
            Assert.Equal("Producto 1", value[0].Nombre);
            Assert.Equal("Producto 2", value[1].Nombre);
        }

        [Fact]
        public async Task GetByUser_ShouldReturnNotFound_WhenNoProductsFound()
        {
            // Arrange
            int usuarioId = 1;
            var productos = new List<Producto>();

            _mockProductoService.Setup(service => service.GetProduto(usuarioId))
                .ReturnsAsync(productos);

            // Act
            var result = await _controller.GetByUser(usuarioId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var jsonResponse = JObject.FromObject(notFoundResult.Value);
            var message = jsonResponse["Message"].ToString();
            Assert.Equal($"No se encontraron productos para el usuario con ID {usuarioId}.", message);
        }

        [Fact]
        public async Task GetByUser_ShouldReturnInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            int usuarioId = 1;

            _mockProductoService.Setup(service => service.GetProduto(usuarioId))
                .ThrowsAsync(new Exception("Error inesperado"));

            // Act
            var result = await _controller.GetByUser(usuarioId);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            var jsonResponse = JObject.FromObject(statusCodeResult.Value); var message = jsonResponse["Message"].ToString();
            var details = jsonResponse["Details"].ToString();

            Assert.Equal("Error al obtener los productos.", message);
            Assert.Equal("Error inesperado", details);
        }

        [Fact]
        public async Task Update_ShouldReturnOk_WhenProductUpdatedSuccessfully()
        {
            // Arrange
            int usuarioId = 1;
            int productoId = 100;
            string codigoBarra = "12345";
            string nombre = "Producto actualizado";
            string marca = "Marca actualizada";
            string categoria = "Categoria actualizada";
            decimal precio = 15.0m;

            _mockProductoService.Setup(service => service.UpdateProducto(usuarioId, productoId, codigoBarra, nombre, marca, categoria, precio))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Update(usuarioId, productoId, codigoBarra, nombre, marca, categoria, precio);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var jsonResponse = JObject.FromObject(okResult.Value);
            var message = jsonResponse["Message"].ToString();
            Assert.Equal("Producto actualizado exitosamente.", message);
        }

        [Fact]
        public async Task Update_ShouldReturnForbidden_WhenUnauthorizedAccessExceptionIsThrown()
        {
            // Arrange
            int usuarioId = 1;
            int productoId = 100;

            _mockProductoService.Setup(service => service.UpdateProducto(usuarioId, productoId, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<decimal>()))
                .ThrowsAsync(new UnauthorizedAccessException("Acceso no autorizado"));

            // Act
            var result = await _controller.Update(usuarioId, productoId, "", "", "", "", 0);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(403, statusCodeResult.StatusCode);
            var jsonResponse = JObject.FromObject(statusCodeResult.Value);
            var message = jsonResponse["Message"].ToString();
            Assert.Equal("Acceso no autorizado", message);
        }
    }
}