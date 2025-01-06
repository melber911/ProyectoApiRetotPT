using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RETOPT.Infrastructure.Data
{
    public class ConnectionService
    {
        private readonly IConfiguration _configuration;
        public ConnectionService(IConfiguration configuration)        
        { 
            _configuration = configuration;
        }
        public string GetConnection() 
        {
            return _configuration.GetConnectionString("ConnectionSQL");
        }
    }
}
