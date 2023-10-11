using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QuoteCalculatorAdmin.BL.FileImport;
using OfficeOpenXml;
using QuoteCalculatorAdmin.Data;
using QuoteCalculatorAdmin.Data.Repository;
using QuoteCalculatorAdmin.Common;
using NLog;
using QuoteCalculatorAdmin.Helper;
using System.Data;
using System.Data.SqlClient;

namespace QuoteCalculatorAdmin.BL
{

    public class FileImportManager
    {
        private static NLog.Logger logger = LogManager.GetCurrentClassLogger();

        public static ImportFileResult Import(int excelSheetId, HttpPostedFileBase file)
        {
            ImportFileTypes importType = (ImportFileTypes)excelSheetId;

            if (importType == ImportFileTypes.RatesCourier)
                return ImportCourierRate(file);
            if (importType == ImportFileTypes.ExpressCourierRates)
                return ImportExpressCourierRate(file);
            if (importType == ImportFileTypes.BranchPostcode)
                return ImportBranchPostCodeExcel(file);
            if (importType == ImportFileTypes.CountryCodes_UK)
                return ImportCountryCodesUKExcel(file);
            if (importType == ImportFileTypes.FCL)
                return ImportFCLExcel(file);
            if (importType == ImportFileTypes.AIR)
                return ImportAIRExcel(file);
            if (importType == ImportFileTypes.GRP)
                return ImportGRPExcel(file);
            if (importType == ImportFileTypes.TradeRegion)
                return ImportTradeRegionExcel(file);
            if (importType == ImportFileTypes.TradeRates)
                return ImportTradeRatesExcel(file);

            if (importType == ImportFileTypes.ExpressCourierCost)
                return ImportExpressCourierCost(file);
            if (importType == ImportFileTypes.BagC2CCost)
                return ImportBagC2CCost(file);
            if (importType == ImportFileTypes.BagImportsUKCost)
                return ImportBagImportsUKCost(file);
            if (importType == ImportFileTypes.SeaFreightCost)
                return ImportSeaFreightCost(file);
            if (importType == ImportFileTypes.CostsCourierEconomy)
                return ImportCostsCourierEconomy(file);
            return null;
        }

        public static ImportFileResult ImportCourierRate(HttpPostedFileBase file)
        {
            logger.Info("**********Courier Rate File Import Start *************");

            int companyId = SessionHelper.CompanyId;

            logger.Info("Courier Rate - Validating Excel...");
            ImportFileResult result = ExcelHelper.ValidateExcelFile(file, "Rates-Courier");

            if (result.IsSuccess == false)
                return result;

            using (var package = new ExcelPackage(file.InputStream))
            {
                var rateSheet = package.Workbook.Worksheets.First(x => x.Name == "Rates-Courier");

                logger.Info("Courier Rate - Validating Column Headers...");
                ExcelHelper.ValidateColumnHeaders(rateSheet, (new[] { "Kgs" }).ToList());

                List<int> zoneNos = new List<int>();

                for (int j = 2; j <= rateSheet.Dimension.End.Column; j++)
                {
                    //check if first 5 charactors "Zone "
                    string colName = ExcelHelper.GetStr(rateSheet, 1, j);

                    if (!colName.StartsWith("Zone ") || !int.TryParse(colName.Split(' ')[1], out int zoneNo))
                        return new ImportFileResult(false, "Invalid Column Name. Col Index: " + j);
                    zoneNos.Add(zoneNo);
                }

                var courierList = new List<courier_rates>();
                for (int rowIterator = 2; rowIterator <= rateSheet.Dimension.End.Row; rowIterator++)
                {
                    for (int j = 0; j < zoneNos.Count; j++)
                    {
                        if (ExcelHelper.HasDecimal(rateSheet, rowIterator, 1))
                        {
                            decimal w = ExcelHelper.GetDecimal(rateSheet, rowIterator, 1);
                            decimal r = ExcelHelper.GetDecimal(rateSheet, rowIterator, j + 2);
                            courierList.Add(new courier_rates()
                            { weight = w, zone = zoneNos[j], rate = r, company = companyId });
                        }
                    }
                }

                using (quotesEntities db = new quotesEntities())
                {
                    logger.Info("Courier Rate Import - removing existing rates...");
                    db.DeleteCourierRates(companyId);

                    logger.Info("Courier Rate Import - Importing new rates. Total entries: " + courierList.Count);
                    db.courier_rates.AddRange(courierList);
                    db.SaveChanges();
                }

                logger.Info("Courier Rate Import - " + courierList.Count + " entries imported successfully.");
                return new ImportFileResult(true, courierList.Count + " entries imported successfully.", null, courierList.Count);
            }
        }


        public static ImportFileResult ImportExpressCourierRate(HttpPostedFileBase file)
        {
            logger.Info("**********Express Courier Rate File Import Start *************");

            int companyId = SessionHelper.CompanyId;

            logger.Info("Express Courier Rate - Validating Excel...");
            ImportFileResult result = ExcelHelper.ValidateExcelFile(file, "Rates-CourierExpress");

            if (result.IsSuccess == false)
                return result;

            using (var package = new ExcelPackage(file.InputStream))
            {
                var rateSheet = package.Workbook.Worksheets.First(x => x.Name == "Rates-CourierExpress");

                logger.Info("Express Courier Rate - Validating Column Headers...");
                ExcelHelper.ValidateColumnHeaders(rateSheet, (new[] { "Kgs" }).ToList());

                List<int> zoneNos = GetCourierZones(rateSheet);

                var courierList = new List<express_courier_rates>();

                for (int rowIterator = 2; rowIterator <= rateSheet.Dimension.End.Row; rowIterator++)
                {
                    for (int j = 0; j < zoneNos.Count; j++)
                    {
                        if (Convert.ToDecimal(rateSheet.Cells[rowIterator, 1].Value) != 0 || rateSheet.Cells[rowIterator, 1].Value != null)
                        {
                            decimal w = ExcelHelper.GetDecimal(rateSheet, rowIterator, 1);
                            decimal r = ExcelHelper.GetDecimal(rateSheet, rowIterator, j + 2);
                            courierList.Add(new express_courier_rates()
                            { weight = w, zone = zoneNos[j], rate = r, company = companyId });
                        }
                    }
                }

                using (quotesEntities db = new quotesEntities())
                {
                    logger.Info("Express Courier Rate Import - removing existing rates...");
                    db.DeleteExpressCourierRates(companyId);

                    logger.Info("Express Courier Rate Import - Importing new rates. Total entries: " + courierList.Count);
                    db.express_courier_rates.AddRange(courierList);
                    db.SaveChanges();
                }

                logger.Info("Express Courier Rate Import - " + courierList.Count + " entries imported successfully.");
                return new ImportFileResult(true, courierList.Count + " entries imported successfully.", null, courierList.Count);
            }
        }

        private static List<int> GetCourierZones(ExcelWorksheet sheet)
        {
            List<int> zoneNos = new List<int>();
            //Validation - Ensure all columns start with Zone
            for (int j = 2; j <= sheet.Dimension.End.Column; j++)
            {
                //check if first 5 charactors "Zone "
                string colName = ExcelHelper.GetStr(sheet, 1, j);

                if (!colName.StartsWith("Zone ") || !int.TryParse(colName.Split(' ')[1], out int zoneNo))
                    throw new Exception("Invalid Column Name. Col Index: " + j);
                zoneNos.Add(zoneNo);
            }
            return zoneNos;
        }

