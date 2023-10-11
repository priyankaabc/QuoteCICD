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
    
    public partial class tbl_baggageQuote
    {
        public int Id { get; set; }
        public Nullable<int> TitleId { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string CountryCode { get; set; }
        public string Telephone { get; set; }
        public string FromCountry { get; set; }
        public string FromCity { get; set; }
        public string ToCountry { get; set; }
        public string PostCode { get; set; }
        public string CityName { get; set; }
        public string ToPostCode { get; set; }
        public Nullable<System.DateTime> EstimatedMoveDate { get; set; }
        public Nullable<bool> IsConditionApply { get; set; }
        public Nullable<int> BranchId { get; set; }
        public Nullable<bool> AirFreightToAirport { get; set; }
        public Nullable<bool> AirFreightToDoor { get; set; }
        public Nullable<bool> Courier { get; set; }
        public Nullable<bool> SeaFreight { get; set; }
        public Nullable<double> AirFreightToAirportFinal { get; set; }
        public Nullable<double> AirFreightToDoorFinal { get; set; }
        public Nullable<double> CourierFinal { get; set; }
        public Nullable<double> SeaFreightFinal { get; set; }
        public Nullable<double> Price { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<System.DateTime> LastExecutionDate { get; set; }
        public Nullable<System.DateTime> NextExecutionDate { get; set; }
        public string InternalNotes { get; set; }
        public string SalesRep { get; set; }
        public string Sr_Code { get; set; }
        public string Sr_Name { get; set; }
        public Nullable<bool> IsSendMailForIncompleteQuote { get; set; }
        public Nullable<int> Company { get; set; }
        public Nullable<bool> IsInquiry { get; set; }
        public Nullable<bool> IsDelete { get; set; }
        public string ReferenceNumber { get; set; }
        public Nullable<double> RoadFreightToDoorFinal { get; set; }
        public Nullable<bool> RoadFreightToDoor { get; set; }
        public Nullable<double> CourierExpressToDoorFinal { get; set; }
        public Nullable<bool> CourierExpressToDoor { get; set; }
    }
}
