using Microsoft.AspNetCore.Authentication.Cookies;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

// (Optional) If you want to force your MVC app to listen on a specific URL,
// uncomment the next line. Otherwise, rely on launchSettings.json.
// builder.WebHost.UseUrls("http://localhost:5001");

// 1) Make HttpContext available in Views/Layouts
builder.Services.AddHttpContextAccessor();

// 2) Register a named HttpClient for talking to your Web API
builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"]);
    client.DefaultRequestHeaders.Accept
          .Add(new MediaTypeWithQualityHeaderValue("application/json"));
});

// 3) Your custom service that wraps login/attach-token logic
builder.Services.AddTransient<AuthService>();

// 4) Configure Cookie Authentication
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.ExpireTimeSpan = TimeSpan.FromHours(1);
        options.SlidingExpiration = true;
    });

// 5) Add MVC controllers with views
builder.Services.AddControllersWithViews();

var app = builder.Build();

// 6) Error handling & HTTPS redirection
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// 7) Enable auth middleware
app.UseAuthentication();
app.UseAuthorization();

// 8) Default route: HomeController → Index
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();
