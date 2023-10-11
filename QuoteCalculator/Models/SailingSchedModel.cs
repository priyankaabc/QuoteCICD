using QuoteCalculator.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuoteCalculator.Models
{
    public class SailingSchedModel
    {
        public string CountryName { get; set; }
        public List<sailingsched> SailingSchedules { get; set; }
    }
}