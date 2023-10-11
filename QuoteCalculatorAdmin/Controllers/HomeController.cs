using OfficeOpenXml;
using QuoteCalculatorAdmin.Common;
using QuoteCalculatorAdmin.Data;
using QuoteCalculatorAdmin.Data.Repository;
using QuoteCalculatorAdmin.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuoteCalculatorAdmin.BL;
using NLog;
using QuoteCalculatorAdmin.Helper;
using QuoteCalculator.Service.Repository.HomeRepository;
using QuoteCalculator.Service.Repository.CommonRepository;
//using static QuoteCalculator.Service.Models.HomeControllerModel;

namespace QuoteCalculatorAdmin.Controllers
{
    public class HomeController : BaseController
    {
        #region private variables
        private readonly IHomeRepository _homeRepository;
        private readonly ICommonRepository _commonRepository;
        private readonly GenericRepository<tbl_VehicleModelList> _dbRepositoryVehicleModelList;
        private readonly GenericRepository<tbl_VehicleMake> _dbRepositoryVehicleMake;
        private readonly GenericRepository<tbl_VehicleType> _dbRepositoryVehicleType;
        private readonly GenericRepository<tbl_VehicleModel> _dbRepositoryVehicleModel;
        private readonly GenericRepository<rates_destinations> _dbRepositoryCountry;
        //private readonly GenericRepository<rates_destinations> _dbRepositoryCity;
        private readonly GenericRepository<tbl_VehicleShippingRates> _dbRepositoryVehicleShippingRates;
        private readonly GenericRepository<tbl_PostCodeUK> _dbRepositoryPostCodeUK;
        private readonly GenericRepository<tbl_ExcelSheetName> _dbRepositoryExcelSheetName;
        private readonly GenericRepository<air_rates> _dbRepositoryAirRates;
        private readonly GenericRepository<courier_rates> _dbRepositoryRatesCourier;
        private readonly GenericRepository<sea_rates> _dbRepositorySeaRate;
        private readonly GenericRepository<bag_imports_uk> _dbRepositoryBagImportsUK;
        private readonly GenericRepository<bag_c2c> _dbRepositoryBagC2C;
        private readonly GenericRepository<matrix_c2c> _dbRepositoryMatrixC2C;
        private readonly GenericRepository<tbl_CreditorProducts> _dbRepositoryCreditorProducts;
        private readonly GenericRepository<tbl_CreditorProducts_Rate> _dbRepositoryCreditorProductsRate;
        private readonly GenericRepository<currency_rate> _dbRepositoryCurrencyRate;
        private readonly GenericRepository<sailingsched> _dbRepositorySailingSched;
        private readonly GenericRepository<branch_postcode> _dbRepositoryBranchPostCode;
        private readonly GenericRepository<road_rates> _dbRepositoryRoadRates;
        #endregion

        #region Constructor
        public HomeController(IHomeRepository homeRepository, ICommonRepository commonRepository)
        {
            _dbRepositoryVehicleModelList = new GenericRepository<tbl_VehicleModelList>();
            _dbRepositoryVehicleMake = new GenericRepository<tbl_VehicleMake>();
            _dbRepositoryVehicleType = new GenericRepository<tbl_VehicleType>();
            _dbRepositoryVehicleModel = new GenericRepository<tbl_VehicleModel>();
            _dbRepositoryCountry = new GenericRepository<rates_destinations>();
            //_dbRepositoryCity = new GenericRepository<rates_destinations>();
            _dbRepositoryVehicleShippingRates = new GenericRepository<tbl_VehicleShippingRates>();
            _dbRepositoryPostCodeUK = new GenericRepository<tbl_PostCodeUK>();
            _dbRepositoryExcelSheetName = new GenericRepository<tbl_ExcelSheetName>();
            _dbRepositoryAirRates = new GenericRepository<air_rates>();
            _dbRepositoryRatesCourier = new GenericRepository<courier_rates>();
            _dbRepositorySeaRate = new GenericRepository<sea_rates>();
            _dbRepositoryBagImportsUK = new GenericRepository<bag_imports_uk>();
            _dbRepositoryBagC2C = new GenericRepository<bag_c2c>();
            _dbRepositoryMatrixC2C = new GenericRepository<matrix_c2c>();
            _dbRepositoryCreditorProducts = new GenericRepository<tbl_CreditorProducts>();
            _dbRepositoryCreditorProductsRate = new GenericRepository<tbl_CreditorProducts_Rate>();
            _dbRepositoryCurrencyRate = new GenericRepository<currency_rate>();
            _dbRepositorySailingSched = new GenericRepository<sailingsched>();
            _dbRepositoryBranchPostCode = new GenericRepository<branch_postcode>();
            _dbRepositoryRoadRates = new GenericRepository<road_rates>();
            _homeRepository = homeRepository;
            _commonRepository = commonRepository;
        }
        quotesEntities excelImportDBEntities = new quotesEntities();

        #endregion

        private static NLog.Logger logger = LogManager.GetCurrentClassLogger();

        public ActionResult Index()
        {
            using (var context = BaseContext.GetDbContext())
            {
                var list = context.tbl_ExcelSheetName.Select(m => new { m.Id, m.Name }).OrderBy(m => new { m.Name }).ToList();
                IEnumerable<SelectListItem> items = list.Select(c => new SelectListItem { Value = Convert.ToString(c.Id), Text = c.Name });
                ViewBag.SpreadSheetList = items;
                //return Json(list, JsonRequestBehavior.AllowGet);
            }
            return View();
        }

