using LoanApp.Web.Models;
using LoanApp.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace LoanApp.Web.Pages.Loan
{
    public class ApplicationDetailsPageModel(ILoanAppApiClient loanAppApiClient, ILogger<QuotationPageModel> logger) : PageModel
    {
        [BindProperty]
        public ViewModel ViewModel { get; set; } = new();

        [TempData]
        public string? ViewModelJson { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!string.IsNullOrEmpty(ViewModelJson))
            {
                ViewModel = JsonSerializer.Deserialize<ViewModel>(ViewModelJson!)!;
            }
            var amount = ViewModel.LoanAmount;
            var term = ViewModel.TermMonths;
            var productType = ViewModel.ProductType;

            try
            {
                var response = await loanAppApiClient.GetQuoteCalculationAsync(amount, term, productType);
                if (response != null)
                {
                    ViewModel.RepaymentAmount = response?.RepaymentAmount;
                    ViewModel.EstablishmentFee = response?.EstablishmentFee;
                    ViewModel.TotalInterest = response?.TotalInterest;
                    ViewModel.TotalRepaymentAmount = response?.TotalRepaymentAmount;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to load application {identiyToken}", ViewModel.IdentityToken);
                ModelState.AddModelError(string.Empty, "Unable to load customer details at this time.");
            }

            ViewModelJson = JsonSerializer.Serialize(ViewModel);
            return Page();
        }

        public async Task<IActionResult> OnPostApply()
        {
            try
            {
                if (!string.IsNullOrEmpty(ViewModelJson))
                {
                    ViewModel = JsonSerializer.Deserialize<ViewModel>(ViewModelJson!)!;

                    var updateLoanApplication = new UpdateLoanApplicationDto
                    {
                        IdentityToken = ViewModel.IdentityToken,
                        Title = ViewModel.Title,
                        FirstName = ViewModel.FirstName,
                        LastName = ViewModel.LastName,
                        DateOfBirth = ViewModel.DateOfBirth,
                        Mobile = ViewModel.MobileNumber,
                        Email = ViewModel.Email,
                        ProductType = ViewModel.ProductType,
                        Term = ViewModel.TermMonths,
                        AmountRequired = ViewModel.LoanAmount,
                        EstablishmentFee = ViewModel.EstablishmentFee,
                        TotalInterest = ViewModel.TotalInterest,
                        RepaymentAmount = ViewModel.RepaymentAmount
                    };

                    var result = await loanAppApiClient.UpdateLoanApplicationAsync(updateLoanApplication);
                    if (!string.IsNullOrEmpty(result))
                    {
                        var loanApplication = await loanAppApiClient.GetLoanApplicationByTokenIdAsync(ViewModel.IdentityToken!);
                        if (loanApplication is not null)
                        {
                            ViewModel.IdentityToken = loanApplication.IdentityToken;
                            ViewModel.LoanAmount = loanApplication.Amount;
                            ViewModel.TermMonths = loanApplication.Term;
                            ViewModel.Title = loanApplication.Title;
                            ViewModel.FirstName = loanApplication.FirstName;
                            ViewModel.LastName = loanApplication.LastName;
                            ViewModel.DateOfBirth = loanApplication.DateOfBirth;
                            ViewModel.MobileNumber = loanApplication.Mobile;
                            ViewModel.Email = loanApplication.Email;
                            ViewModel.RepaymentAmount = loanApplication?.RepaymentAmount;
                            ViewModel.EstablishmentFee = loanApplication?.EstablishmentFee;
                            ViewModel.TotalInterest = loanApplication?.TotalInterest;
                            ViewModel.TotalRepaymentAmount = loanApplication?.TotalRepaymentAmount;
                            ViewModel.Status = loanApplication?.Status;

                            ViewModelJson = JsonSerializer.Serialize(ViewModel);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to update loan application {identiyToken}", ViewModel.IdentityToken);
                ModelState.AddModelError(string.Empty, "Unable to load loan application details at this time.");
            }
            return RedirectToPage("./SuccessPage");
        }
    }
}
