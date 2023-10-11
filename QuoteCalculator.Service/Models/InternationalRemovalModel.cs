using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuoteCalculator.Service.Models
{
    public class InternationalRemovalModel
    {
        public int? Id { get; set; }

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "First Name is required")]
        public string Firstname { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "Last Name is required")]
        public string Lastname { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Display(Name = "Telephone")]
        [Required(ErrorMessage = "Telephone is required")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Enter valid phone number")]
        public string Telephone { get; set; }

        [Display(Name = "Title")]
        [Required(ErrorMessage = "Title is required")]
        public int? TitleId { get; set; }

        [Display(Name = "From Country Code")]
        public string FromCountryName { get; set; }

        [Display(Name = "To Country Code")]
        [Required(ErrorMessage = "To Country is required")]
        public string ToCountryCode { get; set; }

        [Display(Name = "Post Code")]
        [Required(ErrorMessage = "Postcode is required")]
        public string PostCode { get; set; }

        [Display(Name = "City")]
        [Required(ErrorMessage = "City is required")]
        public string CityName { get; set; }

        //[DataType(DataType.Date)]
        [Display(Name = "Estimated Move Date")]
        [Required(ErrorMessage = "Estimated Date is required")]
        public DateTime? EstimatedMoveDate { get; set; }

        [Display(Name = "Condition Applied")]
        public bool? IsConditionApply { get; set; }

        [Display(Name = "Country Code")]
        [Required(ErrorMessage = "Countrycode is required")]
        public string CountryCode { get; set; }

        [Display(Name = "Branch")]
        [Required(ErrorMessage = "Branch is required")]
        public int? BranchId { get; set; }

        [Display(Name = "Home Consultation or Service")]
        public bool? HomeConsultationOrService { get; set; }

        [Display(Name = "Home Video Survery")]
        public bool? HomeVideoSurvery { get; set; }

        [Display(Name = "Quick Online Quote")]
        public bool? QuickOnlineQuote { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Home Consultation Date Time")]
        [Required(ErrorMessage = "Please select Home consultation Time")]
        public DateTime? HomeConsultationDateTime { get; set; }

        [Display(Name = "Day Schedule")]
        [Required(ErrorMessage = "Day Schedule is Required")]
        public int? dayScheduleId { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Appointment Time")]
        [Required(ErrorMessage = "Please select Appointment Time")]
        public DateTime? VideoSurveyAppointmentTime { get; set; }

        public string SelectionType { get; set; }
        public List<QuickQuoteItemsModel> items { get; set; }
        public string SpecialRequirements { get; set; }

        [Required(ErrorMessage = "Please enter Cuft")]
        public string Cuft { get; set; }
        public int? QuickQuoteItemId { get; set; }
        public string SalesRepCode { get; set; }
        public bool? IsDelete { get; set; }
        public bool? IsInquiry { get; set; }
        public int? TotalCount { get; set; }
        public int? TotalFilteredCount { get; set; }
        public string AccessCode { get; set; }
        public string ReferenceNo { get; set; }
        public string daySchedule { get; set; }
        public string StrVideoSurveyAppointmentTime { get; set; }
        public string TitleName { get; set; }
        public string Country_Code { get; set; }
        public string Sr_Code { get; set; }
        public string Sr_Name { get; set; }
        public int VideoSurveyId { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime CreatedDate { get; set; }
        public int Company { get; set; }
        public float Distance { get; set; }
    }

    public class CityListModel
    {
        public string country_code { get; set; }
        public string CityName { get; set; }
    }

    public class QuickQuoteItemsModel
    {
        public int ItemId { get; set; }
        public string Title { get; set; }
        public string Cuft { get; set; }
        public string image { get; set; }
        public string Ftcontainer { get; set; }
        public int? DisplayOrder { get; set; }
        public int company { get; set; }
    }

    //public class InternationRemovalSearchModel : DataTablePaginationModel
    //{
    //    public int CompanyId { get; set; }
    //    public string Firstname { get; set; }        
    //    public string Lastname { get; set; }
    //    public string Email { get; set; }
    //    public string Telephone { get; set; }
    //    public string ToCity { get; set; }
    //    public string RefNo { get; set; }
    //    public string DaySchedule { get; set; }
    //    public string AppoinmentTime { get; set; }
    //    public string AccessCode { get; set; }
    //}
}