        public static ImportFileResult ImportBranchPostCodeExcel(HttpPostedFileBase file)
        {

            logger.Info("**********Branch Postcode File Import Start *************");

            int companyId = SessionHelper.CompanyId;

            logger.Info("Branch Postcode - Validating Excel...");
            ImportFileResult result = ExcelHelper.ValidateExcelFile(file, "Branch_postcode");

            if (result.IsSuccess == false) return result;

            using (var package = new ExcelPackage(file.InputStream))
            {
                var rateSheet = package.Workbook.Worksheets.First(x => x.Name == "Branch_postcode");

                logger.Info("Branch Postcode - Validating Column Headers...");
                result = ExcelHelper.ValidateColumnHeaders(rateSheet, (new[] { "EXB branch", "postcode", "Area", "Zone", "EXR branch", "EXV branch", "EXR quote" }).ToList());
                if (result.IsSuccess == false) return result;

                var postcodeList = new List<branch_postcode>();
                var currentSheet = package.Workbook.Worksheets;
                var workSheet = currentSheet.First();
                var noOfRow = workSheet.Dimension.End.Row;
                for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                {
                    var obj = new branch_postcode();
                    obj.baggage_branch_id = workSheet.Cells[rowIterator, 1].Value == null ? Convert.ToInt32(0) : Convert.ToInt32(workSheet.Cells[rowIterator, 1].Value);
                    obj.postcode = workSheet.Cells[rowIterator, 2].Value.ToString() == null ? string.Empty : workSheet.Cells[rowIterator, 2].Value.ToString();
                    obj.dcr_zone = workSheet.Cells[rowIterator, 4].Value == null ? Convert.ToInt16(0) : Convert.ToInt16(workSheet.Cells[rowIterator, 4].Value);
                    obj.removals_branch_id = workSheet.Cells[rowIterator, 5].Value == null ? 0 : Convert.ToInt32(workSheet.Cells[rowIterator, 5].Value);
                    obj.vehicle_branch_id = workSheet.Cells[rowIterator, 6].Value == null ? 0 : Convert.ToInt32(workSheet.Cells[rowIterator, 6].Value);
                    obj.removals_quotable = workSheet.Cells[rowIterator, 7].Value != null ? workSheet.Cells[rowIterator, 7].Value.ToString() == "Y" ? Convert.ToInt16(1) : Convert.ToInt16(0) : Convert.ToInt16(0);
                    obj.CompanyId = companyId;
                    postcodeList.Add(obj);
                }

                using (quotesEntities db = new quotesEntities())
                {
                    //Delete Existing Data from Table
                    logger.Info("Branch Postcode Import - removing existing rates...");
                    db.branch_postcode.RemoveRange(db.branch_postcode.Where(x => x.CompanyId == companyId));
                    db.SaveChanges();

                    //Insert Data
                    logger.Info("Branch Postcode Import - Importing new postcodes. Total entries: " + postcodeList.Count);
                    db.branch_postcode.AddRange(postcodeList);
                    db.SaveChanges();
                }

                logger.Info("Branch Postcode Import - " + postcodeList.Count + " entries imported successfully.");
                return new ImportFileResult(true, postcodeList.Count + " entries imported successfully.", null, postcodeList.Count);
            }
        }

        public static ImportFileResult ImportCountryCodesUKExcel(HttpPostedFileBase file)
        {
            logger.Info("**********Country Codes UK File Import Start *************");

            int companyId = SessionHelper.CompanyId;

            logger.Info("Country Codes UK - Validating Excel...");
            ImportFileResult result = ExcelHelper.ValidateExcelFile(file, "CountryCodes_UK");

            if (result.IsSuccess == false)
            {
                return new ImportFileResult(false, result.ReturnMessage);
            }

            using (var package = new ExcelPackage(file.InputStream))
            {
                var rateSheet = package.Workbook.Worksheets.First(x => x.Name == "CountryCodes_UK");

                logger.Info("Country Codes UK - Validating Column Headers...");
                result = ExcelHelper.ValidateColumnHeaders(rateSheet, (new[] { "Code", "Description", "State", "Regions" }).ToList());
                if (result.IsSuccess == false)
                {
                    return new ImportFileResult(false, result.ReturnMessage);
                }

                var currentSheet = package.Workbook.Worksheets;
                var workSheet = currentSheet.First();
                var noOfRow = workSheet.Dimension.End.Row;

                DataTable dtResult = new DataTable();
                dtResult.Columns.Add("Code", typeof(string));
                dtResult.Columns.Add("Description", typeof(string));
                dtResult.Columns.Add("State", typeof(string));
                dtResult.Columns.Add("Regions", typeof(string));

                for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                {
                    DataRow dtrow = dtResult.NewRow();
                    //var obj = new tbl_CountryCodesUK();
                    dtrow["Code"] = workSheet.Cells[rowIterator, 1].Value == null ? string.Empty : workSheet.Cells[rowIterator, 1].Value.ToString();
                    dtrow["Description"] = workSheet.Cells[rowIterator, 2].Value == null ? string.Empty : workSheet.Cells[rowIterator, 2].Value.ToString();
                    dtrow["State"] = workSheet.Cells[rowIterator, 3].Value == null ? string.Empty : workSheet.Cells[rowIterator, 3].Value.ToString();
                    dtrow["Regions"] = workSheet.Cells[rowIterator, 4].Value == null ? string.Empty : workSheet.Cells[rowIterator, 4].Value.ToString();
                    dtResult.Rows.Add(dtrow);
                }

                var Company = new SqlParameter
                {
                    ParameterName = "Company",
                    DbType = DbType.Int32,
                    Value = companyId
                };
                var UpdatedBy = new SqlParameter
                {
                    ParameterName = "UpdatedBy",
                    DbType = DbType.Int64,
                    Value = SessionHelper.UserId
                };
                var ImportCountryCodesUk = new SqlParameter
                {
                    ParameterName = "ImportCountryCodesUk",
                    SqlDbType = SqlDbType.Structured,
                    Value = dtResult,
                    TypeName = "dbo.ImportCountryCodesUk"
                };
                int imp_result = 0;
                using (quotesEntities db = new quotesEntities())
                {
                    //Delete and Insert Data
                    logger.Info("Country Codes UK Import - Importing new Country Codes UK. Total entries: " + noOfRow);
                    quotesEntities entityObj = new quotesEntities();
                    imp_result = entityObj.Database.SqlQuery<int>("SP_ImportCountryCodesUK @Company, @UpdatedBy, @ImportCountryCodesUk", Company, UpdatedBy, ImportCountryCodesUk).FirstOrDefault();

                }
                if (imp_result == 0)
                {
                    logger.Info("Country Codes UK Import - " + imp_result + " entries imported successfully.");
                    return new ImportFileResult(false, "failed to import data into database");
                }
                logger.Info("Country Codes UK Import - " + imp_result + " entries imported successfully.");
                return new ImportFileResult(true, imp_result + " entries imported successfully.", null, imp_result);
            }
        }

