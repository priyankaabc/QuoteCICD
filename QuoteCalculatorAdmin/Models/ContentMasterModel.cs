using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuoteCalculatorAdmin.Models
{
    public class ContentMasterModel
    {
        public int ChildContentId { get; set; }
        public Nullable<int> HeadingContentId { get; set; }
        public string ChildContent { get; set; }
        public string CountryCode { get; set; }
        public Nullable<int> DisplayOrder { get; set; }
        public long? TotalCount { get; set; }
        public long? TotalFilteredCount { get; set; }
    }
}