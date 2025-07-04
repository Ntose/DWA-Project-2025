using System.Text.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using Microsoft.AspNetCore.Authorization;

namespace Web_App.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _http;

        public AccountController(IHttpClientFactory http)
            => _http = http;

        [HttpGet, AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
            => View(new LoginViewModel { ReturnUrl = returnUrl });

        [HttpPost, AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var client = _http.CreateClient("WebAPI");
            var payload = new { vm.Username, vm.Password };
            var resp = await client.PostAsJsonAsync("login", payload);

            if (!resp.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Invalid credentials");
                return View(vm);
            }

            // API returns { "token":"…jwt…"}
            using var doc = await JsonDocument.ParseAsync(await resp.Content.ReadAsStreamAsync());
            var token = doc.RootElement.GetProperty("token").GetString();

            // create a claims principal and sign in
            var claims = new List<Claim> {
                new Claim(ClaimTypes.Name, vm.Username),
                new Claim("JWT", token)
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = vm.RememberMe
                });

            if (string.IsNullOrEmpty(vm.ReturnUrl) || !Url.IsLocalUrl(vm.ReturnUrl))
                return RedirectToAction("Index", "Home");

            return LocalRedirect(vm.ReturnUrl);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
        [HttpGet, AllowAnonymous]
        public IActionResult Register(string returnUrl = null)
       => View(new RegisterViewModel { ReturnUrl = returnUrl });

        [HttpPost, AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var client = _http.CreateClient("WebAPI");
            var payload = new
            {
                Username = vm.Username,
                FirstName = vm.FirstName,
                LastName = vm.LastName,
                Phone = vm.Phone,
                Email = vm.Email,
                Password = vm.Password,
                ConfirmPassword = vm.ConfirmPassword
            };

            var resp = await client.PostAsJsonAsync("register", payload);

            if (!resp.IsSuccessStatusCode)
            {
                // Read and display API validation errors
                var problem = await resp.Content.ReadFromJsonAsync<ValidationProblemDetails>();
                foreach (var kv in problem.Errors)
                    ModelState.AddModelError(kv.Key, kv.Value.First());
                return View(vm);
            }

            return RedirectToAction("Login", new { returnUrl = vm.ReturnUrl });
        }
    }
}
