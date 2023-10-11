using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuoteCalculator.Service.Models
{

    public class BaggageQuoteInfoModel
    {        
        public string CartonList { get; set; }     
        public List<BaggageCalculationLineModel> BaggagePriceList = new List<BaggageCalculationLineModel>();
        public string CustomerRefNo { get; set; }
        public bool isMethodSelected { get; set; }
        public decimal DeliveryCharge { get; set; }
        public decimal CollectionCharge { get; set; }
        public bool HasMainCartons { get; set; }        
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
        public string BranchName { get; set; }
        public string AccessCode { get; set; }

    }


    public class SP_GetCollectionDelivery_Result
    {
        public decimal DeliveryCharge { get; set; }
        public decimal CollectionCharge { get; set; }
    }
    public class QuoteAmountList
    {
        public int QuoteId { get; set; }
    }

    public class BaggageItemModel
    {
        public long Id { get; set; }
        public long QuoteId { get; set; }
        public long CartonId { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public int Length { get; set; }
        public int Breadth { get; set; }
        public int Height { get; set; }
        public double Volume { get; set; }
        public int? UserVolume { get; set; }
        public int Quantity { get; set; }
        public int Groweight { get; set; }
        public string MovewareDescription { get; set; }
        public double? AirtFreightToAirport { get; set; }
        public double? Courier { get; set; }
        public double SeaFreight { get; set; }
        public int Company { get; set; }
        public string ShippingTypeDescription { get; set; }

    }



    public class BaggageCalculationModel
    {
        public decimal? AirFreightToAirport { get; set; }
        public decimal? AirFreightToDoor { get; set; }
        public decimal? Courier { get; set; }
        public decimal? SeaFreight { get; set; }
        public decimal? RoadFreightToDoor { get; set; }
        public decimal? CourierExpressToDoor { get; set; }
    }

    public class BaggageCostModel
    {
        public long Id { get; set; }
        public int QuoteId { get; set; }
        public string ShippingType { get; set; }
        public string CostType { get; set; }
        public decimal Cost { get; set; }
    }
    public class BaggageAmountModel
    {
        public int Id { get; set; }
        public int QuoteId { get; set; }
        public string MoveType { get; set; }
        public Nullable<decimal> QuoteAmount { get; set; }
        public string ShippingType { get; set; }
        public string ShippingTypeDescription { get; set; }
        public string TransitionTime { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsBooked { get; set; }
        public long? CustomerReferenceNo { get; set; }
        public int? CustomerQuoteNo { get; set; }
        public int? QuoteSeqNo { get; set; }
        public int? Company { get; set; }

    }
}
