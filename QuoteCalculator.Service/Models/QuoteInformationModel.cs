using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuoteCalculator.Service.Models
{
   public  class QuoteInformationModel
    {
        public List<BaggageQuoteInfoModel> bagageQuoteInfo { get; set; }
        public List<InternatioalRemovalQuoteInfoModel> internationremovalQuoteInfo { get; set; }
    }
    //public class BaggageQuoteInfoModel
    //{
    //    public int Id { get; set; }
    //    public string PostCode { get; set; }
    //    public string FromCity { get; set; }
    //    public string FromCountry { get; set; }
    //    public string ToCountry { get; set; }
    //    public string ToPostCode { get; set; }
    //    public string CityName { get; set; }
    //    public string CartonList { get; set; }
    //    public string TransitionTime { get; set; }
    //    public List<BaggageCalculationLineModel2> BaggagePriceList = new List<BaggageCalculationLineModel2>();
    //    public DateTime CreatedDate { get; set; }
    //    public string CustomerRefNo { get; set; }
    //    public bool AirFreightToAirport { get; set; }
    //    public bool AirFreightToDoor { get; set; }
    //    public bool Courier { get; set; }
    //    public bool SeaFreight { get; set; }
    //    public bool RoadFreightToDoor { get; set; }
    //    public bool CourierExpressToDoor { get; set; }
    //    public bool isMethodSelected { get; set; }
    //    public decimal DeliveryCharge { get; set; }
    //    public decimal CollectionCharge { get; set; }
    //    public string InternalNotes { get; set; }
    //    public bool HasMainCartons { get; set; }
    //}

    public class InternatioalRemovalQuoteInfoModel
    {
        public int Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string CountryCode { get; set; }
        public string ToCountryName { get; set; }
        public string Telephone { get; set; }
        public int TitleId { get; set; }
        public string FromCountryName { get; set; }
        public string ToCountryCode { get; set; }
        public string PostCode { get; set; }
        public string CityName { get; set; }
        public DateTime? EstimatedMoveDate { get; set; }
        public bool? IsConditionApply { get; set; }
        public bool IsBooked { get; set; }
        public int? BranchId { get; set; }
        public DateTime CreatedDate { get; set; }
        public int Company { get; set; }
        public bool? IsDelete { get; set; }
        public decimal? Distance { get; set; }
        public int? dayScheduleId { get; set; }
        public string AccessCode { get; set; }
        public string ReferenceNo { get; set; }

        public decimal? Vehicle { get; set; }
        public decimal? Labour { get; set; }
        public decimal? PackingMaterials { get; set; }
        public decimal? SeaFreight { get; set; }
        public decimal? ReceivingHandling { get; set; }
        public decimal? DestinationCharges { get; set; }
        public decimal? OriginCost { get; set; }
        public decimal? OriginMarkup { get; set; }
        public decimal? Total { get; set; }
        public string Beds { get; set; }
        public string Cuft { get; set; }
        public string Ftcontainer { get; set; }
        public string SpecialRequirements { get; set; }
    }
}
