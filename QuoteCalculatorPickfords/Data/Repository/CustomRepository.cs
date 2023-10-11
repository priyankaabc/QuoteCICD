using QuoteCalculatorPickfords.Common;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;

namespace QuoteCalculatorPickfords.Data.Repository
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
        public static List<GetCity_Result> GetCascadingCityList(string CountryId)
        {
            List<GetCity_Result> dataList;

            using (quotesEntities context = BaseContext.GetDbContext())
            {
                dataList = context.GetCity(CountryId).ToList();
            }

            return dataList.Distinct().ToList();
        }
        public static List<GetVehicleModelList_Result> GetVehicleModelList(string makeId)
        {
            List<GetVehicleModelList_Result> dataList;

            using (quotesEntities context = BaseContext.GetDbContext())
            {
                dataList = context.GetVehicleModelList(makeId).ToList();
            }

            return dataList.ToList();
        }
        public static GetInfoForQuote_Result GetInfoForQuote(int id)
        {
            //            List<GetInfoForQuote_Result> dataList;

            using (quotesEntities context = BaseContext.GetDbContext())
            {
                return context.GetInfoForQuote(id).FirstOrDefault();
            }

            //  return dataList;
        }

        //public static List<GetCity_Result1> GetFCLandGPGData()
        //{
        //    List<GetCity_Result1> dataList;

        //    using (quotesEntities context = BaseContext.GetDbContext())
        //    {
        //        dataList = context.GetCity(CountryId).ToList();
        //    }

        //    return dataList.Distinct().ToList();
        //}

        public static SP_GetXmlData_Result GetXmlData(int? VehicleId)
        {
            using (quotesEntities dbContext = BaseContext.GetDbContext())
            {
                SP_GetXmlData_Result list = dbContext.SP_GetXmlData(VehicleId).FirstOrDefault();
                return list;
            }
        }

        public static SP_GetBaggageXmlData_Result GetBaggageXmlData(int? BaggageQuoteId)
        {
            using (quotesEntities dbContext = BaseContext.GetDbContext())
            {
                SP_GetBaggageXmlData_Result list = dbContext.SP_GetBaggageXmlData(BaggageQuoteId, SessionHelper.COMPANY_ID).FirstOrDefault();
                return list;
            }
        }

        public static List<SP_GetQuoteContents_Result> GetQuoteContents(string countrycode, string quotetype)
        {
            using (quotesEntities dbContext = BaseContext.GetDbContext())
            {
                List<SP_GetQuoteContents_Result> list = dbContext.SP_GetQuoteContents(countrycode, quotetype).ToList();
                return list;
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
        public static SP_GetCollectionDelivery_Result GetCollectionDeliveryData(long? QuoteId)
        {
            using (quotesEntities dbContext = BaseContext.GetDbContext())
            {
                SP_GetCollectionDelivery_Result data = dbContext.SP_GetCollectionDelivery(QuoteId, SessionHelper.COMPANY_ID).FirstOrDefault();
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
                string sqlQuery = "SELECT [dbo].[FN_GetNextCustomerRefNo] ('EXV', '"+email+"','" + countryCode + "','" + cityName +"')";
                //param[] parameters = { "EXV", email, countryCode, cityName };

                quotesEntities entityObj = new quotesEntities();
                long customerRefNo = entityObj.Database.SqlQuery<long>(sqlQuery).FirstOrDefault();
                return customerRefNo;
            }
        }

        public static decimal GetVolumetricsWeight(int quoteId, string deliveryType)
        {
            using (quotesEntities dbContext = BaseContext.GetDbContext())
            {
                string sqlQuery = "SELECT [dbo].[FN_GetVolumetricsWeight] ('" + quoteId + "','" + deliveryType + "','" + SessionHelper.COMPANY_ID + "')";
                //param[] parameters = { "EXV", email, countryCode, cityName };

                quotesEntities entityObj = new quotesEntities();
                decimal volumetricsWeight = entityObj.Database.SqlQuery<decimal>(sqlQuery).FirstOrDefault();
                return Math.Round(volumetricsWeight);
            }
        }
    }
}