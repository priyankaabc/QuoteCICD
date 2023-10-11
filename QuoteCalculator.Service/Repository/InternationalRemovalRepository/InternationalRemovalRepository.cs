using Dapper;
using QuoteCalculator.Service.Common;
using QuoteCalculator.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuoteCalculator.Service.Repository.InternationalRemovalRepository
{
    public class InternationalRemovalRepository : BaseRepository, IInternationalRemovalRepository
    {
        public List<InternationalRemovalModel> GetInternationRemovalList(DataTablePaginationModel model, int CompanyId)
        {
            try
            {
                List<InternationalRemovalModel> RemovalList = new List<InternationalRemovalModel>();
                DynamicParameters parameter = new DynamicParameters();
                parameter.Add("@CompanyId", CompanyId);
                parameter.Add("@PageNumber", model.DtPageNumber);
                parameter.Add("@PageSize", model.DtPageSize);
                parameter.Add("@StrSearch", model.DtSearch != null ? model.DtSearch.Trim() : "");
                parameter.Add("@SortColumn", model.DtSortColumn);
                parameter.Add("@SortOrder", model.DtSortOrder);
                RemovalList = GetAll<InternationalRemovalModel>("SP_GetInternationalRemovalList", parameter);
                return RemovalList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public InternationalRemovalModel GetInternationalRemovalById(int Id)
        {
            try
            {
                DynamicParameters parameter = new DynamicParameters();
                parameter.Add("@Id", Id);
                InternationalRemovalModel InternationalRemoval = Get<InternationalRemovalModel>("SP_GetInternationalRemovalById", parameter);
                return InternationalRemoval;
            }
            catch (Exception ex)
            {
                throw ex;
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

        public int UpdateInternationalRemoval(InternationalRemovalModel InternationalRemovalModel)
        {
            return addupdateInternationalRemoval(InternationalRemovalModel);
        }

        public int addupdateInternationalRemoval(InternationalRemovalModel model)
        {
            //DynamicParameters parameters = new DynamicParameters();
            //parameters.Add("@Id", model.Id);
            //parameters.Add("@Firstname", model.Firstname);
            //parameters.Add("@Lastname", model.Lastname);
            //parameters.Add("@Email", model.Email);
            //parameters.Add("@CountryCode", model.Country_Code);
            //parameters.Add("@Telephone", model.Telephone);
            //parameters.Add("@TitleId", model.TitleId);
            //parameters.Add("@FromCountryName", model.FromCountryName);
            //parameters.Add("@ToCountryCode", model.ToCountryCode);
            //parameters.Add("@PostCode", model.PostCode);
            //parameters.Add("@CityName", model.CityName);
            //parameters.Add("@EstimatedMoveDate", model.EstimatedMoveDate);
            //parameters.Add("@IsConditionApply", model.IsConditionApply);
            //parameters.Add("@HomeConsultationOrService", model.HomeConsultationOrService);
            //parameters.Add("@HomeVideoSurvery", model.HomeVideoSurvery);
            //parameters.Add("@QuickOnlineQuote", model.QuickOnlineQuote);
            //parameters.Add("@HomeConsultationDateTime", model.HomeConsultationDateTime);
            //parameters.Add("@BranchId", model.BranchId);
            //parameters.Add("@Sr_Code", model.Sr_Code);
            //parameters.Add("@Sr_Name", model.Sr_Name);
            //parameters.Add("@VideoSurveyId", model.VideoSurveyId);
            //parameters.Add("@VideoSurveyAppointmentTime", model.VideoSurveyAppointmentTime);
            //parameters.Add("@CreatedDate", model.CreatedDate);
            //parameters.Add("@Company", model.Company);
            //parameters.Add("@IsDelete", model.IsDelete);
            //parameters.Add("@Distance", model.Distance);
            //parameters.Add("@dayScheduleId", model.dayScheduleId);
            //parameters.Add("@SalesRepCode", model.SalesRepCode);
            //int result = Get<int>("sp_InternationalRemoval_addEdit", parameters);
            //return result;

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", model.Id);
            parameters.Add("@Firstname", model.Firstname);
            parameters.Add("@Lastname", model.Lastname);
            parameters.Add("@Email", model.Email);
            parameters.Add("@CountryCode", model.CountryCode);
            parameters.Add("@Telephone", model.Telephone);
            parameters.Add("@TitleId", model.TitleId);
            parameters.Add("@ToCountryCode", model.ToCountryCode);
            parameters.Add("@PostCode", model.PostCode);
            parameters.Add("@CityName", model.CityName);
            parameters.Add("@EstimatedMoveDate", model.EstimatedMoveDate);
            parameters.Add("@HomeConsultationDateTime", model.HomeConsultationDateTime);
            parameters.Add("@dayScheduleId", model.dayScheduleId);
            parameters.Add("@VideoSurveyAppointmentTime", model.VideoSurveyAppointmentTime);
            parameters.Add("@HomeConsultationOrService", model.HomeConsultationOrService);
            parameters.Add("@HomeVideoSurvery", model.HomeVideoSurvery);
            parameters.Add("@QuickOnlineQuote", model.QuickOnlineQuote);
            parameters.Add("@SalesRepCode", model.SalesRepCode);
            parameters.Add("@FromCountryName", model.FromCountryName);
            parameters.Add("@Company", model.Company);
            parameters.Add("@BranchId", model.BranchId);
            parameters.Add("@IsInquiry", model.IsInquiry);

            int result = Get<int>("SP_InsertUpdateRemovalQuotes", parameters);
            return result;
        }
        public IEnumerable<InternationalRemovalModel> GetAllInternationalRemoval()
        {
            IEnumerable<InternationalRemovalModel> InternationalRemovalList;
            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@prmId", 0);
            parameter.Add("@prmCompany", 0);
            InternationalRemovalList = GetAll<InternationalRemovalModel>("sp_GetInternationalRemovalAllListContent", parameter);
            return InternationalRemovalList;
        }

        public int GetInternationalRemovalCalculation(int? QuickQuoteItemId)
        {
            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@QuoteId", QuickQuoteItemId);

            int InternationalRemovalCalculation = ExecuteStoredProcedure("SP_RemovalCalculation", parameter);
            return InternationalRemovalCalculation;
        }

        public RemovalQuoteCalculationModel GetRemovalQuoteCalculationById(int? QuoteId)
        {
            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@Id", QuoteId);

            var RemovalQuoteCalculationModel = Get<RemovalQuoteCalculationModel>("SP_GetRemovalQuoteCalculation", parameter);
            return RemovalQuoteCalculationModel;
        }
    }
}