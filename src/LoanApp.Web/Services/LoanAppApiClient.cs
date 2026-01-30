using LoanApp.Web.Models;
using System.Text.Json;

namespace LoanApp.Web.Services;

public interface ILoanAppApiClient
{
    Task<LoanApplicationDto?> GetLoanApplicationByTokenIdAsync(string identityToken, CancellationToken ct = default);
    Task<LoanApplicationDto?> GetQuoteCalculationAsync(decimal? amount, int? termMonths, string? product, CancellationToken ct = default);
    Task<string?> UpdateLoanApplicationAsync(UpdateLoanApplicationDto updateLoanApplicationDto, CancellationToken ct = default);
}

public sealed class LoanAppApiClient(HttpClient httpClient) : ILoanAppApiClient
{
    private static readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web);
    public async Task<LoanApplicationDto?> GetLoanApplicationByTokenIdAsync(string identityToken, CancellationToken ct = default)
    {
        var endpoint = $"loanApplication/v1/{identityToken}";

        if (string.IsNullOrWhiteSpace(identityToken)) return null;

        return await httpClient.GetFromJsonAsync<LoanApplicationDto>(endpoint, ct);
    }

    public async Task<LoanApplicationDto?> GetQuoteCalculationAsync(decimal? amount, int? termMonths, string? product, CancellationToken ct = default)
    {
        var endpoint = $"quotation/v1/calculate?amount={amount}&term={termMonths}&product={product}";
        return await httpClient.GetFromJsonAsync<LoanApplicationDto>(endpoint, ct);
    }

    public async Task<string?> UpdateLoanApplicationAsync(UpdateLoanApplicationDto updateLoanApplicationDto, CancellationToken ct = default)
    {
        var endpoint = $"loanApplication/v1/";

        var resp = await httpClient.PutAsJsonAsync(endpoint, updateLoanApplicationDto, ct);
        var result = await resp.Content.ReadAsStringAsync(ct);

        if (!resp.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Failed to update loan application. Status code: {resp.StatusCode}");
        }

        return result;
    }
}
