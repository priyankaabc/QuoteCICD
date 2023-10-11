using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuoteCalculator.Service.Models
{
    public class AgentModel
    {
        public int AgentId { get; set; }
        public int BranchId { get; set; }
        public string Name { get; set; }
        public string AgentCode { get; set; }
        public bool IsTradeAgent { get; set; }
        public bool IsImportAgent { get; set; }
        public bool IsActive { get; set; }
        
    }
}
