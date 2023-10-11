using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuoteCalculatorAdmin.Common;
using QuoteCalculatorAdmin.Data;
using QuoteCalculatorAdmin.Data.Repository;

namespace QuoteCalculatorAdmin.Data
{
    [MetadataType(typeof(Metadata))]
    public partial class tbl_Vehicle
    {
        internal class Metadata
        {
            [ScaffoldColumn(false)]
            public int Id { get; set; }

            [Display(Name = "First Name")]
            public string Firstname { get; set; }

            [Display(Name = "Last Name")]
            public string Lastname { get; set; }

            [Display(Name = "Email")]
            public string Email { get; set; }

            [Display(Name = "Telephone")]
            public string Telephone { get; set; }

            [Display(Name = "Title")]
            public int TitleId { get; set; }

            [Display(Name = "From Country Code")]
            public string FromCountryName { get; set; }

            [Display(Name = "To Country Code")]
            public string ToCountryCode { get; set; }

            [Display(Name = "Post Code")]
            public string PostCode { get; set; }

            [Display(Name = "City")]
            public string CityName { get; set; }

            [Display(Name = "Estimated Move Date")]
            public System.DateTime EstimatedMoveDate { get; set; }

            [Display(Name = "Vehicle Type")]
            public Nullable<int> VehicleType { get; set; }

            [Display(Name = "Make")]
            public string VehicleMakeName { get; set; }

            [Display(Name = "Model")]
            public string VehicleModelName { get; set; }

            [Display(Name = "Is Owned Car")]
            public bool IsOwnedCar { get; set; }

            [Display(Name = "Special Requirement")]
            public string SpecialRequirement { get; set; }

            [Display(Name = "Condition Applied")]
            public bool IsConditionApply { get; set; }

            [Display(Name = "Country Code")]
            public string CountryCode { get; set; }

            [Display(Name = "Branch")]
            public Nullable<int> BranchId { get; set; }

            public Nullable<double> FCL { get; set; }
            public Nullable<double> GPG { get; set; }

            [Display(Name = "Quote No")]
            public string QuoteNo { get; set; }

            [Display(Name = "Enquiry No")]
            public string EnquiryNo { get; set; }

            [Display(Name = "Volume FCL")]
            public Nullable<double> Volume_FCL { get; set; }

            [Display(Name = "Volume GRP")]
            public Nullable<double> Volume_GRP { get; set; }

            [Display(Name = "Is FCL")]
            public Nullable<bool> IsFCL { get; set; }

            [Display(Name = "Is GRP")]
            public Nullable<bool> IsGRP { get; set; }
        }
    }

    [MetadataType(typeof(Metadata))]
    public partial class tbl_InternationalRemoval
    {
        internal class Metadata
        {
            [ScaffoldColumn(false)]
            public int Id { get; set; }

            [Display(Name = "First Name")]
            public string Firstname { get; set; }

            [Display(Name = "Last Name")]
            public string Lastname { get; set; }

            [Display(Name = "Email")]
            public string Email { get; set; }

            [Display(Name = "Telephone")]
            public string Telephone { get; set; }

            [Display(Name = "Title")]
            public int TitleId { get; set; }

            [Display(Name = "From Country Code")]
            public string FromCountryName { get; set; }

            [Display(Name = "To Country Code")]
            public string ToCountryCode { get; set; }

            [Display(Name = "Post Code")]
            public string PostCode { get; set; }

            [Display(Name = "City")]
            public string CityName { get; set; }

            [Display(Name = "Estimated Move Date")]
            public System.DateTime EstimatedMoveDate { get; set; }

            [Display(Name = "Condition Applied")]
            public bool IsConditionApply { get; set; }

            [Display(Name = "Country Code")]
            public string CountryCode { get; set; }

            [Display(Name = "Branch")]
            public Nullable<int> BranchId { get; set; }

            [Display(Name = "Home Consultation or Service")]
            public Nullable<bool> HomeConsultationOrService { get; set; }

            [Display(Name = "Home Video Survery")]
            public Nullable<bool> HomeVideoSurvery { get; set; }

            [Display(Name = "Quick Online Quote")]
            public Nullable<bool> QuickOnlineQuote { get; set; }

            [Display(Name = "Home Consultation Date Time")]
            public Nullable<System.DateTime> HomeConsultationDateTime { get; set; }

            [Display(Name ="Day Schedule")]
            public Nullable<int> dayScheduleId { get; set; }

