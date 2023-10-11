using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuoteCalculator.Service.Models
{
    public class OptOutModel
    {
        public string Email { get; set; }
        public bool IsOptOut { get; set; }
        public int? TotalCount { get; set; }
        public int? TotalFilteredCount { get; set; }
    }
}
