using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;
    public AuthController(IConfiguration config) => _config = config;

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginModel model)
    {
        // TODO: Replace with real user validation
        if (model.Username != "admin" || model.Password != "admin")
            return Unauthorized();

        var token = GenerateJwt(model.Username);
        return Ok(new { Token = token });
    }

    private string GenerateJwt(string username)
    {
        var jwtSection = _config.GetSection("Jwt");
        var keyBytes = Encoding.UTF8.GetBytes(jwtSection["Key"]);
        var creds = new SigningCredentials(
                           new SymmetricSecurityKey(keyBytes),
                           SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
      new Claim(JwtRegisteredClaimNames.Sub, username),
      new Claim(ClaimTypes.Name, username),
      new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

        var token = new JwtSecurityToken(
          issuer: jwtSection["Issuer"],
          audience: jwtSection["Audience"],
          claims: claims,
          expires: DateTime.UtcNow.AddMinutes(
                      double.Parse(jwtSection["ExpiryMinutes"])),
          signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public class LoginModel
{
    public string Username { get; set; }
    public string Password { get; set; }
}