            [Display(Name ="Appointment Time")]
            public DateTime? VideoSurveyAppointmentTime { get; set; }
        }

    }

    [MetadataType(typeof(Metadata))]
    public partial class tbl_baggageQuote
    {
        internal class Metadata
        {
            [ScaffoldColumn(false)]
            public int Id { get; set; }

            [Display(Name = "First Name")]
            public string Firstname { get; set; }

            [Display(Name = "Last Name")]
            public string Lastname { get; set; }

            [Display(Name = "Email")]
            public string Email { get; set; }

            [Display(Name = "Telephone")]
            public string Telephone { get; set; }

            [Display(Name = "Title")]
            public int TitleId { get; set; }

            [Display(Name = "From Country")]
            public string FromCountry { get; set; }

            [Display(Name = "From City")]
            public string FromCity { get; set; }

            [Display(Name = "From PostCode")]
            public string PostCode { get; set; }

            [Display(Name = "To PostCode")]
            public string ToPostCode { get; set; }

            [Display(Name = "To City")]
            public string CityName { get; set; }

            [Display(Name = "Estimated Move Date")]
            public DateTime? EstimatedMoveDate { get; set; }

            [Display(Name = "Country Code")]
            public string CountryCode { get; set; }

            [Display(Name = "Branch")]
            public Nullable<int> BranchId { get; set; }

            [Display(Name = "To Country")]
            public string ToCountry { get; set; }

            [Display(Name = "Air Freight To Airport")]
            public Nullable<bool> AirFreightToAirport { get; set; }

            [Display(Name = "Air Freight To Door")]
            public Nullable<bool> AirFreightToDoor { get; set; }

            public Nullable<bool> Courier { get; set; }

            [Display(Name = "Sea Freight")]
            public Nullable<bool> SeaFreight { get; set; }

            public Nullable<double> Price { get; set; }

           
        }
    }

    [MetadataType(typeof(Metadata))]
    public partial class tbl_EmailTemplate
    {
        internal class Metadata
        {
            [ScaffoldColumn(false)]
            public int Id { get; set; }
            [Display(Name = "Service")]
            [Required(ErrorMessage = "Please select service")]
            public int ServiceId { get; set; }

            [Required(ErrorMessage = "Please enter subject")]
            public string Subject { get; set; }

            // [Required(ErrorMessage = "Please enter email template")]
            [AllowHtml]
            public string HtmlContent { get; set; }
            public Nullable<int> CreatedBy { get; set; }
            public Nullable<System.DateTime> CreatedOn { get; set; }
            public Nullable<int> UpdateBy { get; set; }
            public Nullable<System.DateTime> UpdateOn { get; set; }
        }
    }

