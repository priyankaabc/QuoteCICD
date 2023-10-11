using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QuoteCalculatorAdmin.Models
{
    public class SpreadSheetModel
    {
        [Required(ErrorMessage = "Please select Spreadsheet Name")]
        public int SpreadSheetId { get; set; }

    }
}