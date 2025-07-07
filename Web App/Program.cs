// File: WebApp/Program.cs

using System;
using System.Linq;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// 0) Read the WebAPI base URL from configuration
var baseUrl = builder.Configuration.GetValue<string>("WebAPI:BaseUrl");
if (string.IsNullOrWhiteSpace(baseUrl))
    throw new InvalidOperationException("Missing WebAPI:BaseUrl in configuration.");

if (!baseUrl.EndsWith("/"))
    baseUrl += "/";

// 1) HttpClient for authentication endpoints
builder.Services.AddHttpClient("AuthAPI", client =>
{
    client.BaseAddress = new Uri(baseUrl + "auth/");
    client.DefaultRequestHeaders.Accept.Add(
        new MediaTypeWithQualityHeaderValue("application/json"));
});

// 2) HttpClient for data endpoints
builder.Services.AddHttpClient("DataAPI", client =>
{
    client.BaseAddress = new Uri(baseUrl);
    client.DefaultRequestHeaders.Accept.Add(
        new MediaTypeWithQualityHeaderValue("application/json"));
});

// 3) Cookie-based authentication
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(opts =>
    {
        opts.LoginPath = "/Account/Login";
        opts.LogoutPath = "/Account/Logout";
        opts.AccessDeniedPath = "/Account/AccessDenied";
    });

// 4) MVC with Views, filtering out any controllers from the WebAPI assembly
builder.Services
    .AddControllersWithViews()
    .ConfigureApplicationPartManager(apm =>
    {
        // Remove the WebAPI assembly so its controllers are not loaded
        var partsToRemove = apm.ApplicationParts
            .Where(part => part.Name.Equals("WebAPI", StringComparison.OrdinalIgnoreCase))
            .ToList();
        foreach (var part in partsToRemove)
        {
            apm.ApplicationParts.Remove(part);
        }
    });

var app = builder.Build();

// 5) Middleware pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// 6) Default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();
