using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static QuoteCalculator.Service.Models.HomeControllerModel;

namespace QuoteCalculator.Service.Repository.HomeRepository
{
    public interface IHomeRepository
    {
        ExcelSheetName GetExcelSheetNameById(int id);
        int DeleteAllVehicleShippingRates();
        List<VehicleShippingRates> GetVehicleShippingRates();
        int DeleteAllData(string tableName, int? companyId, string companyColumnName);
        rates_destinations RatesDestByCountryCode(string CountryCode, string City, int CompanyId);
        int AddVehicleShippingRates(VehicleShippingRates vehicleShippingRates);
        int AddRatesDestinations(rates_destinations ratesDestination);
        int Add_bag_imports_uk(bag_imports_uk bag_Imports);
        int Add_bagC2C(bag_c2c bagC2C);
        int Add_matrixC2C(matrix_c2c matrixC2C);
        int Add_roadRates(road_rates roadRates);
    }
}
