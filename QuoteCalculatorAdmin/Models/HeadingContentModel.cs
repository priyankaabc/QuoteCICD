using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuoteCalculatorAdmin.Models
{
    public class HeadingContentModel
    {
        public int HeadingContentId { get; set; }
        public string HeadingContent { get; set; }
        public string CountryCode { get; set; }
        public Nullable<int> DisplayOrder { get; set; }
        public string Heading { get; set; }
        public string QuoteType { get; set; }
        public string Company { get; set; }
        public string FromCountryCode { get; set; }
        public int? TotalCount { get; set; }
    }
}