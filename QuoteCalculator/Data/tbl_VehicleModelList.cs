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
    
    public partial class tbl_VehicleModelList
    {
        public int Id { get; set; }
        public string MakeName { get; set; }
        public string ModelName { get; set; }
        public string Production { get; set; }
        public string DisplayName { get; set; }
        public Nullable<double> Length { get; set; }
        public Nullable<double> Width { get; set; }
        public Nullable<double> Height { get; set; }
        public string Display { get; set; }
        public string FitsInContainer { get; set; }
        public Nullable<double> Volume_FCL { get; set; }
        public Nullable<double> Volume_GRP { get; set; }
        public string Type { get; set; }
    }
}