        public static ImportFileResult ImportFCLExcel(HttpPostedFileBase file)
        {
            logger.Info("**********FCL File Import Start *************");

            int companyId = SessionHelper.CompanyId;

            logger.Info("FCL - Validating Excel...");
            ImportFileResult result = ExcelHelper.ValidateExcelFile(file, "FCL");

            if (result.IsSuccess == false)
            {
                return new ImportFileResult(false, result.ReturnMessage);
            }

            using (var package = new ExcelPackage(file.InputStream))
            {
                var rateSheet = package.Workbook.Worksheets.First(x => x.Name == "FCL");

                //logger.Info("FCL - Validating Column Headers...");
                //result = ExcelHelper.ValidateColumnHeaders(rateSheet, (new[] { "", "Fleixstowe", "Southampton", "Thamesport", "Tilbury", "Gateway", "Liverpool", "Grangemouth" }).ToList());
                //if (result.IsSuccess == false)
                //{
                //    return new ImportFileResult(false, result.ReturnMessage);
                //}

                var currentSheet = package.Workbook.Worksheets;
                var workSheet = currentSheet.First(x => x.Name == "FCL");
                var noOfRow = workSheet.Dimension.End.Row;
                var noOfColumn = workSheet.Dimension.End.Column;

                List<string> list = new List<string>();
                for (var i = 2; i < noOfColumn; i++)
                {
                    list.Add(workSheet.Cells[1, i].Value == null ? string.Empty : workSheet.Cells[1, i].Value.ToString());
                }
                string[] header = list.ToArray();

                DataTable dtResult = new DataTable();
                dtResult.Columns.Add("Port", typeof(string));
                dtResult.Columns.Add("POE", typeof(string));
                dtResult.Columns.Add("20FT", typeof(string));
                dtResult.Columns.Add("40FT", typeof(string));
                dtResult.Columns.Add("40HC", typeof(string));

                for (int rowIterator = 3; rowIterator <= noOfRow; rowIterator++)
                {                    
                    for (int i = 0; i < header.Length; i++)
                    {
                        //var ex = header[i];
                        DataRow dtrow = dtResult.NewRow();
                        if (header[i] != "")
                        {
                            dtrow["Port"] = workSheet.Cells[rowIterator, 1].Value == null ? string.Empty : workSheet.Cells[rowIterator, 1].Value.ToString();
                            dtrow["POE"] = header[i];
                            dtrow["20FT"] = workSheet.Cells[rowIterator, i+2].Value == null ? null : workSheet.Cells[rowIterator, i + 2].Value.ToString();
                            dtrow["40FT"] = workSheet.Cells[rowIterator, i+3].Value == null ? null : workSheet.Cells[rowIterator, i + 3].Value.ToString();
                            dtrow["40HC"] = workSheet.Cells[rowIterator, i+4].Value == null ? null : workSheet.Cells[rowIterator, i + 4].Value.ToString();
                            dtResult.Rows.Add(dtrow);
                        }
                    }
                }

                var Company = new SqlParameter
                {
                    ParameterName = "Company",
                    DbType = DbType.Int32,
                    Value = companyId
                };
                var UpdatedBy = new SqlParameter
                {
                    ParameterName = "UpdatedBy",
                    DbType = DbType.Int64,
                    Value = SessionHelper.UserId
                };
                var ImportFCL = new SqlParameter
                {
                    ParameterName = "ImportFCL",
                    SqlDbType = SqlDbType.Structured,
                    Value = dtResult,
                    TypeName = "dbo.ImportFCL"
                };
                int imp_result = 0;
                using (quotesEntities db = new quotesEntities())
                {
                    //Delete and Insert Data
                    logger.Info("FCL - Importing new FCL Rates. Total entries: " + noOfRow);
                    quotesEntities entityObj = new quotesEntities();
                    imp_result = entityObj.Database.SqlQuery<int>("SP_ImportFCLRate @Company, @UpdatedBy, @ImportFCL", Company, UpdatedBy, ImportFCL).FirstOrDefault();

                }
                if (imp_result == 0)
                {
                    logger.Info("FCL - failed to import data into database.");
                    return new ImportFileResult(false, "failed to import data into database");
                }
                logger.Info("FCL - " + imp_result + " entries imported successfully.");
                return new ImportFileResult(true, noOfRow + " entries imported successfully.", null, noOfRow);
            }
        }

        public static ImportFileResult ImportAIRExcel(HttpPostedFileBase file)
        {
            logger.Info("**********AIR File Import Start *************");

            int companyId = SessionHelper.CompanyId;

            logger.Info("AIR - Validating Excel...");
            ImportFileResult result = ExcelHelper.ValidateExcelFile(file, "AIR");

            if (result.IsSuccess == false)
            {
                return new ImportFileResult(false, result.ReturnMessage);
            }

            using (var package = new ExcelPackage(file.InputStream))
            {
                var rateSheet = package.Workbook.Worksheets.First(x => x.Name == "AIR");

                logger.Info("AIR - Validating Column Headers...");
                result = ExcelHelper.ValidateColumnHeaders(rateSheet, (new[] { "WEIGHT KGS", "Flat Rate Cuft", "HAND OUT", "IMP1", "IMP2", "IMP3", "IMP4", "IMP5", "IMP6", "IMP7" }).ToList());
                if (result.IsSuccess == false)
                {
                    return new ImportFileResult(false, result.ReturnMessage);
                }

                var currentSheet = package.Workbook.Worksheets;
                var workSheet = currentSheet.First(x => x.Name == "AIR");
                var noOfRow = workSheet.Dimension.End.Row;
                var noOfColumn = workSheet.Dimension.End.Column;

                List<string> list = new List<string>();
                for (var i = 3; i <= noOfColumn; i++)
                {
                    list.Add(workSheet.Cells[1, i].Value == null ? string.Empty : workSheet.Cells[1, i].Value.ToString());
                }
                string[] header = list.ToArray();

                DataTable dtResult = new DataTable();
                dtResult.Columns.Add("CuftFrom", typeof(string));
                dtResult.Columns.Add("CuftTo", typeof(string));
                dtResult.Columns.Add("IsFlatRate", typeof(string));
                dtResult.Columns.Add("RegionCode", typeof(string));
                dtResult.Columns.Add("Rate", typeof(string));

                for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                {
                    for (int i = 0; i < header.Length; i++)
                    {
                        DataRow dtrow = dtResult.NewRow();
                        var cuft = (workSheet.Cells[rowIterator, 1].Value.ToString()).Split('-');
                        dtrow["CuftFrom"] = cuft[0].Trim();
                        dtrow["CuftTo"] = cuft[1].Trim();
                        dtrow["IsFlatRate"] = workSheet.Cells[rowIterator, 2].Value == null ? null : workSheet.Cells[rowIterator, 2].Value.ToString();
                        dtrow["RegionCode"] = header[i];
                        dtrow["Rate"] = workSheet.Cells[rowIterator, i+3].Value == null ? null : workSheet.Cells[rowIterator, i + 3].Value.ToString();
                        dtResult.Rows.Add(dtrow);
                    }
                }

                var Company = new SqlParameter
                {
                    ParameterName = "Company",
                    DbType = DbType.Int32,
                    Value = companyId
                };
                var UpdatedBy = new SqlParameter
                {
                    ParameterName = "UpdatedBy",
                    DbType = DbType.Int64,
                    Value = SessionHelper.UserId
                };
                var ImportAIR = new SqlParameter
                {
                    ParameterName = "ImportAIR",
                    SqlDbType = SqlDbType.Structured,
                    Value = dtResult,
                    TypeName = "dbo.ImportAIR"
                };
                int imp_result = 0;
                using (quotesEntities db = new quotesEntities())
                {
                    //Delete and Insert Data
                    logger.Info("AIR - Importing AIR Rates. Total entries: " + noOfRow);
                    quotesEntities entityObj = new quotesEntities();
                    imp_result = entityObj.Database.SqlQuery<int>("SP_ImportAIRRate @Company, @UpdatedBy, @ImportAIR", Company, UpdatedBy, ImportAIR).FirstOrDefault();

                }
                if (imp_result == 0)
                {
                    logger.Info("AIR Import - failed to import data into database.");
                    return new ImportFileResult(false, "failed to import data into database");
                }
                logger.Info("AIR Import - " + imp_result + " entries imported successfully.");
                return new ImportFileResult(true, noOfRow + " entries imported successfully.", null, noOfRow);
            }
        }

