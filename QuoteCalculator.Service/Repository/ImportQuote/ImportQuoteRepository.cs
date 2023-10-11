using Dapper;
using QuoteCalculator.Service.Common;
using QuoteCalculator.Service.Models;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuoteCalculator.Service.Repository.ImportQuote
{
    public class ImportQuoteRepository : BaseRepository, IImportQuoteRepository
    {
        public List<ImportsQuoteModel> GetImportsQuoteList(DataTablePaginationModel model, int CompanyId)
        {
            try
            {
                List<ImportsQuoteModel> RemovalList = new List<ImportsQuoteModel>();
                DynamicParameters parameter = new DynamicParameters();
                parameter.Add("@CompanyId", CompanyId);
                parameter.Add("@PageNumber", model.DtPageNumber);
                parameter.Add("@PageSize", model.DtPageSize);
                parameter.Add("@StrSearch", model.DtSearch != null ? model.DtSearch.Trim() : "");
                parameter.Add("@SortColumn", model.DtSortColumn);
                parameter.Add("@SortOrder", model.DtSortOrder);
                RemovalList = GetAll<ImportsQuoteModel>("SP_GetImportsQuoteList_P", parameter);
                return RemovalList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ImportsQuoteModel GetImportsQuoteById(int? Id)
        {
            try
            {
                DynamicParameters parameter = new DynamicParameters();
                parameter.Add("@Id", Id);
                ImportsQuoteModel model = Get<ImportsQuoteModel>("SP_GetImportsQuotesById", parameter);
                return model;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<AddtionalCostModel> GetAdditionalCost()
        {
            try
            {
                var List = GetAll<AddtionalCostModel>("SP_GetAdditionalCostTypes",null);
                return List;
            }
            catch (Exception e)
            {
                throw;
            }
        }
        public int DeleteInternationalRemoval(int? Id)
        {
            try
            {
                DynamicParameters parameter = new DynamicParameters();
                parameter.Add("@prmId", Id);
                int result = ExecuteStoredProcedure("sp_tbl_InternationalRemoval_Delete", parameter);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }            
        }

        public long UpdateImportQuote(ImportsQuoteModel model)
        {
            return addupdateImportQuote(model);
        }

        public long addupdateImportQuote(ImportsQuoteModel model)
        {
            DynamicParameters parameters = new DynamicParameters();
         
            parameters.Add("@Id", model.Id);
            parameters.Add("@AgentId", model.AgentId);
            parameters.Add("@CustomerName", model.CustomerName);
            parameters.Add("@OriginCountry", model.OriginCountry);
            parameters.Add("@OriginTown", model.OriginTown);
            //   parameters.Add("@IsCollectFromBranch", model.IsCollectFromBranch);
            parameters.Add("@POEId", model.POEId);
            parameters.Add("@ServiceId", model.ServiceId);
            parameters.Add("@ContainerSizeId", model.ContainerSizeId);
            parameters.Add("@Company", model.Company);
            parameters.Add("@Branch", model.Branch);
            parameters.Add("@SalesRep", model.SalesRep);
            parameters.Add("@CreatedBy", model.UserId);
            parameters.Add("@Operation", model.Operation);
            parameters.Add("@ShowDestDetail", model.ShowDestDetail);
            parameters.Add("@Note", model.Note);
            long result = Get<long>("SP_InsertUpdateImportsQuotes", parameters);
            return result;
        }
        public void ImportCalculateRate(long QuoteId)
        {
            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@ImportQuoteId", QuoteId);
            ExecuteStoredProcedure("SP_ImportCalculateRate", parameter);
        }

        public CommonModel GetBranchById(long POEId)
        {
            try
            {
                DynamicParameters parameter = new DynamicParameters();
                parameter.Add("@POEId", POEId);
                CommonModel model = Get<CommonModel>("SP_GetBranchById", parameter);
                return model;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public int InsertUpdateCost(long ImportQuoteId,double TotalCost, double UpdatedCost )
        {
            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@TotalCost", TotalCost);
            parameters.Add("@UpdatedCost", UpdatedCost);
            parameters.Add("@ImportQuoteId", ImportQuoteId);
         
            int result = Get<int>("SP_InsertUpdateCost", parameters);
            return result;
        }
        public ImportsQuoteModel GetImportQuoteXMLData(long? ImportQuoteId)
        {
            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@ImportQuoteId", ImportQuoteId);
            var List = Get<ImportsQuoteModel>("SP_GetImportQuoteXMLData", parameter);
            return List;
        }
    

        public List<DestinationDetails> GetImportConsigneeData(long? ImportQuoteId)
        {
            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@ImportQuoteId", ImportQuoteId);
            var List = GetAll<DestinationDetails>("SP_GetImportConsigneeData", parameter);
            return List;
        }
        public List<AddtionalCostModel> GetImportCostData(long? ImportQuoteId)
        {
            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@ImportQuoteId", ImportQuoteId);
            var List = GetAll<AddtionalCostModel>("SP_GetImportCostData", parameter);
            return List;
        }
       public List<DestinationDetails> GetImportsDestinationById(long? Id)
        {
            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@Id", Id);
            var List = GetAll<DestinationDetails>("SP_GetImportsDestinationById", parameter);
            return List;
        }
       public int DeleteDestinationDetails(int? CompanyId, int? DestinationId)
        {
            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@CompanyId", CompanyId);
            parameters.Add("@DestinationId", DestinationId);

            int result = Get<int>("SP_DeleteDestinationDetails", parameters);
            return result;
        }
        public List<AddtionalCostModel> GetAdditionalCostByDestId(long DestId)
        {
            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@DestId", DestId);
            var List = GetAll<AddtionalCostModel>("SP_GetAdditionalCostByDestId", parameter);
            return List;
        }
       public long InsertUpdateImportsDestination(DestinationDetails model,long? ImportQuoteId)
        {

            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@Id", model.Id);
            parameters.Add("@DestAddress1", model.DestAddress1);
            parameters.Add("@DestAddress2", model.DestAddress2);
            parameters.Add("@DestPostcode", model.DestPostcode);
            parameters.Add("@ConsigneeName", model.ConsigneeName);
            parameters.Add("@Kgs", model.Kgs);
            parameters.Add("@VehicleId", model.VehicleId);
            parameters.Add("@CollectFromBranch", model.IsCollectFromBranch);
            //   parameters.Add("@IsCollectFromBranch", model.IsCollectFromBranch);
            parameters.Add("@ImportQuoteId", ImportQuoteId);
            long result = Get<long>("SP_InsertUpdateImportsDestination", parameters);
            return result;
        }
        public int InsertAdditionalCost(ImportsQuoteAddCost model)
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
                parameters.Add("@DestinationId", model.DestinationId);
                parameters.Add("@AdditionalCost", dtCost.AsTableValuedParameter("[dbo].[AdditionalCost]"));
                int result = Get<int>("SP_InsertAdditionalCostForDestination", parameters);
                return result;
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}