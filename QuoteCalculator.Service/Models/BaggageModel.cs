using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace QuoteCalculator.Service.Models
{
    public class BaggageModel
    {
        public List<Movebaggage> moveList { get; set; }
        public List<CartonsModel> cartonList { get; set; }
        public baggageQuote baggageQuote { get; set; }
        public List<BaggageCalculationLineModel> calculationLines { get; set; }
        public List<QuoteAmountList> quoteAmountList { get; set; }
        public bool HasMainCartons
        {
            get
            {
                if (cartonList == null || cartonList.Count == 0)
                    return false;
                return cartonList.Any(x => string.Compare(x.type, "MAIN", true) == 0);
            }
        }
        public string rtReferenceNo { get; set; }
        public bool IsDuplicateQuote { get; set; }
        public bool wasGeneratedBefore { get; set; }
        public BaggageCalculationModel baggageCalculation { get; set; }
        public string fileName { get; set; }
    }

    public class Movebaggage
    {
        public long Id { get; set; }
        public string description { get; set; }
        public int length { get; set; }
        public int breadth { get; set; }
        public int height { get; set; }
        public string dimension { get; set; }
        public int quantity { get; set; }
        public double Volume { get; set; }
        public Nullable<int> UserVolume { get; set; }
        public int CalculatedVolume
        {
            get
            {
                return Convert.ToInt16(Math.Ceiling(length * breadth * height / 28316.8));
            }
        }
        public string FullSizeMoveStr
        {
            get
            {
                string dimensionStr = "";
                dimensionStr = Convert.ToString(quantity + " X " + description + " (" + CalculatedVolume + " cubic feet" + (quantity > 1 ? " each)" : ")"));
                if (length > 0 && breadth > 0 && height > 0)
                {
                    dimensionStr += Convert.ToString(", " + length + " X " + breadth + " X " + height + " Cms");
                }
                if (UserVolume > 0)
                {
                    dimensionStr += Convert.ToString(", " + UserVolume + " Kgs");
                }
                return dimensionStr;
            }
        }
    }

    public class baggageQuote
    {
        public int Id { get; set; }

        [Display(Name = "Title*")]
        [Required(ErrorMessage = "Title is Required")]
        public int? TitleId { get; set; }

        [Display(Name = "First Name*")]
        [Required(ErrorMessage = "First Name is Required")]
        public string Firstname { get; set; }

        [Display(Name = "Last Name*")]
        [Required(ErrorMessage = "Last Name is Required")]
        public string Lastname { get; set; }

        [Display(Name = "Email*")]
        [Required(ErrorMessage = "Email is Required")]
        public string Email { get; set; }

        public string CountryCode { get; set; }

        [Display(Name = "Telephone*")]
        [Required(ErrorMessage = "Telephone is Required")]
        public string Telephone { get; set; }

        [Display(Name = "From Country*")]
        [Required(ErrorMessage = "From Country is Required")]
        public string FromCountry { get; set; }

        [Display(Name = "From City*")]
        [Required(ErrorMessage = "From City is Required")]
        public string FromCity { get; set; }

        [Display(Name = "From Post Code*")]
        [Required(ErrorMessage = "Post Code is Required")]
        public string PostCode { get; set; }

        [Display(Name = "To Country*")]
        [Required(ErrorMessage = "To Country is Required")]
        public string ToCountry { get; set; }

        [Display(Name = "To City*")]
        [Required(ErrorMessage = "To City is Required")]
        public string CityName { get; set; }

        [Display(Name = "To Post Code*")]
        [Required(ErrorMessage = "Post Code is Required")]
        public string ToPostCode { get; set; }

        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Estimated MoveDate is Required")]
        [Display(Name = "Estimated MoveDate*")]
        public System.DateTime? EstimatedMoveDate { get; set; }

        [Display(Name = "Internal Notes")]
        public string InternalNotes { get; set; }

        [Display(Name = "Sales Rep")]
        public string SalesRep { get; set; }

        public int Company { get; set; }

        public bool? IsInquiry { get; set; }

        public bool? IsConditionApply { get; set; }
        public int? BranchId { get; set; }
        public bool? AirFreightToAirport { get; set; }
        public bool? AirFreightToDoor { get; set; }
        public bool? Courier { get; set; }
        public bool? SeaFreight { get; set; }
        public double? Price { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ReferenceNumber { get; set; }
        public int TotalCount { get; set; }
        public string AccessCode { get; set; }
        public string BranchName { get; set; }
    }

    public class CollectionDelivery
    {
        public decimal DeliveryCharge { get; set; }
        public decimal CollectionCharge { get; set; }
    }
    public  class BaggageXmlData
    { 
        public string Branch { get; set; }
        public string QuoteStatus { get; set; }
        public string MethodName { get; set; }
        public string ServiceName { get; set; }
        public string QuoteNo { get; set; }
        public string EstimatedMoveDate { get; set; }
        public string DuplicateEnquiry { get; set; }
        public string TitleName { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
        public string PostCode { get; set; }
        public string ToCountryCode { get; set; }
        public string Suburb { get; set; }
        public string FromCountryCode { get; set; }
        public string IxType { get; set; }
        public Nullable<int> Items { get; set; }
        public string Referral { get; set; }
        public string FromSuburb { get; set; }
        public string ToPostCode { get; set; }
        public string InternalNotes { get; set; }
        public string SalesRep { get; set; }
        public string Mobile { get; set; }
        public string CompanyCode { get; set; }
    }
}