    [MetadataType(typeof(Metadata))]
    public partial class tbl_HeadingContent : IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (CustomRepository.CheckHeadingContentName(HeadingContent, HeadingContentId))
            {
                var fieldName = new[] { "HeadingContent" };
                yield return new ValidationResult("HeadingContent is Already Exists.", fieldName);
            }
        }
        [ScaffoldColumn(false)]
        public List<string> FromCountryList { get; set; }

        public List<string> CountryList { get; set; }

        public List<string> QuoteTypeList { get; set; }

        public List<string> CompanyList { get; set; }

        internal class Metadata
        {
            [ScaffoldColumn(false)]
            public int HeadingContentId { get; set; }

            [Display(Name = "Heading Content")]
            [AllowHtml]
            public string HeadingContent { get; set; }
            [Display(Name = "To Country")]
            //[Required(ErrorMessage = "Select Country")]
            public string CountryCode { get; set; }
            [Display(Name = "Display Order")]
            [Required(ErrorMessage = "Enter Display Order")]
            public Nullable<int> DisplayOrder { get; set; }

            [Display(Name = "Heading")]
            [Required(ErrorMessage = "Enter Heading")]
            public string Heading { get; set; }

            [Display(Name = "Quote Type")]
            [Required(ErrorMessage = "Required atleast 1 Quote Type")]
            public string QuoteType { get; set; }

            [Display(Name = "From Country")]
            //[Required(ErrorMessage = "Select Country")]
            public string FromCountryCode { get; set; }
        }
    }

    [MetadataType(typeof(Metadata))]
    public partial class user : IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (CustomRepository.CheckAdminUserExistsOrNot(username, id))
            {
                var fieldName = new[] { "username" };
                yield return new ValidationResult("User name is already exists.", fieldName);
            }
            if (CustomRepository.CheckAdminEmailExistsOrNot(email, id))
            {
                var fieldName = new[] { "email" };
                yield return new ValidationResult("EmailAddress is already exists.", fieldName);
            }
        }
        internal class Metadata
        {
            [ScaffoldColumn(false)]
            public long id { get; set; }

            [UIHint("TextBox")]
            [Display(Name = "Full Name", Order = 1)]
            [StringLength(50, MinimumLength = 4, ErrorMessage = "Maximum Length is 50 And Minimum length is 4")]
            [Required(ErrorMessage = "Please enter username")]
            public string username { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Password", Order = 2)]
            [StringLength(50, ErrorMessage = "Maximum Length is 50")]
            [Required(ErrorMessage = "Please enter password")]
            public string password { get; set; }

            [UIHint("Email")]
            [Display(Name = "Email Address", Order = 3)]
            [RegularExpression(CommonHelper.RegexEmail, ErrorMessage = "Invalid Email")]
            [StringLength(50, ErrorMessage = "Maximum Length is 50")]
            [Required(ErrorMessage = "Please enter email")]
            public string email { get; set; }

            [UIHint("TextBox")]
            [Display(Name = "Sales Rep. Code", Order = 4)]
            [Required(ErrorMessage = "Please enter Sales Rep. Code")]
            public string SalesRepCode { get; set; }

            [ScaffoldColumn(false)]
            public byte[] enabled { get; set; }
            [ScaffoldColumn(false)]
            public byte[] account_expired { get; set; }
            [ScaffoldColumn(false)]
            public byte[] account_locked { get; set; }
            [ScaffoldColumn(false)]
            public byte[] password_expired { get; set; }
            [ScaffoldColumn(false)]
            public Nullable<long> version { get; set; }
            [ScaffoldColumn(false)]
            public string Token { get; set; }
            [ScaffoldColumn(false)]
            public Nullable<System.DateTime> TokenExpiryDate { get; set; }

            [UIHint("GridForeignKey")]
            [Display(Name = "Role", Order = 4)]
            [Required(ErrorMessage = "Please select Role")]
            public int RoleId { get; set; }
            [UIHint("CheckBox")]
          //  [Display(Name = "Is Active")]
            public Nullable<bool> IsActive { get; set; }

            [UIHint("GridForeignKey")]
            [Display(Name = "Company", Order = 6)]
            [Required(ErrorMessage = "Please select Company")]
            public int CompanyId { get; set; }
        }
    }

    [MetadataType(typeof(Metadata))]
    public partial class tbl_ChildContent
    {
        public List<string> CountryList { get; set; }
        internal class Metadata
        {
            public int ChildContentId { get; set; }
            [Display(Name = "Heading Content")]
            [Required(ErrorMessage = "Select Heading Content")]
            public Nullable<int> HeadingContentId { get; set; }

            [Display(Name = "Child Content")]
            [Required(ErrorMessage = "Enter Child Content")]
            public string ChildContent { get; set; }

            public string CountryCode { get; set; }
            [Display(Name = "Display Order")]
            [Required(ErrorMessage = "Enter DisplayOrder")]
            public Nullable<int> DisplayOrder { get; set; }

        }
    }
    [MetadataType(typeof(Metadata))]
    public partial class tbl_Role : IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (CustomRepository.CheckRoleExistsOrNot(RoleName, RoleId))
            {
                var fieldName = new[] { "RoleName" };
                yield return new ValidationResult("Role name is already exists.", fieldName);
            }

        }
        internal class Metadata
        {
            [ScaffoldColumn(false)]
            public int RoleId { get; set; }

            [UIHint("TextBox")]
            [Display(Name = "Role Name")]
            [Required(ErrorMessage = "Enter Role Name")]
            //  [StringLength(50, ErrorMessageResourceName = "RoleNameLength", ErrorMessageResourceType = typeof(Message))]
            public string RoleName { get; set; }

            [UIHint("CheckBox")]
            [Display(Name = "Is Active")]
            public bool IsActive { get; set; }

        }
        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{
        //    if (CustomRepository.CheckRoleExistsOrNot(RoleName, RoleId))
        //    {
        //        var fieldName = new[] { "RoleName" };
        //        yield return new ValidationResult("Role is Already Exists.", fieldName);
        //    }
        //}


    }

}