using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuoteCalculatorPickfords.Models
{
    public class VehicleQuoteModel
    {
        public bool FCLvalue { get; set; }
        public bool GPGvalue { get; set; }
        public double FCL { get; set; }
        public double GroupageTotal { get; set; }

        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Title { get; set; }
        public string EstimatedMoveDate { get; set; }
        public string MakeName { get; set; }
        public string ModelName { get; set; }
        public string ToCountryName { get; set; }
        public string FromCountryName { get; set; }
        public string CityName { get; set; }
        public int VehicleId { get; set; }
        public string BranchName { get; set; }

        public bool IsFCL { get; set; }
        public bool IsGPG { get; set; }
        public string str { get; set; }
        public string QuoteNo { get; set; }
        
        //public float  { get; set; }
        //public int MyProperty { get; set; }
        //public int MyProperty { get; set; }
    }
}