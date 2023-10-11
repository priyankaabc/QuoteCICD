using QuoteCalculatorAdmin.Data;
using QuoteCalculatorAdmin.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuoteCalculatorAdmin.Common
{
    public class SelectionList
    {
        public static List<tbl_Role> RoleList()
        {
            using (quotesEntities _dbContext = BaseContext.GetDbContext())
            {
                return _dbContext.tbl_Role.Where(m => m.IsActive).OrderBy(x => x.RoleName).ToList();
            }
        }
        public static List<tbl_Company> CompanyList()
        {
            using (quotesEntities _dbContext = BaseContext.GetDbContext())
            {
                return _dbContext.tbl_Company.OrderBy(x => x.Id).ToList();
            }
        }
        public static List<tbl_Title> TitleList()
        {
            using (quotesEntities _dbContext = BaseContext.GetDbContext())
            {
                return _dbContext.tbl_Title.OrderBy(x => x.DisplayOrder).ToList();
            }
        }
        public static List<branch> BranchList()
        {
            using (quotesEntities _dbContext = BaseContext.GetDbContext())
            {
                return _dbContext.branch.ToList();
            }
        }
        public static List<tbl_VehicleType> VehicleTypeList()
        {
            using (quotesEntities _dbContext = BaseContext.GetDbContext())
            {
                return _dbContext.tbl_VehicleType.ToList();
            }
        }
    }
}