using QuoteCalculatorPickfords.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuoteCalculatorPickfords.Models
{
    public class SailingSchedModel
    {
        public string CountryName { get; set; }
        public List<sailingsched> SailingSchedules { get; set; }
    }
}