using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using WebApp.Models;
using WebApp.ViewModels;

public class AccountController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public AccountController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet]
    public IActionResult Login(string returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginVM vm, string returnUrl = null)
    {
        if (!ModelState.IsValid)
            return View(vm);

        var client = _httpClientFactory.CreateClient("ApiClient");
        var payload = JsonConvert.SerializeObject(new
        {
            username = vm.Username,
            password = vm.Password
        });

        var resp = await client.PostAsync(
            "/api/Auth/login",
            new StringContent(payload, Encoding.UTF8, "application/json")
        );

        if (!resp.IsSuccessStatusCode)
        {
            ModelState.AddModelError("", "Invalid credentials");
            return View(vm);
        }

        var json = await resp.Content.ReadAsStringAsync();
        var authRes = JsonConvert.DeserializeObject<AuthResponse>(json);

        // 1) Read JWT to extract claims
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(authRes.Token);
        var claims = jwtToken.Claims;

        // 2) Create identity & sign-in
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = vm.RememberMe,
                ExpiresUtc = authRes.Expiration
            }
        );

        if (string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
            return RedirectToAction("Index", "Home");

        return LocalRedirect(returnUrl);
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }
}
