using Microsoft.AspNetCore.Authorization;
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
                return Conflict(new { message = "User with this email already exists." });

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User registered successfully." });
        }

        // ✅ Login Endpoint
        [HttpPost("login")]
        public async Task<IActionResult> Login(User user)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u =>
                u.Email == user.Email && u.Password == user.Password);

            if (existingUser == null)
                return Unauthorized(new { message = "Invalid email or password." });

            var token = GenerateJwtToken(user.Email);

            return Ok(new
            {
                message = "Login successful",
                token = token
            });
        }

        // 🔐 Generate JWT
        private string GenerateJwtToken(string email)
        {
            var claims = new[] { new Claim(ClaimTypes.Name, email) };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(60),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // ✅ Secure Data Endpoint
        [HttpGet("secure-data")]
        [Authorize]
        public IActionResult GetSecureData()
        {
            return Ok(new { message = "You are authorized to access this secure endpoint." });
        }

        // ====================
        // 🔄 CRUD ENDPOINTS
        // ====================

        // ✅ GET ALL USERS
        [HttpGet]
       // [Authorize]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }
        // ✅ CREATE USER (POST: /api/Users)
        [HttpPost]
        public async Task<IActionResult> CreateUser(User user)
        {
            if (user == null || string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Password))
            {
                return BadRequest(new { message = "Email and password are required." });
            }

            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
            if (existingUser != null)
            {
                return Conflict(new { message = "User already exists." });
            }

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(user); // Or return CreatedAtAction if you want to return a Location header.
        }


        // ✅ GET USER BY ID
        [HttpGet("{id}")]
        //[Authorize]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            return user;
        }

        // ✅ UPDATE USER
        [HttpPut("{id}")]
       // [Authorize]
        public async Task<IActionResult> UpdateUser(int id, User updatedUser)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.Email = updatedUser.Email;
            user.Password = updatedUser.Password;

            await _context.SaveChangesAsync();
            return Ok(new { message = "User updated successfully." });
        }

        // ✅ DELETE USER
        [HttpDelete("{id}")]
        //[Authorize]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User deleted successfully." });
        }
    }
}
