﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuoteCalculatorAdmin.Models
{
    public class CountryListModel
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
}