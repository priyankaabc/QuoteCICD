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

namespace QuoteCalculator.Service.Repository.ChildContentMaster
{
    public class ChildContentMasterRepository : BaseRepository, IChildContentMasterRepository
    {
        public ChildContentMasterRepository()
        {

        }
        public List<ContentMasterModel> GetAllContents(DataTablePaginationModel model)
        {

            try
            {
                
                List<ContentMasterModel> datalist;
                DynamicParameters parameter = new DynamicParameters();
                parameter.Add("@PageNumber", model.DtPageNumber);
                parameter.Add("@PageSize", model.DtPageSize);
                parameter.Add("@StrSearch", model.DtSearch != null ? model.DtSearch.Trim() : "");
                parameter.Add("@SortColumn", model.DtSortColumn);
                parameter.Add("@SortOrder", model.DtSortOrder);
                datalist = GetAll<ContentMasterModel>("sp_GetAllChildContent", parameter);
                return datalist;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ContentMasterModel GetChildContentById(int? Id)
        {
            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@prmcontentId", Id);
            var contentList = Get<ContentMasterModel>("sp_GetChildContentById", parameter);
            return contentList;
        }

        public int AddContent(ContentMasterModel contentModel)
        {
            return addupdateContent(contentModel);
        }
      
        public int UpdateContent(ContentMasterModel contentModel)
        {
            return addupdateContent(contentModel);
        }
        public int addupdateContent(ContentMasterModel contentModel)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", contentModel.ChildContentId);
            parameters.Add("@HeadingContentId", contentModel.HeadingContentId);
            parameters.Add("@ChildContent", contentModel.ChildContent);
            parameters.Add("@CountryCode", contentModel.CountryCode);
            parameters.Add("@DisplayOrder", contentModel.DisplayOrder);
            int result = ExecuteStoredProcedure("sp_tbl_ChildContent_addEdit", parameters);
            return result;
        }
        public int DeleteContent(int? id)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@ChildContentId", id);
            int result = ExecuteStoredProcedure("sp_tbl_ChildContent_Delete", parameters);
            return result;
        }

    
      
        public int addupdateUser(ContentMasterModel contentModel)
        {
            DynamicParameters parameters = new DynamicParameters();
            
            int result = ExecuteStoredProcedure("", parameters);
            return result;
        }
    }
}
