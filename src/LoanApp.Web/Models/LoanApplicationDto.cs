namespace LoanApp.Web.Models;

public sealed class LoanApplicationDto
{
    public string? IdentityToken { get; set; }
    public string? Title { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public string? Mobile { get; set; }
    public string? Email { get; set; }
    public string? ProductType { get; set; }
    public int? Term { get; set; }
    public decimal? Amount { get; set; }
    public decimal? RepaymentAmount { get; set; }
    public decimal? EstablishmentFee { get; set; }
    public decimal? TotalInterest { get; set; }
    public decimal? TotalRepaymentAmount { get; set; }
    public string? Status { get; set; }
}

public sealed class UpdateLoanApplicationDto
{
    public string? IdentityToken { get; set; }
    public string? Title { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public string? Mobile { get; set; }
    public string? Email { get; set; }
    public string? ProductType { get; set; }
    public int? Term { get; set; }
    public decimal? AmountRequired { get; set; }
    public decimal? RepaymentAmount { get; set; }
    public decimal? EstablishmentFee { get; set; }
    public decimal? TotalInterest { get; set; }
}
