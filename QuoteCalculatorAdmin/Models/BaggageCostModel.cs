using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuoteCalculatorAdmin.Models
{
    public class BaggageCostModel
    {
        public long Id { get; set; }
        public int QuoteId { get; set; }
        public string ShippingType { get; set; }
        public string CostType { get; set; }
        public decimal Cost { get; set; }
    }
}