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

namespace RetoPtTests.Controllers
{
    public class ProductoControllerTest
    {
        private readonly Mock<IProductoService> _productoServiceMock;
        private readonly ProductoController _productoController;

        public ProductoControllerTest()
        {
            // Setup Mock
            _productoServiceMock = new Mock<IProductoService>();
            _productoController = new ProductoController(_productoServiceMock.Object);
        }
        [Fact]
        public async Task Create_ShouldReturnCreatedAtAction_WhenProductIsCreated()
        {
            // Aun no funciona
            int usuarioId = 1;
            var productoId = 123;
            _productoServiceMock.Setup(service => service.CreateProducto(usuarioId, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<decimal>()))
                                .ReturnsAsync(productoId);

            // Act
            var result = await _productoController.Create(usuarioId, "123", "ProductoTest", "MarcaTest", "CategoriaTest", 100m);

            // Assert
            var actionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(201, actionResult.StatusCode);
            Assert.Equal("Producto creado exitosamente.", ((dynamic)actionResult.Value).Message);
            Assert.Equal(productoId, ((dynamic)actionResult.Value).ProductoId);
        }

    }
}