using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RETOPT.Application.DTOs;
using RETOPT.Application.Interface;
using RETOPT.Domain.Entity;
using System.Dynamic;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace RETOPT.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _service;
        private readonly IConfiguration _configuration;
        public UsuarioController(IUsuarioService service, IConfiguration configuration)
        {
            _service = service;
            _configuration = configuration;
        }
        // POST: api/Crear User
        [HttpPost("Create")]
        public async Task<IActionResult> CreateUsuario(string UserName, string Password, string Email)
        {
            try
            {
                var userId = await _service.CreateUser(UserName, Password, Email);

                return Ok(new
                {
                    Message = "El usuario se ha creado correctamente.",
                    UserId = userId
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Ocurrió un error al crear el usuario.", Details = ex.Message });
            }
        }

        // POST: api/Users/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _service.AuthenticateUser(dto.Username, dto.Password);
            if (user == null)
                return Unauthorized("Invalid credentials.");

            var token = GenerateJwtToken(user);
            var response = new ExpandoObject() as IDictionary<string, object>;
            response["result"] = token;

            return Ok(response);
        }

        private string GenerateJwtToken(Usuario user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.UsuarioID.ToString()),
                 new Claim(ClaimTypes.Sid, user.UsuarioID.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
