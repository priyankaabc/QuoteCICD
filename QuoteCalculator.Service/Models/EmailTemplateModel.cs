using System.ComponentModel.DataAnnotations;

namespace QuoteCalculator.Service.Models
{
    public class EmailTemplateModel
    {
        public int id { get; set; }

        [Display(Name = "Service")]
        [Required(ErrorMessage = "Service Name is required")]
        public int ServiceId { get; set; }

        public string ServiceName { get; set; }


        [Display(Name = "Email Subject")]
        [Required(ErrorMessage = "Email Subject is required")]

        public string Subject { get; set; }

        [Display(Name = "HTML Content")]
        [Required(ErrorMessage = "HTML Content is required")]
        public string HtmlContent { get; set; }
        public int? UserId { get; set; }
        public long? TotalCount { get; set; }
        public long? TotalFilteredCount { get; set; }
    }
    public class EmailSettingsModel
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
