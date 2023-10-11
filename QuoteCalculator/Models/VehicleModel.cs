using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QuoteCalculator.Models
{
    public class VehicleModel
    {
        public string TocountryCode { get; set; }
        public string CityName { get; set; }
        public string VehicleMakeName { get; set; }
        public string VehicleModelName { get; set; }
        public string VehicleBranchName { get; set; }

        public int Id { get; set; }
        [Display(Name = "Username")]
        [Required(ErrorMessage = "User name is required")]

        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
        public int TitleId { get; set; }
        public int FromCountryId { get; set; }
        public int ToCountryId { get; set; }
        public string PostCode { get; set; }
        public System.DateTime EstimatedMoveDate { get; set; }
        public Nullable<int> VehicleType { get; set; }
        public int VehiclemakeId { get; set; }
        public bool IsOwnedCar { get; set; }
        public string SpecialRequirement { get; set; }
        public bool IsConditionApply { get; set; }

    }
}