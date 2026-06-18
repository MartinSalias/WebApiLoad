using System.Net.Http.Json;
using System.Text.Json;
using ApiLoadTest.Models;

namespace ApiLoadTest.Services;

public static class AuthService
{
    public static async Task<LoginResponse> LoginAsync(LoadTestConfig config)
    {
        using var client = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(15)
        };

        var body = new
        {
            userName = config.UserName,
            password = config.Password,
            companyVATid = config.CompanyVATid
        };

        var response = await client.PostAsJsonAsync(config.LoginUrl, body);
        response.EnsureSuccessStatusCode();

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var loginResponse = await response.Content
            .ReadFromJsonAsync<LoginResponse>(options);

        return loginResponse
            ?? throw new InvalidOperationException("Login returned an empty response.");
    }
}
