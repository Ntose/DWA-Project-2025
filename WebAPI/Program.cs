using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using WebAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// ================================================
// Register DbContext with SQL Server
// ================================================
builder.Services.AddDbContext<HeritageDbContext>(options =>
	options.UseSqlServer(
		builder.Configuration.GetConnectionString("DefaultConnection")));

// ================================================
// Configure controllers and prevent JSON cycles
// ================================================
builder.Services.AddControllers()
	.AddJsonOptions(options =>
	{
		options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
		options.JsonSerializerOptions.WriteIndented = true;
	});

// ================================================
// Configure Swagger with JWT support
// ================================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new() { Title = "Cultural Heritage API", Version = "v1" });

	c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
	{
		Name = "Authorization",
		In = Microsoft.OpenApi.Models.ParameterLocation.Header,
		Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
		Scheme = "Bearer",
		BearerFormat = "JWT",
		Description = "Enter 'Bearer' followed by your JWT token."
	});

	c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
	{
		{
			new Microsoft.OpenApi.Models.OpenApiSecurityScheme
			{
				Reference = new Microsoft.OpenApi.Models.OpenApiReference
				{
					Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
					Id = "Bearer"
				}
			},
			Array.Empty<string>()
		}
	});
});

// ================================================
// Configure JWT authentication
// ================================================
var jwt = builder.Configuration.GetSection("Jwt");
var jwtKey = jwt["Key"];
var jwtIssuer = jwt["Issuer"];
var jwtAudience = jwt["Audience"];

builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
	options.RequireHttpsMetadata = false;
	options.SaveToken = true;
	options.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuer = true,
		ValidIssuer = jwtIssuer,
		ValidateAudience = true,
		ValidAudience = jwtAudience,
		ValidateIssuerSigningKey = true,
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
		ValidateLifetime = true,
		ClockSkew = TimeSpan.Zero
	};
});

builder.Services.AddAuthorization();

var app = builder.Build();

// ================================================
// Middleware pipeline
// ================================================
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
