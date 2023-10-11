using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuoteCalculator.Service.Models
{
    public class RemovalQuoteCalculationModel
    {
        public int QuoteId { get; set; }
        public decimal? Vehicle { get; set; }
        public decimal? Labour { get; set; }
        public decimal? PackingMaterials { get; set; }
        public decimal? SeaFreight { get; set; }
        public decimal? ReceivingHandling { get; set; }
        public decimal? DestinationCharges { get; set; }
        public decimal? OriginCost { get; set; }
        public decimal? OriginMarkup { get; set; }
        public decimal? Total { get; set; }
    }
}
