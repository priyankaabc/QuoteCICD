using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace QuoteCalculator.Models
{
    public class MyQuoteModel
    {
        [Display(Name = "Email")]
        public string EmailAddres { get; set; }

        [Display(Name = "Access Code")]
        public string OTP { get; set; }
    }
}