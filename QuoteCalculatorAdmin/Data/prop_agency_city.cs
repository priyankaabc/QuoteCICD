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
    
    public partial class prop_agency_city
    {
        public long id { get; set; }
        public long agency { get; set; }
        public long city { get; set; }
    
        public virtual city city1 { get; set; }
        public virtual prop_agency prop_agency { get; set; }
    }
}
