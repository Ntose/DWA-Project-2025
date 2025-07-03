using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using WebApp.ViewModels;

public class AuthService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public AuthService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<TokenResponse?> LoginAsync(string username, LoginVm loginVm)
    {
        var client = _httpClientFactory.CreateClient("ApiClient");

        var response = await client.PostAsJsonAsync("api/Auth/login", new
        {
            username = loginVm.Username,
            password = loginVm.Password
        });

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<TokenResponse>();
        }

        return null;
    }
}
