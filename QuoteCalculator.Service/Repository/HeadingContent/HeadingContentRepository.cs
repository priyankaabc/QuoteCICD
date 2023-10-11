using Dapper;
using QuoteCalculator.Service.Common;
using QuoteCalculator.Service.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuoteCalculator.Service.Repository.HeadingContent
{
    public class HeadingContentRepository : BaseRepository, IHeadingContentRepository
    {
        public HeadingContentRepository()
        {

        }

        public int AddContent(HeadingContentModel contentModel)
        {
            return addupdateContent(contentModel);
        }
      
        public int UpdateContent(HeadingContentModel contentModel)
        {
            return addupdateContent(contentModel);
        }
        public int addupdateContent(HeadingContentModel contentModel)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", contentModel.HeadingContentId);
            parameters.Add("@Heading", contentModel.Heading);
            parameters.Add("@HeadingContent", contentModel.HeadingContent);
            parameters.Add("@FromCountryCode", contentModel.FromCountryCode);
            parameters.Add("@CountryCode", contentModel.CountryCode);
            parameters.Add("@QuoteType", contentModel.QuoteType);
            parameters.Add("@Company", contentModel.Company);
            parameters.Add("@DisplayOrder", contentModel.DisplayOrder);
            int result = ExecuteStoredProcedure("sp_tbl_HeaderContent_addEdit", parameters);
            return result;
        }
        public int DeleteContent(int? id)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@id", id);
            int result = ExecuteStoredProcedure("sp_tbl_HeaderContent_Delete", parameters);
            return result;
        }

        public List<HeadingContentModel> GetAllContents(DataTablePaginationModel model)
        {

            try
            {
                
                List<HeadingContentModel> datalist;
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@PageNumber", model.DtPageNumber);
                parameters.Add("@PageSize", model.DtPageSize);
                parameters.Add("@StrSearch", model.DtSearch != null ? model.DtSearch.Trim() : "");
                parameters.Add("@SortColumn", model.DtSortColumn);
                parameters.Add("@SortOrder", model.DtSortOrder);
                datalist = GetAll<HeadingContentModel>("sp_GetAllHeadingContent", parameters);
                return datalist;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public HeadingContentModel GetContentById(int? Id)
        {
            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@prmcotentId", Id);
            var contentList = Get<HeadingContentModel>("sp_GetHeadingContentById", parameter);
            return contentList;
        }

      
        public int addupdateUser(HeadingContentModel contentModel)
        {
            DynamicParameters parameters = new DynamicParameters();
            
            int result = ExecuteStoredProcedure("", parameters);
            return result;
        }
    }
}
