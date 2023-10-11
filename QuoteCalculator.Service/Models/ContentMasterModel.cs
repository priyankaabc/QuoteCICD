using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QuoteCalculator.Service.Models
{
    public class ContentMasterModel
    {
        public int ChildContentId { get; set; }

        [Display(Name = "Heading Content")]
        [Required(ErrorMessage = "Please Select Heading Content")]
        public int? HeadingContentId { get; set; }

        [Display(Name = "Child Content")]
        [Required(ErrorMessage = "Enter Child Content")]
        public string ChildContent { get; set; }

        public string CountryCode { get; set; }

        [Display(Name = "Display Order")]
        [Required(ErrorMessage = "Enter DisplayOrder")]
        [Range(1, int.MaxValue, ErrorMessage = "Display Order should be greater than 0")]
        public int? DisplayOrder { get; set; }

        public long? TotalCount { get; set; }

        public long? TotalFilteredCount { get; set; }
        [Required(ErrorMessage = "Please select country")]

        public List<string> CountryList { get; set; }

    }
}