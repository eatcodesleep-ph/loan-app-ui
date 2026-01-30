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
            var error = JsonSerializer.Deserialize<ErrorResponse>(result, _json);
            if (error != null)
            {
                var validationDetails = error.Errors is not null
                    ? string.Join("; ", error.Errors.Select(kvp => $"{kvp.Key}: {string.Join(", ", kvp.Value ?? Array.Empty<string>())}"))
                    : null;

                throw new Exception($"{(validationDetails is not null ? $"{validationDetails}" : string.Empty)}");
            }
        }

        return result;
    }

    public class ErrorResponse
    {
        public string? Type { get; set; }
        public string? Title { get; set; }
        public int? Status { get; set; }
        public Dictionary<string, string[]?>? Errors { get; set; }
    }
}
