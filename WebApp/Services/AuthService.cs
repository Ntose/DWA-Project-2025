using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

public class AuthService
{
    private readonly IHttpClientFactory _http;
    public AuthService(IHttpClientFactory http) => _http = http;

    public async Task<string> LoginAsync(string user, string pass)
    {
        var client = _http.CreateClient("ApiClient");
        var json = JsonSerializer.Serialize(new { Username = user, Password = pass });
        var resp = await client.PostAsync(
                        "/api/auth/login",
                        new StringContent(json, Encoding.UTF8, "application/json"));

        if (!resp.IsSuccessStatusCode) return null;
        using var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
        return doc.RootElement.GetProperty("Token").GetString();
    }

    public void AttachToken(HttpClient client, string token)
    {
        client.DefaultRequestHeaders.Authorization =
          new AuthenticationHeaderValue("Bearer", token);
    }
}
