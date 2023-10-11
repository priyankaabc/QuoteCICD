using QuoteCalculatorAdmin.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuoteCalculatorAdmin.Models
{
    public class RoleModelOld
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public bool IsActive { get; set; }
        public int? TotalCount { get; set; }
        public virtual ICollection<user> user { get; set; }
    }
}