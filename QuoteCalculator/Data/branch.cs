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
    
    public partial class branch
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public branch()
        {
            this.branch_postcode = new HashSet<branch_postcode>();
            this.branch_postcode1 = new HashSet<branch_postcode>();
            this.branch_postcode2 = new HashSet<branch_postcode>();
        }
    
        public long br_id { get; set; }
        public string br_code { get; set; }
        public string br_branch { get; set; }
        public string br_description { get; set; }
        public string br_docs { get; set; }
        public string br_ip { get; set; }
        public string br_handledby { get; set; }
        public string br_booknow_email { get; set; }
        public short br_order { get; set; }
        public string br_code_full { get; set; }
        public string br_postcode { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<branch_postcode> branch_postcode { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<branch_postcode> branch_postcode1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<branch_postcode> branch_postcode2 { get; set; }
    }
}
