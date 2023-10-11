using Dapper;
using QuoteCalculator.Service.Common;
using QuoteCalculator.Service.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace QuoteCalculator.Service.Repository.EmailRepository
{
    public class EmailTemplateRepository : BaseRepository, IEmailTemplateRepository
    {
        public EmailTemplateRepository()
        {

        }

        public int AddTemplate(EmailTemplateModel templateModel)
        {
            return addupdateTemplate(templateModel);
        }

        public int DeleteTemplate(int? templateId)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@prmId", templateId);
            int result = ExecuteStoredProcedure("sp_tbl_EmailTemplate_Delete", parameters);
            return result;
        }

        public List<EmailTemplateModel> GetAllTemplates(DataTablePaginationModel model)
        {
            try
            {
               
                List<EmailTemplateModel> datalist;
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@PageNumber", model.DtPageNumber);
                parameters.Add("@PageSize", model.DtPageSize);
                parameters.Add("@StrSearch", model.DtSearch != null ? model.DtSearch.Trim() : "");
                parameters.Add("@SortColumn", model.DtSortColumn);
                parameters.Add("@SortOrder", model.DtSortOrder);
                datalist = GetAll<EmailTemplateModel>("sp_GetEmailTemplateList", parameters);
                return datalist;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public EmailTemplateModel GetTemplateById(int? templateId)
        {
            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@prmId", templateId);
            var template = Get<EmailTemplateModel>("sp_GetEmailTemplateById", parameter);
            return template;
        }

        public int UpdateTemplate(EmailTemplateModel templateModel)
        {
            return addupdateTemplate(templateModel);
        }

        public int addupdateTemplate(EmailTemplateModel templateModel)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", templateModel.id);
            parameters.Add("@ServiceId", templateModel.ServiceId);
            parameters.Add("@Subject", templateModel.Subject);
            parameters.Add("@HtmlContent", templateModel.HtmlContent);
            parameters.Add("@loggedUserId", templateModel.UserId);
            int result = ExecuteStoredProcedure("sp_tbl_EmailTemplate_addEdit", parameters);
            return result;
        }
    }
}
