//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace QuoteCalculatorPickfords.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class tbl_HeadingContent
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tbl_HeadingContent()
        {
            this.tbl_ChildContent = new HashSet<tbl_ChildContent>();
        }
    
        public int HeadingContentId { get; set; }
        public string HeadingContent { get; set; }
        public string CountryCode { get; set; }
        public Nullable<int> DisplayOrder { get; set; }
        public string Heading { get; set; }
        public string QuoteType { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tbl_ChildContent> tbl_ChildContent { get; set; }
    }
}
