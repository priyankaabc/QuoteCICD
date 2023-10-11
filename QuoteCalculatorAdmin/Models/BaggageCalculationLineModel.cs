using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuoteCalculatorAdmin.Models
{
    public class BaggageCalculationLineModel
    { 
        public string DeliveryMethodName { get; set; }
        public string TransitionTime { get; set; }
        public decimal Amount { get; set; }
        public string CalcDescription { get; set; }
        public bool isSelected { get; set; }
    }
}