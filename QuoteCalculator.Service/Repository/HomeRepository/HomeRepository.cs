using Dapper;
using QuoteCalculator.Service.Common;
using QuoteCalculator.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static QuoteCalculator.Service.Models.HomeControllerModel;

namespace QuoteCalculator.Service.Repository.HomeRepository
{
    public class HomeRepository : BaseRepository, IHomeRepository
    {
        public int Add_bag_imports_uk(bag_imports_uk bag_Imports)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@kg_from", bag_Imports.kg_from);
            parameters.Add("@zone1", bag_Imports.zone1);
            parameters.Add("@zone2",bag_Imports.zone2);
            parameters.Add("@zone3", bag_Imports.zone3);
            parameters.Add("@zone4", bag_Imports.zone4);
            parameters.Add("@zone5", bag_Imports.zone5);
            parameters.Add("@zone6", bag_Imports.zone6);
            parameters.Add("@zone7", bag_Imports.zone7);
            parameters.Add("@zone8", bag_Imports.zone8);
            parameters.Add("@zone9", bag_Imports.zone9);
            parameters.Add("@zone10", bag_Imports.zone10);
            parameters.Add("@company",bag_Imports.company);

            int result = ExecuteStoredProcedure("SP_InsertBagImportsUK", parameters);

            return result;
        }

        public int AddRatesDestinations(rates_destinations ratesDestination)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@country", ratesDestination.country);
            parameters.Add("@city", ratesDestination.city);
            parameters.Add("@dest_code", ratesDestination.dest_code);
            parameters.Add("@port_code", ratesDestination.port_code);
            parameters.Add("@radius", ratesDestination.radius);
            parameters.Add("@country_code", ratesDestination.country_code);
            parameters.Add("@world_zone", ratesDestination.world_zone);
            parameters.Add("@air", ratesDestination.air);
            parameters.Add("@road", ratesDestination.road);
            parameters.Add("@sea_rates_id", ratesDestination.sea_rates_id);
            parameters.Add("@courier_zone", ratesDestination.courier_zone);
            parameters.Add("@courier_vol_weight", ratesDestination.courier_vol_weight);
            parameters.Add("@courier_express_vol_weight", ratesDestination.courier_express_vol_weight);
            parameters.Add("@dcr_id", ratesDestination.dcr_id);
            parameters.Add("@car_port", ratesDestination.car_port);
            parameters.Add("@bag_orig", ratesDestination.bag_orig);
            parameters.Add("@bag_dest", ratesDestination.bag_dest);
            parameters.Add("@rem_orig", ratesDestination.rem_orig);
            parameters.Add("@rem_dest", ratesDestination.rem_dest);
            parameters.Add("@veh_orig", ratesDestination.veh_orig);
            parameters.Add("@veh_dest", ratesDestination.veh_dest);
            parameters.Add("@fa_orig", ratesDestination.fa_orig);
            parameters.Add("@fa_dest", ratesDestination.fa_dest);
            parameters.Add("@company", ratesDestination.company);
            parameters.Add("@display_order", ratesDestination.display_order);
            parameters.Add("@bag_c2c", ratesDestination.bag_c2c);
            parameters.Add("@bag_imp", ratesDestination.bag_imp);
            parameters.Add("@CompanyId", ratesDestination.CompanyId);

