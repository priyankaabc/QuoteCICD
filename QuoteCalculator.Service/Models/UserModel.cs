using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuoteCalculator.Service.Models
{
    public class UserModel
    {
        public long id { get; set; }
        [Required(ErrorMessage = "Please enter UserName")]
        public string username { get; set; }
        [Required(ErrorMessage = "Please enter Password ")]
        public string password { get; set; }
        [Required(ErrorMessage = "Please enter email address")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string email { get; set; }
        [Required(ErrorMessage = "Please select Role")]
        public int RoleId { get; set; }
        public byte[] enabled { get; set; }
        public byte[] account_expired { get; set; }
        public byte[] account_locked { get; set; }
        public byte[] password_expired { get; set; }
        public long? version { get; set; }
        public string Token { get; set; }
        public DateTime? TokenExpiryDate { get; set; }
        public bool IsActive { get; set; }
        [Required(ErrorMessage = "Please enter Sales Rep. Code")]
        public string SalesRepCode { get; set; }
        [Required(ErrorMessage = "Company is Required")]
        public int CompanyId { get; set; }
        public long? TotalCount { get; set; }
        public long? TotalFilteredCount { get; set; }
        public string RoleName { get; set; }
        public string CompanyName { get; set; }
        //public virtual tbl_Company tbl_Company { get; set; }
        //public virtual tbl_Role tbl_Role { get; set; }
    }
}
