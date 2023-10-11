using QuoteCalculatorPickfords.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuoteCalculatorPickfords.Models
{
    public class BaggageModel
    {
        public List<Movebaggage> moveList { get; set; }
        public List<CartonsModel> cartonList { get; set; }
        public List<BaggageCalculationLineModel> calculationLines { get; set; }

    }
    public class Movebaggage
    {
        public string description { get; set; }
        public int length { get; set; }
        public Nullable<int> breadth { get; set; }
        public int height { get; set; }
        public string dimension { get; set; }
        public int quantity { get; set; }
        public int Volume { get; set; }
        public Nullable<int> UserVolume { get; set; }

    }

    public class BaggageCalculationLineModel
    {
        public string DeliveryMethodName { get; set; }
        public string TransitionTime { get; set; }
        public decimal Amount { get; set; }
        public string CalcDescription { get; set; }
        public bool isSelected { get; set; }
    }


}