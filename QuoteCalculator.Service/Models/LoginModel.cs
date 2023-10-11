using QuoteCalculator.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QuoteCalculator.Service.Models
{
    public class UserLoginModel
    {
        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email is required")]
        [RegularExpression(CommonHelper.RegexEmail, ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Display(Name = "Password")]
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }

 
}