        public static ImportFileResult ImportGRPExcel(HttpPostedFileBase file)
        {
            logger.Info("**********GRP File Import Start *************");

            int companyId = SessionHelper.CompanyId;

            logger.Info("GRP - Validating Excel...");
            ImportFileResult result = ExcelHelper.ValidateExcelFile(file, "GRP");

            if (result.IsSuccess == false)
            {
                return new ImportFileResult(false, result.ReturnMessage);
            }

            using (var package = new ExcelPackage(file.InputStream))
            {
                var rateSheet = package.Workbook.Worksheets.First(x => x.Name == "GRP");

                //logger.Info("GRP - Validating Column Headers...");
                //result = ExcelHelper.ValidateColumnHeaders(rateSheet, (new[] { "", "Fleixstowe", "Southampton", "Thamesport", "Tilbury", "Gateway", "Liverpool", "Grangemouth" }).ToList());
                //if (result.IsSuccess == false)
                //{
                //    return new ImportFileResult(false, result.ReturnMessage);
                //}

                var currentSheet = package.Workbook.Worksheets;
                var workSheet = currentSheet.First(x => x.Name == "GRP");
                var noOfRow = workSheet.Dimension.End.Row;
                var noOfColumn = workSheet.Dimension.End.Column;

                List<string> list = new List<string>();
                for (var i = 3; i <= noOfColumn; i++)
                {
                    list.Add(workSheet.Cells[2, i].Value == null ? string.Empty : workSheet.Cells[2, i].Value.ToString());
                }
                string[] header = list.ToArray();

                DataTable dtResult = new DataTable();
                dtResult.Columns.Add("CuftFrom", typeof(string));
                dtResult.Columns.Add("CuftTo", typeof(string));
                dtResult.Columns.Add("IsFlatRate", typeof(string));
                dtResult.Columns.Add("IMP", typeof(string));
                dtResult.Columns.Add("Ex20", typeof(string));
                dtResult.Columns.Add("Ex40", typeof(string));

                for (int rowIterator = 4; rowIterator <= noOfRow; rowIterator++)
                {
                    for (int i = 0; i < header.Length; i++)
                    {
                        //var ex = header[i];
                        DataRow dtrow = dtResult.NewRow();
                        if (header[i] != "")
                        {
                            var cuft = (workSheet.Cells[rowIterator, 1].Value.ToString()).Split('-');                            
                            dtrow["CuftFrom"] = cuft[0].Trim();
                            dtrow["CuftTo"] = cuft[1].Trim();
                            dtrow["IsFlatRate"] = workSheet.Cells[rowIterator, 2].Value == null ? null : workSheet.Cells[rowIterator, 2].Value.ToString();
                            dtrow["IMP"] = header[i];
                            dtrow["Ex20"] = workSheet.Cells[rowIterator, i + 3].Value == null ? null : workSheet.Cells[rowIterator, i + 3].Value.ToString();
                            dtrow["Ex40"] = workSheet.Cells[rowIterator, i + 4].Value == null ? null : workSheet.Cells[rowIterator, i + 4].Value.ToString();
                            dtResult.Rows.Add(dtrow);
                        }
                    }
                }

                var Company = new SqlParameter
                {
                    ParameterName = "Company",
                    DbType = DbType.Int32,
                    Value = companyId
                };
                var UpdatedBy = new SqlParameter
                {
                    ParameterName = "UpdatedBy",
                    DbType = DbType.Int64,
                    Value = SessionHelper.UserId
                };
                var ImportGRP = new SqlParameter
                {
                    ParameterName = "ImportGRP",
                    SqlDbType = SqlDbType.Structured,
                    Value = dtResult,
                    TypeName = "dbo.ImportGRP"
                };
                int imp_result = 0;
                using (quotesEntities db = new quotesEntities())
                {
                    //Delete and Insert Data
                    logger.Info("GRP - Importing new GRP Rates. Total entries: " + noOfRow);
                    quotesEntities entityObj = new quotesEntities();
                    imp_result = entityObj.Database.SqlQuery<int>("SP_ImportGRPRate @Company, @UpdatedBy, @ImportGRP", Company, UpdatedBy, ImportGRP).FirstOrDefault();

                }
                if (imp_result == 0)
                {
                    logger.Info("GRP - failed to import data into database");
                    return new ImportFileResult(false, "failed to import data into database");
                }
                logger.Info("GRP - " + imp_result + " entries imported successfully.");
                return new ImportFileResult(true, noOfRow + " entries imported successfully.", null, noOfRow);
            }
        }

