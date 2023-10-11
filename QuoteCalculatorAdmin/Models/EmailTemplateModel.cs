using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuoteCalculatorAdmin.Models
{
    public class EmailTemplateModel
    {
        public int id { get; set; }
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public string Subject { get; set; }
        public string HtmlContent { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<int> UpdateBy { get; set; }
        public Nullable<System.DateTime> UpdateOn { get; set; }
        public int? TotalCount { get; set; }
        //public virtual tbl_Service tbl_Service { get; set; }
    }
}