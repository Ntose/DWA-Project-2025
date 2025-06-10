using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
	private readonly IConfiguration _configuration;

	public AuthController(IConfiguration configuration)
	{
		_configuration = configuration;
	}

	// Create a simple LoginModel to capture login credentials.
	public class LoginModel
	{
		public string Username { get; set; }
		public string Password { get; set; }
	}

	[HttpPost("login")]
	public IActionResult Login([FromBody] LoginModel model)
	{
		// Replace this with your actual authentication logic.
		// Here we simply check against hardcoded admin credentials.
		if (model.Username == "admin" && model.Password == "adminPassword")
		{
			var token = GenerateJwtToken(model.Username);
			return Ok(new { token });
		}
		return Unauthorized("Invalid credentials");
	}

	private string GenerateJwtToken(string username)
	{
		var jwtSettings = _configuration.GetSection("JWT");
		var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

		// Create claims; add a claim to define the administrator role.
		var claims = new[]
		{
			new Claim(JwtRegisteredClaimNames.Sub, username),
			new Claim(ClaimTypes.Role, "Administrator"),
			new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
		};

		var creds = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

		var token = new JwtSecurityToken(
			issuer: jwtSettings["Issuer"],
			audience: jwtSettings["Audience"],
			claims: claims,
			expires: DateTime.UtcNow.AddHours(Convert.ToDouble(jwtSettings["ExpiresInHours"])),
			signingCredentials: creds
		);

		return new JwtSecurityTokenHandler().WriteToken(token);
	}
}
