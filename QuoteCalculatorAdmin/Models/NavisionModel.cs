using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuoteCalculatorAdmin.Models
{
    public class NavisionModel
    {
        public string Status { get; set; }
        public string accessToken { get; set; }
    }

    public class NavisionXMLRequestModel
    {
        public string QuoteType { get; set; }
        public string XMLData { get; set; }
        public string accessToken { get; set; }
    }
}