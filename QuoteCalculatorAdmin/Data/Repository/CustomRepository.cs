using QuoteCalculator.Service.Models;
using QuoteCalculatorAdmin.Common;
using QuoteCalculatorAdmin.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace QuoteCalculatorAdmin.Data.Repository
{
    public class BaseContext
    {
        public static quotesEntities GetDbContext()
        {
            quotesEntities context = new quotesEntities();
            context.Configuration.ProxyCreationEnabled = false;
            ObjectContext objectContext = ((IObjectContextAdapter)context).ObjectContext;
            objectContext.CommandTimeout = int.MaxValue;
            return context;
        }
    }
    public class CustomRepository
    {
        public static List<sp_UserAccessPermissions_Result> UserAccessPermissions(int? roleId = null)
        {
            List<sp_UserAccessPermissions_Result> dataList;

            using (quotesEntities context = BaseContext.GetDbContext())
            {
                dataList = context.sp_UserAccessPermissions(roleId).ToList();
            }

            return dataList.OrderBy(m => m.MenuId).ToList();
        }
        public static bool CheckAdminUserExistsOrNot(string username, long id)
        {
            using (quotesEntities dbContext = BaseContext.GetDbContext())
            {
                bool result = dbContext.user.Any(m => m.username.ToLower() == username.ToLower() && m.id != id);
                return result;
            }
        }
        public static bool CheckAdminEmailExistsOrNot(string email, long id)
        {
            using (quotesEntities dbContext = BaseContext.GetDbContext())
            {
                bool result = dbContext.user.Any(m => m.email.ToLower() == email.ToLower() && m.id != id);
                return result;
            }
        }
        public static bool CheckHeadingContentName(string HeadingContent, int HeadingContentId)
        {
            using (quotesEntities dbContext = BaseContext.GetDbContext())
            {
                bool result = dbContext.tbl_HeadingContent.Any(m => m.HeadingContent == HeadingContent && m.HeadingContentId != HeadingContentId);
                return result;
            }
        }
        public static bool CheckRoleExistsOrNot(string RoleName, int RoleId)
        {
            using (quotesEntities dbContext = BaseContext.GetDbContext())
            {
                bool result = dbContext.tbl_Role.Any(m => m.RoleName == RoleName && m.RoleId != RoleId);
                return result;
            }
        }

        public static List<sp_AssignRoles_Result> GetUserRoleRights(int? roleId = null)
        {
            List<sp_AssignRoles_Result> dataList;

            using (quotesEntities context = BaseContext.GetDbContext())
            {
                dataList = context.sp_AssignRoles(roleId).ToList();
            }

            return dataList.OrderBy(m => m.MenuName).ToList();
        }
        public static SP_GetBaggageXmlData_Result GetBaggageXmlData(int? BaggageQuoteId, int CompanyId)
        {
            using (quotesEntities dbContext = BaseContext.GetDbContext())
            {
                SP_GetBaggageXmlData_Result list = dbContext.SP_GetBaggageXmlData(BaggageQuoteId, CompanyId).FirstOrDefault();
                return list;
            }
        }
        public static SP_GetCollectionDelivery_Result GetCollectionDeliveryData(long? QuoteId, int Company)
        {
            using (quotesEntities dbContext = BaseContext.GetDbContext())
            {
                SP_GetCollectionDelivery_Result data = dbContext.SP_GetCollectionDelivery(QuoteId, Company).FirstOrDefault();
                return data;
            }
        }
        public static sp_GetdataForEmailSending_Result GetQuoteData(int? Id, int? value, string shippingType)
        {
            using (quotesEntities dbContext = BaseContext.GetDbContext())
            {
                sp_GetdataForEmailSending_Result data = dbContext.sp_GetdataForEmailSending(Id, value, shippingType).FirstOrDefault();
                return data;
            }
        }
        public static List<SP_GetQuoteContents_Result> GetQuoteContents(string fromCountryCode, string countrycode, string quotetype)
        {
            using (quotesEntities dbContext = BaseContext.GetDbContext())
            {
                List<SP_GetQuoteContents_Result> list = dbContext.SP_GetQuoteContents(fromCountryCode, countrycode, quotetype,SessionHelper.CompanyId).ToList();
                return list;
            }
        }
        public static decimal GetVolumetricsWeight(int quoteId, string deliveryType)
        {
            using (quotesEntities dbContext = BaseContext.GetDbContext())
            {
                string sqlQuery = "SELECT [dbo].[FN_GetVolumetricsWeight] ('" + quoteId + "','" + deliveryType + "','" + SessionHelper.CompanyId + "')";
                //param[] parameters = { "EXV", email, countryCode, cityName };

                quotesEntities entityObj = new quotesEntities();
                decimal volumetricsWeight = entityObj.Database.SqlQuery<decimal>(sqlQuery).FirstOrDefault();
                return Math.Round(volumetricsWeight);
            }
        }
        public static List<sp_GetAllHeadingContent_Result> GetAllHeadingContent()
        {
            using (quotesEntities dbContext = BaseContext.GetDbContext())
            {
                List<sp_GetAllHeadingContent_Result> list = dbContext.sp_GetAllHeadingContent().ToList();
                return list;
            }
        }
        public static List<sp_GetAllChildContent_Result> GetAllChildContent()
        {
            using (quotesEntities dbContext = BaseContext.GetDbContext())
            {
                List<sp_GetAllChildContent_Result> list = dbContext.sp_GetAllChildContent().ToList();
                return list;
            }
        }
        public static SP_GetCollectionDelivery_Result GetCollectionDeliveryData(long? QuoteId)
        {
            using (quotesEntities dbContext = BaseContext.GetDbContext())
            {
                SP_GetCollectionDelivery_Result data = dbContext.SP_GetCollectionDelivery(QuoteId, SessionHelper.CompanyId).FirstOrDefault();
                return data;
            }
        }
        public static SP_GetRemovalXmlData_Result GetRemovalXmlData(long? QuoteId)
        {
            using (quotesEntities dbContext = BaseContext.GetDbContext())
            {
                SP_GetRemovalXmlData_Result data = dbContext.SP_GetRemovalXmlData(QuoteId).FirstOrDefault();
                return data;
            }
        }

        public static long GetNextCustomerRefNo(string moveType, string email, string countryCode, string cityName)
        {
            using (quotesEntities dbContext = BaseContext.GetDbContext())
            {
                string sqlQuery = "SELECT [dbo].[FN_GetNextCustomerRefNo] ('" + moveType + "', '" + email + "','" + countryCode + "','" + cityName + "')";
                //param[] parameters = { "EXV", email, countryCode, cityName };

                quotesEntities entityObj = new quotesEntities();
                long customerRefNo = entityObj.Database.SqlQuery<long>(sqlQuery).FirstOrDefault();
                return customerRefNo;
            }
        }
    }
}