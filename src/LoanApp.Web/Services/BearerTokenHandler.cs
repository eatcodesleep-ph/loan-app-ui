using System.Net;

namespace LoanApp.Web.Services;

public sealed class BearerTokenHandler(ITokenService tokenService) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
    {
        request.Headers.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer",
                await tokenService.GetAccessTokenAsync(ct));

        var response = await base.SendAsync(request, ct);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            response.Dispose();
            request.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer",
                    await tokenService.RefreshAccessTokenAsync(ct));
            response = await base.SendAsync(request, ct);
        }

        return response;
    }
}
