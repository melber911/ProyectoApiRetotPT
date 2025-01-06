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
    }
}