        public static ImportFileResult ImportTradeRegionExcel(HttpPostedFileBase file)
        {
            logger.Info("**********Trade Region File Import Start *************");

            int companyId = SessionHelper.CompanyId;

            logger.Info("Trade Region - Validating Excel...");
            ImportFileResult result = ExcelHelper.ValidateExcelFile(file, "TradeRegion");

            if (result.IsSuccess == false)
            {
                return new ImportFileResult(false, result.ReturnMessage);
            }

            using (var package = new ExcelPackage(file.InputStream))
            {
                var rateSheet = package.Workbook.Worksheets.First(x => x.Name == "TradeRegion");

                logger.Info("Trade Region  - Validating Column Headers...");
                result = ExcelHelper.ValidateColumnHeaders(rateSheet, (new[] { "Country", "PoECode", "PoECity", "DestinationCode", "DestinationCity", "ZoneNo", "ZoneName", "Distance" }).ToList());
                if (result.IsSuccess == false)
                {
                    return new ImportFileResult(false, result.ReturnMessage);
                }

                var currentSheet = package.Workbook.Worksheets;
                var workSheet = currentSheet.First();
                var noOfRow = workSheet.Dimension.End.Row;

                DataTable dtResult = new DataTable();
                dtResult.Columns.Add("Country", typeof(string));
                dtResult.Columns.Add("PoECode", typeof(string));
                dtResult.Columns.Add("PoECity", typeof(string));
                dtResult.Columns.Add("DestinationCode", typeof(string));
                dtResult.Columns.Add("DestinationCity", typeof(string));
                dtResult.Columns.Add("ZoneNo", typeof(string));
                dtResult.Columns.Add("ZoneName", typeof(string));
                dtResult.Columns.Add("Distance", typeof(string));

                for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                {
                    DataRow dtrow = dtResult.NewRow();
                    dtrow["Country"] = workSheet.Cells[rowIterator, 1].Value == null ? string.Empty : workSheet.Cells[rowIterator, 1].Value.ToString();
                    dtrow["PoECode"] = workSheet.Cells[rowIterator, 2].Value == null ? string.Empty : workSheet.Cells[rowIterator, 2].Value.ToString();
                    dtrow["PoECity"] = workSheet.Cells[rowIterator, 3].Value == null ? string.Empty : workSheet.Cells[rowIterator, 3].Value.ToString();
                    dtrow["DestinationCode"] = workSheet.Cells[rowIterator, 4].Value == null ? string.Empty : workSheet.Cells[rowIterator, 4].Value.ToString();
                    dtrow["DestinationCity"] = workSheet.Cells[rowIterator, 5].Value == null ? string.Empty : workSheet.Cells[rowIterator, 5].Value.ToString();
                    dtrow["ZoneNo"] = workSheet.Cells[rowIterator, 6].Value == null ? string.Empty : workSheet.Cells[rowIterator, 6].Value.ToString();
                    dtrow["ZoneName"] = workSheet.Cells[rowIterator, 7].Value == null ? string.Empty : workSheet.Cells[rowIterator, 7].Value.ToString();
                    dtrow["Distance"] = workSheet.Cells[rowIterator, 8].Value == null ? string.Empty : workSheet.Cells[rowIterator, 8].Value.ToString();
                    dtResult.Rows.Add(dtrow);
                }

                var Company = new SqlParameter
                {
                    ParameterName = "Company",
                    DbType = DbType.Int32,
                    Value = companyId
                };
                var UpdatedBy = new SqlParameter
                {
                    ParameterName = "UpdatedBy",
                    DbType = DbType.Int64,
                    Value = SessionHelper.UserId
                };
                var ImportTradeRegion = new SqlParameter
                {
                    ParameterName = "ImportTradeRegion",
                    SqlDbType = SqlDbType.Structured,
                    Value = dtResult,
                    TypeName = "dbo.ImportTradeRegion"
                };
                int imp_result = 0;
                using (quotesEntities db = new quotesEntities())
                {
                    //Delete and Insert Data
                    logger.Info("Trade Region Import - Importing new Trade Region. Total entries: " + noOfRow);
                    quotesEntities entityObj = new quotesEntities();
                    imp_result = entityObj.Database.SqlQuery<int>("SP_ImportTradeRegion @Company, @UpdatedBy, @ImportTradeRegion", Company, UpdatedBy, ImportTradeRegion).FirstOrDefault();

                }
                if (imp_result == 0)
                {
                    logger.Info("Trade Region Import - failed to import data into database.");
                    return new ImportFileResult(false, "failed to import data into database");
                }
                logger.Info("Trade Region Import - " + imp_result + " entries imported successfully.");
                return new ImportFileResult(true, imp_result + " entries imported successfully.", null, imp_result);
            }
        }

        public static ImportFileResult ImportTradeRatesExcel(HttpPostedFileBase file)
        {
            logger.Info("**********Trade Rates File Import Start *************");

            int companyId = SessionHelper.CompanyId;

            logger.Info("Trade Rates - Validating Excel...");
            ImportFileResult result = ExcelHelper.ValidateExcelFile(file, "Rates");

            if (result.IsSuccess == false)
            {
                return new ImportFileResult(false, result.ReturnMessage);
            }

            using (var package = new ExcelPackage(file.InputStream))
            {
                var rateSheet = package.Workbook.Worksheets.First(x => x.Name == "Rates");

                logger.Info("Trade Rates  - Validating Column Headers...");
                result = ExcelHelper.ValidateColumnHeaders(rateSheet, (new[] { "Country", "PoE", "Zone Name", "CuFt", "ChargeCuFt", "Dest", "Freight", "Handling", "Tarrif" }).ToList());
                if (result.IsSuccess == false)
                {
                    return new ImportFileResult(false, result.ReturnMessage);
                }

                var currentSheet = package.Workbook.Worksheets;
                var workSheet = currentSheet.First(x => x.Name == "Rates");
                var noOfRow = workSheet.Dimension.End.Row;

                DataTable dtResult = new DataTable();
                dtResult.Columns.Add("Country", typeof(string));
                dtResult.Columns.Add("PoECode", typeof(string));
                dtResult.Columns.Add("ZoneName", typeof(string));
                dtResult.Columns.Add("CuFtFrom", typeof(string));
                dtResult.Columns.Add("CuFtTo", typeof(string));
                dtResult.Columns.Add("ChargeCuFt", typeof(string));
                dtResult.Columns.Add("Dest", typeof(string));
                dtResult.Columns.Add("Freight", typeof(string));
                dtResult.Columns.Add("Handling", typeof(string));
                dtResult.Columns.Add("Tarrif", typeof(string));

                for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                {
                    DataRow dtrow = dtResult.NewRow();
                    dtrow["Country"] = workSheet.Cells[rowIterator, 1].Value == null ? string.Empty : workSheet.Cells[rowIterator, 1].Value.ToString();
                    dtrow["PoECode"] = workSheet.Cells[rowIterator, 2].Value == null ? string.Empty : workSheet.Cells[rowIterator, 2].Value.ToString();
                    dtrow["ZoneName"] = workSheet.Cells[rowIterator, 3].Value == null ? string.Empty : workSheet.Cells[rowIterator, 3].Value.ToString();
                    var cuft = (workSheet.Cells[rowIterator, 4].Value.ToString()).Split('-');
                    if (cuft?.Count() > 0)
                    {
                        dtrow["CuFtFrom"] = cuft[0].Trim();
                        dtrow["CuFtTo"] = cuft[1].Trim();
                    }
                    else
                    {
                        dtrow["CuFtFrom"] = string.Empty;
                        dtrow["CuFtTo"] = string.Empty;
                    }
                    dtrow["ChargeCuFt"] = workSheet.Cells[rowIterator, 5].Value == null ? string.Empty : workSheet.Cells[rowIterator, 5].Value.ToString();
                    dtrow["Dest"] = workSheet.Cells[rowIterator, 6].Value == null ? string.Empty : workSheet.Cells[rowIterator, 6].Value.ToString();
                    dtrow["Freight"] = workSheet.Cells[rowIterator, 7].Value == null ? string.Empty : workSheet.Cells[rowIterator, 7].Value.ToString();
                    dtrow["Handling"] = workSheet.Cells[rowIterator, 8].Value == null ? string.Empty : workSheet.Cells[rowIterator, 8].Value.ToString();
                    dtrow["Tarrif"] = workSheet.Cells[rowIterator, 9].Value == null ? string.Empty : workSheet.Cells[rowIterator, 9].Value.ToString();
                    dtResult.Rows.Add(dtrow);
                }

                var Company = new SqlParameter
                {
                    ParameterName = "Company",
                    DbType = DbType.Int32,
                    Value = companyId
                };
                var UpdatedBy = new SqlParameter
                {
                    ParameterName = "UpdatedBy",
                    DbType = DbType.Int64,
                    Value = SessionHelper.UserId
                };
                var ImportTradeRate = new SqlParameter
                {
                    ParameterName = "ImportTradeRate",
                    SqlDbType = SqlDbType.Structured,
                    Value = dtResult,
                    TypeName = "dbo.ImportTradeRate"
                };
                int imp_result = 0;
                using (quotesEntities db = new quotesEntities())
                {
                    //Delete and Insert Data
                    logger.Info("Trade Rates Import - Importing new Trade Rates. Total entries: " + noOfRow);
                    quotesEntities entityObj = new quotesEntities();
                    imp_result = entityObj.Database.SqlQuery<int>("SP_ImportTradeRate @Company, @UpdatedBy, @ImportTradeRate", Company, UpdatedBy, ImportTradeRate).FirstOrDefault();

                }
                if (imp_result == 0)
                {
                    logger.Info("Trade Rates Import - failed to import data into database.");
                    return new ImportFileResult(false, "failed to import data into database");
                }
                logger.Info("Trade Rates Import -failed to import data into database.");
                return new ImportFileResult(true, imp_result + " entries imported successfully.", null, imp_result);
            }
        }

