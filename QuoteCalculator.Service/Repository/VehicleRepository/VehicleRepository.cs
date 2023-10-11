using Dapper;
using QuoteCalculator.Service.Common;
using QuoteCalculator.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuoteCalculator.Service.Repository.VehicleRepository
{
    public class VehicleRepository : BaseRepository, IVehicleRepository
    {
        public EditVehicleModel GetVehicleById(int id)
        {
            EditVehicleModel em = new EditVehicleModel();
            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@Id", id);

            em = Get<EditVehicleModel>("SP_GetVehicleById", parameter);

            return em;
        }

        public List<VehicleModel> GetVehicleList(int CompanyId, DataTablePaginationModel model)
        {

            try
            {
                List<VehicleModel> vehicleList = new List<VehicleModel>();
                DynamicParameters parameter = new DynamicParameters();
                parameter.Add("@Company", CompanyId);
                parameter.Add("@PageNumber", model.DtPageNumber);
                parameter.Add("@PageSize", model.DtPageSize);
                parameter.Add("@StrSearch", model.DtSearch != null ? model.DtSearch.Trim() : "");
                parameter.Add("@SortColumn", model.DtSortColumn);
                parameter.Add("@SortOrder", model.DtSortOrder);

                vehicleList = GetAll<VehicleModel>("sp_GetVehicleAllListContent", parameter);

                return vehicleList;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public int DeleteVehicle(int? Id)
        {
            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@Id", Id);

            int result = ExecuteStoredProcedure("sp_DeleteVehicleById", parameter);
            return result;
        }
    }
}
