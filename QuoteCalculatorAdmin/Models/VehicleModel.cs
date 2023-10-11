using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuoteCalculatorAdmin.Models
{
    public class VehicleModel
    {
        public int TitleId { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
        public string CityName { get; set; }
        public string QuoteNo { get; set; }
        public string EnquiryNo { get; set; }
        public int Company { get; set; }
        public int Id { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string AccessCode { get; set; }

        public string TitleName { get; set; }
        public long? TotalCount { get; set; }
        public long? TotalFilteredCount { get; set; }
    }
}