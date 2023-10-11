using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuoteCalculator.Service.Models
{
    public class RoleModel
    {
        public int RoleId { get; set; }

        [Required(ErrorMessage = "Role Name is required")]
        public string RoleName { get; set; }
        public bool IsActive { get; set; }
        public long? TotalCount { get; set; }
        public long? TotalFilteredCount { get; set; }
        //public virtual ICollection<user> user { get; set; }
    }
}
