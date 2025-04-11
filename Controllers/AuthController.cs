using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GCRAS.Models; // Asegúrate de que este namespace coincida con la ubicación de tu modelo User
using System;
using Microsoft.Extensions.Configuration;

namespace GCRAS.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Define la ruta base para este controlador, ej: /api/auth
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration; // Inyección de dependencia para acceder a la configuración (como la clave secreta de JWT)
        private readonly AppDbContext _context; // Inyección de dependencia para acceder a la base de datos (si la estás utilizando)
        private readonly HttpClient _httpClient; // Cliente HTTP para hacer peticiones a la API externa
        // Constructor del controlador, recibe las dependencias necesarias
        public AuthController(IConfiguration configuration, AppDbContext context, HttpClient httpClient)
        {
            _configuration = configuration;
            _context = context;
            _httpClient = httpClient;
        }

        // Endpoint para el inicio de sesión de usuarios registrados
        [HttpPost("login")] // Responde a las peticiones HTTP POST en la ruta /api/auth/login
        public async Task<IActionResult> Login([FromBody] UserLoginModel model) // Recibe un objeto UserLoginModel en el cuerpo de la petición
        {
            // Simulación de la llamada a la API externa para autenticar usuarios registrados
            if (model.Username == "usuarioRegistrado" && model.Password == "password")
            {
                // Crear un objeto de usuario (esto en una aplicación real vendría de la base de datos)
                var user = new User { Id = 1, Username = model.Username, Role = "UsuarioRegistrado" };
                // Generar un token JWT para el usuario autenticado
                var token = GenerateJwtToken(user);
                // Devolver una respuesta OK (código 200) con el token generado
                return Ok(new { Token = token });
            }
            else if (model.Username == "admin" && model.Password == "admin")
            {
                // Crear un objeto de usuario administrador (esto en una aplicación real vendría de la base de datos)
                var user = new User { Id = 2, Username = model.Username, Role = "Admin" };
                // Generar un token JWT para el usuario administrador
                var token = GenerateJwtToken(user);
                // Devolver una respuesta OK (código 200) con el token generado
                return Ok(new { Token = token });
            }

            // Si las credenciales no coinciden con ninguna de las simulaciones, devolver un error de no autorizado (código 401)
            return Unauthorized("Credenciales inválidas");
        }

        // Endpoint para la creación de usuarios temporales
        [HttpPost("temporary")] // Responde a las peticiones HTTP POST en la ruta /api/auth/temporary
        public IActionResult CreateTemporaryUser()
        {
            // Generar un nombre de usuario temporal único usando un GUID
            string temporaryUsername = "temp_" + Guid.NewGuid().ToString().Substring(0, 8);

            // Crear un nuevo objeto de usuario temporal
            var temporaryUser = new User
            {
                Username = temporaryUsername,
                Role = "UsuarioTemporal", // Asignar un rol específico para usuarios temporales
                IsTemporary = true // Marcar al usuario como temporal para diferenciarlo de los usuarios registrados
            };

            // Generar un token JWT para el usuario temporal creado
            var token = GenerateJwtToken(temporaryUser);

            // Devolver una respuesta OK (código 200) con el nombre de usuario temporal y el token generado
            return Ok(new { Username = temporaryUsername, Token = token });
        }

        // Método privado para generar un token JWT a partir de un objeto de usuario
        private string GenerateJwtToken(User user)
        {
            // Crear una lista de "claims" (afirmaciones) que se incluirán en el token
            var claims = new List<Claim>
            {
                // El NameIdentifier claim usualmente contiene el ID del usuario
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                // El Name claim contiene el nombre de usuario
                new Claim(ClaimTypes.Name, user.Username),
                // El Role claim contiene el rol del usuario
                new Claim(ClaimTypes.Role, user.Role)
            };

            // Obtener la clave secreta para firmar el token desde la configuración
            // Si no se encuentra en la configuración, se utiliza una clave por defecto (¡NO USAR EN PRODUCCIÓN!)
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"] ?? "tu_clave_secreta_super_larga"));
            // Crear las credenciales de firma utilizando la clave y el algoritmo de firma
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Describir cómo se generará el token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims), // Asignar las claims al token
                Expires = DateTime.UtcNow.AddHours(1), // Establecer la duración del token (en este caso, 1 hora)
                SigningCredentials = creds, // Asignar las credenciales de firma
                // Obtener el emisor y la audiencia del token desde la configuración (buenas prácticas)
                Issuer = _configuration["Jwt:Issuer"] ?? "tu_emisor",
                Audience = _configuration["Jwt:Audience"] ?? "tu_audiencia"
            };

            // Crear un manejador de tokens JWT
            var tokenHandler = new JwtSecurityTokenHandler();
            // Crear el token JWT basado en la descripción
            var token = tokenHandler.CreateToken(tokenDescriptor);
            // Escribir el token en formato string para enviarlo en la respuesta
            return tokenHandler.WriteToken(token);
        }
    }

    // Clase modelo para recibir las credenciales de inicio de sesión
    public class UserLoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}