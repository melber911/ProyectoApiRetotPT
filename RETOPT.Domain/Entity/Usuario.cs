using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RETOPT.Domain.Entity
{
    public class Usuario
    {
        public int UsuarioID { get; set; }
        public string Username { get; set; }
        public string PassHash { get; set; }
        public string Email { get; set; }
        public DateTime FechaCrea { get; set; }

    }
}
