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
    
    public partial class sea_rates
    {
        public long id { get; set; }
        public long zone { get; set; }
        public string Destination { get; set; }
        public decimal to25ls { get; set; }
        public decimal to25rate { get; set; }
        public decimal over25ls { get; set; }
        public decimal over25rate { get; set; }
        public decimal over50ls { get; set; }
        public decimal over50rate { get; set; }
        public decimal min { get; set; }
        public Nullable<int> company { get; set; }
    }
}
