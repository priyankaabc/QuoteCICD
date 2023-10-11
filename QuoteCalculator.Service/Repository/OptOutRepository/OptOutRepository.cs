using Dapper;
using QuoteCalculator.Service.Common;
using QuoteCalculator.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuoteCalculator.Service.Repository.OptOutRepository
{
    public class OptOutRepository : BaseRepository, IOptOutRepository
    {
        public List<OptOutModel> GetOptOutList(DataTablePaginationModel model)
        {
            try
            {
                List<OptOutModel> OptOutList = new List<OptOutModel>();
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@PageNumber", model.DtPageNumber);
                parameters.Add("@PageSize", model.DtPageSize);
                parameters.Add("@StrSearch", model.DtSearch != null ? model.DtSearch.Trim() : "");
                parameters.Add("@SortColumn", model.DtSortColumn);
                parameters.Add("@SortOrder", model.DtSortOrder);
                OptOutList = GetAll<OptOutModel>("SP_OptOutResult", parameters);
                return OptOutList;
            }
            catch (Exception ex)
            {
                throw ex;
            }            
        }

        public long ChangeStatus(string Email, int LoggedInUserId)
        {
            try
            {
                DynamicParameters parameter = new DynamicParameters();
                parameter.Add("@email", Email);
                parameter.Add("@userId", LoggedInUserId);
                long Result = ExecuteStoredProcedure("SP_SetOptIn", parameter);
                return Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
