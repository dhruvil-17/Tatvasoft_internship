using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserApi.Data;
using UserApi.Models;

namespace UserApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserDbContext _context;
        private readonly IConfiguration _config;

        public UsersController(UserDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // ✅ Register Endpoint
        [HttpPost("register")]
        public async Task<IActionResult> Register(User user)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
            if (existingUser != null)
            {
                return Conflict(new { message = "User with this email already exists." });
            }

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User registered successfully." });
        }

        // ✅ Login Endpoint with JWT Token
        [HttpPost("login")]
        public async Task<IActionResult> Login(User user)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u =>
                u.Email == user.Email && u.Password == user.Password);

            if (existingUser == null)
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }

            // 🔐 Generate JWT Token
            var token = GenerateJwtToken(user.Email);

            return Ok(new
            {
                message = "Login successful",
                token = token
            });
        }

        // 🔐 JWT Generator Method
        private string GenerateJwtToken(string email)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(60),
                signingCredentials: creds
            );
            Console.WriteLine(token);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // ✅ Example of protected endpoint
        [HttpGet("secure-data")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public IActionResult GetSecureData()
        {
            return Ok(new { message = "You are authorized to access this secure endpoint." });
        }
    }
}
