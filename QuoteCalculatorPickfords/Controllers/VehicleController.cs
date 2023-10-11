using NLog;
using QuoteCalculatorPickfords.Common;
using QuoteCalculatorPickfords.Data;
using QuoteCalculatorPickfords.Data.Repository;
using QuoteCalculatorPickfords.Helper;
using QuoteCalculatorPickfords.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace QuoteCalculatorPickfords.Controllers
{
    public class VehicleController : Controller
    {
        private static NLog.Logger logger = LogManager.GetCurrentClassLogger();
        #region private variables
        private readonly GenericRepository<tbl_Vehicle> _dbRepositoryVehicle;
        private readonly GenericRepository<tbl_VehicleShippingRates> _dbRepositoryVehicleShippingRate;
        private readonly GenericRepository<tbl_VehicleModelList> _dbRepositoryVehicleModelList;
        private readonly GenericRepository<tbl_Quote> _dbRepositoryQuote;
        //private readonly GenericRepository<tbl_VehicleMake> _dbRepositoryVehicleMake;
        private readonly GenericRepository<tbl_VehicleType> _dbRepositoryVehicleType;
        private readonly GenericRepository<rates_destinations> _dbRepositoryCountry;
        private readonly GenericRepository<tbl_Title> _dbRepositorytitle;
        private readonly GenericRepository<uk> _dbRepositoryUKPostCode;
        private readonly GenericRepository<branch_postcode> _dbRepositoryUKBranchPostCode;
        private readonly GenericRepository<branch> _dbRepositoryBranch;
        private readonly GenericRepository<tbl_EmailTemplate> _dbRepositoryEmailTemplete;
        public readonly GenericRepository<EmailSettings> _dbRepositoryEmailSettings;
        public readonly GenericRepository<tbl_RequestTracking> _dbRepositoryRequestTracking;
        public readonly GenericRepository<tbl_BlockIpOrEmail> _dbRepositoryBlockIpOrEmail;
        public readonly GenericRepository<tbl_WhiteListIp> _dbRepositoryWhiteListIp;
        public readonly GenericRepository<tbl_EmailTemplate> _dbRepositoryEmailTemplate;
        public readonly GenericRepository<tbl_QuoteAmount> _dbRepositoryQuoteAmount;
        public readonly GenericRepository<source> _dbRepositorySource;

        #endregion

        #region Constructor
        public VehicleController()
        {
            _dbRepositoryVehicle = new GenericRepository<tbl_Vehicle>();
            _dbRepositoryVehicleShippingRate = new GenericRepository<tbl_VehicleShippingRates>();
            _dbRepositoryVehicleModelList = new GenericRepository<tbl_VehicleModelList>();
            _dbRepositoryQuote = new GenericRepository<tbl_Quote>();
            //_dbRepositoryVehicleMake = new GenericRepository<tbl_VehicleMake>();
            _dbRepositoryVehicleType = new GenericRepository<tbl_VehicleType>();
            _dbRepositoryCountry = new GenericRepository<rates_destinations>();
            _dbRepositorytitle = new GenericRepository<tbl_Title>();
            _dbRepositoryUKPostCode = new GenericRepository<uk>();
            _dbRepositoryUKBranchPostCode = new GenericRepository<branch_postcode>();
            _dbRepositoryBranch = new GenericRepository<branch>();
            _dbRepositoryEmailTemplete = new GenericRepository<tbl_EmailTemplate>();
            _dbRepositoryEmailSettings = new GenericRepository<EmailSettings>();
            _dbRepositoryRequestTracking = new GenericRepository<tbl_RequestTracking>();
            _dbRepositoryBlockIpOrEmail = new GenericRepository<tbl_BlockIpOrEmail>();
            _dbRepositoryWhiteListIp = new GenericRepository<tbl_WhiteListIp>();
            _dbRepositoryEmailTemplate = new GenericRepository<tbl_EmailTemplate>();
            _dbRepositoryQuoteAmount = new GenericRepository<tbl_QuoteAmount>();
            _dbRepositorySource = new GenericRepository<source>();
        }
        #endregion

        #region Methods

        [HttpGet]
        public ActionResult Index(string countryCode)
        {
            //ViewBag.VehicleMakeList = _dbRepositoryVehicleModelList.GetEntities().Select(m => new { m.MakeName }).Distinct().ToList();
            //ViewBag.VehicleTypeList = _dbRepositoryVehicleType.GetEntities().Select(m => new { m.Id, m.TypeName }).Distinct().ToList();
            //ViewBag.SelectedCountryList = _dbRepositoryCountry.GetEntities().Where(m => m.country == "United Kingdom").Select(m => new { m.id, m.country }).OrderBy(m => new { m.country }).ToList();
            //var countryList = _dbRepositoryCountry.GetEntities().Where(m => m.veh_dest == 1).Select(m => new { m.country_code, m.country }).Distinct().OrderBy(m => new { m.country }).ToList();
            //countryList.Insert(0, new { country_code = "US", country = "USA" });
            //countryList.Insert(0, new { country_code = "AE", country = "UNITED ARAB EMIRATES" });
            //countryList.Insert(0, new { country_code = "TH", country = "THAILAND" });
            //countryList.Insert(0, new { country_code = "ZA", country = "SOUTH AFRICA" });
            //countryList.Insert(0, new { country_code = "SG", country = "SINGAPORE" });
            //countryList.Insert(0, new { country_code = "NZ", country = "NEW ZEALAND" });
            //countryList.Insert(0, new { country_code = "MT", country = "MALTA" });
            //countryList.Insert(0, new { country_code = "MY", country = "MALAYSIA" });
            //countryList.Insert(0, new { country_code = "IN", country = "INDIA" });
            //countryList.Insert(0, new { country_code = "HK", country = "HONG KONG" });
            //countryList.Insert(0, new { country_code = "CY", country = "CYPRUS" });
            //countryList.Insert(0, new { country_code = "CA", country = "CANADA" });
            //countryList.Insert(0, new { country_code = "AU", country = "AUSTRALIA" });
            //ViewBag.CountryList = countryList;
            //ViewBag.TitleList = _dbRepositorytitle.GetEntities().ToList();

            ////tbl_Vehicle obj = new tbl_Vehicle();
            ////obj.PostCode = "HA11BQ";
            //return View();
            return RedirectToAction("Index", "Baggage");
        }

        [HttpPost]
        public ActionResult Index(tbl_Vehicle model)
        {
            SessionHelper.ToCountryCode = model.ToCountryCode;
            SessionHelper.QuoteType = "1";
            if (!ModelState.IsValid)
            {
                ViewBag.VehicleMakeList = _dbRepositoryVehicleModelList.GetEntities().Select(m => new { m.MakeName }).Distinct().ToList();
                ViewBag.VehicleTypeList = _dbRepositoryVehicleType.GetEntities().Select(m => new { m.Id, m.TypeName }).Distinct().ToList();
                ViewBag.SelectedCountryList = _dbRepositoryCountry.GetEntities().Where(m => m.country == "United Kingdom").Select(m => new { m.id, m.country }).OrderBy(m => new { m.country }).ToList();
                ViewBag.CountryList = _dbRepositoryCountry.GetEntities().Where(m => m.veh_dest == 1).Select(m => new { m.country_code, m.country }).Distinct().ToList();
                ViewBag.TitleList = _dbRepositorytitle.GetEntities().ToList();

                return View(model);
            }

            model.FromCountryName = "United Kingdom"; /*Convert.ToInt32(countryObj.id);*/
            string message = string.Empty;
            if (model.Telephone.Substring(0, 1) != "0")
            {
                model.Telephone = "0" + model.Telephone;
            }
            model.EstimatedMoveDate = Convert.ToDateTime(model.EstimatedMoveDate);
            model.CreatedDate = DateTime.Now;
            model.Company = SessionHelper.COMPANY_ID;
            try
            {
                var requestedUrlAry = Request.UrlReferrer.Query.Split('=');
                if (requestedUrlAry[0].Contains("agentcode"))
                {
                    var companyName = string.Empty;
                    if (SessionHelper.COMPANY_ID == 1)
                        companyName = "AP";
                    else if (SessionHelper.COMPANY_ID == 2)
                        companyName = "PF";
                    int sourceId = Convert.ToInt32(requestedUrlAry[1]);
                    var sourceObj = _dbRepositorySource.GetEntities().Where(m => m.sr_apcompany == companyName && m.id == sourceId).FirstOrDefault();

                    if (sourceObj != null)
                    {
                        model.Sr_Code = sourceObj.sr_code;
                        model.Sr_Name = sourceObj.sr_name;
                    }
                }

                message = _dbRepositoryVehicle.Insert(model);
                if (message == "")
                {
                    string ip = Request.UserHostAddress;
                    tbl_RequestTracking requestTracking = new tbl_RequestTracking();
                    requestTracking.Vehicle = model.Email;
                    requestTracking.IpAddress = ip;
                    requestTracking.IsMailSend = false;
                    requestTracking.CreatedDate = DateTime.Now;
                    _dbRepositoryRequestTracking.Insert(requestTracking);

                    string emailSuspendLink = "<a href='" + Url.Action("SetEmailRequest", "Vehicle", new RouteValueDictionary(new { key = "Email", value = model.Email, status = 1 }), System.Web.HttpContext.Current.Request.Url.Scheme) + "'>" + "Suspend" + "</a>";
                    string emailOverrideLink = "<a href='" + Url.Action("SetEmailRequest", "Vehicle", new RouteValueDictionary(new { key = "Email", value = model.Email, status = 2 }), System.Web.HttpContext.Current.Request.Url.Scheme) + "'>" + "Override" + "</a>";
                    string emailWarnLink = "<a href='" + Url.Action("SetEmailRequest", "Vehicle", new RouteValueDictionary(new { key = "Email", value = model.Email, status = 3 }), System.Web.HttpContext.Current.Request.Url.Scheme) + "'>" + "Warn" + "</a>";

                    string ipSuspendLink = "<a href='" + Url.Action("SetEmailRequest", "Vehicle", new RouteValueDictionary(new { key = "Ip", value = ip, status = 1 }), System.Web.HttpContext.Current.Request.Url.Scheme) + "'>" + "Suspend" + "</a>";
                    string ipOverrideLink = "<a href='" + Url.Action("SetEmailRequest", "Vehicle", new RouteValueDictionary(new { key = "Ip", value = ip, status = 2 }), System.Web.HttpContext.Current.Request.Url.Scheme) + "'>" + "Override" + "</a>";
                    string ipWarnLink = "<a href='" + Url.Action("SetEmailRequest", "Vehicle", new RouteValueDictionary(new { key = "Ip", value = ip, status = 3 }), System.Web.HttpContext.Current.Request.Url.Scheme) + "'>" + "Warn" + "</a>";

                    string bodyTemplate = System.IO.File.ReadAllText(Server.MapPath("~/Template/QuoteGuardWarning.html"));
                    var html = "";
                    var statusLink = "";
                    var requestIp = _dbRepositoryRequestTracking.GetEntities().Where(m => m.IpAddress == ip && (DbFunctions.TruncateTime(m.CreatedDate.Value) == DateTime.Today)).ToList();
                    var requestEmail = _dbRepositoryRequestTracking.GetEntities().Where(m => m.Vehicle == model.Email && (DbFunctions.TruncateTime(m.CreatedDate.Value) == DateTime.Today)).ToList();
                    var requestIpEmail = _dbRepositoryRequestTracking.GetEntities().Where(m => m.Vehicle == model.Email && m.IpAddress == ip && (DbFunctions.TruncateTime(m.CreatedDate.Value) == DateTime.Today)).ToList();
                    var Obj = new tbl_BlockIpOrEmail();
                    var isMailSend = false;
                    if (requestIpEmail.Count() >= 10)
                    {
                        if (requestIpEmail.Count() % 10 == 0)
                        {
                            isMailSend = true;
                            var requestTrackings = _dbRepositoryRequestTracking.GetEntities().ToList();
                            for (int i = 0; i < requestTrackings.Count; i++)
                            {
                                requestTrackings[i].IsMailSend = true;
                                _dbRepositoryRequestTracking.Update(requestTrackings[i]);
                            }

                            Obj.Ip = ip;
                            Obj.Email = model.Email;
                            Obj.Status = 1;
                            Obj.CreatedDate = DateTime.Now;
                            _dbRepositoryBlockIpOrEmail.Insert(Obj);
                            var totalIpEmailCount = _dbRepositoryRequestTracking.GetEntities()
                                                    .Where(m => m.IpAddress == ip && m.Vehicle == model.Email).Count();
                            var totalEmailCount = _dbRepositoryRequestTracking.GetEntities().Where(m => m.Vehicle == model.Email).Count();
                            html = "<tr><td> Ip Address: </td><td>" + ip + "</td></tr>" + "<tr><td></td><td>" + requestIpEmail.Count().ToString() +
                                " today " + totalIpEmailCount.ToString() + " total  </td></tr>" + "<tr><td></td><td> Status: Warn </td></tr>" +
                                "<tr><td> Email Address: </td><td>" + model.Email + "</td></tr>" +
                                    "<tr><td></td><td>" + requestEmail.Count().ToString() + " today " +
                                    totalEmailCount.ToString() + " total  </td></tr>" +
                                      "<tr><td></td><td> Status: Warn </td></tr>";
                            statusLink = "<tr><td> To set status for Email Address:</td><td><p>" + emailSuspendLink + " " + emailOverrideLink +
                                            " " + emailWarnLink + "</p></td></tr>" + "<tr><td> To set status for Ip Address</td><td><p>" +
                                            ipSuspendLink + " " + ipOverrideLink + " " + ipWarnLink + "</p></td></tr>";
                        }
                    }
                    else if (requestIp.Count() >= 10)
                    {
                        if (requestIp.Count() % 10 == 0)
                        {
                            isMailSend = true;
                            var requestTrackings = _dbRepositoryRequestTracking.GetEntities().ToList();
                            for (int i = 0; i < requestTrackings.Count; i++)
                            {
                                requestTrackings[i].IsMailSend = true;
                                _dbRepositoryRequestTracking.Update(requestTrackings[i]);
                            }
                            Obj.Ip = ip;
                            Obj.Status = 1;
                            Obj.CreatedDate = DateTime.Now;
                            _dbRepositoryBlockIpOrEmail.Insert(Obj);

                            var totalIpCount = _dbRepositoryRequestTracking.GetEntities().Where(m => m.IpAddress == ip).Count();
                            html = "<tr><td> Ip Address:</td><td>" + ip + "</td></tr>" +
                                    "<tr><td></td><td>" + requestIp.Count().ToString() + " today " +
                                    totalIpCount.ToString() + " total  </td></tr>" +
                                      "<tr><td></td><td> Status: Warn </td></tr>";

                            statusLink = "<tr><td> To set status for Ip Address</td><td><p>" +
                                ipSuspendLink + " " + ipOverrideLink + " " + ipWarnLink + "</p></td></tr>";
                        }
                    }
                    else if (requestEmail.Count() >= 10)
                    {
                        if (requestEmail.Count() % 10 == 0)
                        {
                            isMailSend = true;
                            var requestTrackings = _dbRepositoryRequestTracking.GetEntities().ToList();
                            for (int i = 0; i < requestTrackings.Count; i++)
                            {
                                requestTrackings[i].IsMailSend = true;
                                _dbRepositoryRequestTracking.Update(requestTrackings[i]);
                            }

                            Obj.Email = model.Email;
                            Obj.Status = 1;
                            Obj.CreatedDate = DateTime.Now;
                            _dbRepositoryBlockIpOrEmail.Insert(Obj);

                            var totalEmailCount = _dbRepositoryRequestTracking.GetEntities().Where(m => m.Vehicle == model.Email).Count();
                            var todayIpCount = _dbRepositoryRequestTracking.GetEntities()
                                 .Where(m => m.IpAddress == ip && (DbFunctions.TruncateTime(m.CreatedDate.Value) == DateTime.Today)).Count();
                            var totalemailcount = _dbRepositoryRequestTracking.GetEntities().Where(m => m.Vehicle == model.Email).Count();
                            html = "<tr><td> EmailAddress:</td><td>" + model.Email + "</td></tr>" + "<tr><td></td><td>" + requestEmail.Count().ToString() +
                                   " today " + totalEmailCount.ToString() + " total  </td></tr>" + "<tr><td></td><td> Status: Warn </td></tr>";
                            statusLink = "<tr><td> To set status for Email address:</td><td><p>" + emailSuspendLink + " " + emailOverrideLink
                                            + " " + emailWarnLink + "</p></td></tr>";
                        }
                    }
                    if (isMailSend == true)
                    {
                        bodyTemplate = bodyTemplate.Replace("[@statusLink]", statusLink);
                        bodyTemplate = bodyTemplate.Replace("[@template]", html);
                        Task task = new Task(() => EmailHelper.SendAsyncEmail(null, "Quote guard - warning", bodyTemplate, "LON", "DisplayVehicle", true));
                        task.Start();
                    }

                    var blockIpList = _dbRepositoryBlockIpOrEmail.GetEntities().Where(m => m.Ip == ip).ToList();
                    var blockEmailList = _dbRepositoryBlockIpOrEmail.GetEntities().Where(m => m.Email == model.Email).ToList();

                    if (blockIpList.LastOrDefault() != null)
                    {
                        if (blockIpList.LastOrDefault().Status == 2)
                        {
                            if (blockEmailList.LastOrDefault() != null)
                            {
                                if (blockEmailList.LastOrDefault().Status == 2)
                                {
                                    return RedirectToAction("Quote", "Vehicle", new { @vehicleId = HttpUtility.UrlEncode(model.Id.ToString()) });
                                }
                                else { return RedirectToAction("Success"); }
                            }
                        }
                        else
                            return RedirectToAction("Success");
                    }
                    if (blockEmailList.LastOrDefault() != null)
                    {
                        if (blockEmailList.LastOrDefault().Status == 2)
                        {
                            if (blockIpList.LastOrDefault() != null)
                            {
                                if (blockIpList.LastOrDefault().Status == 2)
                                {
                                    return RedirectToAction("Quote", "Vehicle", new { @vehicleId = HttpUtility.UrlEncode(model.Id.ToString()) });
                                }
                            }
                        }
                        else
                            return RedirectToAction("Success");
                    }
                    return RedirectToAction("Quote", "Vehicle", new { @vehicleId = CommonHelper.Encode(model.Id.ToString()) });

                }
            }

            catch (Exception ex)
            {
                logger.Error(ex);
                message = CommonHelper.GetErrorMessage(ex);
                TempData[CustomEnums.NotifyType.Error.GetDescription()] = message;
            }
            return RedirectToAction("Quote", "Vehicle", new { @vehicleId = CommonHelper.Encode(model.Id.ToString()) });
        }

        public ActionResult SetEmailRequest(string key, string value, int? status)
        {
            string ip = Request.UserHostAddress;
            var ipDetails = _dbRepositoryWhiteListIp.GetEntities().Where(m => m.IpAddress == ip).FirstOrDefault();
            if (ipDetails != null)
            {
                tbl_BlockIpOrEmail blockIpOrEmail = new tbl_BlockIpOrEmail();
                blockIpOrEmail.CreatedDate = DateTime.Now;
                blockIpOrEmail.Status = status;
                if (key == "Ip")
                {
                    blockIpOrEmail.Ip = value;
                    _dbRepositoryBlockIpOrEmail.Insert(blockIpOrEmail);
                }
                if (key == "Email")
                {
                    blockIpOrEmail.Email = value;
                    _dbRepositoryBlockIpOrEmail.Insert(blockIpOrEmail);
                }
                ViewBag.RequestTracking = "Status of " + key + " change successfully";
            }
            else { ViewBag.RequestTracking = "You do not have rights to change the status of " + key; }
            return PartialView("EmailRequestTracking");
        }

        [HttpGet]
        public ActionResult Quote(string vehicleId, bool? isMyQuote)
        {
            var id = Convert.ToInt32(CommonHelper.Decode(vehicleId));
            HttpUtility.UrlDecode(id.ToString());
            var Vehicleobj = _dbRepositoryVehicle.GetEntities().Where(m => m.Id == id).FirstOrDefault();
            //Rates Destination - get the port code
            var CityCode = _dbRepositoryCountry.GetEntities().Where(x => x.country_code == Vehicleobj.ToCountryCode && (x.city == Vehicleobj.CityName)).FirstOrDefault();
            var Shippinglist = _dbRepositoryVehicleShippingRate.GetEntities().Where(x => x.Code == CityCode.port_code).FirstOrDefault();
            var Modellist = _dbRepositoryVehicleModelList.GetEntities().Where(x => x.MakeName == Vehicleobj.VehicleMakeName && (x.ModelName == Vehicleobj.VehicleModelName)).FirstOrDefault();

            VehicleQuoteModel Quoteobj = new VehicleQuoteModel();
            Quoteobj.VehicleId = id;
            string status = string.Empty;
            var vehicleObjModel = CustomRepository.GetInfoForQuote(id);
            if (vehicleObjModel != null)
            {
                var branchDtl = _dbRepositoryBranch.GetEntities().Where(m => m.br_id == vehicleObjModel.BranchId).FirstOrDefault();
                Quoteobj.BranchName = branchDtl.br_branch;
                Quoteobj.Firstname = vehicleObjModel.Firstname;
                Quoteobj.Lastname = vehicleObjModel.Lastname;
                Quoteobj.Title = vehicleObjModel.Title;
                Quoteobj.EstimatedMoveDate = vehicleObjModel.EstimatedMoveDate;
                Quoteobj.MakeName = vehicleObjModel.MakeName;
                Quoteobj.ModelName = vehicleObjModel.ModelName;
                Quoteobj.ToCountryName = vehicleObjModel.ToCountryName;
                Quoteobj.FromCountryName = vehicleObjModel.FromCountryName;
                Quoteobj.CityName = vehicleObjModel.CityName;
                Quoteobj.QuoteNo = "AP" + vehicleObjModel.QuoteNo;
                Quoteobj.IsFCL = Vehicleobj.IsFCL == true ? true : false;
                Quoteobj.IsGPG = Vehicleobj.IsGRP == true ? true : false;
            }

            if (Shippinglist != null && Modellist != null && Modellist.FitsInContainer == "OK")
            {
                var emailTemplateObj = _dbRepositoryEmailTemplate.GetEntities().Where(m => m.ServiceId == 1003).FirstOrDefault();
                string bodyTemplate = string.Empty;
                if (emailTemplateObj != null)
                {
                    bodyTemplate = emailTemplateObj.HtmlContent;
                }
                //var ratesObj = _dbRepositoryCountry.GetEntities().Where(m => m.country == Quoteobj.ToCountryName).FirstOrDefault();

                //bodyTemplate = bodyTemplate.Replace("#logo#", System.Web.Hosting.HostingEnvironment.MapPath("~/Content/images/logo.png"));
                //bodyTemplate = bodyTemplate.Replace("#logo#", Server.MapPath("~") + @"Content\images\logo.png");
                //bodyTemplate = bodyTemplate.Replace("#title#", Quoteobj.Title);
                bodyTemplate = bodyTemplate.Replace("#custName#", Quoteobj.Firstname);
                //bodyTemplate = bodyTemplate.Replace("#make#", Quoteobj.MakeName);
                //bodyTemplate = bodyTemplate.Replace("#model#", Quoteobj.ModelName);
                //bodyTemplate = bodyTemplate.Replace("#branchName#", Quoteobj.BranchName);
                //bodyTemplate = bodyTemplate.Replace("#toCity#", Quoteobj.CityName);
                //bodyTemplate = bodyTemplate.Replace("#toCountry#", Quoteobj.ToCountryName);
                //bodyTemplate = bodyTemplate.Replace("#fromCountry_Code#", ratesObj == null ? string.Empty : ratesObj.country_code);

                if (vehicleObjModel.BranchId == 1)
                {
                    double TotalSum = ((Shippinglist.Groupage_GLA_PerMt * Modellist.Length) / 1000).Value;
                    Quoteobj.FCL = Convert.ToDouble(string.Format("{0:#.00}", Shippinglist.FCL_GLA.Value));
                    Quoteobj.GroupageTotal = Convert.ToDouble(string.Format("{0:#.00}", (TotalSum + Shippinglist.Groupage_GLA_L_S).Value));
                    Quoteobj.FCLvalue = Shippinglist.FCL;
                    Quoteobj.GPGvalue = Shippinglist.GPG;
                    if (isMyQuote == true)
                    {
                        SessionHelper.ToCountryCode = Vehicleobj.ToCountryCode;
                        SessionHelper.QuoteType = "1";
                        var CustomerRefNo = _dbRepositoryQuoteAmount.GetEntities().Where(m => m.QuoteId == id && m.MoveType == "EXV").FirstOrDefault();
                        ViewBag.ReferenceNo = string.Concat(CustomerRefNo.CustomerReferenceNo + "/" + CustomerRefNo.CustomerQuoteNo);
                        return View(Quoteobj);
                    }
                    status = UpdateVehicleDetails(Quoteobj.VehicleId, Quoteobj.FCL, Quoteobj.GroupageTotal);
                    var QuoteAmountobj = _dbRepositoryQuoteAmount.GetEntities().Where(x => x.QuoteId == id && x.MoveType == "EXV").FirstOrDefault();
                    //bodyTemplate = bodyTemplate.Replace("#reference#", QuoteAmountobj.CustomerReferenceNo + "/" + QuoteAmountobj.CustomerQuoteNo);
                    //bodyTemplate = bodyTemplate.Replace("#FCLprice#", (Math.Round(Quoteobj.FCL, 2, MidpointRounding.ToEven)).ToString());
                    //bodyTemplate = bodyTemplate.Replace("#Groupageprice#", (Math.Round(Quoteobj.GroupageTotal, 2, MidpointRounding.ToEven)).ToString());
                    Task task = new Task(() => EmailHelper.SendAsyncEmail(Vehicleobj.Email, emailTemplateObj.Subject, bodyTemplate, "GLA", "DisplayVehicle", true));
                    task.Start();

                    return View(Quoteobj);
                }
                if (vehicleObjModel.BranchId == 2)
                {
                    double TotalSum = ((Shippinglist.Groupage_Lon_PerMt * Modellist.Length) / 1000).Value;
                    Quoteobj.FCL = Convert.ToDouble(string.Format("{0:#.00}", Shippinglist.FCL_LON.Value));
                    Quoteobj.GroupageTotal = Convert.ToDouble(string.Format("{0:#.00}", (TotalSum + Shippinglist.Groupage_Lon_L_S).Value));
                    Quoteobj.FCLvalue = Shippinglist.FCL;
                    Quoteobj.GPGvalue = Shippinglist.GPG;
                    if (isMyQuote == true)
                    {
                        SessionHelper.ToCountryCode = Vehicleobj.ToCountryCode;
                        SessionHelper.QuoteType = "1";
                        var CustomerRefNo = _dbRepositoryQuoteAmount.GetEntities().Where(m => m.QuoteId == id && m.MoveType == "EXV").FirstOrDefault();
                        ViewBag.ReferenceNo = string.Concat(CustomerRefNo.CustomerReferenceNo + "/" + CustomerRefNo.CustomerQuoteNo);
                        return View(Quoteobj);
                    }
                    status = UpdateVehicleDetails(Quoteobj.VehicleId, Quoteobj.FCL, Quoteobj.GroupageTotal);
                    var QuoteAmountobj = _dbRepositoryQuoteAmount.GetEntities().Where(x => x.QuoteId == id && x.MoveType == "EXV").FirstOrDefault();
                    //bodyTemplate = bodyTemplate.Replace("#reference#", QuoteAmountobj.CustomerReferenceNo + "/" + QuoteAmountobj.CustomerQuoteNo);
                    //bodyTemplate = bodyTemplate.Replace("#FCLprice#", Quoteobj.FCL.ToString());
                    //bodyTemplate = bodyTemplate.Replace("#Groupageprice#", Quoteobj.GroupageTotal.ToString());
                    Task task = new Task(() => EmailHelper.SendAsyncEmail(Vehicleobj.Email, emailTemplateObj.Subject, bodyTemplate, "LON", "DisplayVehicle", true));
                    task.Start();
                    return View(Quoteobj);
                }
                if (vehicleObjModel.BranchId == 3)
                {
                    double TotalSum = ((Shippinglist.Groupage_MCR_PerMt * Modellist.Length) / 1000).Value;
                    Quoteobj.FCL = Convert.ToDouble(string.Format("{0:#.00}", Shippinglist.FCL_MCR.Value));
                    Quoteobj.GroupageTotal = Convert.ToDouble(string.Format("{0:#.00}", (TotalSum + Shippinglist.Groupage_MCR_L_S).Value));
                    Quoteobj.FCLvalue = Shippinglist.FCL;
                    Quoteobj.GPGvalue = Shippinglist.GPG;
                    if (isMyQuote == true)
                    {
                        SessionHelper.ToCountryCode = Vehicleobj.ToCountryCode;
                        SessionHelper.QuoteType = "1";
                        var CustomerRefNo = _dbRepositoryQuoteAmount.GetEntities().Where(m => m.QuoteId == id && m.MoveType == "EXV").FirstOrDefault();
                        ViewBag.ReferenceNo = string.Concat(CustomerRefNo.CustomerReferenceNo + "/" + CustomerRefNo.CustomerQuoteNo);
                        return View(Quoteobj);
                    }
                    status = UpdateVehicleDetails(Quoteobj.VehicleId, Quoteobj.FCL, Quoteobj.GroupageTotal);
                    var QuoteAmountobj = _dbRepositoryQuoteAmount.GetEntities().Where(x => x.QuoteId == id && x.MoveType == "EXV").FirstOrDefault();
                    //bodyTemplate = bodyTemplate.Replace("#reference#", QuoteAmountobj.CustomerReferenceNo + "/" + QuoteAmountobj.CustomerQuoteNo);
                    //bodyTemplate = bodyTemplate.Replace("#FCLprice#", Quoteobj.FCL.ToString());
                    //bodyTemplate = bodyTemplate.Replace("#Groupageprice#", Quoteobj.GroupageTotal.ToString());
                    Task task = new Task(() => EmailHelper.SendAsyncEmail(Vehicleobj.Email, emailTemplateObj.Subject, bodyTemplate, "MAN", "DisplayVehicle", true));
                    task.Start();
                    return View(Quoteobj);
                }
            }
            else
            {
                long customerReferenceNo = CustomRepository.GetNextCustomerRefNo("EXV", Vehicleobj.Email, Vehicleobj.ToCountryCode, Vehicleobj.CityName);
                var customerQuotes = _dbRepositoryQuoteAmount.GetEntities().Where(x => x.CustomerReferenceNo == customerReferenceNo);
                int customerQuoteNo = 1;

                if (customerQuotes != null)
                {
                    var lastCustQt = customerQuotes.OrderByDescending(x => x.CustomerQuoteNo).FirstOrDefault();
                    if (lastCustQt != null)
                        customerQuoteNo = (lastCustQt.CustomerQuoteNo ?? 0) + 1;
                    else
                        customerQuoteNo = 1;
                }

                tbl_QuoteAmount QuoteAmounts = new tbl_QuoteAmount();
                QuoteAmounts.QuoteId = Convert.ToInt32(id);
                QuoteAmounts.MoveType = "EXV";
                QuoteAmounts.QuoteAmount = 0;
                QuoteAmounts.ShippingType = "ENQUIRY";
                QuoteAmounts.ShippingTypeDescription = "ENQUIRY";
                QuoteAmounts.TransitionTime = "";
                QuoteAmounts.CreatedOn = System.DateTime.Now;
                QuoteAmounts.CustomerReferenceNo = customerReferenceNo;
                QuoteAmounts.CustomerQuoteNo = customerQuoteNo;
                QuoteAmounts.QuoteSeqNo = 0;
                QuoteAmounts.Company = SessionHelper.COMPANY_ID;
                string insert = _dbRepositoryQuoteAmount.Insert(QuoteAmounts);

                SP_GetXmlData_Result xmlResult = CustomRepository.GetXmlData(id);
                if (xmlResult != null)
                {
                    if ((xmlResult.FCL == 0 || xmlResult.FCL == null) && (xmlResult.GPG == 0 || xmlResult.GPG == null))
                    {
                        string file = string.Concat(customerReferenceNo + "/" + customerQuoteNo);
                        ViewBag.ReferenceNo = file;
                        string path = XMLHelper.GenerateXmlEnquiryFile(xmlResult, file);
                        XMLHelper.XmlAPICall(path, 0); //Default id = 0
                    }
                }
                return View(Quoteobj);
            }
            //else if (Shippinglist != null && Modellist == null)
            //{
            //    Quoteobj.FCL = Shippinglist.FCL_LON.Value;
            //    Quoteobj.FCLvalue = Shippinglist.FCL;
            //    Quoteobj.GPGvalue = Shippinglist.GPG;
            //    UpdateVehicleDetails(Quoteobj.VehicleId, Quoteobj.FCL, Quoteobj.GroupageTotal);
            //    return View(Quoteobj);
            //}
            //else if (Modellist != null && Shippinglist == null)
            //{
            //    double TotalSum = ((Shippinglist.Groupage_Lon_PerMt * Modellist.Length) / 1000).Value;
            //    Quoteobj.GroupageTotal = (TotalSum + Shippinglist.Groupage_Lon_L_S).Value;
            //    Quoteobj.FCLvalue = Shippinglist.FCL;
            //    Quoteobj.GPGvalue = Shippinglist.GPG;
            //    UpdateVehicleDetails(Quoteobj.VehicleId, Quoteobj.FCL, Quoteobj.GroupageTotal);
            //    return View(Quoteobj);
            //}
            //else { }

            status = UpdateVehicleDetails(Quoteobj.VehicleId, Quoteobj.FCL, Quoteobj.GroupageTotal);
            return View(Quoteobj);
        }

        [HttpPost]
        public string Quote(int? VehicleId, string str)
        {
            string message = string.Empty;
            string emailStatus = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(str))
                {
                    sp_GetdataForEmailSending_Result xmlEmailResult;
                    tbl_Vehicle model = _dbRepositoryVehicle.SelectById(VehicleId);
                    if (str.Contains("FCL"))
                    {
                        model.IsFCL = true;
                        model.IsGRP = false;
                        message = _dbRepositoryVehicle.Update(model);
                        xmlEmailResult = CustomRepository.GetQuoteData(VehicleId, 1, "FCL");
                    }
                    else
                    {
                        model.IsFCL = false;
                        model.IsGRP = true;
                        message = _dbRepositoryVehicle.Update(model);
                        xmlEmailResult = CustomRepository.GetQuoteData(VehicleId, 1, "GPG");
                    }

                    //sp_GetdataForEmailSending_Result xmlEmailResult = CustomRepository.GetQuoteData(VehicleId, 1);
                    SP_GetXmlData_Result xmlResult = CustomRepository.GetXmlData(VehicleId);
                    xmlEmailResult.quoteName = "vehicle";
                    List<string> xmlFiles = new List<string>();
                    if (str.Contains("FCL"))
                    {
                        var CustomerRefNo = _dbRepositoryQuoteAmount.GetEntities().Where(m => m.QuoteId == VehicleId && m.MoveType == "EXV" && m.ShippingType == "FCL").FirstOrDefault();
                        string file = string.Concat(CustomerRefNo.CustomerReferenceNo + "/" + CustomerRefNo.CustomerQuoteNo + "." + CustomerRefNo.QuoteSeqNo);
                        string FCL = XMLHelper.GenerateXmlFCLFile(xmlResult, file, true);
                        xmlFiles.Add(FCL);
                    }
                    else
                    {
                        var CustomerRefNo = _dbRepositoryQuoteAmount.GetEntities().Where(m => m.QuoteId == VehicleId && m.MoveType == "EXV" && m.ShippingType == "GPG").FirstOrDefault();
                        string file = string.Concat(CustomerRefNo.CustomerReferenceNo + "/" + CustomerRefNo.CustomerQuoteNo + "." + CustomerRefNo.QuoteSeqNo);
                        string GPG = XMLHelper.GenerateXmlGDPFile(xmlResult, file, true);
                        xmlFiles.Add(GPG);
                    }

                    if (xmlFiles.Count > 0)
                    {
                        Task tasks = new Task(() => SendXmlFies(xmlFiles));
                        tasks.Start();
                    }
                    Task task = new Task(() => emailStatus = XMLHelper.SendEmail(xmlEmailResult, 1));
                    Task taskquote = new Task(() => emailStatus = XMLHelper.SendEmail(xmlEmailResult, 2));
                    task.Start();
                    taskquote.Start();
                }
                return message;
            }
            catch (Exception e)
            {
                logger.Error(e);
                return e.Message;
            }
        }

        private void SendXmlFies(List<string> xmlFiles)
        {
            foreach (string xmlFile in xmlFiles)
            {
                XMLHelper.XmlAPICall(xmlFile, 0);
            }
        }

        public ActionResult Success()
        {
            return View();
        }

        public JsonResult GetVehicleModelList(string MakeName)
        {
            var MakeNameParameter = new SqlParameter
            {
                ParameterName = "MakeName",
                DbType = DbType.String,
                Value = MakeName
            };
            quotesEntities obj = new quotesEntities();
            MakeNameListModel model = new MakeNameListModel();
            var vehicleModelList = obj.Database.SqlQuery<MakeNameListModel>("GetVehicleModelList @MakeName", MakeNameParameter).ToList();
            return Json(vehicleModelList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCityListByCountryId(string countryCode)
        {

            var countryCodeParameter = new SqlParameter
            {
                ParameterName = "CountryCode",
                DbType = DbType.String,
                Value = countryCode
            };
            quotesEntities obj = new quotesEntities();
            CityListModel model = new CityListModel();
            var vehicleCityList = obj.Database.SqlQuery<CityListModel>("GetCity @CountryCode", countryCodeParameter).ToList();
            return Json(vehicleCityList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckValidPostCode(string postCode)
        {
            var code = postCode;
            ViewBag.BranchPostCode = 0;
            var postCodeObj = _dbRepositoryUKPostCode.GetEntities().Where(m => m.zip.Replace(" ", "") == postCode.Replace(" ", "")).FirstOrDefault();
            if (postCodeObj != null)
            {
                postCode = postCode.Replace(" ", "");
                int postCodeLength = postCode.Length;
                var zipCode = postCode.Substring(0, postCodeLength - 3);
                for (int i = 4; i >= 2; i--)
                {
                    var splitedCode = postCode.Substring(0, i);
                    var branchPostCode = _dbRepositoryUKBranchPostCode.GetEntities().Where(m => m.postcode == splitedCode).FirstOrDefault();
                    if (branchPostCode != null)
                    {
                        if (branchPostCode.vehicle_branch_id != 0)
                        {
                            ViewBag.BranchPostCode = branchPostCode.vehicle_branch_id;
                            return Json(branchPostCode.vehicle_branch_id, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }
            if (ViewBag.BranchPostCode == 0)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        public string UpdateVehicleDetails(int? id, double? fcl, double? gpg)
        {
            if (id == null)
                return null;

            string message = string.Empty;
            string emailStatus = string.Empty;
            tbl_Vehicle vehicle = _dbRepositoryVehicle.SelectById(id);
            vehicle.FCL = fcl;
            vehicle.GPG = gpg;

            //Get Volume from VehicleModelList Table
            tbl_VehicleModelList vehicleModelList = _dbRepositoryVehicleModelList.GetEntities().Where(m => m.MakeName.Contains(vehicle.VehicleMakeName) && m.ModelName.Contains(vehicle.VehicleModelName)).FirstOrDefault();
            if (vehicleModelList != null)
            {
                vehicle.Volume_FCL = vehicleModelList.Volume_FCL;
                vehicle.Volume_GRP = vehicleModelList.Volume_GRP;
            }



            int QuoteSeqNo = 0;
            long customerReferenceNo = CustomRepository.GetNextCustomerRefNo("EXV", vehicle.Email, vehicle.ToCountryCode, vehicle.CityName);
            var customerQuotes = _dbRepositoryQuoteAmount.GetEntities().Where(x => x.CustomerReferenceNo == customerReferenceNo);
            int customerQuoteNo = 1;

            if (customerQuotes != null)
            {
                var lastCustQt = customerQuotes.OrderByDescending(x => x.CustomerQuoteNo).FirstOrDefault();
                if (lastCustQt != null)
                    customerQuoteNo = (lastCustQt.CustomerQuoteNo ?? 0) + 1;
                else
                    customerQuoteNo = 1;
            }

            var quoteAmounts = _dbRepositoryQuoteAmount.GetEntities().Where(m => m.QuoteId == id && m.MoveType == "EXV").ToList();
            foreach (var quoteAmount in quoteAmounts)
                _dbRepositoryQuoteAmount.Delete(quoteAmount.Id);

            tbl_QuoteAmount QuoteAmount = new tbl_QuoteAmount();

            if (fcl != null && fcl > 0)
            {
                QuoteAmount = new tbl_QuoteAmount();
                QuoteAmount.QuoteId = Convert.ToInt32(id);
                QuoteAmount.MoveType = "EXV";
                QuoteAmount.QuoteAmount = Convert.ToDecimal(fcl);
                QuoteAmount.ShippingType = "FCL";
                QuoteAmount.ShippingTypeDescription = "By exclusive container";
                QuoteAmount.TransitionTime = "";
                QuoteAmount.CreatedOn = System.DateTime.Now;
                QuoteAmount.CustomerReferenceNo = customerReferenceNo;
                QuoteSeqNo += 1;
                QuoteAmount.CustomerQuoteNo = customerQuoteNo;
                QuoteAmount.QuoteSeqNo = QuoteSeqNo;
                string insert = _dbRepositoryQuoteAmount.Insert(QuoteAmount);
            }
            if (gpg != null && gpg > 0)
            {
                QuoteAmount = new tbl_QuoteAmount();
                QuoteAmount.QuoteId = Convert.ToInt32(id);
                QuoteAmount.MoveType = "EXV";
                QuoteAmount.QuoteAmount = Convert.ToDecimal(gpg);
                QuoteAmount.ShippingType = "GPG";
                QuoteAmount.ShippingTypeDescription = "By shared container";
                QuoteAmount.TransitionTime = "";
                QuoteAmount.CreatedOn = System.DateTime.Now;
                QuoteAmount.CustomerReferenceNo = customerReferenceNo;
                QuoteSeqNo += 1;
                QuoteAmount.CustomerQuoteNo = customerQuoteNo;
                QuoteAmount.QuoteSeqNo = QuoteSeqNo;
                string insert = _dbRepositoryQuoteAmount.Insert(QuoteAmount);
            }

            message = _dbRepositoryVehicle.Update(vehicle);

            //XML Generation and Make SOAP Request
            if (string.IsNullOrEmpty(message))
            {
                //SP_GetXmlData_Result xmlResult = CustomRepository.GetXmlData(id);
                SP_GetXmlData_Result xmlResult = CustomRepository.GetXmlData(id);
                //sp_GetdataForEmailSending_Result quoteObj = CustomRepository.GetQuoteData(id, 1);

                if (xmlResult != null)
                {
                    if (xmlResult.FCL != 0)
                    {
                        var CustomerRefNo = _dbRepositoryQuoteAmount.GetEntities().Where(m => m.QuoteId == id && m.MoveType == "EXV" && m.ShippingType == "FCL").FirstOrDefault();
                        string file = string.Concat(CustomerRefNo.CustomerReferenceNo + "/" + CustomerRefNo.CustomerQuoteNo + "." + CustomerRefNo.QuoteSeqNo);
                        string path = XMLHelper.GenerateXmlFCLFile(xmlResult, file);
                        ViewBag.ReferenceNo = string.Concat(CustomerRefNo.CustomerReferenceNo + "/" + CustomerRefNo.CustomerQuoteNo);
                        XMLHelper.XmlAPICall(path, 0); //Default id = 0
                        //GetCitiesByCountryAsync(path);
                        //Task task = new Task(() => emailStatus = XMLHelper.SendEmail(quoteObj, "User"));
                        //task.Start();
                        //XMLHelper.XmlAPICall(path,0); //Default id = 0
                    }
                    if (xmlResult.GPG != 0)
                    {
                        var CustomerRefNo = _dbRepositoryQuoteAmount.GetEntities().Where(m => m.QuoteId == id && m.MoveType == "EXV" && m.ShippingType == "GPG").FirstOrDefault();
                        string file = string.Concat(CustomerRefNo.CustomerReferenceNo + "/" + CustomerRefNo.CustomerQuoteNo + "." + CustomerRefNo.QuoteSeqNo);
                        string path = XMLHelper.GenerateXmlGDPFile(xmlResult, file);
                        ViewBag.ReferenceNo = string.Concat(CustomerRefNo.CustomerReferenceNo + "/" + CustomerRefNo.CustomerQuoteNo);
                        XMLHelper.XmlAPICall(path, 0); //Default id = 0
                        //GetCitiesByCountryAsync(path);
                        //emailStatus = XMLHelper.SendEmail(quoteObj, "User");
                        //XMLHelper.XmlAPICall(path,0); //Default id = 0
                    }
                }
                return emailStatus;
            }
            return emailStatus;
        }
        //public async Task<ActionResult> GetCitiesByCountryAsync(string path) // Asynchronous Action Method
        //{
        //    //SP_GetXmlData_Result xmlResult = CustomRepository.GetXmlData(id);
        //    //string path = XMLHelper.GenerateXmlFCLFile(xmlResult);
        //    XMLHelper.XmlAPICall(path, 0); //Default id = 0
        //    return View();

        //}
        public ActionResult ThankYou()
        {
            return View();
        }
        #endregion
    }
}