using Dapper;
using QuoteCalculator.Service.Common;
using QuoteCalculator.Service.Models;
using QuoteCalculator.Service.Repository.ChildContentMaster;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuoteCalculator.Service.Repository.TradeQuote
{
    public class TradeQuoteRepository : BaseRepository, ITradeQuoteRepository
    {
        public TradeQuoteRepository()
        {

        }
        public List<TradeQuoteModel> GetTradeQuoteList(DataTablePaginationModel model,int Company)
        {

            try
            {
                var CompanyId = new SqlParameter
                {
                    ParameterName = "CompanyId",
                    DbType = DbType.Int64,
                    Value = Company
                };
                List<TradeQuoteModel> datalist;
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@CompanyId", CompanyId.Value);
                parameters.Add("@PageNumber", model.DtPageNumber);
                parameters.Add("@PageSize", model.DtPageSize);
                parameters.Add("@StrSearch", model.DtSearch != null ? model.DtSearch.Trim() : "");
                parameters.Add("@SortColumn", model.DtSortColumn);
                parameters.Add("@SortOrder", model.DtSortOrder);
                datalist = GetAll<TradeQuoteModel>("SP_GetTradeQuotesList_p", parameters);
                return datalist;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<AddtionalCostModel> GetAdditionalCost(long? TradeQuoteId)
        {
            try
            {
                DynamicParameters parameter = new DynamicParameters();
                parameter.Add("@TradeQuoteId", TradeQuoteId);
                var List = GetAll<AddtionalCostModel>("SP_GetAdditionalCostTypes", parameter);
                return List;
            }
            catch (Exception e)
            {
                throw;
            }
        }
        public List<AddtionalCostModel> GetTradeCostData(long? TradeQuoteId)
        {
            try
            {
                DynamicParameters parameter = new DynamicParameters();
                parameter.Add("@TradeQuoteId", TradeQuoteId);
                var List = GetAll<AddtionalCostModel>("SP_GetTradeCostData", parameter);
                return List;
            }
            catch (Exception e)
            {
                throw;
            }
        }
        public void TradesCalculateRate(int? paramTradeQuoteId)
        {
            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@TradeQuoteId", paramTradeQuoteId);
           ExecuteStoredProcedure("SP_TradesCalculateRate", parameter);
        }

        public TradeQuoteModel GetTradeQuoteById(int? Id)
        {
            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@Id", Id);
            var List = Get<TradeQuoteModel>("SP_GetTradeQuotesById", parameter);
            return List;
        }
        public TradeQuoteModel GetTradeQuoteXMLData(long? TradeQuoteId)
        {
            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@TradeQuoteId", TradeQuoteId);
            var List = Get<TradeQuoteModel>("SP_GetTradeQuoteXMLData", parameter);
            return List;
        }
        public int UpdateTradeQuote(TradeQuoteModel model)
        {
           
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", model.Id);
            parameters.Add("@AgentId", model.AgentId);
            parameters.Add("@ClientName", model.ClientName);
            parameters.Add("@DestAddress1", model.DestAddress1);
            parameters.Add("@DestAddress2", model.DestAddress2);
            parameters.Add("@DestCountry", model.DestCountryId);
            parameters.Add("@DestCode", model.DestCode);
            parameters.Add("@DestPort", model.DestPort);
            parameters.Add("@Volume", model.Volume);
            parameters.Add("@ServiceId", model.ServiceId);
            parameters.Add("@SalesRep", model.SalesRep);
            parameters.Add("@Company", model.Company);
            parameters.Add("@Branch", model.Branch);
            parameters.Add("@CreatedBy", model.UserId);
            int result = Get<int>("SP_InsertUpdateTradeQuotes", parameters);
            return result;
        }
        public int InsertAdditionalCost(TradeQuoteAddCost model)
        {
            try
            {
                DynamicParameters parameters = new DynamicParameters();
                DataTable dtCost = new DataTable("AdditionalCost");
                dtCost.Columns.Add("Type");
                dtCost.Columns.Add("Cost");

                foreach (var item in model.AddCostList)
                {
                    DataRow dtrow = dtCost.NewRow();
                    dtrow["Type"] = item.Type;
                    dtrow["Cost"] = item.Cost;
                    dtCost.Rows.Add(dtrow);
                }
                parameters.Add("@CompanyId", model.CompanyId);
                parameters.Add("@UserId", model.UserId);
                parameters.Add("@TradeQuoteId", model.TradeQuoteId);
                parameters.Add("@ImportQuoteId", null);
                parameters.Add("@TotalPrice", model.TotalPrice);
                parameters.Add("@AdjustedPrice", model.AdjustedPrice);
                parameters.Add("@Profit", model.Profit);
                parameters.Add("@AdjustedProfit", model.AdjustedProfit);
                parameters.Add("@AdditionalCost", dtCost.AsTableValuedParameter("[dbo].[AdditionalCost]"));
                int result = Get<int>("SP_InsertAdditionalCost", parameters);
                return result;
            }
            catch(Exception e)
            {
                throw;
            }
        }

        public List<TradeQuoteModel> GetDestCodeByCountryId(string CountryCode)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@CountryCode", CountryCode);
            var result = GetAll<TradeQuoteModel>("SP_GetTradeDestCodeByCountry", parameters);
            return result;
        }
    }
}
