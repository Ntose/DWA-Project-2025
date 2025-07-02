using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;
using WebAPI;
using WebAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// 1) DbContext
builder.Services.AddDbContext<HeritageDbContext>(opts =>
	opts.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2) Controllers + JSON options
builder.Services.AddControllers()
	.AddJsonOptions(o =>
	{
		o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
		o.JsonSerializerOptions.WriteIndented = true;
	});

// 3) AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// 4) Swagger + JWT setup
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
	// Define the BearerAuth scheme that's in use
	c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
	{
		Description = "JWT Authorization header using the Bearer scheme. " +
					  "Enter 'Bearer' [space] and then your token.",
		Name = "Authorization",
		In = ParameterLocation.Header,
		Type = SecuritySchemeType.ApiKey,
		Scheme = "Bearer"
	});

	// Require the BearerAuth scheme for all endpoints
	c.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{
			new OpenApiSecurityScheme
			{
				Reference = new OpenApiReference
				{
					Id   = "Bearer",
					Type = ReferenceType.SecurityScheme
				}
			},
			new List<string>()
		}
	});
});

// 5) JWT Authentication
var jwtSection = builder.Configuration.GetSection("Jwt");
var keyBytes = Encoding.UTF8.GetBytes(jwtSection["Key"]);

builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(opts =>
{
	opts.RequireHttpsMetadata = false;
	opts.SaveToken = true;
	opts.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuerSigningKey = true,
		IssuerSigningKey = new SymmetricSecurityKey(keyBytes),

		ValidateIssuer = true,
		ValidIssuer = jwtSection["Issuer"],

		ValidateAudience = true,
		ValidAudience = jwtSection["Audience"],

		ValidateLifetime = true,
		ClockSkew = TimeSpan.Zero
	};
});

builder.Services.AddAuthorization();

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
