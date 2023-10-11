using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuoteCalculator.Service.Models
{
    public class QuoteAmountModel
    {
        public int Id { get; set; }
        public int QuoteId { get; set; }
        public string MoveType { get; set; }
        public float QuoteAmount { get; set; }
        public string ShippingType { get; set; }
        public string ShippingTypeDescription { get; set; }
        public string TransitionTime { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsBooked { get; set; }
        public int CustomerReferenceNo { get; set; }
        public int CustomerQuoteNo { get; set; }
        public int QuoteSeqNo { get; set; }
        public int Company { get; set; }
    }

    public class GuideLinkModel
    {
        public int Id { get; set; }
        public string CountryCode { get; set; }
        public string CityName { get; set; }
        public string VehicleURL { get; set; }
        public string RemovalURL { get; set; }
    }
}
