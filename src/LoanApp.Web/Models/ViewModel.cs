using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace LoanApp.Web.Models
{
    public class ViewModel
    {
        [HiddenInput]
        public string? IdentityToken { get; set; }

        [Required, Display(Name = "Title"), StringLength(10)]
        public string? Title { get; set; }

        [Required, Display(Name = "First name"), StringLength(50)]
        public string? FirstName { get; set; }

        [Required, Display(Name = "Last name"), StringLength(50)]
        public string? LastName { get; set; }

        [DataType(DataType.Date), Display(Name = "Birthdate")]
        public DateOnly? DateOfBirth { get; set; }

        [Required, Phone, Display(Name = "Mobile number"), StringLength(15)]
        public string? MobileNumber { get; set; }

        [Required, EmailAddress, Display(Name = "Email address"), StringLength(50)]
        public string? Email { get; set; }

        [Required, Range(1, double.MaxValue), Display(Name = "Loan amount")]
        public decimal? LoanAmount { get; set; }

        [Required, Range(1, int.MaxValue), Display(Name = "Term (months)")]
        public int? TermMonths { get; set; }

        [Display(Name = "Select Product")]
        public string? ProductType { get; set; }
        public decimal? RepaymentAmount { get; set; }
        public decimal? EstablishmentFee { get; set; }
        public decimal? TotalInterest { get; set; }
        public decimal? TotalRepaymentAmount { get; set; }
        public string? Status { get; set; }
    }
}
