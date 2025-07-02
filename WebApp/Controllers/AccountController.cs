using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using WebApp.ViewModels;

public class AccountController : Controller
{
    private readonly IHttpClientFactory _httpFactory;

    public AccountController(IHttpClientFactory httpFactory)
    {
        _httpFactory = httpFactory;
    }

    // GET: /Account/Login
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    // POST: /Account/Login
    [HttpPost]
    public async Task<IActionResult> Login(LoginVm vm, string? returnUrl = null)
    {
        if (!ModelState.IsValid) return View(vm);

        var client = _httpFactory.CreateClient("ApiClient");

        var json = JsonSerializer.Serialize(vm);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await client.PostAsync("/api/auth/login", content);

        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError("", "Invalid username or password.");
            return View(vm);
        }

        var body = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<AuthResultVm>(body,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (result is null || string.IsNullOrEmpty(result.Token))
        {
            ModelState.AddModelError("", "Login failed.");
            return View(vm);
        }

        // Parse token to extract claims
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(result.Token);

        var claims = jwt.Claims.ToList();
        claims.Add(new Claim("access_token", result.Token));

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            principal, new AuthenticationProperties { IsPersistent = vm.RememberMe });

        return RedirectToLocal(returnUrl);
    }

    // GET: /Account/Register
    [HttpGet]
    public IActionResult Register() => View();

    // POST: /Account/Register
    [HttpPost]
    public async Task<IActionResult> Register(RegisterVm vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var client = _httpFactory.CreateClient("ApiClient");

        var json = JsonSerializer.Serialize(vm);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await client.PostAsync("/api/auth/register", content);

        if (response.IsSuccessStatusCode)
            return RedirectToAction(nameof(Login));

        var error = await response.Content.ReadAsStringAsync();
        ModelState.AddModelError("", error ?? "Registration failed.");
        return View(vm);
    }

    // POST: /Account/Logout
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    private IActionResult RedirectToLocal(string? returnUrl)
    {
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectToAction("Index", "Home");
    }
}
