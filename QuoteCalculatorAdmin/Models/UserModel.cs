using QuoteCalculatorAdmin.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuoteCalculatorAdmin.Models
{
    public class UserModel
    {
        public long id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string email { get; set; }
        public int RoleId { get; set; }
        public byte[] enabled { get; set; }
        public byte[] account_expired { get; set; }
        public byte[] account_locked { get; set; }
        public byte[] password_expired { get; set; }
        public Nullable<long> version { get; set; }
        public string Token { get; set; }
        public Nullable<System.DateTime> TokenExpiryDate { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string SalesRepCode { get; set; }
        public int CompanyId { get; set; }
        public int? TotalCount { get; set; }

        public virtual tbl_Company tbl_Company { get; set; }
        public virtual tbl_Role tbl_Role { get; set; }
        public string RoleName { get; set; }
        public string CompanyName { get; set; }

    }
}