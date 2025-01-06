using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Newtonsoft.Json.Linq;
using RETOPT.API.Controllers;
using RETOPT.Application.DTOs;
using RETOPT.Application.Interface;
using RETOPT.Domain.Entity;
using System.Dynamic;
using Xunit;

namespace RetoPtTests.Controllers
{   
    public class UsuarioControllerTests
    {

        private readonly Mock<IUsuarioService> _mockService;
        private readonly IConfiguration _configuration;
        private readonly UsuarioController _controller;

        public UsuarioControllerTests()
        {
            // Configura el mock del servicio
            _mockService = new Mock<IUsuarioService>();

            // Configura la carga de la configuración desde appsettings.json
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())  // Establece el directorio base (asegurando que cargue appsettings.json)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            _configuration = builder.Build();

            // Inicializa el controlador con las dependencias mockeadas
            _controller = new UsuarioController(_mockService.Object, _configuration);
        }

        [Fact]
        public async Task LoginUser_ShouldReturnOk_WithToken_WhenValidCredentials()
        {
            // Configura los valores de entrada
            var Usuario = new LoginDto()
            {
                Username = "joel",
                Password = "123456"
            };

            //var userName = "joel";
            //var password = "123456";

            // Configura el comportamiento del mock para simular que el usuario fue autenticado correctamente
            _mockService.Setup(service => service.AuthenticateUser(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new Usuario { Username = "joel" });

            // Actúa: Llama al método LoginUser
            var result = await _controller.Login(Usuario);

            // Verifica que el resultado no sea nulo
            Assert.NotNull(result);

            // Verifica que el resultado sea de tipo OkObjectResult
            var okResult = Assert.IsType<OkObjectResult>(result);

            // Verifica que el resultado tenga la estructura esperada
            var response = Assert.IsType<ExpandoObject>(okResult.Value);

            // Accede a las propiedades del ExpandoObject
            var dictionaryResponse = response as IDictionary<string, object>;

            // Verifica que el "result" no esté vacío (el token)
            var token = dictionaryResponse["result"] as string;
            Assert.NotNull(token);
            Assert.NotEmpty(token);
        }

        [Fact]
        public async Task LoginUser_ShouldReturnUnauthorized_WhenInvalidCredentials()
        {
            // Configura los valores de entrada
            var Usuario = new LoginDto()
            {
                Username = "joel",
                Password = "555555"
            };


            // Configura el comportamiento del mock para simular que el usuario no fue encontrado
            _mockService.Setup(service => service.AuthenticateUser(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((Usuario)null); // Null indica que el usuario no fue encontrado o las credenciales no son válidas

            // Actúa: Llama al método LoginUser
            var result = await _controller.Login(Usuario);

            // Verifica que el resultado sea un Unauthorized (401)
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);

            // Verificar el mensaje de error
            Assert.Equal("Invalid credentials.", unauthorizedResult.Value);
        }

        [Fact]
        public async Task CreateUsuario_ShouldReturnOk_WhenUserCreatedSuccessfully()
        {
            // Configura los valores de entrada
            var userName = "newUser";
            var password = "password123";
            var email = "newuser@example.com";

            // Configura el comportamiento del mock para simular que el usuario se crea correctamente
            var mockUserId = 123;
            _mockService.Setup(service => service.CreateUser(userName, password, email))
                        .ReturnsAsync(mockUserId);

            // Actúa: Llama al método CreateUsuario
            var result = await _controller.CreateUsuario(userName, password, email);

            // Verifica que el resultado sea un OkObjectResult (200)
            var okResult = Assert.IsType<OkObjectResult>(result);

            // Verifica el valor de la respuesta
            var response = okResult.Value;

            // Asegúrate de que la respuesta no sea null
            Assert.NotNull(response);

            // Convertir la respuesta a JObject para acceder a las propiedades dinámicamente
            var responseObject = JObject.FromObject(response);

            // Verifica las propiedades de la respuesta usando JObject
            Assert.Equal("El usuario se ha creado correctamente.", responseObject["Message"].ToString());
            Assert.Equal(mockUserId, (int)responseObject["UserId"]);
        }

        [Fact]
        public async Task CreateUsuario_ShouldReturnBadRequest_WhenExceptionOccurs()
        {
            // Configura los valores de entrada
            var userName = "newUser";
            var password = "password123";
            var email = "newuser@example.com";

            // Configura el comportamiento del mock para simular una excepción durante la creación del usuario
            _mockService.Setup(service => service.CreateUser(userName, password, email))
                        .ThrowsAsync(new Exception("Error al crear el usuario"));

            // Actúa: Llama al método CreateUsuario
            var result = await _controller.CreateUsuario(userName, password, email);

            // Verifica que el resultado sea un BadRequestObjectResult (400)
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);

            // Verifica el valor de la respuesta
            var response = badRequestResult.Value;

            // Agregar impresión para depurar el contenido de la respuesta
            Console.WriteLine(response); // Esto debería mostrar el contenido de la respuesta

            // Verifica que la respuesta no sea null
            Assert.NotNull(response);

            // Convertir la respuesta a JObject
            var responseObject = JObject.FromObject(response);

            // Verifica las propiedades de la respuesta usando JObject
            Assert.Equal("Ocurrió un error al crear el usuario.", responseObject["Message"].ToString());
            Assert.Equal("Error al crear el usuario", responseObject["Details"].ToString());
        }
    }
}
