//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace QuoteCalculator.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class tbl_AdditionalQuickQuoteItems
    {
        public int ItemId { get; set; }
        public string Beds { get; set; }
        public string Cuft { get; set; }
        public string Ftcontainer { get; set; }
        public string SpecialRequirements { get; set; }
        public Nullable<int> InternationalRemovalId { get; set; }
        public Nullable<int> QuickQuoteItemId { get; set; }
    
        public virtual tbl_InternationalRemoval tbl_InternationalRemoval { get; set; }
        public virtual tbl_QuickQuoteItems tbl_QuickQuoteItems { get; set; }
        public string[] strSpecialRequirements { get; set; }
    }
}
