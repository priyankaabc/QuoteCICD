using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuoteCalculator.Models
{
    public class QuoteInformationModel
    {
        public List<VehicleQuoteInfoModel> vehicleQuoteInfo { get; set; }
        public List<BaggageQuoteInfoModel> bagageQuoteInfo { get; set; }
        public List<InternationalRemovalInfoModel> removalQuoteInfo { get; set; }
    }
    public class VehicleQuoteInfoModel
    {
        public int Id { get; set; }
        public string FromCountryName { get; set; }
        public string ToCountryName { get; set; }
        public string CityName { get; set; }
        public string Email { get; set; }
        public Nullable<double> FCL { get; set; }
        public Nullable<double> GPG { get; set; }
        public Nullable<bool> IsFCL { get; set; }
        public Nullable<bool> IsGRP { get; set; }
        public string PostCode { get; set; }
        public string VehicleMakeName { get; set; }
        public string VehicleModelName { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CustomerRefNo { get; set; }
    }
    public class BaggageQuoteInfoModel
    {
        public int Id { get; set; }
        public string PostCode { get; set; }
        public string FromCity { get; set; }
        public string FromCountry { get; set; }
        public string ToCountry { get; set; }
        public string ToPostCode { get; set; }
        public string CityName { get; set; }
        public string CartonList { get; set; }
        public bool HasMainCartons { get; set; }
        public string TransitionTime { get; set; }
        public List<BaggageCalculationLineModel> BaggagePriceList = new List<BaggageCalculationLineModel>();
        public DateTime CreatedDate { get; set; }
        public string CustomerRefNo { get; set; }
        public bool AirFreightToAirport { get; set; }
        public bool AirFreightToDoor { get; set; }
        public bool Courier { get; set; }
        public bool SeaFreight { get; set; }
        public bool RoadFreight { get; set; }
        public bool CourierExpress { get; set; }
        public bool isMethodSelected { get; set; }
        public string InternalNotes { get; set; }
        public decimal DeliveryCharge { get; set; }
        public decimal CollectionCharge { get; set; }
    }


    public class InternationalRemovalInfoModel
    {
        public int Id { get; set; }
        public string FromCountryName { get; set; }
        public string ToCountryName { get; set; }
        public string CityName { get; set; }
        public string Email { get; set; }
        public string PostCode { get; set; }
        public string Description { get; set; }
        public string SpecialRequirements { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CustomerRefNo { get; set; }
        public bool Courier { get; set; }
        public string[] strSpecialRequirements { get; set; }
    }
}