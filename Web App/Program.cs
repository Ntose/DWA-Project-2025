// File: WebApp/Program.cs

using System;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// 0. grab the one URI & guard
var baseUrl = builder.Configuration.GetValue<string>("WebAPI:BaseUrl");
if (string.IsNullOrWhiteSpace(baseUrl))
    throw new InvalidOperationException("Missing WebAPI:BaseUrl in configuration.");

if (!baseUrl.EndsWith("/"))
    baseUrl += "/";

// 1. AuthAPI for /api/auth/*
builder.Services.AddHttpClient("AuthAPI", client =>
{
    client.BaseAddress = new Uri(baseUrl + "auth/");
    client.DefaultRequestHeaders.Accept.Add(
        new MediaTypeWithQualityHeaderValue("application/json"));
});

// 2. DataAPI for /api/*
builder.Services.AddHttpClient("DataAPI", client =>
{
    client.BaseAddress = new Uri(baseUrl);
    client.DefaultRequestHeaders.Accept.Add(
        new MediaTypeWithQualityHeaderValue("application/json"));
});

// 3. cookie auth
builder.Services
  .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
  .AddCookie(opts =>
  {
      opts.LoginPath = "/Account/Login";
      opts.LogoutPath = "/Account/Logout";
      opts.AccessDeniedPath = "/Account/AccessDenied";
  });

// 4. MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();
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
app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
app.Run();
