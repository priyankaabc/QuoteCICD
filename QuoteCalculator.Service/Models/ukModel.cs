using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuoteCalculator.Service.Models
{
    public class ukModel
    {
        public string zip { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public string a1 { get; set; }
        public string a2 { get; set; }
        public string state { get; set; }
    }
    public partial class UKbranchpostcode
    {
        public long id { get; set; }
        public long baggage_branch_id { get; set; }
        public string postcode { get; set; }
        public Nullable<short> dcr_zone { get; set; }
        public long removals_branch_id { get; set; }
        public long vehicle_branch_id { get; set; }
        public short removals_quotable { get; set; }
        public int CompanyId { get; set; }

    }
}
