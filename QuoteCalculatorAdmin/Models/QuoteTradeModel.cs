using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QuoteCalculatorAdmin.Models
{
    public class TradeQuoteModelold
    {
        public long Id { get; set; }
        public string TradeQuoteId { get; set; }

        //[Required(ErrorMessage = "Client Name is Required")]
        public string ClientName { get; set; }

        [Required(ErrorMessage = "Agent is Required")]
        public int AgentId { get; set; }
        public string AgentName { get; set; }
        public string AgentCode { get; set; }
        public string DestCountryId { get; set; }

        //[Required(ErrorMessage = "Destination Country is Required")]
        public string DestCountry { get; set; }
        public string Suburb { get; set; }

        [Required(ErrorMessage = "Destination Code is Required")]
        public string DestCode { get; set; }
        public string DestCityName { get; set; }

        //[Required(ErrorMessage = "Destination Address 1 is Required")]
        public string DestAddress1 { get; set; }
        public string DestAddress2 { get; set; }
        public string DestPort { get; set; }
        public string SalesRep { get; set; }

        [Required(ErrorMessage = "Branch is Required")]
        public string Branch { get; set; }

        [Required(ErrorMessage = "Service is Required")]
        public int ServiceId { get; set; }

        [Required(ErrorMessage = "Volume is Required")]
        public decimal? Volume { get; set; }

        [Display(Name = "Service")]
        public string StrService { get; set; }
        public string IxType { get; set; }
        public decimal? Tariff { get; set; }
        public decimal? TotalCost { get; set; }
        public decimal? HandlingFee { get; set; }
        public decimal? DestinationFee { get; set; }
        public string RefNo { get; set; }
        public decimal? FreightFee { get; set; }
        public decimal? AdjustedPrice { get; set; }
        public decimal? AdjustedProfit { get; set; }
        public string CompanyCode { get; set; }

        public int? TotalCount { get; set; }
    }   
}