        public static ImportFileResult ImportExpressCourierCost(HttpPostedFileBase file)
        {
            logger.Info("**********Express Courier Cost File Import Start *************");

            int companyId = SessionHelper.CompanyId;

            logger.Info("Express Courier Cost - Validating Excel...");
            ImportFileResult result = ExcelHelper.ValidateExcelFile(file, "Costs-CourierExpress");

            if (result.IsSuccess == false)
            {
                return new ImportFileResult(false, result.ReturnMessage);
            }

            using (var package = new ExcelPackage(file.InputStream))
            {
                var rateSheet = package.Workbook.Worksheets.First(x => x.Name == "Costs-CourierExpress");

                logger.Info("Express Courier Cost - Validating Column Headers...");
                result = ExcelHelper.ValidateColumnHeaders(rateSheet, (new[] { "Kgs" }).ToList());
                if (result.IsSuccess == false)
                {
                    return new ImportFileResult(false, result.ReturnMessage);
                }
                //List<int> zoneNos = GetCourierZones(rateSheet);

                var currentSheet = package.Workbook.Worksheets;
                var workSheet = currentSheet.First();
                var noOfRow = workSheet.Dimension.End.Row;
                var noOfColumn = workSheet.Dimension.End.Column;

                List<string> list = new List<string>();
                for (var i = 2; i <= noOfColumn; i++)
                {
                    list.Add(workSheet.Cells[1, i].Value == null ? string.Empty : workSheet.Cells[1, i].Value.ToString());
                }
                string[] header = list.ToArray();

                DataTable dtResult = new DataTable();
                dtResult.Columns.Add("Zone", typeof(string));
                dtResult.Columns.Add("Weight", typeof(string));
                dtResult.Columns.Add("Rate", typeof(string));

                for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                {
                    for (int i = 0; i < header.Length; i++)
                    {
                        DataRow dtrow = dtResult.NewRow();
                        dtrow["Zone"] = header[i];
                        dtrow["Weight"] = workSheet.Cells[rowIterator, 1].Value == null ? null : workSheet.Cells[rowIterator, 1].Value.ToString();
                        dtrow["Rate"] = workSheet.Cells[rowIterator, i+2].Value == null ? null : workSheet.Cells[rowIterator, i+2].Value.ToString();
                        dtResult.Rows.Add(dtrow);
                    }
                }

                var Company = new SqlParameter
                {
                    ParameterName = "Company",
                    DbType = DbType.Int32,
                    Value = companyId
                };
                var UpdatedBy = new SqlParameter
                {
                    ParameterName = "UpdatedBy",
                    DbType = DbType.Int64,
                    Value = SessionHelper.UserId
                };
                var CourierExpressCost = new SqlParameter
                {
                    ParameterName = "CourierExpressCost",
                    SqlDbType = SqlDbType.Structured,
                    Value = dtResult,
                    TypeName = "dbo.CourierExpressCost"
                };
                int imp_result = 0;
                using (quotesEntities db = new quotesEntities())
                {
                    //Delete and Insert Data
                    logger.Info("Courier Express Cost - Importing Courier Express Cost. Total entries: " + noOfRow);
                    quotesEntities entityObj = new quotesEntities();
                    imp_result = entityObj.Database.SqlQuery<int>("SP_ImportCourierExpressCost @Company, @UpdatedBy, @CourierExpressCost", Company, UpdatedBy, CourierExpressCost).FirstOrDefault();

                }
                if (imp_result == 0)
                {
                    logger.Info("Courier Express Cost Import - failed to import data into database.");
                    return new ImportFileResult(false, "failed to import data into database");
                }
                logger.Info("Courier Express Cost Import - " + imp_result + " entries imported successfully.");
                return new ImportFileResult(true, noOfRow + " entries imported successfully.", null, noOfRow);
            }
        }

        public static ImportFileResult ImportBagC2CCost(HttpPostedFileBase file)
        {
            logger.Info("**********Bag C2C Cost File Import Start *************");

            int companyId = SessionHelper.CompanyId;

            logger.Info("Bag C2C Cost - Validating Excel...");
            ImportFileResult result = ExcelHelper.ValidateExcelFile(file, "Costs-C2C");

            if (result.IsSuccess == false)
            {
                return new ImportFileResult(false, result.ReturnMessage);
            }

            using (var package = new ExcelPackage(file.InputStream))
            {
                var rateSheet = package.Workbook.Worksheets.First(x => x.Name == "Costs-C2C");

                logger.Info("Bag C2C Cost - Validating Column Headers...");
                result = ExcelHelper.ValidateColumnHeaders(rateSheet, 
                    (new[] { "KG", "", "Zone A", "Zone B", "Zone C", "Zone D", "Zone E", "Zone F", "Zone G", "Zone H", "Zone I", "Zone J", "Zone K", "Zone L", "Zone M", "Zone N", "Zone O", "Zone P", "Zone Q", "Zone R", "Zone S", "Zone T", "Zone U", "Zone V", "Zone W", "Zone X", "Zone Y", "Zone Z", "Zone AA", "Zone AB", "Zone AC", "Zone AD", "Zone AE", "Zone AF", "Zone AG", "Zone AH", "Zone AI", "Zone AJ", "Zone AK", "Zone AL", "Zone AM", "Zone AN", "Zone AO", "Zone AP", "Zone AQ", "Zone AR", "Zone AS", "Zone AT", "Zone AU", "Zone AV", "Zone AW", "Zone AX", "Zone AY", "Zone AZ", "Zone BA", "Zone BB", "Zone BC", "Zone BD", "Zone BE", "Zone BF", "Zone BG", "Zone BH", "Zone BI", "Zone BJ", "Zone BK", "Zone BL", "Zone BM", "Zone BN", "Zone BO", "Zone BP", "Zone BQ", "Zone BR", "Zone BS" }).ToList());
                if (result.IsSuccess == false)
                {
                    return new ImportFileResult(false, result.ReturnMessage);
                }

                var currentSheet = package.Workbook.Worksheets;
                var workSheet = currentSheet.First(x => x.Name == "Costs-C2C");
                var noOfRow = workSheet.Dimension.End.Row;
                var noOfColumn = workSheet.Dimension.End.Column;

                List<string> list = new List<string>();
                for (var i = 2; i <= noOfColumn; i++)
                {
                    list.Add(workSheet.Cells[1, i].Value == null ? string.Empty : workSheet.Cells[1, i].Value.ToString());
                }
                string[] header = list.ToArray();

                DataTable dtResult = new DataTable();
                dtResult.Columns.Add("KgFrom", typeof(string));
                dtResult.Columns.Add("KgTo", typeof(string));
                dtResult.Columns.Add("Zone", typeof(string));
                dtResult.Columns.Add("Rate", typeof(string));

                for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                {
                    decimal from, to;
                    if (!decimal.TryParse(Convert.ToString(workSheet.Cells[rowIterator, 1].Value), out from))
                        continue;
                    to = Convert.ToDecimal(workSheet.Cells[rowIterator, 2].Value);

                    for (int i = 0; i < header.Length-1; i++)
                    {
                        DataRow dtrow = dtResult.NewRow();
                        dtrow["KgFrom"] = from.ToString();
                        dtrow["KgTo"] = to.ToString();
                        dtrow["Zone"] = Convert.ToString(workSheet.Cells[1, i + 3].Value).Replace("Zone ", "");
                        dtrow["Rate"] = workSheet.Cells[rowIterator, i + 3].Value == null ? null : workSheet.Cells[rowIterator, i + 3].Value.ToString();
                        dtResult.Rows.Add(dtrow);
                    }

                }

                var Company = new SqlParameter
                {
                    ParameterName = "Company",
                    DbType = DbType.Int32,
                    Value = companyId
                };
                var UpdatedBy = new SqlParameter
                {
                    ParameterName = "UpdatedBy",
                    DbType = DbType.Int64,
                    Value = SessionHelper.UserId
                };
                var BagC2CCost = new SqlParameter
                {
                    ParameterName = "BagC2CCost",
                    SqlDbType = SqlDbType.Structured,
                    Value = dtResult,
                    TypeName = "dbo.BagC2CCost"
                };
                int imp_result = 0;
                using (quotesEntities db = new quotesEntities())
                {
                    //Delete and Insert Data
                    logger.Info("Bag C2C Cost - Importing Courier Express Cost. Total entries: " + noOfRow);
                    quotesEntities entityObj = new quotesEntities();
                    imp_result = entityObj.Database.SqlQuery<int>("SP_ImportBagC2CCost @Company, @UpdatedBy, @BagC2CCost", Company, UpdatedBy, BagC2CCost).FirstOrDefault();

                }
                if (imp_result == 0)
                {
                    logger.Info("Bag C2C Cost Import - failed to import data into database.");
                    return new ImportFileResult(false, "failed to import data into database");
                }
                logger.Info("Bag C2C CostIR Import - " + imp_result + " entries imported successfully.");
                return new ImportFileResult(true, noOfRow + " entries imported successfully.", null, noOfRow);
            }
        }


