using QuoteCalculator.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuoteCalculator.Models
{
    public class QuickQuoteItemsModel
    {
        public string Beds { get; set; }
        public string Cuft { get; set; }
        //public string Ftcontainer { get; set; }
        public string SpecialRequirements { get; set; }
        public int InternationalRemovalId { get; set; }
        public int QuickQuoteItemId { get; set; }
        public List<tbl_QuickQuoteItems> items { get;set;}
    }
}