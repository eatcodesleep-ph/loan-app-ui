using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace LoanApp.Web.Services;

public interface ITokenService
{
    Task<string> GetAccessTokenAsync(CancellationToken ct = default);
    Task<string> RefreshAccessTokenAsync(CancellationToken ct = default);
}

internal sealed class TokenService(HttpClient httpClient, IMemoryCache cache, IOptions<AuthOptions> options) : ITokenService
{
    private const string CacheKey = "api_jwt_access_token";
    private static readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web);

    public Task<string> GetAccessTokenAsync(CancellationToken ct = default)
    {
        if (cache.TryGetValue(CacheKey, out string? token) && !string.IsNullOrWhiteSpace(token))
            return Task.FromResult(token);
        return RefreshAccessTokenAndCacheAsync(ct);
    }

    public Task<string> RefreshAccessTokenAsync(CancellationToken ct = default)
    {
        cache.Remove(CacheKey);
        return RefreshAccessTokenAndCacheAsync(ct);
    }

    private async Task<string> RefreshAccessTokenAndCacheAsync(CancellationToken ct)
    {
        var endpoint = "authentication/v1";
        var grantType = options.Value.GrantType;
        var clientId = options.Value.ClientId;
        var clientSecret = options.Value.ClientSecret;
        var scope = options.Value.Scope;

        var request = new TokenRequest(grantType, clientId, clientSecret, scope);

        var resp = await httpClient.PostAsJsonAsync(endpoint, request, ct);
        var result = await resp.Content.ReadAsStringAsync(ct);

        if (!resp.IsSuccessStatusCode) throw new InvalidOperationException($"Token request failed ({(int)resp.StatusCode}): {result}");

        var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(result, _json) ?? throw new InvalidOperationException("Invalid token response.");

        if (string.IsNullOrWhiteSpace(tokenResponse.AccessToken)) throw new InvalidOperationException("Token response missing access token.");

        cache.Set(CacheKey, tokenResponse.AccessToken, TimeSpan.FromMinutes(60));
        return tokenResponse.AccessToken;
    }

    private sealed class TokenResponse
    {
        public string? AccessToken { get; set; }
    }
    private record TokenRequest(string? GrantType, string? ClientId, string? ClientSecret, string? Scope);
}