            int result = ExecuteStoredProcedure("SP_InsertRatesDestinations", parameters);
            return result;
        }

        public int AddVehicleShippingRates(VehicleShippingRates vehicleShippingRates)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@CountryCode", vehicleShippingRates.CountryCode);
            parameters.Add("@City", vehicleShippingRates.City);
            parameters.Add("@Code", vehicleShippingRates.Code);
            parameters.Add("@FCL", vehicleShippingRates.FCL);
            parameters.Add("@GPG", vehicleShippingRates.GPG);
            parameters.Add("@FCL_LON", vehicleShippingRates.FCL_LON);
            parameters.Add("@FCL_MCR", vehicleShippingRates.FCL_MCR);
            parameters.Add("@FCL_GLA", vehicleShippingRates.FCL_GLA);
            parameters.Add("@Groupage_Lon_PerMt", vehicleShippingRates.Groupage_Lon_PerMt);
            parameters.Add("@Groupage_Lon_L_S", vehicleShippingRates.Groupage_Lon_L_S);
            parameters.Add("@Groupage_MCR_PerMt", vehicleShippingRates.Groupage_MCR_PerMt);
            parameters.Add("@Groupage_MCR_L_S", vehicleShippingRates.Groupage_MCR_L_S);
            parameters.Add("@Groupage_GLA_PerMt", vehicleShippingRates.Groupage_GLA_PerMt);
            parameters.Add("@Groupage_GLA_L_S", vehicleShippingRates.Groupage_GLA_L_S);
            parameters.Add("@TransitTimes_FCL", vehicleShippingRates.TransitTimes_FCL);
            parameters.Add("@TransitTimes_GRP", vehicleShippingRates.TransitTimes_GRP);

            int result = ExecuteStoredProcedure("SP_InsertVehicleShippingRates", parameters);

            return result;
        }

        public int DeleteAllData(string tableName, int? companyId, string companyColumnName)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@tblName", tableName);
            parameters.Add("@CompanyId", companyId);
            parameters.Add("@CompanyColumn", companyColumnName);

            var result = ExecuteStoredProcedure("SP_DeleteAllData", parameters);
            return result;
        }

        public int DeleteAllVehicleShippingRates()
        {
            var result = ExecuteStoredProcedure("SP_DeleteAllVehicleShippingRates");
            return result;
        }

        public ExcelSheetName GetExcelSheetNameById(int id)
        {
            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@Id", id);

            var result = Get<ExcelSheetName>("GetExcelSheetById", parameter);

            return result;
        }


        public List<VehicleShippingRates> GetVehicleShippingRates()
        {
            var result = GetAll<VehicleShippingRates>("GetVehicleShippingRates");
            return result;
        }

        public rates_destinations RatesDestByCountryCode(string CountryCode, string City, int CompanyId)
        {
            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@CountryCode", CountryCode);
            parameter.Add("@City", City);
            parameter.Add("@CompanyId", CompanyId);

            var result = Get<rates_destinations>("SP_RatesDestByCountryCode", parameter);

            return result;
        }

        public int Add_bagC2C(bag_c2c bagC2C)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@kg_from", bagC2C.kg_from);
            parameters.Add("@kg_to", bagC2C.kg_to);
            parameters.Add("@zone", bagC2C.zone);
            parameters.Add("@rate", bagC2C.rate);
            parameters.Add("@company", bagC2C.company);

            int result = ExecuteStoredProcedure("SP_InsertBagC2C", parameters);
            return result;
        }

        public int Add_matrixC2C(matrix_c2c matrixC2C)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@origin_zone_no", matrixC2C.origin_zone_no);
            parameters.Add("@destination_zone_no", matrixC2C.destination_zone_no);
            parameters.Add("@bag_zone_code", matrixC2C.bag_zone_code);
            parameters.Add("company", matrixC2C.company);

            int result = ExecuteStoredProcedure("SP_InsertMatrixC2C", parameters);
            return result;
        }

        public int Add_roadRates(road_rates roadRates)
        {
            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@rates_destinations_id", roadRates.rates_destinations_id);
            parameters.Add("@cost_3_cuft", roadRates.cost_3_cuft);
            parameters.Add("@cost_6_cuft", roadRates.cost_6_cuft);
            parameters.Add("@cost_9_cuft", roadRates.cost_9_cuft);
            parameters.Add("@cost_12_cuft", roadRates.cost_12_cuft);
            parameters.Add("@cost_15_cuft", roadRates.cost_15_cuft);
            parameters.Add("@cost_18_cuft", roadRates.cost_18_cuft);
            parameters.Add("@cost_21_cuft", roadRates.cost_21_cuft);
            parameters.Add("@cost_24_cuft", roadRates.cost_24_cuft);
            parameters.Add("@cost_27_cuft", roadRates.cost_27_cuft);
            parameters.Add("@cost_30_cuft", roadRates.cost_30_cuft);
            parameters.Add("@cost_33_cuft", roadRates.cost_33_cuft);
            parameters.Add("@cost_36_cuft", roadRates.cost_36_cuft);
            parameters.Add("@cost_39_cuft", roadRates.cost_39_cuft);
            parameters.Add("@cost_42_cuft", roadRates.cost_42_cuft);
            parameters.Add("@cost_45_cuft", roadRates.cost_45_cuft);
            parameters.Add("@cost_68_cuft", roadRates.cost_68_cuft);
            parameters.Add("@cost_90_cuft", roadRates.cost_90_cuft);
            parameters.Add("@cost_100_cuft", roadRates.cost_100_cuft);
            parameters.Add("@company", roadRates.company);

            int result = ExecuteStoredProcedure("SP_InsertRoadRates", parameters);
            return result;
        }
    }
}
