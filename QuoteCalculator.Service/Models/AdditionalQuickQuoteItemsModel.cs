using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuoteCalculator.Service.Models
{
   public class AdditionalQuickQuoteItemsModel
    {
        public int ItemId { get; set; }
        public string Beds { get; set; }
        public string Cuft { get; set; }
        public string Ftcontainer { get; set; }
        public string SpecialRequirements { get; set; }
        public int InternationalRemovalId { get; set; }
        public int QuickQuoteItemId { get; set; }
        public string[] strSpecialRequirements { get; set; }
        public int CompanyId{get; set; }
    }
}
