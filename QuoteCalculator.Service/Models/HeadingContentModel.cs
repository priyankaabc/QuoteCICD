using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace QuoteCalculator.Service.Models
{
    public class HeadingContentModel
    {
        public int HeadingContentId { get; set; }

        [Display(Name = "Heading Content")]
        [AllowHtml]
        public string HeadingContent { get; set; }

        [Display(Name = "To Country")]
        public string CountryCode { get; set; }

        [Display(Name = "Display Order")]
        //    [Required(ErrorMessage = "Display Order is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Display Order should be greater than 0")]
        public int? DisplayOrder { get; set; }

        [Display(Name = "Heading")]
        [Required(ErrorMessage = "Heading is required")]
        public string Heading { get; set; }

        [Display(Name = "Quote Type")]
        [Required(ErrorMessage = "Required atleast 1 Quote Type")]
        public string QuoteType { get; set; }
       
        public string Company { get; set; }

        [Display(Name = "From Country")]
        public string FromCountryCode { get; set; }

        [Required(ErrorMessage = "From Country is Required")]
        public List<string> FromCountryList { get; set; }

        [Required(ErrorMessage = "To Country is required ")]
        public List<string> CountryList { get; set; }
        
        public List<string> QuoteTypeList { get; set; }

        [Required(ErrorMessage = "Company is required")]
        public List<string> CompanyList { get; set; }

        public long? TotalCount { get; set; }

        public long? TotalFilteredCount { get; set; }
    }
}
