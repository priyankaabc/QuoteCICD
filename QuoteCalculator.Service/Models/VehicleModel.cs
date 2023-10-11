using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuoteCalculator.Service.Models
{
    public class VehicleModel
    {
        public int TitleId { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
        public string CityName { get; set; }
        public string QuoteNo { get; set; }
        public string EnquiryNo { get; set; }
        public int Company { get; set; }
        public int Id { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string AccessCode { get; set; }

        public string TitleName { get; set; }
        public long? TotalCount { get; set; }
        public long? TotalFilteredCount { get; set; }
    }
    public class EditVehicleModel
    {
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
