using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuoteCalculator.Service.Models
{
    public class HomeControllerModel
    {
        public class VehicleModelList
        {
            public int Id { get; set; }
            public string MakeName { get; set; }
            public string ModelName { get; set; }
            public string Production { get; set; }
            public string DisplayName { get; set; }
            public Nullable<double> Length { get; set; }
            public Nullable<double> Width { get; set; }
            public Nullable<double> Height { get; set; }
            public string Display { get; set; }
            public string FitsInContainer { get; set; }
            public Nullable<double> Volume_FCL { get; set; }
            public Nullable<double> Volume_GRP { get; set; }
            public string Type { get; set; }
        }

        public class VehicleMake
        {
            public int Id { get; set; }
            public string MakeName { get; set; }
        }

        public class VehicleType
        {
            public int Id { get; set; }
            public string TypeName { get; set; }
        }

        public class VehicleModel
        {
            public int Id { get; set; }
            public string ModelName { get; set; }
        }

        public class rates_destinations
        {
            public long id { get; set; }
            public string country_code { get; set; }
            public string country { get; set; }
            public string city { get; set; }
            public string dest_code { get; set; }
            public string port_code { get; set; }
            public Nullable<int> radius { get; set; }
            public string world_zone { get; set; }
            public Nullable<bool> air { get; set; }
            public string road { get; set; }
            public Nullable<long> sea_rates_id { get; set; }
            public Nullable<long> courier_zone { get; set; }
            public Nullable<long> courier_vol_weight { get; set; }
            public Nullable<long> dcr_id { get; set; }
            public string car_port { get; set; }
            public Nullable<short> bag_orig { get; set; }
            public Nullable<short> bag_dest { get; set; }
            public Nullable<short> rem_orig { get; set; }
            public Nullable<short> rem_dest { get; set; }
            public Nullable<short> veh_orig { get; set; }
            public Nullable<short> veh_dest { get; set; }
            public Nullable<short> fa_orig { get; set; }
            public Nullable<short> fa_dest { get; set; }
            public string company { get; set; }
            public Nullable<int> display_order { get; set; }
            public Nullable<int> bag_c2c { get; set; }
            public Nullable<int> bag_imp { get; set; }
            public int CompanyId { get; set; }
            public Nullable<long> courier_express_vol_weight { get; set; }
        }

        public class VehicleShippingRates
        {
            public int Id { get; set; }
            public string CountryCode { get; set; }
            public string City { get; set; }
            public string Code { get; set; }
            public bool FCL { get; set; }
            public bool GPG { get; set; }
            public Nullable<double> FCL_LON { get; set; }
            public Nullable<double> FCL_MCR { get; set; }
            public Nullable<double> FCL_GLA { get; set; }
            public Nullable<double> Groupage_Lon_PerMt { get; set; }
            public Nullable<double> Groupage_Lon_L_S { get; set; }
            public Nullable<double> Groupage_MCR_PerMt { get; set; }
            public Nullable<double> Groupage_MCR_L_S { get; set; }
            public Nullable<double> Groupage_GLA_PerMt { get; set; }
            public Nullable<double> Groupage_GLA_L_S { get; set; }
            public string TransitTimes_FCL { get; set; }
            public string TransitTimes_GRP { get; set; }
        }

        public class PostCodeUK
        {
            public int Id { get; set; }
            public string State { get; set; }
            public string Code { get; set; }
            public string Housename { get; set; }
            public string Streetname { get; set; }
            public string City { get; set; }
            public string PostCode { get; set; }
            public int CompanyId { get; set; }
        }

        public class ExcelSheetName
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class air_rates
        {
            public long id { get; set; }
            public long rates_destinations_id { get; set; }
            public decimal airMin { get; set; }
            public decimal airlt50kg { get; set; }
            public decimal airlt100kg { get; set; }
            public decimal airlt150kg { get; set; }
            public decimal airlt200kg { get; set; }
            public decimal airlt300kg { get; set; }
            public decimal airover300kg { get; set; }
            public decimal doorMin { get; set; }
            public decimal door25plus { get; set; }
            public decimal door50plus { get; set; }
            public decimal door100plus { get; set; }
            public decimal door150plus { get; set; }
            public decimal door200plus { get; set; }
            public decimal door300plus { get; set; }
            public decimal handling { get; set; }
            public Nullable<int> company { get; set; }
        }

        public class courier_rates
        {
            public long id { get; set; }
            public long zone { get; set; }
            public decimal weight { get; set; }
            public decimal rate { get; set; }
            public Nullable<int> company { get; set; }
        }

        public class sea_rates
        {
            public long id { get; set; }
            public long zone { get; set; }
            public string Destination { get; set; }
            public decimal to25ls { get; set; }
            public decimal to25rate { get; set; }
            public decimal over25ls { get; set; }
            public decimal over25rate { get; set; }
            public decimal over50ls { get; set; }
            public decimal over50rate { get; set; }
            public decimal min { get; set; }
            public Nullable<int> company { get; set; }
        }

        public class bag_imports_uk
        {
            public long id { get; set; }
            public decimal kg_from { get; set; }
            public Nullable<decimal> kg_to { get; set; }
            public decimal zone1 { get; set; }
            public decimal zone2 { get; set; }
            public decimal zone3 { get; set; }
            public decimal zone4 { get; set; }
            public decimal zone5 { get; set; }
            public decimal zone6 { get; set; }
            public decimal zone7 { get; set; }
            public decimal zone8 { get; set; }
            public decimal zone9 { get; set; }
            public decimal zone10 { get; set; }
            public Nullable<int> company { get; set; }
            public Nullable<decimal> zone11 { get; set; }
            public Nullable<decimal> zone12 { get; set; }
            public Nullable<decimal> zone13 { get; set; }
            public Nullable<decimal> zone14 { get; set; }
            public Nullable<decimal> zone15 { get; set; }
            public Nullable<decimal> zone16 { get; set; }
        }

        public class bag_c2c
        {
            public long id { get; set; }
            public decimal kg_from { get; set; }
            public Nullable<decimal> kg_to { get; set; }
            public string zone { get; set; }
            public decimal rate { get; set; }
            public Nullable<int> company { get; set; }
        }

        public class matrix_c2c
        {
            public long id { get; set; }
            public int origin_zone_no { get; set; }
            public int destination_zone_no { get; set; }
            public string bag_zone_code { get; set; }
            public Nullable<int> company { get; set; }
        }

        public class CreditorProducts
        {
            public long Id { get; set; }
            public string Branch { get; set; }
            public string CreditorProduct { get; set; }
            public Nullable<int> CreditorCode { get; set; }
            public string Method { get; set; }
            public string RemovalType { get; set; }
            public string Service { get; set; }
            public string IntType { get; set; }
            public string RateFixed { get; set; }
            public string OriginCode { get; set; }
            public string OriginRegion { get; set; }
            public string OriginDescriptionCity { get; set; }
            public string OriginState { get; set; }
            public string OriginCountry { get; set; }
            public string OriginDistance { get; set; }
            public string DestCode { get; set; }
            public string DestRegion { get; set; }
            public string DestDescriptionCity { get; set; }
            public string DestState { get; set; }
            public string DestCountry { get; set; }
            public Nullable<int> DestDistance { get; set; }
            public string Currency { get; set; }
            public Nullable<int> Days { get; set; }
            public Nullable<decimal> RateValue { get; set; }
            public Nullable<int> Margin { get; set; }
            public Nullable<decimal> MinimumCost { get; set; }
            public Nullable<decimal> MaximumCost { get; set; }
            public string TransitTimeshort { get; set; }
            public string TransitTimelong { get; set; }
            public string TransitType { get; set; }
            public Nullable<long> rateid { get; set; }
            public string Comments { get; set; }
            public Nullable<System.DateTime> DateFrom { get; set; }
            public Nullable<System.DateTime> DateTo { get; set; }
            public string Time { get; set; }
            public string Ratetype { get; set; }
            public Nullable<int> QtyMin { get; set; }
            public Nullable<int> QtyMax { get; set; }
            public string InternalComment { get; set; }
            public string LoadType { get; set; }
            public string CreditorProductStatus { get; set; }
        }

        public class CreditorProducts_Rate
        {
            public long Id { get; set; }
            public long rateid { get; set; }
            public Nullable<decimal> Base { get; set; }
            public Nullable<decimal> Break { get; set; }
            public Nullable<decimal> Rate { get; set; }
        }

        public class currency_rate
        {
            public long id { get; set; }
            public string @base { get; set; }
            public string currency { get; set; }
            public decimal rate { get; set; }
        }

        public class sailingsched
        {
            public long id { get; set; }
            public string location { get; set; }
            public string port { get; set; }
            public string vessel { get; set; }
            public string loading { get; set; }
            public string eta { get; set; }
            public string agent { get; set; }
            public string country { get; set; }
            public string url { get; set; }
            public System.DateTime lastupdate { get; set; }
        }

        public class branch_postcode
        {
            public long id { get; set; }
            public long baggage_branch_id { get; set; }
            public string postcode { get; set; }
            public Nullable<short> dcr_zone { get; set; }
            public long removals_branch_id { get; set; }
            public long vehicle_branch_id { get; set; }
            public short removals_quotable { get; set; }
            public int CompanyId { get; set; }
        }

        public class road_rates
        {
            public long id { get; set; }
            public long rates_destinations_id { get; set; }
            public decimal cost_3_cuft { get; set; }
            public decimal cost_6_cuft { get; set; }
            public decimal cost_9_cuft { get; set; }
            public decimal cost_12_cuft { get; set; }
            public decimal cost_15_cuft { get; set; }
            public decimal cost_18_cuft { get; set; }
            public decimal cost_21_cuft { get; set; }
            public decimal cost_24_cuft { get; set; }
            public decimal cost_27_cuft { get; set; }
            public decimal cost_30_cuft { get; set; }
            public decimal cost_33_cuft { get; set; }
            public decimal cost_36_cuft { get; set; }
            public decimal cost_39_cuft { get; set; }
            public decimal cost_42_cuft { get; set; }
            public decimal cost_45_cuft { get; set; }
            public decimal cost_68_cuft { get; set; }
            public decimal cost_90_cuft { get; set; }
            public decimal cost_100_cuft { get; set; }
            public Nullable<int> company { get; set; }
        }
    }
}
