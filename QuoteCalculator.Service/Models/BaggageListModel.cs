using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuoteCalculator.Service.Models
{
    public class BaggageListModel
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }

        public string Email { get; set; }

        public string Telephone { get; set; }

        public string CityName { get; set; }

        public string ReferenceNumber { get; set; }

        public string SalesRep { get; set; }

        public string AccessCode { get; set; }

        public string BranchName { get; set; }
        public string TitleName { get; set; }
        public long? TotalCount { get; set; }

        public long? TotalFilteredCount { get; set; }
    }
}
