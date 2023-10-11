using QuoteCalculatorAdmin.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QuoteCalculatorAdmin.Models
{
    public class ForgotPasswordModel
    {
        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email is required")]
        [RegularExpression(CommonHelper.RegexEmail, ErrorMessage = "Invalid email address")]
        public string Email { get; set; }
    }
}