        public static ImportFileResult ImportBagImportsUKCost(HttpPostedFileBase file)
        {
            logger.Info("**********Bag Imports UK Cost File Import Start *************");

            int companyId = SessionHelper.CompanyId;

            logger.Info("Bag Imports UK Cost - Validating Excel...");
            ImportFileResult result = ExcelHelper.ValidateExcelFile(file, "Cost-ImportBag");

            if (result.IsSuccess == false)
            {
                return new ImportFileResult(false, result.ReturnMessage);
            }

            using (var package = new ExcelPackage(file.InputStream))
            {
                var rateSheet = package.Workbook.Worksheets.First(x => x.Name == "Cost-ImportBag");

                logger.Info("Bag Imports UK Cost - Validating Column Headers...");
                result = ExcelHelper.ValidateColumnHeaders(rateSheet,
                    (new[] { "KG", ""}).ToList());
                if (result.IsSuccess == false)
                {
                    return new ImportFileResult(false, result.ReturnMessage);
                }

                var currentSheet = package.Workbook.Worksheets;
                var workSheet = currentSheet.First(x => x.Name == "Cost-ImportBag");
                var noOfRow = workSheet.Dimension.End.Row;
                var noOfColumn = workSheet.Dimension.End.Column;

                List<string> list = new List<string>();
                for (var i = 2; i <= noOfColumn; i++)
                {
                    list.Add(workSheet.Cells[1, i].Value == null ? string.Empty : workSheet.Cells[1, i].Value.ToString());
                }
                string[] header = list.ToArray();

                DataTable dtResult = new DataTable();
                dtResult.Columns.Add("KgFrom", typeof(decimal));
                dtResult.Columns.Add("KgTo", typeof(decimal));
                dtResult.Columns.Add("Zone", typeof(string));
                dtResult.Columns.Add("Rate", typeof(decimal));

                for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                {
                    decimal from, to;
                    if (!decimal.TryParse(Convert.ToString(workSheet.Cells[rowIterator, 1].Value), out from))
                        continue;
                    //to = Convert.ToDecimal(workSheet.Cells[rowIterator, 2].Value);
                    to = 0;
                    for (int i = 0; i < header.Length-1; i++)
                    {
                        DataRow dtrow = dtResult.NewRow();
                        dtrow["KgFrom"] = from.ToString();
                        dtrow["KgTo"] = to.ToString();
                        dtrow["Zone"] = Convert.ToString(workSheet.Cells[1, i + 3].Value).Replace("Zone ", "");
                        dtrow["Rate"] = string.IsNullOrEmpty(Convert.ToString(workSheet.Cells[rowIterator, i + 3].Value)) ? "0" : workSheet.Cells[rowIterator, i + 3].Value;
                        dtResult.Rows.Add(dtrow);
                    }

                }

                var Company = new SqlParameter
                {
                    ParameterName = "Company",
                    DbType = DbType.Int32,
                    Value = companyId
                };
                var UpdatedBy = new SqlParameter
                {
                    ParameterName = "UpdatedBy",
                    DbType = DbType.Int64,
                    Value = SessionHelper.UserId
                };
                var BagImportsUKCost = new SqlParameter
                {
                    ParameterName = "BagImportsUKCost",
                    SqlDbType = SqlDbType.Structured,
                    Value = dtResult,
                    TypeName = "dbo.BagImportsUKCost"
                };
                int imp_result = 0;
                using (quotesEntities db = new quotesEntities())
                {
                    //Delete and Insert Data
                    logger.Info("Bag Imports UK Cost - Importing Courier Express Cost. Total entries: " + noOfRow);
                    quotesEntities entityObj = new quotesEntities();
                    imp_result = entityObj.Database.SqlQuery<int>("SP_ImportBagImportsUKCost @Company, @UpdatedBy, @BagImportsUKCost", Company, UpdatedBy, BagImportsUKCost).FirstOrDefault();

                }
                if (imp_result == 0)
                {
                    logger.Info("Bag Imports UK Cost Import - failed to import data into database.");
                    return new ImportFileResult(false, "failed to import data into database");
                }
                logger.Info("Bag Imports UK Cost Import - " + imp_result + " entries imported successfully.");
                return new ImportFileResult(true, noOfRow + " entries imported successfully.", null, noOfRow);
            }
        }

