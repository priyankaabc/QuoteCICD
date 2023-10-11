using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuoteCalculator.Service.Models
{
    public class ImportsQuoteModel
    {
        public long Id { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }

        [Required(ErrorMessage = "Customer Name is Required")]
        public string CustomerName { get; set; }

        [Required(ErrorMessage = "Agent is Required")]
        public int? AgentId { get; set; }
        public string AgentName { get; set; }
        public string StrCountry { get; set; }

        [Required(ErrorMessage = "Origin Country is Required")]
        public string OriginCountry { get; set; }
        public string OriginCityId { get; set; }

        [Required(ErrorMessage = "Origin Town is Required")]
        public string OriginTown { get; set; }

        [Required(ErrorMessage = "Destination Address1 is Required")]
        public string DestAddress1 { get; set; }
        public string DestAddress2 { get; set; }
        public string DestinationCounty { get; set; }

        [Required(ErrorMessage = "Destination Postcode is Required")]
        public string DestPostcode { get; set; }

        [Required(ErrorMessage = "POE is Required")]
        public int POEId { get; set; }
        public string StrPOE { get; set; }

        [Required(ErrorMessage = "Branch is Required")]
        public string Branch { get; set; }
        public string StrBranch { get; set; }

        [Required(ErrorMessage = "Service is Required")]
        public int ServiceId { get; set; }

        [Required(ErrorMessage = "Container Size is Required")]
        public int ContainerSizeId { get; set; }
        public string StrContainerSize { get; set; }

        public string StrService { get; set; }
        public decimal? Rate { get; set; }
        public decimal? TotalCost { get; set; }

        public string POE { get; set; }
        public string Service { get; set; }
        public string RefNo { get; set; }

        //public List<Destination> Destinations { get; set; }

        public string ConsigneeName { get; set; }
        public decimal? Kgs { get; set; }
        public string Vehicle { get; set; }
        public int? VehicleId { get; set; }
        public bool? IsCollectFromBranch { get; set; }
        public List<AddtionalCostModel> AdditinalCostList { get; set; }
        public string TotalConsignee { get; set; }
        public string IxType { get; set; }
        public string SalesRep { get; set; }
        public string AgentCode { get; set; }
        public string Operation { get; set; }
        public bool ShowDestDetail { get; set; }
        public string CompanyCode { get; set; }
        public string Note { get; set; }
        public long? TotalCount { get; set; }
        public long? TotalFilteredCount { get; set; }
        public int Company { get; set; }
        public long UserId { get; set; }

    }
   
    public class DestinationDetails
    {
        [ScaffoldColumn(false)]
        public int Id { get; set; }

        //[Required]
        //[DisplayName("Consignee Name")]
        //[Remote("IsProductName_Available", "Validation")]
        [Required(ErrorMessage = "Consignee Name is Required")]
        [RegularExpression(@"^.*[a-zA-Z0-9]+.*$", ErrorMessage = "Enter valid Consignee Name" )]
        public string ConsigneeName { get; set; }      

        [DisplayName("Volume/Kgs")]
        [Range(0, int.MaxValue, ErrorMessage = "Volume is not valid")]
        [DataType("Decimal")]        
        public decimal Kgs { get; set; }
        
        //public string Vehicle { get; set; }
        public int? VehicleId { get; set; }
        //[Required(ErrorMessage = "Destination Address1 is Required")]
        public string DestAddress1 { get; set; }
        public string DestAddress2 { get; set; }

        //[Required(ErrorMessage = "Destination Postcode is Required")]
        public string DestPostcode { get; set; }

        [DisplayName("Collect From Branch")]
        public bool IsCollectFromBranch { get; set; }
        //public List<AddtionalCostModel> AdditinalCostList { get; set; }

        //[UIHint("Vehicle")]
        //public int Vehicle { get; set; }
        [UIHint("Vehicle")]
        public string VehicleName { get; set; }

        [DataType("Decimal")]
        public decimal? Cost { get; set; }

        public string TotalCost { get; set; }
        public string AdditionalCost { get; set; }
        public string Address { get; set; }
        public string CollectFromBranch { get; set; }
        public decimal? Tariff { get; set; }
        public string StrRate { get; set; }
    }

    //public class AddtionalCostModel
    //{
    //    public long? AdditionalCostId { get; set; }
    //    public string Type { get; set; }

    //    [DataType("Decimal")]
    //    public decimal? Cost { get; set; }
    //    public long? DestId { get; set; }
    //}

    public class VehicleViewModel
    {
        public int Value { get; set; }
        public string Text { get; set; }
    }
    public class ImportsQuoteAddCost
    {
        public List<AddtionalCostModel> AddCostList { get; set; }
        public int CompanyId { get; set; }
        public long UserId { get; set; }
        public long DestinationId { get; set; }
        
    }
}