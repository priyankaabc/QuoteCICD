using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuoteCalculatorPickfords.Models
{
    public class BaggageCalculationModel
    {
        public decimal? AirFreightToAirport { get; set; }
        public decimal? AirFreightToDoor { get; set; }
        public decimal? Courier { get; set; }
        public decimal? SeaFreight { get; set; }
    }
}