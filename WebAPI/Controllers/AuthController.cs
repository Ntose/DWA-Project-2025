using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebAPI.Data;
using WebAPI.Data.Entities;

namespace WebAPI.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AuthController : ControllerBase
	{
		private readonly HeritageDbContext _context;
		private readonly IConfiguration _config;

		public AuthController(HeritageDbContext ctx, IConfiguration cfg)
		{
			_context = ctx;
			_config = cfg;
		}

		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] RegisterRequest model)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			if (!await _context.Database.CanConnectAsync())
				return StatusCode(503, "DB unavailable");

			// ensure unique username/email
			var dup = await _context.ApplicationUser
				.AnyAsync(u => u.Username == model.Username || u.Email == model.Email);
			if (dup)
				return BadRequest("Username or email already exists.");

			var user = new ApplicationUser
			{
				Username = model.Username,
				Email = model.Email,
				PasswordHash = model.Password,   // plain-text for demo
				FirstName = model.FirstName,
				LastName = model.LastName,
				Phone = model.Phone,
				DateRegistered = DateTime.UtcNow
			};

			_context.ApplicationUser.Add(user);
			await _context.SaveChangesAsync();

			return Ok(new
			{
				user.Id,
				user.Username,
				user.Email,
				user.FirstName,
				user.LastName,
				user.Phone
			});
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginRequest model)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var user = await _context.ApplicationUser
				.FirstOrDefaultAsync(u =>
					u.Username == model.Username &&
					u.PasswordHash == model.Password);

			if (user == null)
				return Unauthorized("Invalid credentials");

			var token = GenerateJwtToken(user);
			return Ok(new { token });
		}

		private string GenerateJwtToken(ApplicationUser u)
		{
			var claims = new[]
			{
				new Claim(ClaimTypes.NameIdentifier, u.Id.ToString()),
				new Claim(ClaimTypes.Name, u.Username),
				new Claim(ClaimTypes.Email, u.Email)
			};

			var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);
			var creds = new SigningCredentials(
				new SymmetricSecurityKey(key),
				SecurityAlgorithms.HmacSha256);

			var jwt = new JwtSecurityToken(
				issuer: _config["Jwt:Issuer"],
				audience: _config["Jwt:Audience"],
				claims: claims,
				expires: DateTime.UtcNow.AddHours(3),
				signingCredentials: creds);

			return new JwtSecurityTokenHandler().WriteToken(jwt);
		}
	}

	public class RegisterRequest
	{
		[Required] public string Username { get; set; }
		[Required]
		[EmailAddress]
		public string Email { get; set; }
		[Required] public string Password { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Phone { get; set; }
	}

	public class LoginRequest
	{
		[Required] public string Username { get; set; }
		[Required] public string Password { get; set; }
	}
}
