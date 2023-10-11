using QuoteCalculator.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace QuoteCalculator.Service.Repository.VehicleRepository
{
    public interface IVehicleRepository
    {
        List<VehicleModel> GetVehicleList(int CompanyId, DataTablePaginationModel model);

        EditVehicleModel GetVehicleById(int id);

        int DeleteVehicle(int? Id);
    }
}
