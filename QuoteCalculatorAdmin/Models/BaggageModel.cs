using QuoteCalculatorAdmin.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QuoteCalculatorAdmin.Models
{
    public class BaggageModel
    {
        public List<Movebaggage> moveList { get; set; }
        public List<QuoteCalculator.Service.Models.CartonsModel> cartonList { get; set; }
        public baggageQuote baggageQuote { get; set; }
        public List<BaggageCalculationLineModel> calculationLines { get; set; }
        public List<QuoteCalculator.Service.Models.QuoteAmountList> quoteAmountList { get; set; }
        public bool HasMainCartons
        {
            get
            {
                if (cartonList == null || cartonList.Count == 0)
                    return false;
                return cartonList.Count(x => string.Compare(x.type, "MAIN", true) == 0) > 0;
            }
        }
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
        public int? TitleId { get; set; }

        //[Required(ErrorMessage = "First Name required")]
        [Display(Name = "First Name*")]
        public string Firstname { get; set; }

        //[Required(ErrorMessage = "Last Name required")]
        [Display(Name = "Last Name*")]
        public string Lastname { get; set; }

        //[Required(ErrorMessage = "Email Name required")]
        [Display(Name = "Email*")]
        public string Email { get; set; }

        public string CountryCode { get; set; }

        [Display(Name = "Telephone*")]
        //[Required(ErrorMessage = "Telephone is Required")]
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
        //[Required(ErrorMessage = "Internal Note is Required")]
        public string InternalNotes { get; set; }

        [Display(Name = "Sales Rep")]
        //[Required(ErrorMessage = "Sales Rep is Required")]
        public string SalesRep { get; set; }

        public int Company { get; set; }

        public bool? IsInquiry { get; set; }

        public bool? IsConditionApply { get; set; }
        public Nullable<int> BranchId { get; set; }
        public Nullable<bool> AirFreightToAirport { get; set; }
        public Nullable<bool> AirFreightToDoor { get; set; }
        public Nullable<bool> Courier { get; set; }
        public Nullable<bool> SeaFreight { get; set; }
        public Nullable<double> Price { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ReferenceNumber { get; set; }
        public string AccessCode { get; set; }
        public string BranchName { get; set; }
        public string TitleName { get; set; }
        public long? TotalCount { get; set; }
        public long? TotalFilteredCount { get; set; }
    }

    public class QuoteAmountList
    {
        public int QuoteId { get; set; }
    }
}