//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace QuoteCalculatorAdmin.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class tbl_QuickQuoteItems
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tbl_QuickQuoteItems()
        {
            this.tbl_AdditionalQuickQuoteItems = new HashSet<tbl_AdditionalQuickQuoteItems>();
        }
    
        public int ItemId { get; set; }
        public string Title { get; set; }
        public string Cuft { get; set; }
        public string image { get; set; }
        public string Ftcontainer { get; set; }
        public Nullable<int> DisplayOrder { get; set; }
        public int company { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tbl_AdditionalQuickQuoteItems> tbl_AdditionalQuickQuoteItems { get; set; }
    }
}
