using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuoteCalculator.Models
{
    public class VideoSurveyModel
    {
        public long QuoteId { get; set; }
        public string firstName { get; set; }
        public string title { get; set; }
        public string surName { get; set; }
        public string email { get; set; }
        public string mobileNumber { get; set; }
        public string addressPickup { get; set; }
        public string addressDropoff { get; set; }
    }
}