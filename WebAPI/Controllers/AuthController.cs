// File: Controllers/AuthController.cs

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Data;
using WebAPI.Data.Entities;
using WebAPI.Dtos.Auth;


namespace WebAPI.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AuthController : ControllerBase
	{
		private readonly HeritageDbContext _context;
		private readonly IConfiguration _config;

		public AuthController(HeritageDbContext context, IConfiguration config)
		{
			_context = context;
			_config = config;
		}

		/// <summary>
		/// Registers a new user with the role "User".
		/// </summary>
		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] Dtos.Auth.RegisterRequest model)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			// Check for existing username or email
			bool exists = await _context.ApplicationUser
				.AnyAsync(u => u.Username == model.Username || u.Email == model.Email);
			if (exists)
				return BadRequest("Username or email already in use.");

			var user = new ApplicationUser
			{
				Username = model.Username,
				Email = model.Email,
				PasswordHash = model.Password,   // Plain text for demo; hash in production
				FirstName = model.FirstName,
				LastName = model.LastName,
				Phone = model.Phone,
				DateRegistered = DateTime.UtcNow,
				Role = "User"
			};

			_context.ApplicationUser.Add(user);
			await _context.SaveChangesAsync();

			// Optionally log the registration
			_context.Log.Add(new Log
			{
				Level = "Info",
				Message = $"New user registered with id={user.Id}, username={user.Username}."
			});
			await _context.SaveChangesAsync();

			return Ok(new
			{
				user.Id,
				user.Username,
				user.Email,
				user.Role
			});
		}

		/// <summary>
		/// Authenticates a user and returns a JWT.
		/// </summary>
		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] Dtos.Auth.LoginRequest model)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var user = await _context.ApplicationUser
				.FirstOrDefaultAsync(u =>
					u.Username == model.Username &&
					u.PasswordHash == model.Password);
			if (user == null)
				return Unauthorized("Invalid credentials.");

			string token = GenerateJwtToken(user);
			return Ok(new { token });
		}

		/// <summary>
		/// Changes the password of the currently authenticated user.
		/// </summary>
		[HttpPost("changepassword")]
		[Authorize]
		public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest model)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			// Get userId from JWT
			string? sid = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (sid == null || !int.TryParse(sid, out var userId))
				return Unauthorized();

			var user = await _context.ApplicationUser.FindAsync(userId);
			if (user == null)
				return NotFound("User not found.");

			if (user.PasswordHash != model.OldPassword)
				return BadRequest("Old password is incorrect.");

			user.PasswordHash = model.NewPassword;
			await _context.SaveChangesAsync();

			_context.Log.Add(new Log
			{
				Level = "Info",
				Message = $"User with id={user.Id} changed password."
			});
			await _context.SaveChangesAsync();

			return NoContent();
		}

		// ===== Helper =====

		/// <summary>
		/// Generates a JWT for the given user.
		/// </summary>
		private string GenerateJwtToken(ApplicationUser user)
		{
			var jwtSection = _config.GetSection("Jwt");
			var keyBytes = Encoding.UTF8.GetBytes(jwtSection["Key"]!);

			var creds = new SigningCredentials(
				new SymmetricSecurityKey(keyBytes),
				SecurityAlgorithms.HmacSha256);

			var claims = new[]
			{
				new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
				new Claim(ClaimTypes.Name,           user.Username),
				new Claim(ClaimTypes.Email,          user.Email),
				new Claim(ClaimTypes.Role,           user.Role)
			};

			var token = new JwtSecurityToken(
				issuer: jwtSection["Issuer"],
				audience: jwtSection["Audience"],
				claims: claims,
				expires: DateTime.UtcNow.AddHours(3),
				signingCredentials: creds);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}
	}
}