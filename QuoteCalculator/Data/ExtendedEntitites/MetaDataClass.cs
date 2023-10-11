using QuoteCalculator.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuoteCalculator.Data
{
    [MetadataType(typeof(Metadata))]
    public partial class tbl_Vehicle
    {
        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{
        //    if (CustomRepository.CheckSecurityPersonExistsOrNot(UserName, SecurityUserId))
        //    {
        //        var fieldName = new[] { "UserName" };
        //        yield return new ValidationResult("UserName with same name is Already Exists.", fieldName);
        //    }
        //    if (CustomRepository.CheckEmailExistsOrNot(EmailAddress, SecurityUserId))
        //    {
        //        var fieldName = new[] { "EmailAddress" };
        //        yield return new ValidationResult("EmailAddress is Already Exists.", fieldName);
        //    }
        //}


        //[Required(ErrorMessage = "Country is Required")]
        //[Display(Name = "Country moving TO*")]
        //public int ToCountryId { get; set; }
        internal class Metadata
        {
            public Nullable<int> BranchId { get; set; }
            [Required(ErrorMessage = "First Name is Required")]
            [StringLength(50, ErrorMessage = "Maximum Length is 50")]
            [Display(Name = "First Name*")]
            public string Firstname { get; set; }

            [Required(ErrorMessage = "Last Name is Required")]
            [StringLength(50, ErrorMessage = "Maximum Length is 50")]
            [Display(Name = "Last Name*")]
            public string Lastname { get; set; }

            [Required(ErrorMessage = "Email Address is Required")]
            [StringLength(50, ErrorMessage = "Maximum Length is 50 ")]
            [RegularExpression(CommonHelper.RegexEmail, ErrorMessage = "Invalid Email")]
            [Display(Name = "Email*")]
            public string Email { get; set; }

            [Display(Name = "Telephone*")]
            [Required(ErrorMessage = "Telephone is Required")]
            //[RegularExpression(CommonHelper.RegexMobile, ErrorMessage = "Invalid Mobile No")]
            public string Telephone { get; set; }

            //[Required(ErrorMessage = "Title is Required")]
            [Display(Name = "Title*")]
            public int TitleId { get; set; }

            [Display(Name = "Country Moving FROM*")]
            public int FromCountryName { get; set; }

            [Required(ErrorMessage = "Country is Required")]
            [Display(Name = "Country Moving - TO*")]
            public string ToCountryCode { get; set; }

            [Required(ErrorMessage = "Postcode is Required")]
            [Display(Name = "Moving FROM - PostCode*")]
            public string PostCode { get; set; }

            [Required(ErrorMessage = "City is Required")]
            [Display(Name = "Moving TO - City*")]
            public int CityName { get; set; }

            [DataType(DataType.Date)]
            [Required(ErrorMessage = "Estimated MoveDate is Required")]
            [Display(Name = "Estimated Move Date*")]
            public DateTime EstimatedMoveDate { get; set; }

            //[Required(ErrorMessage = "Vehicle Type is Required")]
            [Display(Name = "Type of vehicle*")]
            public int VehicleType { get; set; }

            [Required(ErrorMessage = "Vehicle Make is Required")]
            [Display(Name = "Make*")]
            public string VehicleMakeName { get; set; }

            [Required(ErrorMessage = "Vehicle Model is Required")]
            [Display(Name = "Model")]
            public string VehicleModelName { get; set; }

            public bool IsOwnedCar { get; set; }

            public string SpecialRequirement { get; set; }

            public bool IsConditionApply { get; set; }

            [Display(Name = "Country Code*")]
            public string CountryCode { get; set; }
        }

    }
    [MetadataType(typeof(Metadata))]
    public partial class tbl_baggageQuote
    {
        internal class Metadata
        {

            [Required(ErrorMessage = "First Name is Required")]
            [StringLength(50, ErrorMessage = "Maximum Length is 50")]
            [Display(Name = "First Name*")]
            public string Firstname { get; set; }

            [Required(ErrorMessage = "Last Name is Required")]
            [StringLength(50, ErrorMessage = "Maximum Length is 50")]
            [Display(Name = "Last Name*")]
            public string Lastname { get; set; }

            [Required(ErrorMessage = "Email Address is Required")]
            [StringLength(50, ErrorMessage = "Maximum Length is 50 ")]
            [RegularExpression(CommonHelper.RegexEmail, ErrorMessage = "Invalid Email")]
            [Display(Name = "Email*")]
            public string Email { get; set; }


            [Display(Name = "Telephone*")]
            [Required(ErrorMessage = "Telephone is Required")]
            // [RegularExpression(CommonHelper.RegexMobile, ErrorMessage = "Invalid Mobile No")]
            public string Telephone { get; set; }

            [Required(ErrorMessage = "Title is Required")]
            [Display(Name = "Title*")]
            public int TitleId { get; set; }

            [Display(Name = "Country Moving FROM*")]
            public string FromCountry { get; set; }

            [Required(ErrorMessage = "Country is Required")]
            [Display(Name = "Country Moving TO*")]
            public string ToCountry { get; set; }

            [Display(Name = "Moving FROM - City*")]
            public string FromCity { get; set; }

            //[Required(ErrorMessage = "Post Code is Required")]
            [Display(Name = "Moving FROM - PostCode*")]
            public string PostCode { get; set; }

            //[Required(ErrorMessage = "City is Required")]
            [Display(Name = "Moving TO - City*")]
            public string CityName { get; set; }

            [Display(Name = "Moving TO - PostCode*")]
            public string ToPostCode { get; set; }

            [DataType(DataType.Date)]
            [Required(ErrorMessage = "Estimated MoveDate is Required")]
            [Display(Name = "Estimated Move Date*")]
            public DateTime EstimatedMoveDate { get; set; }

            [Required(ErrorMessage = "Please accept terms and condition ")]
            public bool IsConditionApply { get; set; }

            [Display(Name = "Country Code*")]
            public string CountryCode { get; set; }
        }

    }

    [MetadataType(typeof(Metadata))]
    public partial class tbl_InternationalRemoval
    {
        internal class Metadata
        {
            [Required(ErrorMessage = "First name is required")]
            [StringLength(50, ErrorMessage = "Maximum Length is 50")]
            [Display(Name = "First Name*")]
            public string Firstname { get; set; }

            [Required(ErrorMessage = "Last name is required")]
            [StringLength(50, ErrorMessage = "Maximum Length is 50")]
            [Display(Name = "Last Name*")]
            public string Lastname { get; set; }

            [Required(ErrorMessage = "Email address is required")]
            [StringLength(50, ErrorMessage = "Maximum Length is 50 ")]
            [RegularExpression(CommonHelper.RegexEmail, ErrorMessage = "Invalid Email")]
            [Display(Name = "Email*")]
            public string Email { get; set; }

            [Display(Name = "Country Code*")]
            public string CountryCode { get; set; }

            [Display(Name = "Telephone*")]
            [Required(ErrorMessage = "Telephone is required")]
            //[RegularExpression(CommonHelper.RegexMobile, ErrorMessage = "Invalid Mobile No")]
            public string Telephone { get; set; }

            [Required(ErrorMessage = "Title is required")]
            [Display(Name = "Title* ")]
            public int TitleId { get; set; }

            [Display(Name = "Moving FROM - Country*")]
            public string FromCountryName { get; set; }

            [Required(ErrorMessage = "Country is required")]
            [Display(Name = "Moving TO - Country*")]
            public string ToCountryCode { get; set; }

            [Required(ErrorMessage = "Postcode is required")]
            [Display(Name = "Moving FROM - PostCode*")]
            public string PostCode { get; set; }

            [Required(ErrorMessage = "City is required")]
            [Display(Name = "Which city are you moving to?*")]
            public string CityName { get; set; }

            [DataType(DataType.Date)]
            [Required(ErrorMessage = "Estimated move date is required")]
            [Display(Name = "Estimated Move Date*")]
            public DateTime EstimatedMoveDate { get; set; }

            public Nullable<bool> IsConditionApply { get; set; }
            public Nullable<bool> HomeConsultationOrService { get; set; }
            public Nullable<bool> HomeVideoSurvery { get; set; }
            public Nullable<bool> QuickOnlineQuote { get; set; }

        }
    }

}