        public ActionResult GetSpreadSheetList()
        {
            using (var context = BaseContext.GetDbContext())
            {
                var list = context.tbl_ExcelSheetName.Select(m => new { m.Id, m.Name }).OrderBy(m => new { m.Name }).ToList();
                return Json(list, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult Upload(SpreadSheetModel model, HttpPostedFileBase file)
        {
            if (model.SpreadSheetId == 0)
            {
                return View("Index", model);
            }
            try
            {
                if (file != null)
                {
                    int CompanyId = SessionHelper.CompanyId;
                    var spreadSheetObj = _homeRepository.GetExcelSheetNameById(model.SpreadSheetId);
                    //var spreadSheetObj = _dbRepositoryExcelSheetName.GetEntities().Where(m => m.Id == model.SpreadSheetId).FirstOrDefault();
                    //if (file.FileName != null)
                    //{
                    //    if (file.FileName.Split('.')[0] != spreadSheetObj.Name)
                    //    {
                    //        TempData[CustomEnums.NotifyType.Error.GetDescription()] = "Please verify uploaded sheet and selected sheet boh are same";
                    //        return RedirectToAction("Index");
                    //    }
                    //}
                    if (model.SpreadSheetId == 1) //WorksheetCommonName Is not "Vehicles", So it directly returns "Data is Succesfully Enter in Database. "
                    {
                        var WorksheetCommonName = "";
                        byte[] CommmonfileBytes = new byte[file.ContentLength];
                        var Commondata = file.InputStream.Read(CommmonfileBytes, 0, Convert.ToInt32(file.ContentLength));
                        var Commonpackage = new ExcelPackage(file.InputStream);
                        var CommoncurrentSheet = Commonpackage.Workbook.Worksheets;
                        foreach (ExcelWorksheet item1 in CommoncurrentSheet)
                        {
                            WorksheetCommonName = item1.Name;
                            if (WorksheetCommonName == "Vehicles")
                            {
                                //Insert Data   
                                var vehicleList = new List<tbl_VehicleModelList>();
                                if (Request != null)
                                {
                                    if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                                    {
                                        string fileName = file.FileName;
                                        string fileContentType = file.ContentType;
                                        byte[] fileBytes = new byte[file.ContentLength];
                                        var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
                                        using (var package = new ExcelPackage(file.InputStream))
                                        {
                                            var currentSheet = package.Workbook.Worksheets;
                                            var workSheet = currentSheet.First();
                                            var a = workSheet.Column(1);
                                            var noOfCol = workSheet.Dimension.End.Column;
                                            var noOfRow = workSheet.Dimension.End.Row;

                                            //List<string> selectedColumnName = new List<string>();
                                            //foreach (ExcelWorksheet worksheet in currentSheet)
                                            //{
                                            //    if (worksheet.ToString() == "Vehicles")
                                            //    {
                                            //        for (int i = 1; i <= noOfCol; i++)
                                            //        {
                                            //            if (worksheet.Cells[1, i].Value != null)
                                            //            {
                                            //                selectedColumnName.Add(worksheet.Cells[1, i].Value.ToString());
                                            //            }
                                            //        }
                                            //    }
                                            //}
                                            //List<string> dbColumnName = new List<string>();
                                            //var properties = typeof(tbl_VehicleModelList).GetProperties();
                                            //foreach (var item in properties)
                                            //{
                                            //    if (!item.Name.Contains("tbl_"))
                                            //    {
                                            //        dbColumnName.Add(item.Name);
                                            //    }
                                            //}
                                            //if (dbColumnName.Count() - 1 != selectedColumnName.Count())
                                            //{
                                            //    TempData[CustomEnums.NotifyType.Error.GetDescription()] = "Column name missing in sheet";
                                            //    return RedirectToAction("Index");
                                            //}
                                            //for (int i = 1; i < dbColumnName.Count(); i++)
                                            //{
                                            //    var selectedcol = string.Empty;
                                            //    if (selectedColumnName[i - 1].Contains('('))
                                            //    {
                                            //        selectedColumnName[i - 1] = selectedColumnName[i - 1].Split('(')[0];
                                            //        if (selectedColumnName[i - 1].Contains(')'))
                                            //        {
                                            //            selectedColumnName[i - 1] = selectedColumnName[i - 1].Split('(')[1];
                                            //            selectedcol = selectedColumnName[i - 1];
                                            //        }
                                            //    }
                                            //    else
                                            //    {
                                            //        selectedcol = selectedColumnName[i - 1];
                                            //    }
                                            //    if (!dbColumnName[i].Replace("?", "").Replace("_", "").ToLower().Contains(selectedcol.Replace("?", "").Replace("_", "").Replace(" ", "").ToLower()))
                                            //    {
                                            //        // Column of both sheet are same but name is different
                                            //        TempData[CustomEnums.NotifyType.Error.GetDescription()] = "Column name does not match";
                                            //        return RedirectToAction("Index");
                                            //    }
                                            //}

                                            //Delete Data
                                            List<tbl_VehicleMake> DmodelVehicleMake = _dbRepositoryVehicleMake.GetEntities().ToList();
                                            if (DmodelVehicleMake.Count > 0)
                                            {
                                                foreach (var item in DmodelVehicleMake)
                                                {
                                                    _dbRepositoryVehicleMake.Delete(item.Id);
                                                }
                                            }
                                            List<tbl_VehicleModel> DmodelVehicleModel = _dbRepositoryVehicleModel.GetEntities().ToList();
                                            if (DmodelVehicleModel.Count > 0)
                                            {
                                                foreach (var item in DmodelVehicleModel)
                                                {
                                                    _dbRepositoryVehicleModel.Delete(item.Id);
                                                }
                                            }
                                            List<tbl_VehicleType> DmodelVehicleType = _dbRepositoryVehicleType.GetEntities().ToList();
                                            if (DmodelVehicleType.Count > 0)
                                            {
                                                foreach (var item in DmodelVehicleType)
                                                {
                                                    _dbRepositoryVehicleType.Delete(item.Id);
                                                }
                                            }
                                            List<tbl_VehicleModelList> DmodelVehicleList = _dbRepositoryVehicleModelList.GetEntities().ToList();
                                            if (DmodelVehicleList.Count > 0)
                                            {
                                                foreach (var item in DmodelVehicleList)
                                                {
                                                    _dbRepositoryVehicleModelList.Delete(item.Id);
                                                }
                                            }
                                            //coloum Add in another table
                                            for (int coloumnIterator = 1; coloumnIterator <= noOfCol; coloumnIterator++)
                                            {
                                                if (coloumnIterator == 1)
                                                {
                                                    var vehicleMakeList = new List<tbl_VehicleMake>();

                                                    for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                                                    {
                                                        tbl_VehicleMake modelVehicleMake = new tbl_VehicleMake();
                                                        modelVehicleMake.MakeName = workSheet.Cells[rowIterator, coloumnIterator].Value.ToString();
                                                        vehicleMakeList.Add(modelVehicleMake);
                                                    }
                                                    foreach (tbl_VehicleMake item in vehicleMakeList)
                                                    {
                                                        var Ifexit = _dbRepositoryVehicleMake.GetEntities().Where(x => x.MakeName == item.MakeName).FirstOrDefault();
                                                        if (Ifexit == null)
                                                        {
                                                            excelImportDBEntities.tbl_VehicleMake.Add(item);
                                                            _dbRepositoryVehicleMake.Insert(item);
                                                        }
                                                        //excelImportDBEntities.SaveChanges();
                                                    }
                                                }
                                                else if (coloumnIterator == 2)
                                                {
                                                    var vehicleModelList = new List<tbl_VehicleModel>();
                                                    for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                                                    {
                                                        tbl_VehicleModel modelVehicleModel = new tbl_VehicleModel();
                                                        modelVehicleModel.ModelName = workSheet.Cells[rowIterator, coloumnIterator].Value.ToString();
                                                        vehicleModelList.Add(modelVehicleModel);
                                                    }
                                                    foreach (tbl_VehicleModel item in vehicleModelList)
                                                    {
                                                        var Ifexit = _dbRepositoryVehicleModel.GetEntities().Where(x => x.ModelName == item.ModelName).FirstOrDefault();
                                                        if (Ifexit == null)
                                                        {
                                                            excelImportDBEntities.tbl_VehicleModel.Add(item);
                                                            //_dbRepositoryVehicleMake.Insert(item);
                                                            _dbRepositoryVehicleModel.Insert(item);
                                                        }
                                                        //excelImportDBEntities.SaveChanges();
                                                    }
                                                }
                                                else if (coloumnIterator == 12)
                                                {
                                                    var VehicleTypeList = new List<tbl_VehicleType>();

                                                    for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                                                    {
                                                        tbl_VehicleType modelVehicleType = new tbl_VehicleType();
                                                        modelVehicleType.TypeName = workSheet.Cells[rowIterator, coloumnIterator].Value.ToString();
                                                        VehicleTypeList.Add(modelVehicleType);
                                                    }
                                                    foreach (tbl_VehicleType item in VehicleTypeList)
                                                    {
                                                        var Ifexit = _dbRepositoryVehicleType.GetEntities().Where(x => x.TypeName == item.TypeName).FirstOrDefault();
                                                        if (Ifexit == null)
                                                        {
                                                            excelImportDBEntities.tbl_VehicleType.Add(item);
                                                        }
                                                        excelImportDBEntities.SaveChanges();
                                                    }
                                                }
                                            }
                                            //row Add in table 
                                            for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                                            {
                                                var vehicle = new tbl_VehicleModelList();
                                                var Makename = workSheet.Cells[rowIterator, 1].Value.ToString();
                                                var Modelname = workSheet.Cells[rowIterator, 2].Value.ToString();
                                                var Typename = workSheet.Cells[rowIterator, 12].Value.ToString();
                                                vehicle.MakeName = Makename;
                                                vehicle.ModelName = Modelname;
                                                vehicle.Production = workSheet.Cells[rowIterator, 3].Value == null ? string.Empty : workSheet.Cells[rowIterator, 3].Value.ToString();
                                                vehicle.DisplayName = workSheet.Cells[rowIterator, 4].Value == null ? string.Empty : workSheet.Cells[rowIterator, 4].Value.ToString();
                                                vehicle.Length = workSheet.Cells[rowIterator, 5].Value == null ? 0 : float.Parse(workSheet.Cells[rowIterator, 5].Value.ToString());
                                                vehicle.Width = workSheet.Cells[rowIterator, 6].Value == null ? 0 : float.Parse(workSheet.Cells[rowIterator, 6].Value.ToString());
                                                vehicle.Height = workSheet.Cells[rowIterator, 7].Value == null ? 0 : float.Parse(workSheet.Cells[rowIterator, 7].Value.ToString());
                                                vehicle.Display = workSheet.Cells[rowIterator, 8].Value == null ? string.Empty : workSheet.Cells[rowIterator, 8].Value.ToString();
                                                vehicle.FitsInContainer = workSheet.Cells[rowIterator, 9].Value == null ? string.Empty : workSheet.Cells[rowIterator, 9].Value.ToString();
                                                vehicle.Volume_FCL = workSheet.Cells[rowIterator, 10].Value == null ? 0 : float.Parse(workSheet.Cells[rowIterator, 10].Value.ToString());
                                                vehicle.Volume_GRP = workSheet.Cells[rowIterator, 11].Value == null ? 0 : float.Parse(workSheet.Cells[rowIterator, 11].Value.ToString());
                                                vehicle.Type = Typename;
                                                vehicleList.Add(vehicle);
                                            }
                                        }
                                        foreach (var item in vehicleList)
                                        {
                                            excelImportDBEntities.tbl_VehicleModelList.Add(item);
                                        }
                                        excelImportDBEntities.SaveChanges();
                                    }
                                }
                            }
                            else
                            {
                                TempData[CustomEnums.NotifyType.Success.GetDescription()] = "Data is Succesfully Enter in Database. ";
                                return RedirectToAction("Index");
                                //TempData[CustomEnums.NotifyType.Error.GetDescription()] = "file is not corect format.";
                                //return RedirectToAction("Index");
                            }
                        }
                    }
                    else if (model.SpreadSheetId == 2)
                    {
                        //List<QuoteCalculator.Service.Models.HomeControllerModel.VehicleShippingRates> DmodelVehicleshippingrate = _homeRepository.GetVehicleShippingRates(); //_dbRepositoryVehicleShippingRates.GetEntities().ToList();
                        //if (DmodelVehicleshippingrate.Count > 0)
                        //{
                        //    foreach (var item in DmodelVehicleshippingrate)
                        //    {
                        //        _dbRepositoryVehicleShippingRates.Delete(item.Id);
                        //    }
                        //}
                        _homeRepository.DeleteAllVehicleShippingRates();

                        //Insert Data
                        List<QuoteCalculator.Service.Models.HomeControllerModel.VehicleShippingRates> vehicleShippingRateList = new List<QuoteCalculator.Service.Models.HomeControllerModel.VehicleShippingRates>();
                        if (Request != null)
                        {
                            if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                            {
                                string fileName = file.FileName;
                                string fileContentType = file.ContentType;
                                byte[] fileBytes = new byte[file.ContentLength];
                                var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
                                using (var package = new ExcelPackage(file.InputStream))
                                {
                                    var currentSheet = package.Workbook.Worksheets;
                                    int noOfCol = 0, noOfRow = 0;
                                    ExcelWorksheet workSheet;

                                    foreach (ExcelWorksheet item1 in currentSheet)
                                    {
                                        var WorksheetName = item1.Name;
                                        if (WorksheetName == "Destinations")
                                        {
                                            workSheet = item1;
                                            foreach (var modelworksheet in item1.Workbook.Worksheets)
                                            {
                                                if (modelworksheet.Name == "Destinations")
                                                {
                                                    noOfCol = modelworksheet.Dimension.End.Column;
                                                    noOfRow = modelworksheet.Dimension.End.Row;
                                                }
                                            }
                                            //for (int coloumnIterator = 1; coloumnIterator <= noOfCol; coloumnIterator++)
                                            //{
                                            //    if (coloumnIterator == 1)
                                            //    {
                                            //        var vehicleCountryList = new List<tbl_Country>();
                                            //        for (int rowIterator = 4; rowIterator <= noOfRow; rowIterator++)
                                            //        {
                                            //            //tbl_Country modelCountry = new tbl_Country();
                                            //            //modelCountry.CountryName = workSheet.Cells[rowIterator, coloumnIterator].Value == null ? string.Empty : workSheet.Cells[rowIterator, coloumnIterator].Value.ToString();
                                            //            //vehicleCountryList.Add(modelCountry);
                                            //        }
                                            //        foreach (tbl_Country item in vehicleCountryList)
                                            //        {
                                            //            //var Ifexit = _dbRepositoryCountry.GetEntities().Where(x => x.country == item.CountryName).FirstOrDefault();
                                            //            //if (Ifexit == null)
                                            //            //{
                                            //            //    excelImportDBEntities.tbl_Country.Add(item);
                                            //            //}
                                            //            //excelImportDBEntities.SaveChanges();
                                            //        }
                                            //    }
                                            //    else if (coloumnIterator == 2)
                                            //    {
                                            //        var vehicleCityList = new List<tbl_City>();
                                            //        for (int rowIterator = 4; rowIterator <= noOfRow; rowIterator++)
                                            //        {
                                            //            //tbl_City modelCityModel = new tbl_City();
                                            //            //modelCityModel.CityName = workSheet.Cells[rowIterator, coloumnIterator].Value == null ? string.Empty : workSheet.Cells[rowIterator, coloumnIterator].Value.ToString();
                                            //            //modelCityModel.Code = workSheet.Cells[rowIterator, 3].Value == null ? string.Empty : workSheet.Cells[rowIterator, 3].Value.ToString();
                                            //            //vehicleCityList.Add(modelCityModel);
                                            //        }
                                            //        foreach (tbl_City item in vehicleCityList)
                                            //        {
                                            //            //var Ifexit = _dbRepositoryCity.GetEntities().Where(x => x.city == item.CityName).FirstOrDefault();
                                            //            //if (Ifexit == null)
                                            //            //{
                                            //            //    excelImportDBEntities.tbl_City.Add(item);
                                            //            //}
                                            //            //excelImportDBEntities.SaveChanges();
                                            //        }
                                            //    }
                                            //}

                                            //row Add in table 
                                            for (int rowIterator = 4; rowIterator <= noOfRow; rowIterator++)
                                            {
                                                var country = workSheet.Cells[rowIterator, 1].Value;
                                                var city = workSheet.Cells[rowIterator, 2].Value;
                                                if (country != null && city != null)
                                                {
                                                    var vehicle = new QuoteCalculator.Service.Models.HomeControllerModel.VehicleShippingRates();
                                                    var Countryname = workSheet.Cells[rowIterator, 1].Value.ToString();
                                                    var Cityname = workSheet.Cells[rowIterator, 2].Value.ToString();

                                                    rates_destinations modelCountry = _dbRepositoryCountry.GetEntities().Where(x => x.country == Countryname).FirstOrDefault();
                                                    //rates_destinations modelCity = _dbRepositoryCity.GetEntities().Where(x => x.city == Cityname).FirstOrDefault();

                                                    vehicle.CountryCode = modelCountry.country_code;
                                                    vehicle.City = Cityname;
                                                    vehicle.Code = workSheet.Cells[rowIterator, 3].Value == null ? string.Empty : workSheet.Cells[rowIterator, 3].Value.ToString();
                                                    vehicle.FCL = workSheet.Cells[rowIterator, 4].Value == null ? false : true;
                                                    vehicle.GPG = workSheet.Cells[rowIterator, 5].Value == null ? false : true;
                                                    vehicle.FCL_LON = workSheet.Cells[rowIterator, 6].Value == null ? 0 : float.Parse(workSheet.Cells[rowIterator, 6].Value.ToString());
                                                    vehicle.FCL_MCR = workSheet.Cells[rowIterator, 7].Value == null ? 0 : float.Parse(workSheet.Cells[rowIterator, 7].Value.ToString());
                                                    vehicle.FCL_GLA = workSheet.Cells[rowIterator, 8].Value == null ? 0 : float.Parse(workSheet.Cells[rowIterator, 8].Value.ToString());
                                                    vehicle.Groupage_Lon_PerMt = workSheet.Cells[rowIterator, 9].Value == null ? 0 : float.Parse(workSheet.Cells[rowIterator, 9].Value.ToString());
                                                    vehicle.Groupage_Lon_L_S = workSheet.Cells[rowIterator, 10].Value == null ? 0 : float.Parse(workSheet.Cells[rowIterator, 10].Value.ToString());
                                                    vehicle.Groupage_MCR_PerMt = workSheet.Cells[rowIterator, 11].Value == null ? 0 : float.Parse(workSheet.Cells[rowIterator, 11].Value.ToString());
                                                    vehicle.Groupage_MCR_L_S = workSheet.Cells[rowIterator, 12].Value == null ? 0 : float.Parse(workSheet.Cells[rowIterator, 12].Value.ToString());
                                                    vehicle.Groupage_GLA_PerMt = workSheet.Cells[rowIterator, 13].Value == null ? 0 : float.Parse(workSheet.Cells[rowIterator, 13].Value.ToString());
                                                    vehicle.Groupage_GLA_L_S = workSheet.Cells[rowIterator, 14].Value == null ? 0 : float.Parse(workSheet.Cells[rowIterator, 14].Value.ToString());
                                                    vehicle.TransitTimes_FCL = workSheet.Cells[rowIterator, 15].Value == null ? string.Empty : workSheet.Cells[rowIterator, 15].Value.ToString();
                                                    vehicle.TransitTimes_GRP = workSheet.Cells[rowIterator, 16].Value == null ? string.Empty : workSheet.Cells[rowIterator, 16].Value.ToString();
                                                    vehicleShippingRateList.Add(vehicle);
                                                }
                                                else
                                                {
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                                foreach (var item in vehicleShippingRateList)
                                {

                                    //excelImportDBEntities.tbl_VehicleShippingRates.Add(item);
                                    _homeRepository.AddVehicleShippingRates(item);
                                }
                                //excelImportDBEntities.SaveChanges();
                            }
                        }
                    }
                    else if (model.SpreadSheetId == 3) //This value never comes
                    {
                        //Delete Data
                        List<tbl_PostCodeUK> DmodelVehiclePostCodeUK = _dbRepositoryPostCodeUK.GetEntities().ToList();
                        if (DmodelVehiclePostCodeUK.Count > 0)
                        {
                            foreach (var item in DmodelVehiclePostCodeUK)
                            {
                                _dbRepositoryPostCodeUK.Delete(item.Id);
                            }
                        }
                        //Insert Data
                        var vehiclePostCodeUKList = new List<tbl_PostCodeUK>();
                        if (Request != null)
                        {
                            //var id = Request.Files["SpreadSheetName"];
                            //HttpPostedFileBase file = Request.Files["UploadedFile"];
                            if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                            {
                                string fileName = file.FileName;
                                string fileContentType = file.ContentType;
                                byte[] fileBytes = new byte[file.ContentLength];
                                var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
                                using (var package = new ExcelPackage(file.InputStream))
                                {
                                    var currentSheet = package.Workbook.Worksheets;
                                    var workSheet = currentSheet.First();
                                    var noOfCol = workSheet.Dimension.End.Column;
                                    var noOfRow = workSheet.Dimension.End.Row;

                                    //row Add in table 
                                    for (int rowIterator = 1; rowIterator <= noOfRow; rowIterator++)
                                    {
                                        var vehicle = new tbl_PostCodeUK();
                                        vehicle.State = workSheet.Cells[rowIterator, 1].Value == null ? string.Empty : workSheet.Cells[rowIterator, 1].Value.ToString();
                                        vehicle.Code = workSheet.Cells[rowIterator, 2].Value == null ? string.Empty : workSheet.Cells[rowIterator, 2].Value.ToString();
                                        vehicle.Housename = workSheet.Cells[rowIterator, 3].Value == null ? string.Empty : workSheet.Cells[rowIterator, 3].Value.ToString();
                                        vehicle.Streetname = workSheet.Cells[rowIterator, 4].Value == null ? string.Empty : workSheet.Cells[rowIterator, 4].Value.ToString();
                                        vehicle.City = workSheet.Cells[rowIterator, 5].Value == null ? string.Empty : workSheet.Cells[rowIterator, 5].Value.ToString();
                                        vehicle.PostCode = workSheet.Cells[rowIterator, 6].Value == null ? string.Empty : workSheet.Cells[rowIterator, 6].Value.ToString();
                                        vehiclePostCodeUKList.Add(vehicle);

                                    }
                                    foreach (var item in vehiclePostCodeUKList)
                                    {
                                        excelImportDBEntities.tbl_PostCodeUK.Add(item);
                                    }
                                    excelImportDBEntities.SaveChanges();
                                }
                            }
                        }
                    }
                    else if (model.SpreadSheetId == 4)
                    {
                        //Insert Data
                        var rateList = new List<QuoteCalculator.Service.Models.HomeControllerModel.rates_destinations>();
                        if (Request != null)
                        {
                            if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                            {
                                string fileName = file.FileName;
                                string fileContentType = file.ContentType;
                                byte[] fileBytes = new byte[file.ContentLength];
                                var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
                                using (var package = new ExcelPackage(file.InputStream))
                                {
                                    var currentSheet = package.Workbook.Worksheets;
                                    var workSheet = currentSheet.First();
                                    var noOfCol = workSheet.Dimension.End.Column;
                                    var noOfRow = workSheet.Dimension.End.Row;

                                    // delete data


                                    //List<rates_destinations> rateDeatinationDtl = _dbRepositoryCountry.GetEntities().Where(x => x.CompanyId == CompanyId).ToList();
                                    //if (rateDeatinationDtl.Count > 0)
                                    //{
                                    //    foreach (var item in rateDeatinationDtl)
                                    //    {
                                    //        _dbRepositoryCountry.Delete(item.id);
                                    //    }
                                    //}

                                    var result = _homeRepository.DeleteAllData("quotes.rates_destinations", CompanyId, "CompanyId");

                                    if (result != -1)
                                    {
                                        //row Add in table  
                                        for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                                        {
                                            var rates = new QuoteCalculator.Service.Models.HomeControllerModel.rates_destinations();
                                            rates.country = workSheet.Cells[rowIterator, 1].Value == null ? string.Empty : workSheet.Cells[rowIterator, 1].Value.ToString();
                                            rates.city = workSheet.Cells[rowIterator, 2].Value == null ? string.Empty : workSheet.Cells[rowIterator, 2].Value.ToString();
                                            rates.dest_code = workSheet.Cells[rowIterator, 3].Value == null ? string.Empty : workSheet.Cells[rowIterator, 3].Value.ToString();
                                            rates.port_code = workSheet.Cells[rowIterator, 4].Value == null ? string.Empty : workSheet.Cells[rowIterator, 4].Value.ToString();
                                            rates.radius = workSheet.Cells[rowIterator, 5].Value == null ? 0 : Convert.ToInt32(workSheet.Cells[rowIterator, 5].Value);
                                            rates.country_code = workSheet.Cells[rowIterator, 6].Value == null ? string.Empty : workSheet.Cells[rowIterator, 6].Value.ToString();
                                            rates.world_zone = workSheet.Cells[rowIterator, 7].Value == null ? string.Empty : workSheet.Cells[rowIterator, 7].Value.ToString();
                                            rates.air = workSheet.Cells[rowIterator, 8].Value != null ? workSheet.Cells[rowIterator, 8].Value.ToString() == "y" ? true : false : false;
                                            rates.road = workSheet.Cells[rowIterator, 9].Value == null ? string.Empty : workSheet.Cells[rowIterator, 9].Value.ToString();
                                            rates.sea_rates_id = Convert.ToInt32(workSheet.Cells[rowIterator, 10].Value);
                                            rates.courier_zone = Convert.ToInt32(workSheet.Cells[rowIterator, 11].Value);
                                            rates.courier_vol_weight = Convert.ToInt32(workSheet.Cells[rowIterator, 12].Value);
                                            rates.courier_express_vol_weight = Convert.ToInt32(workSheet.Cells[rowIterator, 13].Value);
                                            rates.dcr_id = Convert.ToInt32(workSheet.Cells[rowIterator, 14].Value);
                                            rates.car_port = workSheet.Cells[rowIterator, 15].Value == null ? string.Empty : workSheet.Cells[rowIterator, 15].Value.ToString();
                                            rates.bag_orig = 0;
                                            rates.bag_dest = workSheet.Cells[rowIterator, 16].Value != null ? workSheet.Cells[rowIterator, 16].Value.ToString() == "Y" ? Convert.ToInt16(1) : Convert.ToInt16(0) : Convert.ToInt16(0);
                                            rates.rem_orig = workSheet.Cells[rowIterator, 17].Value != null ? workSheet.Cells[rowIterator, 17].Value.ToString() == "Y" ? Convert.ToInt16(1) : Convert.ToInt16(0) : Convert.ToInt16(0);
                                            rates.rem_dest = workSheet.Cells[rowIterator, 18].Value != null ? workSheet.Cells[rowIterator, 18].Value.ToString() == "Y" ? Convert.ToInt16(1) : Convert.ToInt16(0) : Convert.ToInt16(0);
                                            rates.veh_orig = Convert.ToInt32(workSheet.Cells[rowIterator, 14].Value) > Convert.ToInt16(0) ? Convert.ToInt16(1) : Convert.ToInt16(0);
                                            rates.veh_dest = workSheet.Cells[rowIterator, 20].Value != null ? workSheet.Cells[rowIterator, 20].Value.ToString() == "Y" ? Convert.ToInt16(1) : Convert.ToInt16(0) : Convert.ToInt16(0);
                                            rates.fa_orig = workSheet.Cells[rowIterator, 21].Value != null ? workSheet.Cells[rowIterator, 21].Value.ToString() == "Y" ? Convert.ToInt16(1) : Convert.ToInt16(0) : Convert.ToInt16(0);
                                            rates.fa_dest = workSheet.Cells[rowIterator, 22].Value != null ? workSheet.Cells[rowIterator, 22].Value.ToString() == "Y" ? Convert.ToInt16(1) : Convert.ToInt16(0) : Convert.ToInt16(0);
                                            rates.company = string.Empty;
                                            rates.display_order = 0;
                                            rates.bag_c2c = workSheet.Cells[rowIterator, 23].Value == null ? 0 : Convert.ToInt32(workSheet.Cells[rowIterator, 23].Value);
                                            rates.bag_imp = workSheet.Cells[rowIterator, 24].Value == null ? 0 : Convert.ToInt32(workSheet.Cells[rowIterator, 24].Value);
                                            rates.CompanyId = CompanyId;
                                            rateList.Add(rates);
                                        }

                                        foreach (var item in rateList)
                                        {
                                            //excelImportDBEntities.rates_destinations.Add(item);
                                            _homeRepository.AddRatesDestinations(item);
                                        }
                                        //excelImportDBEntities.SaveChanges();
                                    }
                                }
                            }
                        }
                    }
                    else if (model.SpreadSheetId == 1006)
                    {
                        //Insert Data
                        var bagsImportUKList = new List<bag_imports_uk>();
                        if (Request != null)
                        {
                            if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                            {
                                string fileName = file.FileName;
                                string fileContentType = file.ContentType;
                                byte[] fileBytes = new byte[file.ContentLength];
                                var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
                                using (var package = new ExcelPackage(file.InputStream))
                                {
                                    var currentSheet = package.Workbook.Worksheets;
                                    var workSheet = currentSheet.FirstOrDefault(x => x.Name == "Bag Imports UK");

                                    if (workSheet == null)
                                    {
                                        TempData[CustomEnums.NotifyType.Error.GetDescription()] = "Worksheet Bag Imporks UK does not exist in the file.";
                                        return RedirectToAction("Index");
                                    }

                                    var noOfCol = workSheet.Dimension.End.Column;
                                    var noOfRow = workSheet.Dimension.End.Row;

                                    List<string> expectedColumns = new List<string>()
                                            { "KG","","Zone 1","Zone 2","Zone 3","Zone 4","Zone 5","Zone 6","Zone 7","Zone 8","Zone 9","Zone 10" };

                                    for (int i = 0; i < expectedColumns.Count; i++)
                                    {
                                        if (Convert.ToString(workSheet.Cells[1, i + 1].Value) != expectedColumns[i])
                                        {
                                            TempData[CustomEnums.NotifyType.Error.GetDescription()] = "Column name missing in sheet";
                                            return RedirectToAction("Index");
                                        }
                                    }


                                    // delete data
                                    //List<bag_imports_uk> bagImportsList = _dbRepositoryBagImportsUK.GetEntities().Where(x => x.company == CompanyId).ToList();
                                    //if (bagImportsList.Count > 0)
                                    //{
                                    //    foreach (var item in bagImportsList)
                                    //    {
                                    //        _dbRepositoryBagImportsUK.Delete(item.id);
                                    //    }
                                    //}

                                    var result = _homeRepository.DeleteAllData("quotes.bag_imports_uk", CompanyId, "company");

                                    //row Add in table 
                                    var bagImportsList = new List<QuoteCalculator.Service.Models.HomeControllerModel.bag_imports_uk>();
                                    for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                                    {
                                        var bagImportsUK = new QuoteCalculator.Service.Models.HomeControllerModel.bag_imports_uk();
                                        if (!decimal.TryParse(Convert.ToString(workSheet.Cells[rowIterator, 1].Value), out decimal from))
                                            continue;

                                        bagImportsUK.kg_from = Convert.ToDecimal(workSheet.Cells[rowIterator, 1].Value);
                                        //bagImportsUK.kg_to = Convert.ToDecimal(workSheet.Cells[rowIterator, 2].Value);

                                        bagImportsUK.zone1 = Convert.ToDecimal(workSheet.Cells[rowIterator, 3].Value);
                                        bagImportsUK.zone2 = Convert.ToDecimal(workSheet.Cells[rowIterator, 4].Value);
                                        bagImportsUK.zone3 = Convert.ToDecimal(workSheet.Cells[rowIterator, 5].Value);
                                        bagImportsUK.zone4 = Convert.ToDecimal(workSheet.Cells[rowIterator, 6].Value);
                                        bagImportsUK.zone5 = Convert.ToDecimal(workSheet.Cells[rowIterator, 7].Value);
                                        bagImportsUK.zone6 = Convert.ToDecimal(workSheet.Cells[rowIterator, 8].Value);
                                        bagImportsUK.zone7 = Convert.ToDecimal(workSheet.Cells[rowIterator, 9].Value);
                                        bagImportsUK.zone8 = Convert.ToDecimal(workSheet.Cells[rowIterator, 10].Value);
                                        bagImportsUK.zone9 = Convert.ToDecimal(workSheet.Cells[rowIterator, 11].Value);
                                        bagImportsUK.zone10 = Convert.ToDecimal(workSheet.Cells[rowIterator, 12].Value);
                                        bagImportsUK.company = CompanyId;
                                        bagImportsList.Add(bagImportsUK);

                                    }

                                    foreach (var item in bagImportsList)
                                    {
                                        //excelImportDBEntities.bag_imports_uk.Add(item);
                                        _homeRepository.Add_bag_imports_uk(item);
                                    }
                                }
                            }
                        }
                    }
                    else if (model.SpreadSheetId == 1007)
                    {
                        //Insert Data
                        if (Request != null)
                        {
                            if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                            {
                                string fileName = file.FileName;
                                string fileContentType = file.ContentType;
                                byte[] fileBytes = new byte[file.ContentLength];
                                var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
                                using (var package = new ExcelPackage(file.InputStream))
                                {
                                    var currentSheet = package.Workbook.Worksheets;
                                    var workSheet = currentSheet.FirstOrDefault(x => x.Name == "Bag C2C");

                                    if (workSheet == null)
                                    {
                                        TempData[CustomEnums.NotifyType.Error.GetDescription()] = "Worksheet Bag C2C does not exist in the file.";
                                        return RedirectToAction("Index");
                                    }

                                    var noOfCol = workSheet.Dimension.End.Column;
                                    var noOfRow = workSheet.Dimension.End.Row;

                                    List<string> expectedColumns = new List<string>()
                                    { "KG","","Zone A","Zone B","Zone C","Zone D","Zone E","Zone F","Zone G","Zone H","Zone I","Zone J","Zone K","Zone L","Zone M","Zone N","Zone O","Zone P","Zone Q","Zone R","Zone S","Zone T","Zone U","Zone V","Zone W","Zone X","Zone Y","Zone Z","Zone AA","Zone AB","Zone AC","Zone AD","Zone AE","Zone AF","Zone AG","Zone AH","Zone AI","Zone AJ","Zone AK","Zone AL","Zone AM","Zone AN","Zone AO","Zone AP","Zone AQ","Zone AR","Zone AS","Zone AT","Zone AU","Zone AV","Zone AW","Zone AX","Zone AY","Zone AZ","Zone BA","Zone BB","Zone BC","Zone BD","Zone BE","Zone BF","Zone BG","Zone BH","Zone BI","Zone BJ","Zone BK","Zone BL","Zone BM","Zone BN","Zone BO","Zone BP","Zone BQ","Zone BR","Zone BS" };

                                    for (int i = 0; i < expectedColumns.Count; i++)
                                    {
                                        if (Convert.ToString(workSheet.Cells[1, i + 1].Value) != expectedColumns[i])
                                        {
                                            TempData[CustomEnums.NotifyType.Error.GetDescription()] = "Column name missing in sheet";
                                            return RedirectToAction("Index");
                                        }
                                    }


                                    // delete data
                                    //List<bag_c2c> bagC2CList = _dbRepositoryBagC2C.GetEntities().Where(x => x.company == CompanyId).ToList();
                                    //foreach (var item in bagC2CList)
                                    //{
                                    //    _dbRepositoryBagC2C.Delete(item.id);
                                    //}

                                    var result = _homeRepository.DeleteAllData("quotes.bag_c2c", CompanyId, "company");

                                    if (result != -1)
                                    {
                                        var bagC2CList = new List<QuoteCalculator.Service.Models.HomeControllerModel.bag_c2c>();
                                        //row Add in table 
                                        for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                                        {

                                            decimal from, to;
                                            if (!decimal.TryParse(Convert.ToString(workSheet.Cells[rowIterator, 1].Value), out from))
                                                continue;
                                            to = Convert.ToDecimal(workSheet.Cells[rowIterator, 2].Value); ;

                                            for (int j = 0; j < expectedColumns.Count() - 2; j++)
                                            {
                                                var bagC2C = new QuoteCalculator.Service.Models.HomeControllerModel.bag_c2c();
                                                bagC2C.kg_from = from;
                                                bagC2C.kg_to = to;
                                                bagC2C.zone = Convert.ToString(workSheet.Cells[1, j + 3].Value).Replace("Zone ", "");
                                                bagC2C.rate = Convert.ToDecimal(workSheet.Cells[rowIterator, j + 3].Value);
                                                bagC2C.company = CompanyId;
                                                bagC2CList.Add(bagC2C);
                                            }
                                        }

                                        foreach (var item in bagC2CList)
                                        {
                                            //excelImportDBEntities.bag_c2c.Add(item);
                                            _homeRepository.Add_bagC2C(item);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (model.SpreadSheetId == 1008)
                    {
                        //Insert Data
                        if (Request != null)
                        {
                            if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                            {
                                string fileName = file.FileName;
                                string fileContentType = file.ContentType;
                                byte[] fileBytes = new byte[file.ContentLength];
                                var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
                                using (var package = new ExcelPackage(file.InputStream))
                                {
                                    var currentSheet = package.Workbook.Worksheets;
                                    var workSheet = currentSheet.FirstOrDefault(x => x.Name == "Matrix C2C");

                                    if (workSheet == null)
                                    {
                                        TempData[CustomEnums.NotifyType.Error.GetDescription()] = "Worksheet Matrix C2C does not exist in the file.";
                                        return RedirectToAction("Index");
                                    }

                                    var noOfCol = workSheet.Dimension.End.Column;
                                    var noOfRow = workSheet.Dimension.End.Row;

                                    List<string> expectedColumns = new List<string>()
                                            { "1","2","3","4","5","6","7","8","9", "10","11", "12", "13" };

                                    for (int i = 0; i < expectedColumns.Count; i++)
                                    {
                                        if (Convert.ToString(workSheet.Cells[6, i + 3].Value) != expectedColumns[i])
                                        {
                                            TempData[CustomEnums.NotifyType.Error.GetDescription()] = "Column name missing in sheet";
                                            return RedirectToAction("Index");
                                        }
                                    }


                                    // delete data
                                    //List<matrix_c2c> matrixC2CList = _dbRepositoryMatrixC2C.GetEntities().Where(x => x.company == CompanyId).ToList();
                                    //foreach (var item in matrixC2CList)
                                    //{
                                    //    _dbRepositoryMatrixC2C.Delete(item.id);
                                    //}

                                    var result = _homeRepository.DeleteAllData("quotes.matrix_c2c", CompanyId, "company");

                                    var matrixC2CList = new List<QuoteCalculator.Service.Models.HomeControllerModel.matrix_c2c>();
                                    //row Add in table 
                                    for (int rowIterator = 8; rowIterator <= 20; rowIterator++)
                                    {

                                        //if (!decimal.TryParse(Convert.ToString(workSheet.Cells[rowIterator, 1].Value), out decimal from))
                                        //    continue;

                                        for (int j = 1; j <= 13; j++)
                                        {
                                            var matrixC2C = new QuoteCalculator.Service.Models.HomeControllerModel.matrix_c2c();
                                            matrixC2C.origin_zone_no = rowIterator - 7;
                                            matrixC2C.destination_zone_no = j;
                                            matrixC2C.bag_zone_code = Convert.ToString(workSheet.Cells[rowIterator, j + 2].Value);
                                            matrixC2C.company = CompanyId;
                                            matrixC2CList.Add(matrixC2C);
                                        }
                                    }

                                    foreach (var item in matrixC2CList)
                                    {
                                        //excelImportDBEntities.matrix_c2c.Add(item);
                                        _homeRepository.Add_matrixC2C(item);
                                    }
                                    //excelImportDBEntities.SaveChanges();
                                }
                            }
                        }
                    }
                    else if (model.SpreadSheetId == 1009)// Throws An Exception
                    {
                        //Insert Data
                        if (Request != null)
                        {
                            if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                            {

                                //string fileName = file.FileName;
                                //string fileContentType = file.ContentType;
                                //byte[] fileBytes = new byte[file.ContentLength];
                                //var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));

                                string fileContents;
                                using (StreamReader reader = new StreamReader(file.InputStream))
                                {
                                    fileContents = reader.ReadToEnd();
                                }



                                using (var package = new ExcelPackage())
                                {
                                    ExcelWorksheet workSheet = null;
                                    workSheet = package.Workbook.Worksheets.Add("CreditorProducts");
                                    ExcelTextFormat format = new ExcelTextFormat()
                                    {
                                        Delimiter = ',',
                                        TextQualifier = '"'
                                    };
                                    workSheet.Cells[1, 1].LoadFromText(fileContents, format); //This Line throws an exception

                                    //var currentSheet = package.Workbook.Worksheets;
                                    //var workSheet = currentSheet.FirstOrDefault(x => x.Name == "CreditorProducts");

                                    if (workSheet == null)
                                    {
                                        TempData[CustomEnums.NotifyType.Error.GetDescription()] = "Worksheet Creditor Products does not exist in the file.";
                                        return RedirectToAction("Index");
                                    }

                                    var noOfCol = workSheet.Dimension.End.Column;
                                    var noOfRow = workSheet.Dimension.End.Row;

                                    List<string> expectedColumns = new List<string>()
                                    { "Branch","*Creditor Product","*Creditor Code","Method","Removal Type","Service","Int Type","*Rate Fixed","Origin Code","Origin Region","Origin Description/City","Origin State","Origin Country","Origin Distance","Dest Code","Dest Region","Dest Description/City","Dest State","Dest Country","Dest Distance","Currency","Days","Rate Value","Margin","Minimum Cost","Maximum Cost","Base 1","Break 1","Rate 1","Base 2","Break 2","Rate 2","Base 3","Break 3","Rate 3","Base 4","Break 4","Rate 4","Base 5","Break 5","Rate 5","Base 6","Break 6","Rate 6","Base 7","Break 7","Rate 7","Base 8","Break 8","Rate 8","Base 9","Break 9","Rate 9","Base 10","Break 10","Rate 10","Base 11","Break 11","Rate 11","Base 12","Break 12","Rate 12","Base 13","Break 13","Rate 13","Base 14","Break 14","Rate 14","Base 15","Break 15","Rate 15","Base 16","Break 16","Rate 16","Base 17","Break 17","Rate 17","Base 18","Break 18","Rate 18","Base 19","Break 19","Rate 19","Base 20","Break 20","Rate 20","Transit Time (short)","Transit Time (long)","Transit Type","rateid","Comments","Date From","Date To","Time","Ratetype","QtyMin","QtyMax","Internal Comment","Load Type","Creditor Product Status" };

                                    for (int i = 0; i < expectedColumns.Count; i++)
                                    {
                                        if (Convert.ToString(workSheet.Cells[1, i + 1].Value) != expectedColumns[i])
                                        {
                                            TempData[CustomEnums.NotifyType.Error.GetDescription()] = "Column name missing in sheet";
                                            return RedirectToAction("Index");
                                        }
                                    }


                                    // delete data
                                    List<tbl_CreditorProducts> CreditorProductsList = _dbRepositoryCreditorProducts.GetEntities().ToList();
                                    foreach (var item in CreditorProductsList)
                                    {
                                        _dbRepositoryCreditorProducts.Delete(item.Id);
                                    }
                                    CreditorProductsList = new List<tbl_CreditorProducts>();

                                    List<tbl_CreditorProducts_Rate> CreditorProductsRateList = _dbRepositoryCreditorProductsRate.GetEntities().ToList();
                                    foreach (var item in CreditorProductsRateList)
                                    {
                                        _dbRepositoryCreditorProductsRate.Delete(item.Id);
                                    }

                                    CreditorProductsRateList = new List<tbl_CreditorProducts_Rate>();
                                    //row Add in table 
                                    for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                                    {

                                        decimal from, to;
                                        if (!decimal.TryParse(Convert.ToString(workSheet.Cells[rowIterator, 90].Value), out from))
                                            continue;
                                        //to = Convert.ToDecimal(workSheet.Cells[rowIterator, 2].Value); ;

                                        var CreditorProduct = new tbl_CreditorProducts();
                                        CreditorProduct.Branch = workSheet.Cells[rowIterator, 1].Value == null ? string.Empty : workSheet.Cells[rowIterator, 1].Value.ToString();
                                        CreditorProduct.CreditorProduct = workSheet.Cells[rowIterator, 2].Value == null ? string.Empty : workSheet.Cells[rowIterator, 2].Value.ToString();
                                        CreditorProduct.CreditorCode = workSheet.Cells[rowIterator, 3].Value == null ? 0 : Convert.ToInt32(workSheet.Cells[rowIterator, 3].Value);
                                        CreditorProduct.Method = workSheet.Cells[rowIterator, 4].Value == null ? string.Empty : workSheet.Cells[rowIterator, 4].Value.ToString();
                                        CreditorProduct.RemovalType = workSheet.Cells[rowIterator, 5].Value == null ? string.Empty : workSheet.Cells[rowIterator, 5].Value.ToString();
                                        CreditorProduct.Service = workSheet.Cells[rowIterator, 6].Value == null ? string.Empty : workSheet.Cells[rowIterator, 6].Value.ToString();
                                        CreditorProduct.IntType = workSheet.Cells[rowIterator, 7].Value == null ? string.Empty : workSheet.Cells[rowIterator, 7].Value.ToString();
                                        CreditorProduct.RateFixed = workSheet.Cells[rowIterator, 8].Value == null ? string.Empty : workSheet.Cells[rowIterator, 8].Value.ToString();
                                        CreditorProduct.OriginCode = workSheet.Cells[rowIterator, 9].Value == null ? string.Empty : workSheet.Cells[rowIterator, 9].Value.ToString();
                                        CreditorProduct.OriginRegion = workSheet.Cells[rowIterator, 10].Value == null ? string.Empty : workSheet.Cells[rowIterator, 10].Value.ToString();
                                        CreditorProduct.OriginDescriptionCity = workSheet.Cells[rowIterator, 11].Value == null ? string.Empty : workSheet.Cells[rowIterator, 11].Value.ToString();
                                        CreditorProduct.OriginState = workSheet.Cells[rowIterator, 12].Value == null ? string.Empty : workSheet.Cells[rowIterator, 12].Value.ToString();
                                        CreditorProduct.OriginCountry = workSheet.Cells[rowIterator, 13].Value == null ? string.Empty : workSheet.Cells[rowIterator, 13].Value.ToString();
                                        CreditorProduct.OriginDistance = workSheet.Cells[rowIterator, 14].Value == null ? string.Empty : workSheet.Cells[rowIterator, 14].Value.ToString();
                                        CreditorProduct.DestCode = workSheet.Cells[rowIterator, 15].Value == null ? string.Empty : workSheet.Cells[rowIterator, 15].Value.ToString();
                                        CreditorProduct.DestRegion = workSheet.Cells[rowIterator, 16].Value == null ? string.Empty : workSheet.Cells[rowIterator, 16].Value.ToString();
                                        CreditorProduct.DestDescriptionCity = workSheet.Cells[rowIterator, 17].Value == null ? string.Empty : workSheet.Cells[rowIterator, 17].Value.ToString();
                                        CreditorProduct.DestState = workSheet.Cells[rowIterator, 18].Value == null ? string.Empty : workSheet.Cells[rowIterator, 18].Value.ToString();
                                        CreditorProduct.DestCountry = workSheet.Cells[rowIterator, 19].Value == null ? string.Empty : workSheet.Cells[rowIterator, 19].Value.ToString();
                                        CreditorProduct.DestDistance = workSheet.Cells[rowIterator, 20].Value == null ? 0 : Convert.ToInt32(workSheet.Cells[rowIterator, 20].Value);
                                        CreditorProduct.Currency = workSheet.Cells[rowIterator, 21].Value == null ? string.Empty : workSheet.Cells[rowIterator, 21].Value.ToString();
                                        CreditorProduct.Days = workSheet.Cells[rowIterator, 22].Value == null ? 0 : Convert.ToInt32(workSheet.Cells[rowIterator, 22].Value);
                                        CreditorProduct.RateValue = workSheet.Cells[rowIterator, 23].Value == null ? 0 : Convert.ToDecimal(workSheet.Cells[rowIterator, 23].Value);
                                        CreditorProduct.Margin = workSheet.Cells[rowIterator, 24].Value == null ? 0 : Convert.ToInt32(workSheet.Cells[rowIterator, 24].Value);
                                        CreditorProduct.MinimumCost = workSheet.Cells[rowIterator, 25].Value == null ? 0 : Convert.ToDecimal(workSheet.Cells[rowIterator, 25].Value);
                                        CreditorProduct.MaximumCost = workSheet.Cells[rowIterator, 26].Value == null ? 0 : Convert.ToDecimal(workSheet.Cells[rowIterator, 26].Value);
                                        CreditorProduct.TransitTimeshort = workSheet.Cells[rowIterator, 87].Value == null ? string.Empty : workSheet.Cells[rowIterator, 87].Value.ToString();
                                        CreditorProduct.TransitTimelong = workSheet.Cells[rowIterator, 88].Value == null ? string.Empty : workSheet.Cells[rowIterator, 88].Value.ToString();
                                        CreditorProduct.TransitType = workSheet.Cells[rowIterator, 89].Value == null ? string.Empty : workSheet.Cells[rowIterator, 89].Value.ToString();
                                        CreditorProduct.rateid = workSheet.Cells[rowIterator, 90].Value == null ? 0 : Convert.ToInt32(workSheet.Cells[rowIterator, 90].Value);
                                        CreditorProduct.Comments = workSheet.Cells[rowIterator, 91].Value == null ? string.Empty : workSheet.Cells[rowIterator, 91].Value.ToString();
                                        CreditorProduct.DateFrom = workSheet.Cells[rowIterator, 92].Value == null ? (DateTime?)null : DateTime.ParseExact(workSheet.Cells[rowIterator, 92].Value.ToString(), "dd/MM/yy", null);
                                        CreditorProduct.DateTo = workSheet.Cells[rowIterator, 93].Value == null ? (DateTime?)null : DateTime.ParseExact(workSheet.Cells[rowIterator, 93].Value.ToString(), "dd/MM/yy", null);
                                        CreditorProduct.Time = workSheet.Cells[rowIterator, 94].Value == null ? string.Empty : workSheet.Cells[rowIterator, 94].Value.ToString();
                                        CreditorProduct.Ratetype = workSheet.Cells[rowIterator, 95].Value == null ? string.Empty : workSheet.Cells[rowIterator, 95].Value.ToString();
                                        CreditorProduct.QtyMin = workSheet.Cells[rowIterator, 96].Value == null ? 0 : Convert.ToInt32(workSheet.Cells[rowIterator, 96].Value);
                                        CreditorProduct.QtyMax = workSheet.Cells[rowIterator, 97].Value == null ? 0 : Convert.ToInt32(workSheet.Cells[rowIterator, 97].Value);
                                        CreditorProduct.InternalComment = workSheet.Cells[rowIterator, 98].Value == null ? string.Empty : workSheet.Cells[rowIterator, 98].Value.ToString();
                                        CreditorProduct.LoadType = workSheet.Cells[rowIterator, 99].Value == null ? string.Empty : workSheet.Cells[rowIterator, 99].Value.ToString();
                                        CreditorProduct.CreditorProductStatus = workSheet.Cells[rowIterator, 100].Value == null ? string.Empty : workSheet.Cells[rowIterator, 100].Value.ToString();


                                        CreditorProductsList.Add(CreditorProduct);

                                        AddCreditRate(CreditorProductsRateList, workSheet, rowIterator, 27, 90);
                                        AddCreditRate(CreditorProductsRateList, workSheet, rowIterator, 30, 90);
                                        AddCreditRate(CreditorProductsRateList, workSheet, rowIterator, 33, 90);
                                        AddCreditRate(CreditorProductsRateList, workSheet, rowIterator, 36, 90);
                                        AddCreditRate(CreditorProductsRateList, workSheet, rowIterator, 39, 90);
                                        AddCreditRate(CreditorProductsRateList, workSheet, rowIterator, 42, 90);
                                        AddCreditRate(CreditorProductsRateList, workSheet, rowIterator, 45, 90);
                                        AddCreditRate(CreditorProductsRateList, workSheet, rowIterator, 48, 90);
                                        AddCreditRate(CreditorProductsRateList, workSheet, rowIterator, 51, 90);
                                        AddCreditRate(CreditorProductsRateList, workSheet, rowIterator, 54, 90);
                                        AddCreditRate(CreditorProductsRateList, workSheet, rowIterator, 57, 90);
                                        AddCreditRate(CreditorProductsRateList, workSheet, rowIterator, 60, 90);
                                        AddCreditRate(CreditorProductsRateList, workSheet, rowIterator, 63, 90);
                                        AddCreditRate(CreditorProductsRateList, workSheet, rowIterator, 66, 90);
                                        AddCreditRate(CreditorProductsRateList, workSheet, rowIterator, 69, 90);

                                        AddCreditRate(CreditorProductsRateList, workSheet, rowIterator, 72, 90);
                                        AddCreditRate(CreditorProductsRateList, workSheet, rowIterator, 75, 90);
                                        AddCreditRate(CreditorProductsRateList, workSheet, rowIterator, 78, 90);
                                        AddCreditRate(CreditorProductsRateList, workSheet, rowIterator, 81, 90);
                                        AddCreditRate(CreditorProductsRateList, workSheet, rowIterator, 84, 90);

                                    }

                                    foreach (var item in CreditorProductsList)
                                    {
                                        excelImportDBEntities.tbl_CreditorProducts.Add(item);
                                    }

                                    foreach (var item in CreditorProductsRateList)
                                    {
                                        excelImportDBEntities.tbl_CreditorProducts_Rate.Add(item);
                                    }
                                    excelImportDBEntities.SaveChanges();
                                }
                            }
                        }
                    }
                    else if (model.SpreadSheetId == 1010) // Column is missing in the sheet
                    {
                        //Insert Data
                        if (Request != null)
                        {
                            if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                            {
                                string fileName = file.FileName;
                                string fileContentType = file.ContentType;
                                byte[] fileBytes = new byte[file.ContentLength];
                                var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));

                                using (var package = new ExcelPackage(file.InputStream))
                                {
                                    var workSheet = package.Workbook.Worksheets.First();

                                    if (workSheet == null)
                                    {
                                        TempData[CustomEnums.NotifyType.Error.GetDescription()] = "Worksheet does not exist in the file.";
                                        return RedirectToAction("Index");
                                    }

                                    var noOfCol = workSheet.Dimension.End.Column;
                                    var noOfRow = workSheet.Dimension.End.Row;

                                    List<string> expectedColumns = new List<string>()
                                    { "Base","Currency","Rate"};

                                    for (int i = 0; i < expectedColumns.Count; i++) //The Columns are not as expected, Maybe because of the sheet
                                    {
                                        if (Convert.ToString(workSheet.Cells[1, i + 1].Value) != expectedColumns[i])
                                        {
                                            TempData[CustomEnums.NotifyType.Error.GetDescription()] = "Column name missing in sheet";
                                            return RedirectToAction("Index");
                                        }
                                    }

                                    // delete data
                                    List<currency_rate> currencyRateList = _dbRepositoryCurrencyRate.GetEntities().ToList();
                                    if (currencyRateList.Count > 0)
                                    {
                                        foreach (var item in currencyRateList)
                                        {
                                            _dbRepositoryCurrencyRate.Delete(item.id);
                                        }
                                    }
                                    currencyRateList = new List<currency_rate>();

                                    //row Add in table 
                                    for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                                    {
                                        var currencyRate = new currency_rate();

                                        currencyRate.@base = workSheet.Cells[rowIterator, 1].Value == null ? string.Empty : workSheet.Cells[rowIterator, 1].Value.ToString();
                                        currencyRate.currency = workSheet.Cells[rowIterator, 2].Value == null ? string.Empty : workSheet.Cells[rowIterator, 2].Value.ToString();
                                        currencyRate.rate = Convert.ToDecimal(workSheet.Cells[rowIterator, 3].Value);

                                        currencyRateList.Add(currencyRate);
                                    }

                                    foreach (var item in currencyRateList)
                                    {
                                        excelImportDBEntities.currency_rate.Add(item);
                                    }
                                    excelImportDBEntities.SaveChanges();
                                }
                            }
                        }
                    }
                    else if (model.SpreadSheetId == 1011) //TODO
                    {
                        return ImportSailingSchedule(file, "Nationwide");
                    }
                    else if (model.SpreadSheetId == 1017)
                    {
                        return ImportSailingSchedule(file, "London");
                    }
                    else if (model.SpreadSheetId == 1018) //TODO
                    {
                        return ImportSailingSchedule(file, "Manchester");
                    }
                    else if (model.SpreadSheetId == 1019) //TODO
                    {
                        return ImportSailingSchedule(file, "Glasgow");
                    }

                    else if (model.SpreadSheetId == 1014) //Branch Postcode
                    {
                        ImportFileResult result = FileImportManager.Import(model.SpreadSheetId, file);
                        TempData[result.NotificationDescription] = result.ReturnMessage;
                        return RedirectToAction("Index");
                    }
                    else if (model.SpreadSheetId == 1015)
                    {
                        if (Request != null)
                        {
                            if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                            {
                                string fileName = file.FileName;
                                string fileContentType = file.ContentType;
                                byte[] fileBytes = new byte[file.ContentLength];
                                var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
                                using (var package = new ExcelPackage(file.InputStream))
                                {
                                    var currentSheet = package.Workbook.Worksheets;
                                    var workSheet = currentSheet.First();
                                    foreach (var item in currentSheet)
                                    {
                                        var sheetName = item;
                                        var noOfCol = 0;
                                        var noOfRow = 0;
                                        //List<road_rates> roadRates = _dbRepositoryRoadRates.GetEntities().Where(x => x.company == CompanyId).ToList();
                                        if (sheetName.Name.Replace("-", "") == spreadSheetObj.Name)
                                        {
                                            workSheet = item;
                                            noOfCol = workSheet.Dimension.End.Column;
                                            noOfRow = workSheet.Dimension.End.Row;

                                            if (spreadSheetObj.Name == "RatesRoad")
                                            {
                                                //if (roadRates.Count > 0)
                                                //{
                                                //    foreach (var rates in roadRates)
                                                //    {
                                                //        _dbRepositoryRoadRates.Delete(rates.id);
                                                //    }
                                                //}
                                                var result = _homeRepository.DeleteAllData("quotes.road_rates", CompanyId, "company");
                                                if (result != -1)
                                                {
                                                    var roadRateList = new List<QuoteCalculator.Service.Models.HomeControllerModel.road_rates>();
                                                    for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                                                    {
                                                        var rates = new QuoteCalculator.Service.Models.HomeControllerModel.road_rates();
                                                        var countryCode = workSheet.Cells[rowIterator, 1].Value;
                                                        var city = workSheet.Cells[rowIterator, 3].Value;
                                                        if (countryCode != null && city != null)
                                                        {
                                                            var rateDestinationObj = _homeRepository.RatesDestByCountryCode(countryCode.ToString(), city.ToString(), CompanyId);
                                                            //var rateDestinationObj = _dbRepositoryCountry.GetEntities().Where(m => m.country_code == countryCode.ToString() && m.city == city.ToString() && m.CompanyId == CompanyId).FirstOrDefault();
                                                            if (rateDestinationObj != null)
                                                            {
                                                                rates.rates_destinations_id = rateDestinationObj.id;
                                                                rates.cost_3_cuft = workSheet.Cells[rowIterator, 5].Value != null ? Convert.ToDecimal(workSheet.Cells[rowIterator, 5].Value) : 0;
                                                                rates.cost_6_cuft = workSheet.Cells[rowIterator, 7].Value != null ? Convert.ToDecimal(workSheet.Cells[rowIterator, 7].Value) : 0;
                                                                rates.cost_9_cuft = workSheet.Cells[rowIterator, 9].Value != null ? Convert.ToDecimal(workSheet.Cells[rowIterator, 9].Value) : 0;
                                                                rates.cost_12_cuft = workSheet.Cells[rowIterator, 11].Value != null ? Convert.ToDecimal(workSheet.Cells[rowIterator, 11].Value) : 0;
                                                                rates.cost_15_cuft = workSheet.Cells[rowIterator, 13].Value != null ? Convert.ToDecimal(workSheet.Cells[rowIterator, 13].Value) : 0;
                                                                rates.cost_18_cuft = workSheet.Cells[rowIterator, 15].Value != null ? Convert.ToDecimal(workSheet.Cells[rowIterator, 15].Value) : 0;
                                                                rates.cost_21_cuft = workSheet.Cells[rowIterator, 17].Value != null ? Convert.ToDecimal(workSheet.Cells[rowIterator, 17].Value) : 0;
                                                                rates.cost_24_cuft = workSheet.Cells[rowIterator, 19].Value != null ? Convert.ToDecimal(workSheet.Cells[rowIterator, 19].Value) : 0;
                                                                rates.cost_27_cuft = workSheet.Cells[rowIterator, 21].Value != null ? Convert.ToDecimal(workSheet.Cells[rowIterator, 21].Value) : 0;
                                                                rates.cost_30_cuft = workSheet.Cells[rowIterator, 23].Value != null ? Convert.ToDecimal(workSheet.Cells[rowIterator, 23].Value) : 0;
                                                                rates.cost_33_cuft = workSheet.Cells[rowIterator, 25].Value != null ? Convert.ToDecimal(workSheet.Cells[rowIterator, 25].Value) : 0;
                                                                rates.cost_36_cuft = workSheet.Cells[rowIterator, 27].Value != null ? Convert.ToDecimal(workSheet.Cells[rowIterator, 27].Value) : 0;
                                                                rates.cost_39_cuft = workSheet.Cells[rowIterator, 29].Value != null ? Convert.ToDecimal(workSheet.Cells[rowIterator, 29].Value) : 0;
                                                                rates.cost_42_cuft = workSheet.Cells[rowIterator, 31].Value != null ? Convert.ToDecimal(workSheet.Cells[rowIterator, 31].Value) : 0;
                                                                rates.cost_45_cuft = workSheet.Cells[rowIterator, 33].Value != null ? Convert.ToDecimal(workSheet.Cells[rowIterator, 33].Value) : 0;
                                                                rates.cost_68_cuft = workSheet.Cells[rowIterator, 35].Value != null ? Convert.ToDecimal(workSheet.Cells[rowIterator, 35].Value) : 0;
                                                                rates.cost_90_cuft = workSheet.Cells[rowIterator, 37].Value != null ? Convert.ToDecimal(workSheet.Cells[rowIterator, 37].Value) : 0;
                                                                rates.cost_100_cuft = workSheet.Cells[rowIterator, 39].Value != null ? Convert.ToDecimal(workSheet.Cells[rowIterator, 39].Value) : 0;
                                                                rates.company = CompanyId;
                                                                roadRateList.Add(rates);
                                                            }
                                                        }
                                                    }
                                                    foreach (var obj in roadRateList)
                                                    {
                                                        //excelImportDBEntities.road_rates.Add(obj);
                                                        _homeRepository.Add_roadRates(obj);
                                                    }
                                                    //excelImportDBEntities.SaveChanges();
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (model.SpreadSheetId == 1016)
                    {
                        FileImportManager.Import(model.SpreadSheetId, file);
                    }
                    else if (model.SpreadSheetId == 1020 || model.SpreadSheetId == 1021 || model.SpreadSheetId == 1022 || model.SpreadSheetId == 1023 || model.SpreadSheetId == 1025 || model.SpreadSheetId == 1026)
                    {
                        var response = FileImportManager.Import(spreadSheetObj.Id, file);
                        if (!response.IsSuccess)
                        {
                            TempData[CustomEnums.NotifyType.Error.GetDescription()] = response.ReturnMessage;
                            return RedirectToAction("Index");
                        }
                    }


                    /******* Quote Cost Files *******/
                    else if (model.SpreadSheetId == 1027 || model.SpreadSheetId == 1028 || model.SpreadSheetId == 1029 || model.SpreadSheetId == 1030 || model.SpreadSheetId == 1031)
                    {
                        var response = FileImportManager.Import(model.SpreadSheetId, file);
                        if (!response.IsSuccess)
                        {
                            TempData[CustomEnums.NotifyType.Error.GetDescription()] = response.ReturnMessage;
                            return RedirectToAction("Index");
                        }
                    }
                    /******* Quote Cost Files *******/

                    else
                    {
                        if (Request != null)
                        {
                            if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                            {
                                string fileName = file.FileName;
                                string fileContentType = file.ContentType;
                                byte[] fileBytes = new byte[file.ContentLength];
                                var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
                                using (var package = new ExcelPackage(file.InputStream))
                                {
                                    var currentSheet = package.Workbook.Worksheets;
                                    var workSheet = currentSheet.First();
                                    foreach (var item in currentSheet)
                                    {
                                        var sheetName = item;
                                        var noOfCol = 0;
                                        var noOfRow = 0;
                                        //List<air_rates> airRates = _dbRepositoryAirRates.GetEntities().Where(x => x.company == CompanyId).ToList();
                                        if (sheetName.Name.Replace("-", "") == spreadSheetObj.Name)
                                        {
                                            workSheet = item;
                                            noOfCol = workSheet.Dimension.End.Column;
                                            noOfRow = workSheet.Dimension.End.Row;

                                            if (spreadSheetObj.Name == "RatesAir")
                                            {
                                                //List<string> selectedColumnName = new List<string>();
                                                //foreach (ExcelWorksheet sheet in currentSheet)
                                                //{
                                                //    if (sheet.Name.Replace("-", "").ToString() == "RatesAir")
                                                //    {
                                                //        for (int i = 1; i <= noOfCol; i++)
                                                //        {
                                                //            if (sheet.Cells[1, i].Value != null)
                                                //            {
                                                //                var colname = sheet.Cells[1, i].Value.ToString();
                                                //                if (colname != "ISO" && colname != "MW/ISO Code" && colname != "Country" && colname != "City")
                                                //                {
                                                //                    selectedColumnName.Add(sheet.Cells[1, i].Value.ToString());
                                                //                }
                                                //            }
                                                //        }
                                                //    }
                                                //}
                                                //List<string> dbColumnName = new List<string>();
                                                //var properties = typeof(air_rates).GetProperties();
                                                //foreach (var itemName in properties)
                                                //{
                                                //    if (!itemName.Name.Contains("tbl_"))
                                                //    {
                                                //        dbColumnName.Add(itemName.Name);
                                                //    }
                                                //}
                                                //if (dbColumnName.Count() - 2 != selectedColumnName.Count())
                                                //{
                                                //    TempData[CustomEnums.NotifyType.Error.GetDescription()] = "Column name missing in sheet";
                                                //    return RedirectToAction("Index");
                                                //}
                                                //for (int i = 0; i < selectedColumnName.Count(); i++)
                                                //{
                                                //    var selectedcol = string.Empty;
                                                //    if (selectedColumnName[i].Contains('('))
                                                //    {
                                                //        selectedColumnName[i] = selectedColumnName[i].Split('(')[0];
                                                //        if (selectedColumnName[i].Contains(')'))
                                                //        {
                                                //            selectedColumnName[i] = selectedColumnName[i].Split('(')[1];
                                                //            selectedcol = selectedColumnName[i];
                                                //        }
                                                //    }
                                                //    else
                                                //    {
                                                //        selectedcol = selectedColumnName[i];
                                                //    }
                                                //    var colexists = false;
                                                //    for (int j = 2; j < dbColumnName.Count(); j++)
                                                //    {
                                                //        if (dbColumnName[j].Replace("?", "").Replace("_", "").ToLower().Contains(selectedcol.Replace("?", "").Replace("_", "").Replace(" ", "").ToLower()))
                                                //        {
                                                //            colexists = true;
                                                //        }
                                                //    }
                                                //    if (!colexists)
                                                //    {
                                                //        TempData[CustomEnums.NotifyType.Error.GetDescription()] = "Column name does not match";
                                                //        return RedirectToAction("Index");
                                                //    }
                                                //}

                                                //if (airRates.Count > 0)
                                                //{
                                                //    foreach (var rates in airRates)
                                                //    {
                                                //        _dbRepositoryAirRates.Delete(rates.id);
                                                //    }
                                                //}
                                                _homeRepository.DeleteAllData("quotes.air_rates", CompanyId ,"company");

                                                var airRateList = new List<air_rates>();
                                                for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                                                {
                                                    var rates = new air_rates();
                                                    var countryCode = workSheet.Cells[rowIterator, 1].Value;
                                                    var city = workSheet.Cells[rowIterator, 4].Value;
                                                    if (countryCode != null && city != null)
                                                    {
                                                        var rateDestinationObj = _dbRepositoryCountry.GetEntities().Where(m => m.country_code == countryCode.ToString() && m.city == city.ToString() && m.CompanyId == CompanyId).FirstOrDefault();
                                                        if (rateDestinationObj != null)
                                                        {
                                                            rates.rates_destinations_id = rateDestinationObj.id;
                                                            rates.airMin = workSheet.Cells[rowIterator, 5].Value != null ? Convert.ToDecimal(workSheet.Cells[rowIterator, 5].Value) : 0;
                                                            rates.airlt50kg = workSheet.Cells[rowIterator, 7].Value != null ? Convert.ToDecimal(workSheet.Cells[rowIterator, 7].Value) : 0;
                                                            rates.airlt100kg = workSheet.Cells[rowIterator, 9].Value != null ? Convert.ToDecimal(workSheet.Cells[rowIterator, 9].Value) : 0;
                                                            rates.airlt150kg = workSheet.Cells[rowIterator, 11].Value != null ? Convert.ToDecimal(workSheet.Cells[rowIterator, 11].Value) : 0;
                                                            rates.airlt200kg = workSheet.Cells[rowIterator, 13].Value != null ? Convert.ToDecimal(workSheet.Cells[rowIterator, 13].Value) : 0;
                                                            rates.airlt300kg = workSheet.Cells[rowIterator, 15].Value != null ? Convert.ToDecimal(workSheet.Cells[rowIterator, 15].Value) : 0;
                                                            rates.airover300kg = workSheet.Cells[rowIterator, 17].Value != null ? Convert.ToDecimal(workSheet.Cells[rowIterator, 17].Value) : 0;
                                                            rates.doorMin = workSheet.Cells[rowIterator, 18].Value != null ? Convert.ToDecimal(workSheet.Cells[rowIterator, 18].Value) : 0;
                                                            rates.door25plus = workSheet.Cells[rowIterator, 20].Value != null ? Convert.ToDecimal(workSheet.Cells[rowIterator, 20].Value) : 0;
                                                            rates.door50plus = workSheet.Cells[rowIterator, 22].Value != null ? Convert.ToDecimal(workSheet.Cells[rowIterator, 22].Value) : 0;
                                                            rates.door100plus = workSheet.Cells[rowIterator, 24].Value != null ? Convert.ToDecimal(workSheet.Cells[rowIterator, 24].Value) : 0;
                                                            rates.door150plus = workSheet.Cells[rowIterator, 26].Value != null ? Convert.ToDecimal(workSheet.Cells[rowIterator, 26].Value) : 0;
                                                            rates.door200plus = workSheet.Cells[rowIterator, 28].Value != null ? Convert.ToDecimal(workSheet.Cells[rowIterator, 28].Value) : 0;
                                                            rates.door300plus = workSheet.Cells[rowIterator, 30].Value != null ? Convert.ToDecimal(workSheet.Cells[rowIterator, 30].Value) : 0;
                                                            rates.handling = workSheet.Cells[rowIterator, 31].Value != null ? Convert.ToDecimal(workSheet.Cells[rowIterator, 31].Value) : 0;
                                                            rates.company = CompanyId;
                                                            airRateList.Add(rates);
                                                        }
                                                    }
                                                }
                                                foreach (var obj in airRateList)
                                                {
                                                    excelImportDBEntities.air_rates.Add(obj);
                                                }
                                                excelImportDBEntities.SaveChanges();
                                            }

                                            if (spreadSheetObj.Name == "RatesCourier")
                                            {
                                                FileImportManager.Import(spreadSheetObj.Id, file);
                                            }
                                            if (spreadSheetObj.Name == "RatesSea")
                                            {
                                      //List<sea_rates> seaRateList = _homeRepository.GetSeaRateByCompany(CompanyId); _dbRepositorySeaRate.GetEntities().Where(x => x.company == CompanyId).ToList();
                                                workSheet = item;
                                                noOfCol = workSheet.Dimension.End.Column;
                                                noOfRow = workSheet.Dimension.End.Row;

                                                //if (seaRateList.Count > 0)
                                                //{
                                                //    foreach (var rates in seaRateList)
                                                //    {
                                                //        //_dbRepositorySeaRate.Delete(rates.id);
                                                //    }
                                                //}
                                                _homeRepository.DeleteAllData("quotes.sea_rates", CompanyId, "company");
                                                var seaList = new List<sea_rates>();
                                                for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                                                {
                                                    var seaRates = new sea_rates();
                                                    seaRates.zone = Convert.ToInt64(workSheet.Cells[rowIterator, 1].Value);
                                                    seaRates.Destination = workSheet.Cells[rowIterator, 2].Value != null ? workSheet.Cells[rowIterator, 2].Value.ToString() : string.Empty;
                                                    seaRates.to25ls = Convert.ToDecimal(workSheet.Cells[rowIterator, 3].Value);
                                                    seaRates.to25rate = Convert.ToDecimal(workSheet.Cells[rowIterator, 4].Value);
                                                    seaRates.over25ls = Convert.ToDecimal(workSheet.Cells[rowIterator, 5].Value);
                                                    seaRates.over25rate = Convert.ToDecimal(workSheet.Cells[rowIterator, 6].Value);
                                                    seaRates.over50ls = Convert.ToDecimal(workSheet.Cells[rowIterator, 7].Value);
                                                    seaRates.over50rate = Convert.ToDecimal(workSheet.Cells[rowIterator, 8].Value);
                                                    seaRates.min = Convert.ToDecimal(workSheet.Cells[rowIterator, 9].Value);
                                                    seaRates.company = CompanyId;
                                                    seaList.Add(seaRates);
                                                }

                                                foreach (var obj in seaList)
                                                {
                                                    excelImportDBEntities.sea_rates.Add(obj);
                                                }
                                                excelImportDBEntities.SaveChanges();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    TempData[CustomEnums.NotifyType.Success.GetDescription()] = "Data is Succesfully Enter in Database. ";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData[CustomEnums.NotifyType.Error.GetDescription()] = "Attach File is Required.Please Upload any one file";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData[CustomEnums.NotifyType.Error.GetDescription()] = "Error is" + ex.Message.ToString();
                return RedirectToAction("Index");
            }
        }

        private void AddCreditRate(List<tbl_CreditorProducts_Rate> rates, ExcelWorksheet workSheet, int rowNo, int colNo, int rateIdCoNo)
        {
            try
            {
                var creditorProductRate = new tbl_CreditorProducts_Rate();
                creditorProductRate.Base = workSheet.Cells[rowNo, colNo].Value == null ? 0 : Convert.ToDecimal(workSheet.Cells[rowNo, colNo].Value);
                creditorProductRate.Break = workSheet.Cells[rowNo, colNo + 1].Value == null ? 0 : Convert.ToDecimal(workSheet.Cells[rowNo, colNo + 1].Value);
                creditorProductRate.Rate = workSheet.Cells[rowNo, colNo + 2].Value == null ? 0 : Convert.ToDecimal(workSheet.Cells[rowNo, colNo + 2].Value);
                creditorProductRate.rateid = workSheet.Cells[rowNo, rateIdCoNo].Value == null ? 0 : Convert.ToInt32(workSheet.Cells[rowNo, rateIdCoNo].Value);

                if (creditorProductRate.Base != 0 || creditorProductRate.Break != 0 || creditorProductRate.Rate != 0)
                    rates.Add(creditorProductRate);
            }
            catch (Exception ex)
            {
                TempData[CustomEnums.NotifyType.Error.GetDescription()] = "Error is" + ex.Message.ToString();
            }
        }

        private ActionResult ImportSailingSchedule(HttpPostedFileBase file, string location) //Doesn't get the expected Column
        {
            //Insert Data
            try
            {
                var sailingList = new List<sailingsched>();
                if (Request != null)
                {
                    if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                    {
                        byte[] fileBytes = new byte[file.ContentLength];
                        var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
                        using (var package = new ExcelPackage(file.InputStream))
                        {
                            var workSheet = package.Workbook.Worksheets.First();
                            var noOfCol = workSheet.Dimension.End.Column;
                            var noOfRow = workSheet.Dimension.End.Row;

                            List<string> expectedColumns = new List<string>() { "PORT", "VESSEL", "Closing", "ETA", "DESTINATION_AGENT", "COUNTRY", "AGENT_URL" };

                            if (noOfCol < expectedColumns.Count)
                            {
                                TempData[CustomEnums.NotifyType.Error.GetDescription()] = "The number of columns [" + noOfCol + "] in the file does not match with expected columns [" + expectedColumns.Count + "].";
                                return RedirectToAction("Index");
                            }

                            for (int i = 1; i <= expectedColumns.Count; i++)
                            {
                                if (string.Compare(Convert.ToString(workSheet.Cells[1, i].Value), expectedColumns[i - 1], true) != 0)
                                {
                                    TempData[CustomEnums.NotifyType.Error.GetDescription()] = "The column [" + workSheet.Cells[1, i].Value + "] does not match with expected column name: [" + expectedColumns[i - 1] + "].";
                                    return RedirectToAction("Index");
                                }
                            }

                            // delete data
                            List<sailingsched> sailingscheds = _dbRepositorySailingSched.GetEntities().Where(x => x.location == location).ToList();
                            foreach (var item in sailingscheds)
                            {
                                _dbRepositorySailingSched.Delete(item.id);
                            }
                            sailingscheds.Clear();

                            //row Add in table 
                            for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                            {
                                var sailingSched = new sailingsched
                                {
                                    location = location,
                                    port = GetCellValueStr(workSheet, rowIterator, 1),
                                    vessel = GetCellValueStr(workSheet, rowIterator, 2),
                                    loading = GetCellValueStr(workSheet, rowIterator, 3, format: "MMM dd, yyyy"),
                                    eta = GetCellValueStr(workSheet, rowIterator, 4, format: "MMM dd, yyyy"),
                                    agent = GetCellValueStr(workSheet, rowIterator, 5),
                                    country = GetCellValueStr(workSheet, rowIterator, 6),
                                    url = GetCellValueStr(workSheet, rowIterator, 7),
                                    lastupdate = DateTime.Now
                                };
                                if (string.IsNullOrEmpty(sailingSched.port))
                                    continue;

                                sailingscheds.Add(sailingSched);
                            }

                            foreach (var item in sailingscheds)
                            {
                                excelImportDBEntities.sailingsched.Add(item);
                            }
                            excelImportDBEntities.SaveChanges();
                        }
                    }
                }

                TempData[CustomEnums.NotifyType.Success.GetDescription()] = "Data is Succesfully Enter in Database. ";
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                TempData[CustomEnums.NotifyType.Error.GetDescription()] = CommonHelper.GetErrorMessage(ex);
            }
            return RedirectToAction("Index");
        }

        private string GetCellValueStr(ExcelWorksheet sheet, int rowNo, int colNo, bool emptyIfNull = true, string format = null)
        {
            if (sheet == null)
                return null;

            if (sheet.Cells[rowNo, 1].Value == null)
                return emptyIfNull ? string.Empty : null;
            if (sheet.Cells[rowNo, colNo].Value.GetType().Name == "DateTime" && !string.IsNullOrEmpty(format))
                return ((DateTime)sheet.Cells[rowNo, colNo].Value).ToString(format);

            return Convert.ToString(sheet.Cells[rowNo, colNo].Value);
        }


    }
}