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
    
    public partial class tbl_PostCodeUK
    {
        public int Id { get; set; }
        public string State { get; set; }
        public string Code { get; set; }
        public string Housename { get; set; }
        public string Streetname { get; set; }
        public string City { get; set; }
        public string PostCode { get; set; }
        public int CompanyId { get; set; }
    
        public virtual tbl_Company tbl_Company { get; set; }
    }
}
