using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuoteCalculatorAdmin.BL.FileImport
{
    public enum ImportFileTypes
    {
        None = 0,
        VehicleModelList = 1,
        VehicleShippingRate = 2,
        RateDestination = 3,
        RatesAir = 5,
        RatesCourier = 7,
        RatesSea = 1005,
        BagImportsUK = 1006,
        BagC2C = 1007,
        MatrixC2C = 1008,
        CreditorProducts = 1009,
        CurrencyRates = 1010,
        SailingScheduleforLondon = 1011,
        SailingScheduleforManchester = 1012,
        SailingScheduleforGlasgow = 1013,
        BranchPostcode = 1014,
        RatesRoad = 1015,
        ExpressCourierRates = 1016,
        CountryCodes_UK = 1020,
        GRP = 1021,
        FCL = 1022,
        AIR = 1023,
        Vehicle = 1024,
        TradeRegion = 1025,
        TradeRates = 1026,
        ExpressCourierCost = 1027,
        BagC2CCost = 1028,
        BagImportsUKCost = 1029,
        SeaFreightCost = 1030,
        CostsCourierEconomy = 1031

    }
}