using Xunit;
using Moq;
using System.Data.SqlClient;
using RETOPT.Infrastructure.Interface;
namespace RetoPtTests.Controllers
{
    
    public class UsuarioRepositoryTest
    {
        private readonly Mock<SqlConnection> _mockConnection;
        private readonly Mock<SqlCommand> _mockCommand;
        private readonly Mock<IUsuarioRepository> _usrurioRepositoryMock;

        public UsuarioRepositoryTest()
        {
            _mockConnection = new Mock<SqlConnection>();
            _mockCommand = new Mock<SqlCommand>();
            _usrurioRepositoryMock = new Mock<IUsuarioRepository>();
        }

        [Fact]
        public async Task CreateUser_ReturnsUserId_WhenSuccess()
        {
            // Arrange
            var mockConnectionString = "FakeConnectionString";
            var username = "testuser";
            var passwordHash = "hashedpassword";
            var email = "test@example.com";
            var expectedUserId = 123;

            var mockCommand = new Mock<SqlCommand>();
            mockCommand.Setup(cmd => cmd.ExecuteScalarAsync()).ReturnsAsync(expectedUserId);

            var mockConnection = new Mock<SqlConnection>();
            mockConnection.Setup(conn => conn.CreateCommand()).Returns(mockCommand.Object);

            var userRepository = new _usrurioRepositoryMock(mockConnectionString);

            // Act
            var result = await userRepository.CreateUser(username, passwordHash, email);

            // Assert
            Assert.Equal(expectedUserId, result);
        }
    }
}
