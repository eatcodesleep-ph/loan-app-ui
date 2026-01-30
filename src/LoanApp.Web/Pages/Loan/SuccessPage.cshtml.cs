using LoanApp.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace LoanApp.Web.Pages.Loan;

public class SuccessPageModel : PageModel
{
    [BindProperty]
    public ViewModel ViewModel { get; set; } = new();

    [TempData]
    public string? ViewModelJson { get; set; }
    public IActionResult OnGet()
    {
        if (!string.IsNullOrEmpty(ViewModelJson))
        {
            ViewModel = JsonSerializer.Deserialize<ViewModel>(ViewModelJson!)!;
        }

        return Page();
    }
}
