using QuoteCalculator.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuoteCalculator.Models
{
    public class BaggageModel
    {
        public List<Movebaggage> moveList { get; set; }
        public List<CartonsModel> cartonList { get; set; }
        public List<BaggageCalculationLineModel> calculationLines { get; set; }
        public bool HasMainCartons
        {
            get
            {
                if (cartonList == null || cartonList.Count == 0)
                    return false;
                return cartonList.Count(x => string.Compare(x.type, "MAIN", true) == 0) > 0;                
            }
        }

    }
    public class Movebaggage
    {
        public string description { get; set; }
        public int length { get; set; }
        public int breadth { get; set; }
        public int height { get; set; }
        public string dimension { get; set; }
        public int quantity { get; set; }
        public double Volume { get; set; }
        public Nullable<int> UserVolume { get; set; }
        public int? CalculatedVolume
        {
            get
            {
                return Convert.ToInt16(Math.Round(length * breadth * height / 28316.8));
            }
        }

        public string FullSizeMoveStr
        {
            get
            {
                string dimensionStr = "";
                dimensionStr = Convert.ToString(quantity + " X " + description + " (" + CalculatedVolume + " cubic feet" + (quantity > 1 ? " each)" : ")"));
                if (length > 0 && breadth > 0 && height > 0)
                {
                    dimensionStr += Convert.ToString(", " + length + " X " + breadth + " X " + height + " Cms");
                }
                if (UserVolume > 0)
                {
                    dimensionStr += Convert.ToString(", " + UserVolume + " Kgs");
                }
                return dimensionStr;
            }
        }

        public int? TotalCuft
        {
            get
            {
                return Convert.ToInt32(quantity * CalculatedVolume);
            }
        }
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