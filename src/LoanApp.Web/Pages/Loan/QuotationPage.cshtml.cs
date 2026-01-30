using LoanApp.Web.Models;
using LoanApp.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace LoanApp.Web.Pages.Loan;

public class QuotationPageModel(ILoanAppApiClient loanAppApiClient, ILogger<QuotationPageModel> logger) : PageModel
{
    public decimal MaxLoanAmount { get; private set; } = 100_000m;
    public decimal MaxLoanAmountMin { get; private set; } = 2_000m;
    public decimal LoanAmountStep { get; private set; } = 1_000m;
    public int MinTermMonths { get; private set; } = 2;
    public int MaxTermMonths { get; private set; } = 60;

    [BindProperty]
    public ViewModel ViewModel { get; set; } = new();

    [TempData]
    public string? ViewModelJson { get; set; }

    public async Task<IActionResult> OnGetAsync(string? reference)
    {
        if (!string.IsNullOrEmpty(ViewModelJson))
        {
            ViewModel = JsonSerializer.Deserialize<ViewModel>(ViewModelJson!)!;
        }
        else
        {
            ViewModel.IdentityToken = reference;

            if (!string.IsNullOrWhiteSpace(ViewModel.IdentityToken))
            {
                try
                {
                    var response = await loanAppApiClient.GetLoanApplicationByTokenIdAsync(ViewModel.IdentityToken);
                    if (response != null)
                    {
                        ViewModel.LoanAmount = response.Amount;
                        ViewModel.TermMonths = response.Term;
                        ViewModel.Title = response.Title;
                        ViewModel.FirstName = response.FirstName;
                        ViewModel.LastName = response.LastName;
                        ViewModel.DateOfBirth = response.DateOfBirth;
                        ViewModel.MobileNumber = response.Mobile;
                        ViewModel.Email = response.Email;
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to load loan application {identiyToken}", ViewModel.IdentityToken);
                    ModelState.AddModelError(string.Empty, "Unable to load loan application details at this time.");

                    return Page();
                }
            }
        }

        return Page();
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid) return Page();

        ViewModelJson = JsonSerializer.Serialize(ViewModel);
        return RedirectToPage("./ApplicationDetailsPage");
    }
}