        public static ImportFileResult ImportSeaFreightCost(HttpPostedFileBase file)
        {
            logger.Info("**********Sea Freight Cost File Import Start *************");

            int companyId = SessionHelper.CompanyId;

            logger.Info("Sea Freight Cost - Validating Excel...");
            ImportFileResult result = ExcelHelper.ValidateExcelFile(file, "Cost-SeaFreight");

            if (result.IsSuccess == false)
            {
                return new ImportFileResult(false, result.ReturnMessage);
            }

            using (var package = new ExcelPackage(file.InputStream))
            {
                var rateSheet = package.Workbook.Worksheets.First(x => x.Name == "Cost-SeaFreight");

                logger.Info("Sea Freight Cost  - Validating Column Headers...");
                result = ExcelHelper.ValidateColumnHeaders(rateSheet, (new[] { "Zones", "DESTINATION", "CUFT Form", "CUFT To", "Dest", "Freight", "Origin", "Additional" }).ToList());
                if (result.IsSuccess == false)
                {
                    return new ImportFileResult(false, result.ReturnMessage);
                }

                var currentSheet = package.Workbook.Worksheets;
                var workSheet = currentSheet.First(x => x.Name == "Cost-SeaFreight");
                var noOfRow = workSheet.Dimension.End.Row;

                DataTable dtResult = new DataTable();
                dtResult.Columns.Add("Zone", typeof(string));
                dtResult.Columns.Add("Destination", typeof(string));
                dtResult.Columns.Add("CuftForm", typeof(string));
                dtResult.Columns.Add("CuftTo", typeof(string));
                dtResult.Columns.Add("Dest", typeof(string));
                dtResult.Columns.Add("Freight", typeof(string));
                dtResult.Columns.Add("Origin", typeof(string));
                dtResult.Columns.Add("Additional", typeof(string));

                for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                {
                    DataRow dtrow = dtResult.NewRow();
                    dtrow["Zone"] = workSheet.Cells[rowIterator, 1].Value == null ? string.Empty : workSheet.Cells[rowIterator, 1].Value.ToString();
                    dtrow["Destination"] = workSheet.Cells[rowIterator, 2].Value == null ? string.Empty : workSheet.Cells[rowIterator, 2].Value.ToString();
                    dtrow["CuftForm"] = workSheet.Cells[rowIterator, 3].Value == null ? string.Empty : workSheet.Cells[rowIterator, 3].Value.ToString();
                    dtrow["CuftTo"] = workSheet.Cells[rowIterator, 4].Value == null ? string.Empty : workSheet.Cells[rowIterator, 4].Value.ToString();
                    dtrow["Dest"] = workSheet.Cells[rowIterator, 5].Value == null ? string.Empty : workSheet.Cells[rowIterator, 5].Value.ToString();
                    dtrow["Freight"] = workSheet.Cells[rowIterator, 6].Value == null ? string.Empty : workSheet.Cells[rowIterator, 6].Value.ToString();
                    dtrow["Origin"] = workSheet.Cells[rowIterator, 7].Value == null ? string.Empty : workSheet.Cells[rowIterator, 7].Value.ToString();
                    dtrow["Additional"] = workSheet.Cells[rowIterator, 8].Value == null ? string.Empty : workSheet.Cells[rowIterator, 8].Value.ToString();
                    dtResult.Rows.Add(dtrow);
                }

                var Company = new SqlParameter
                {
                    ParameterName = "Company",
                    DbType = DbType.Int32,
                    Value = companyId
                };
                var UpdatedBy = new SqlParameter
                {
                    ParameterName = "UpdatedBy",
                    DbType = DbType.Int64,
                    Value = SessionHelper.UserId
                };
                var SeaFreightCost = new SqlParameter
                {
                    ParameterName = "SeaFreightCost",
                    SqlDbType = SqlDbType.Structured,
                    Value = dtResult,
                    TypeName = "dbo.SeaFreightCost"
                };
                int imp_result = 0;
                using (quotesEntities db = new quotesEntities())
                {
                    //Delete and Insert Data
                    logger.Info("Sea Freight Cost Import - Importing new Trade Region. Total entries: " + noOfRow);
                    quotesEntities entityObj = new quotesEntities();
                    imp_result = entityObj.Database.SqlQuery<int>("SP_ImportSeaFreightCost @Company, @UpdatedBy, @SeaFreightCost", Company, UpdatedBy, SeaFreightCost).FirstOrDefault();

                }
                if (imp_result == 0)
                {
                    logger.Info("Sea Freight Cost Import - failed to import data into database.");
                    return new ImportFileResult(false, "failed to import data into database");
                }
                logger.Info("Sea Freight Cost Import - " + imp_result + " entries imported successfully.");
                return new ImportFileResult(true, imp_result + " entries imported successfully.", null, imp_result);
            }
        }
        public static ImportFileResult ImportCostsCourierEconomy(HttpPostedFileBase file)
        {
            logger.Info("**********Express Courier Cost File Import Start *************");

            int companyId = SessionHelper.CompanyId;

            logger.Info("Express Courier Cost - Validating Excel...");
            ImportFileResult result = ExcelHelper.ValidateExcelFile(file, "Costs-CourierEconomy");

            if (result.IsSuccess == false)
            {
                return new ImportFileResult(false, result.ReturnMessage);
            }

            using (var package = new ExcelPackage(file.InputStream))
            {
                var rateSheet = package.Workbook.Worksheets.First(x => x.Name == "Costs-CourierEconomy");

                logger.Info("Costs Courier Economy - Validating Column Headers...");
                result = ExcelHelper.ValidateColumnHeaders(rateSheet, (new[] { "Kgs" }).ToList());
                if (result.IsSuccess == false)
                {
                    return new ImportFileResult(false, result.ReturnMessage);
                }
                //List<int> zoneNos = GetCourierZones(rateSheet);

                var currentSheet = package.Workbook.Worksheets;
                var workSheet = currentSheet.First();
                var noOfRow = workSheet.Dimension.End.Row;
                var noOfColumn = workSheet.Dimension.End.Column;

                List<string> list = new List<string>();
                for (var i = 2; i <= noOfColumn; i++)
                {
                    list.Add(workSheet.Cells[1, i].Value == null ? string.Empty : workSheet.Cells[1, i].Value.ToString());
                }
                string[] header = list.ToArray();

                DataTable dtResult = new DataTable();
                dtResult.Columns.Add("Zone", typeof(string));
                dtResult.Columns.Add("Weight", typeof(string));
                dtResult.Columns.Add("Rate", typeof(string));

                for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                {
                    for (int i = 0; i < header.Length; i++)
                    {
                        DataRow dtrow = dtResult.NewRow();
                        dtrow["Zone"] = header[i];
                        dtrow["Weight"] = workSheet.Cells[rowIterator, 1].Value == null ? null : workSheet.Cells[rowIterator, 1].Value.ToString();
                        dtrow["Rate"] = workSheet.Cells[rowIterator, i + 2].Value == null ? null : workSheet.Cells[rowIterator, i + 2].Value.ToString();
                        dtResult.Rows.Add(dtrow);
                    }
                }

                var Company = new SqlParameter
                {
                    ParameterName = "Company",
                    DbType = DbType.Int32,
                    Value = companyId
                };
                var UpdatedBy = new SqlParameter
                {
                    ParameterName = "UpdatedBy",
                    DbType = DbType.Int64,
                    Value = SessionHelper.UserId
                };
                var CourierEconomyCost = new SqlParameter
                {
                    ParameterName = "CourierEconomyCost",
                    SqlDbType = SqlDbType.Structured,
                    Value = dtResult,
                    TypeName = "dbo.CourierEconomyCost"
                };
                int imp_result = 0;
                using (quotesEntities db = new quotesEntities())
                {
                    //Delete and Insert Data
                    logger.Info("Courier Economy Cost - Importing Courier Economy Cost. Total entries: " + noOfRow);
                    quotesEntities entityObj = new quotesEntities();
                    imp_result = entityObj.Database.SqlQuery<int>("SP_ImportCourierEconomyCost @Company, @UpdatedBy, @CourierEconomyCost", Company, UpdatedBy, CourierEconomyCost).FirstOrDefault();

                }
                if (imp_result == 0)
                {
                    logger.Info("Courier Economy Cost Import - failed to import data into database.");
                    return new ImportFileResult(false, "failed to import data into database");
                }
                // Test RB
                logger.Info("Courier Express Cost Import - " + imp_result + " entries imported successfully.");
                return new ImportFileResult(true, noOfRow + " entries imported successfully.", null, noOfRow);
            }
        }
    }
}
