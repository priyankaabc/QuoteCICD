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
    
    public partial class removal_ORIG
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public removal_ORIG()
        {
            this.r_quotes = new HashSet<r_quotes>();
        }
    
        public int e_enquiry_no { get; set; }
        public System.DateTime e_enquiry_dt { get; set; }
        public string e_apcompany { get; set; }
        public string e_branch { get; set; }
        public string e_division { get; set; }
        public string e_type { get; set; }
        public string e_estimator { get; set; }
        public System.DateTime e_estim_dt { get; set; }
        public string e_estim_time { get; set; }
        public System.DateTime e_move_dt { get; set; }
        public string e_source { get; set; }
        public string e_comments { get; set; }
        public string e_surname { get; set; }
        public string e_title { get; set; }
        public string e_firstname { get; set; }
        public string e_initials { get; set; }
        public string e_salutation { get; set; }
        public string e_addr1 { get; set; }
        public string e_addr2 { get; set; }
        public string e_addr3 { get; set; }
        public string e_addr4 { get; set; }
        public string e_postcode { get; set; }
        public string e_region { get; set; }
        public string e_tel_home { get; set; }
        public string e_tel_work { get; set; }
        public string e_fax { get; set; }
        public string e_email { get; set; }
        public short e_emailq { get; set; }
        public string e_mobile { get; set; }
        public string e_company { get; set; }
        public string e_contact { get; set; }
        public string e_a_addr1 { get; set; }
        public string e_a_addr2 { get; set; }
        public string e_a_addr3 { get; set; }
        public string e_a_addr4 { get; set; }
        public string e_a_postcode { get; set; }
        public System.DateTime e_folup1_dt { get; set; }
        public string e_folup1_com { get; set; }
        public System.DateTime e_folup2_dt { get; set; }
        public string e_folup2_com { get; set; }
        public System.DateTime e_folup3_dt { get; set; }
        public string e_folup3_com { get; set; }
        public System.DateTime e_folup4_dt { get; set; }
        public string e_folup4_com { get; set; }
        public System.DateTime e_folup5_dt { get; set; }
        public string e_folup5_com { get; set; }
        public System.DateTime e_fol_next { get; set; }
        public short e_lost { get; set; }
        public short e_accepted { get; set; }
        public string e_lost_to { get; set; }
        public string e_ap_number { get; set; }
        public System.DateTime e_adate { get; set; }
        public System.DateTime e_del_dt { get; set; }
        public string e_del_time { get; set; }
        public System.DateTime e_col_dt { get; set; }
        public string e_col_time { get; set; }
        public int e_a_quote_no { get; set; }
        public int e_h_quote_no { get; set; }
        public System.DateTime e_hi_quote { get; set; }
        public System.DateTime e_emailfu1 { get; set; }
        public System.DateTime e_emailfu2 { get; set; }
        public string e_notes { get; set; }
        public System.DateTime e_added { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<r_quotes> r_quotes { get; set; }
    }
}
