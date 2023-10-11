using NLog;
using QuoteCalculator.Common;
using QuoteCalculator.Data;
using QuoteCalculator.Data.Repository;
using QuoteCalculator.Helper;
using QuoteCalculator.Models;
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

namespace QuoteCalculator.Controllers
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
        public readonly GenericRepository<tbl_EmailSettings> _dbRepositoryEmailSettings;
        public readonly GenericRepository<tbl_RequestTracking> _dbRepositoryRequestTracking;
        public readonly GenericRepository<tbl_BlockIpOrEmail> _dbRepositoryBlockIpOrEmail;
        public readonly GenericRepository<tbl_WhiteListIp> _dbRepositoryWhiteListIp;
        public readonly GenericRepository<tbl_EmailTemplate> _dbRepositoryEmailTemplate;
        public readonly GenericRepository<tbl_QuoteAmount> _dbRepositoryQuoteAmount;
        public readonly GenericRepository<source> _dbRepositorySource;
        public readonly GenericRepository<tbl_CountryCode> _dbRepositoryCountryCode;
        public readonly GenericRepository<tbl_GuideLink> _dbRepositoryGuideLink;

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
            _dbRepositoryEmailSettings = new GenericRepository<tbl_EmailSettings>();
            _dbRepositoryRequestTracking = new GenericRepository<tbl_RequestTracking>();
            _dbRepositoryBlockIpOrEmail = new GenericRepository<tbl_BlockIpOrEmail>();
            _dbRepositoryWhiteListIp = new GenericRepository<tbl_WhiteListIp>();
            _dbRepositoryEmailTemplate = new GenericRepository<tbl_EmailTemplate>();
            _dbRepositoryQuoteAmount = new GenericRepository<tbl_QuoteAmount>();
            _dbRepositorySource = new GenericRepository<source>();
            _dbRepositoryCountryCode = new GenericRepository<tbl_CountryCode>();
            _dbRepositoryGuideLink = new GenericRepository<tbl_GuideLink>();
        }
        #endregion

        #region Methods

        [HttpGet]
        public ActionResult Index(string countryCode, string vehicleId)
        {
            tbl_Vehicle model = new tbl_Vehicle();
            try
            {
                if (HttpContext.Request.Url.GetLeftPart(UriPartial.Authority) == System.Configuration.ConfigurationManager.AppSettings["PicfordUrl"])
                {
                    //Pickfords
                    SessionHelper.COMPANY_ID = 2;
                    return RedirectToAction("Index", "Baggage");
                }
                else if (HttpContext.Request.Url.GetLeftPart(UriPartial.Authority) == System.Configuration.ConfigurationManager.AppSettings["ExcessUrl"])
                {
                    //Excess
                    SessionHelper.COMPANY_ID = 3;
                    return RedirectToAction("Index", "Baggage");
                }
                else
                {
                    //Anglo
                    SessionHelper.COMPANY_ID = 1;
                    var id = 0;
                    if (vehicleId != null)
                    {
                        id = Convert.ToInt32(CommonHelper.Decode(vehicleId));
                    }
                    ViewBag.VehicleMakeList = _dbRepositoryVehicleModelList.GetEntities().Select(m => new { m.MakeName }).Distinct().ToList();
                    ViewBag.VehicleTypeList = _dbRepositoryVehicleType.GetEntities().Select(m => new { m.Id, m.TypeName }).Distinct().ToList();
                    ViewBag.SelectedCountryList = _dbRepositoryCountry.GetEntities().Where(m => m.country == "United Kingdom").Select(m => new { m.id, m.country }).ToList();
                    var countryList = _dbRepositoryCountry.GetEntities().Where(m => m.veh_dest == 1).Select(m => new { m.country_code, m.country }).Distinct().ToList();
                    countryList.Insert(0, new { country_code = "US", country = "USA" });
                    countryList.Insert(0, new { country_code = "AE", country = "UNITED ARAB EMIRATES" });
                    countryList.Insert(0, new { country_code = "TH", country = "THAILAND" });
                    countryList.Insert(0, new { country_code = "ZA", country = "SOUTH AFRICA" });
                    countryList.Insert(0, new { country_code = "SG", country = "SINGAPORE" });
                    countryList.Insert(0, new { country_code = "NZ", country = "NEW ZEALAND" });
                    countryList.Insert(0, new { country_code = "MT", country = "MALTA" });
                    countryList.Insert(0, new { country_code = "MY", country = "MALAYSIA" });
                    countryList.Insert(0, new { country_code = "IN", country = "INDIA" });
                    countryList.Insert(0, new { country_code = "HK", country = "HONG KONG" });
                    countryList.Insert(0, new { country_code = "CY", country = "CYPRUS" });
                    countryList.Insert(0, new { country_code = "CA", country = "CANADA" });
                    countryList.Insert(0, new { country_code = "AU", country = "AUSTRALIA" });
                    ViewBag.CountryList = countryList;
                    ViewBag.TitleList = _dbRepositorytitle.GetEntities().OrderBy(x => x.DisplayOrder).ToList();
                    ViewBag.CountryCodeList = _dbRepositoryCountryCode.GetEntities().Distinct().OrderBy(m => m.CountryName).ToList();
                    model.CountryCode = "+44";

                    model.EstimatedMoveDate = DateTime.Now;
                    if (id > 0)
                    {
                        model = _dbRepositoryVehicle.GetEntities().Where(m => m.Id == id).FirstOrDefault();
                    }
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                TempData[CustomEnums.NotifyType.Error.GetDescription()] = CommonHelper.GetErrorMessage(ex);
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(tbl_Vehicle model)
        {
            try
            {
                SessionHelper.ToCountryCode = model.ToCountryCode;
                SessionHelper.QuoteType = "1";
                model.Firstname = model.Firstname.First().ToString().ToUpper() + model.Firstname.Substring(1);
                model.Lastname = model.Lastname.First().ToString().ToUpper() + model.Lastname.Substring(1);
                if (!ModelState.IsValid)
                {
                    ViewBag.VehicleMakeList = _dbRepositoryVehicleModelList.GetEntities().Select(m => new { m.MakeName }).Distinct().ToList();
                    ViewBag.VehicleTypeList = _dbRepositoryVehicleType.GetEntities().Select(m => new { m.Id, m.TypeName }).Distinct().ToList();
                    ViewBag.SelectedCountryList = _dbRepositoryCountry.GetEntities().Where(m => m.country == "United Kingdom").Select(m => new { m.id, m.country }).ToList();
                    ViewBag.CountryList = _dbRepositoryCountry.GetEntities().Where(m => m.veh_dest == 1).Select(m => new { m.country_code, m.country }).Distinct().ToList();
                    ViewBag.TitleList = _dbRepositorytitle.GetEntities().ToList();
                    ViewBag.CountryCodeList = _dbRepositoryCountryCode.GetEntities().Distinct().ToList();

                    return View(model);
                }

                model.FromCountryName = "United Kingdom"; /*Convert.ToInt32(countryObj.id);*/
                string message = string.Empty;
                //if (model.Telephone.Substring(0, 1) != "0")
                //{
                //    model.Telephone = "0" + model.Telephone;
                //}
                model.Telephone = model.Telephone;
                model.EstimatedMoveDate = Convert.ToDateTime(model.EstimatedMoveDate);
                model.CreatedDate = DateTime.Now;
                model.Company = SessionHelper.COMPANY_ID;
                if (model.PostCode != null && model.PostCode != string.Empty)
                {
                    string pc = model.PostCode.Replace(" ", "");
                    pc = pc.Substring(0, pc.Length - 3) + " " + pc.Substring(pc.Length - 3, 3);
                    var postCodeObj = _dbRepositoryUKPostCode.GetEntities().Where(m => m.zip == pc).FirstOrDefault();
                    model.PostCode = postCodeObj.zip;
                }

                try
                {
                    var sURLVariables = Request.UrlReferrer.Query.Split('&');
                    var isAgentcode = false;
                    int sourceId = 0;
                    if (sURLVariables.Count() == 1)
                    {
                        isAgentcode = sURLVariables[0].Contains("agentcode");
                        if (isAgentcode == true)
                            sourceId = Convert.ToInt32(sURLVariables[0].Split('=')[1]);
                        isAgentcode = true;
                    }
                    if (sURLVariables.Count() == 2)
                    {
                        isAgentcode = sURLVariables[1].Contains("agentcode");
                        if (isAgentcode == true)
                            sourceId = Convert.ToInt32(sURLVariables[1].Split('=')[1]);
                        isAgentcode = true;
                    }
                    if (isAgentcode == true)
                    {
                        var companyName = string.Empty;
                        if (SessionHelper.COMPANY_ID == 1)
                            companyName = "AP";
                        else if (SessionHelper.COMPANY_ID == 2)
                            companyName = "PF";

                        var sourceObj = _dbRepositorySource.GetEntities().Where(m => m.sr_apcompany == companyName && m.id == sourceId).FirstOrDefault();
                        if (sourceObj != null)
                        {
                            model.Sr_Code = sourceObj.sr_code;
                            model.Sr_Name = sourceObj.sr_name;
                        }
                    }
                    model.VehicleType = _dbRepositoryVehicleType.GetEntities().Select(m => m.Id).FirstOrDefault();

                    message = _dbRepositoryVehicle.Insert(model);
                    SessionHelper.VehicleId = model.Id;
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
                        if (requestIpEmail.Count() >= 1000)
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
                        else if (requestIp.Count() >= 1000)
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
                        else if (requestEmail.Count() >= 1000)
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
                                        return RedirectToAction("Quote", "Vehicle", new { @vehicleId = CommonHelper.Encode(model.Id.ToString()) });
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
                                        return RedirectToAction("Quote", "Vehicle", new { @vehicleId = CommonHelper.Encode(model.Id.ToString()) });
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
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                TempData[CustomEnums.NotifyType.Error.GetDescription()] = CommonHelper.GetErrorMessage(ex);
            }
            return RedirectToAction("Quote", "Vehicle", new { @vehicleId = CommonHelper.Encode(model.Id.ToString()) });
        }

        public ActionResult SetEmailRequest(string key, string value, int? status)
        {
            try
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
            catch (Exception ex)
            {
                logger.Error(ex);
                TempData[CustomEnums.NotifyType.Error.GetDescription()] = CommonHelper.GetErrorMessage(ex);
            }
            return PartialView("EmailRequestTracking");
        }

        [HttpGet]
        public async Task<ActionResult> Quote(string vehicleId, bool? isMyQuote)
        {
            VehicleQuoteModel Quoteobj = new VehicleQuoteModel();
            try
            {
                var id = Convert.ToInt32(CommonHelper.Decode(vehicleId));
                var Vehicleobj = _dbRepositoryVehicle.GetEntities().Where(m => m.Id == id).FirstOrDefault();
                //Rates Destination - get the port code
                var CityCode = _dbRepositoryCountry.GetEntities().Where(x => x.country_code == Vehicleobj.ToCountryCode && (x.city == Vehicleobj.CityName)).FirstOrDefault();
                var Shippinglist = _dbRepositoryVehicleShippingRate.GetEntities().Where(x => x.Code == CityCode.port_code).FirstOrDefault();
                var Modellist = _dbRepositoryVehicleModelList.GetEntities().Where(x => x.MakeName == Vehicleobj.VehicleMakeName && (x.ModelName == Vehicleobj.VehicleModelName)).FirstOrDefault();


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
                    bodyTemplate = bodyTemplate.Replace("#CustName#", Quoteobj.Firstname);
                    var GuidLink = _dbRepositoryGuideLink.GetEntities().Where(m => m.CountryCode == Vehicleobj.ToCountryCode && (m.CityName == Vehicleobj.CityName || m.CityName == null)).FirstOrDefault();
                    if (GuidLink != null)
                        bodyTemplate = bodyTemplate.Replace("#Guide#", "(excluding incidental costs noted in our exclusions and detailed in our helpful <b><a target='_blank' href='" + GuidLink.VehicleURL + "'>Guide</a></b>)");
                    else
                        bodyTemplate = bodyTemplate.Replace("#Guide#", "(excluding incidental costs noted in our exclusions)");

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
                        status = await UpdateVehicleDetails(Quoteobj.VehicleId, Quoteobj.FCL, Quoteobj.GroupageTotal);
                        var QuoteAmountobj = _dbRepositoryQuoteAmount.GetEntities().Where(x => x.QuoteId == id && x.MoveType == "EXV").FirstOrDefault();
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
                        status = await UpdateVehicleDetails(Quoteobj.VehicleId, Quoteobj.FCL, Quoteobj.GroupageTotal);
                        var QuoteAmountobj = _dbRepositoryQuoteAmount.GetEntities().Where(x => x.QuoteId == id && x.MoveType == "EXV").FirstOrDefault();
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
                        status = await UpdateVehicleDetails(Quoteobj.VehicleId, Quoteobj.FCL, Quoteobj.GroupageTotal);
                        var QuoteAmountobj = _dbRepositoryQuoteAmount.GetEntities().Where(x => x.QuoteId == id && x.MoveType == "EXV").FirstOrDefault();
                        Task task = new Task(() => EmailHelper.SendAsyncEmail(Vehicleobj.Email, emailTemplateObj.Subject, bodyTemplate, "MAN", "DisplayVehicle", true));
                        task.Start();
                        return View(Quoteobj);
                    }
                }
                else
                {
                    long customerReferenceNo = CustomRepository.GetNextCustomerRefNo("EXV", Vehicleobj.Email, Vehicleobj.ToCountryCode, Vehicleobj.CityName);
                    var customerQuotes = _dbRepositoryQuoteAmount.GetEntities().Where(x => x.CustomerReferenceNo == customerReferenceNo);
                    int customerQuoteNo = CustomRepository.GetNextCustomerRefQuoteNo("EXV", customerReferenceNo, Convert.ToInt64(id));

                    tbl_QuoteAmount QuoteAmounts = new tbl_QuoteAmount();
                    QuoteAmounts.QuoteId = Convert.ToInt32(id);
                    QuoteAmounts.MoveType = "EXV";
                    QuoteAmounts.QuoteAmount = 0;
                    QuoteAmounts.ShippingType = "ENQUIRY";
                    QuoteAmounts.ShippingTypeDescription = "ENQUIRY";
                    QuoteAmounts.TransitionTime = "";
                    QuoteAmounts.CreatedOn = System.DateTime.Now;
                    QuoteAmounts.CustomerReferenceNo = customerReferenceNo;
                    QuoteAmounts.CustomerQuoteNo = (int)customerQuoteNo;
                    QuoteAmounts.QuoteSeqNo = 0;
                    QuoteAmounts.Company = SessionHelper.COMPANY_ID;
                    string insert = _dbRepositoryQuoteAmount.Insert(QuoteAmounts);

                    var vehObj = _dbRepositoryVehicle.GetEntities().Where(m => m.EnquiryNo != null && m.Id != id).OrderByDescending(m => m.Id).FirstOrDefault();
                    if (vehObj != null)
                    {
                        if (vehObj.EnquiryNo == null)
                            Vehicleobj.EnquiryNo = String.Format("{0:D4}", 1);
                        else
                            Vehicleobj.EnquiryNo = String.Format("{0:D4}", (Convert.ToInt32(vehObj.EnquiryNo) + 1));
                    }
                    else
                    {
                        Vehicleobj.EnquiryNo = String.Format("{0:D4}", 1);
                    }
                    _dbRepositoryVehicle.Update(Vehicleobj);

                    var emailTemplateObj = _dbRepositoryEmailTemplate.GetEntities().Where(m => m.ServiceId == 1017).FirstOrDefault();
                    string bodyTemplate = string.Empty;
                    if (emailTemplateObj != null)
                    {
                        bodyTemplate = emailTemplateObj.HtmlContent;
                    }
                    bodyTemplate = bodyTemplate.Replace("#CustName#", Quoteobj.Firstname);

                    var GuidLink = _dbRepositoryGuideLink.GetEntities().Where(m => m.CountryCode == Vehicleobj.ToCountryCode && (m.CityName == Vehicleobj.CityName || m.CityName == null)).FirstOrDefault();
                    if (GuidLink != null)
                        bodyTemplate = bodyTemplate.Replace("#Guide#", " (excluding incidental costs noted in our exclusions and detailed in our helpful <b><a target='_blank' href='" + GuidLink.VehicleURL + "'>Guide</a></b>)");
                    else
                        bodyTemplate = bodyTemplate.Replace("#Guide#", "(excluding incidental costs noted in our exclusions)");
                    string branch = vehicleObjModel.BranchId == 1 ? "GLA" : (vehicleObjModel.BranchId == 2 ? "LON" : "MAN");
                    Task task = new Task(() => EmailHelper.SendAsyncEmail(Vehicleobj.Email, emailTemplateObj.Subject, bodyTemplate, branch, "DisplayVehicle", true));
                    task.Start();

                    SP_GetXmlData_Result xmlResult = CustomRepository.GetXmlData(id);
                    if (xmlResult != null)
                    {
                        if ((xmlResult.FCL == 0 || xmlResult.FCL == null) && (xmlResult.GPG == 0 || xmlResult.GPG == null))
                        {
                            string file = string.Concat(customerReferenceNo + "/" + customerQuoteNo);
                            ViewBag.ReferenceNo = file;
                            string path = await XMLHelper.GenerateXmlEnquiryFile(xmlResult, file);
                            //XMLHelper.XmlAPICall(path, 0); //Default id = 0
                        }
                    }
                    return View(Quoteobj);
                }

                status = await UpdateVehicleDetails(Quoteobj.VehicleId, Quoteobj.FCL, Quoteobj.GroupageTotal);
                return View(Quoteobj);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                TempData[CustomEnums.NotifyType.Error.GetDescription()] = CommonHelper.GetErrorMessage(ex);
            }
            return View(Quoteobj);
        }

        [HttpPost]
        public async Task<string> Quote(int? VehicleId, string str)
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
                        string FCL = await XMLHelper.GenerateXmlFCLFile(xmlResult, file, true);
                        xmlFiles.Add(FCL);
                    }
                    else
                    {
                        var CustomerRefNo = _dbRepositoryQuoteAmount.GetEntities().Where(m => m.QuoteId == VehicleId && m.MoveType == "EXV" && m.ShippingType == "GPG").FirstOrDefault();
                        string file = string.Concat(CustomerRefNo.CustomerReferenceNo + "/" + CustomerRefNo.CustomerQuoteNo + "." + CustomerRefNo.QuoteSeqNo);
                        string GPG = await XMLHelper.GenerateXmlGDPFile(xmlResult, file, true);
                        xmlFiles.Add(GPG);
                    }

                    //if (xmlFiles.Count > 0)
                    //{
                    //    Task tasks = new Task(() => SendXmlFies(xmlFiles));
                    //    tasks.Start();
                    //}
                    Task task = new Task(() => emailStatus = XMLHelper.SendEmail(xmlEmailResult, 1,"",""));
                    Task taskquote = new Task(() => emailStatus = XMLHelper.SendEmail(xmlEmailResult, 2,"",""));
                    task.Start();
                    taskquote.Start();
                }
                return message;
            }
            catch (Exception e)
            {
                logger.Error(e);
                TempData[CustomEnums.NotifyType.Error.GetDescription()] = CommonHelper.GetErrorMessage(e);
                return e.Message;
            }
        }

        //private void SendXmlFies(List<string> xmlFiles)
        //{
        //    try
        //    {
        //        foreach (string xmlFile in xmlFiles)
        //        {
        //            XMLHelper.XmlAPICall(xmlFile, 0);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error(ex);
        //        TempData[CustomEnums.NotifyType.Error.GetDescription()] = CommonHelper.GetErrorMessage(ex);
        //    }
        //}

        public ActionResult Success()
        {
            return View();
        }

        public JsonResult GetVehicleModelList(string MakeName)
        {
            try
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
            catch (Exception ex)
            {
                logger.Error(ex);
                TempData[CustomEnums.NotifyType.Error.GetDescription()] = CommonHelper.GetErrorMessage(ex);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCityListByCountryId(string countryCode)
        {
            try
            {
                var countryCodeParameter = new SqlParameter
                {
                    ParameterName = "CountryCode",
                    DbType = DbType.String,
                    Value = countryCode
                };
                var companyCodeParameter = new SqlParameter
                {
                    ParameterName = "CompanyCode",
                    DbType = DbType.Int32,
                    Value = SessionHelper.COMPANY_ID
                };
                quotesEntities obj = new quotesEntities();
                CityListModel model = new CityListModel();
                var vehicleCityList = obj.Database.SqlQuery<CityListModel>("GetCity @CountryCode, @CompanyCode", countryCodeParameter, companyCodeParameter).ToList();
                return Json(vehicleCityList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                TempData[CustomEnums.NotifyType.Error.GetDescription()] = CommonHelper.GetErrorMessage(ex);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckValidPostCode(string postCode)
        {
            try
            {
                var code = postCode;
                ViewBag.BranchPostCode = 0;
                string pc = postCode.Replace(" ", "");
                pc = pc.Substring(0, pc.Length - 3) + " " + pc.Substring(pc.Length - 3, 3);

                //var postCodeObj = _dbRepositoryUKPostCode.GetEntities().Where(m => m.zip.Replace(" ", "") == postCode.Replace(" ", "")).FirstOrDefault();
                var postCodeObj = _dbRepositoryUKPostCode.GetEntities().Where(m => m.zip == pc).FirstOrDefault();
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
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                TempData[CustomEnums.NotifyType.Error.GetDescription()] = CommonHelper.GetErrorMessage(ex);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        public async Task<string> UpdateVehicleDetails(int? id, double? fcl, double? gpg)
        {
            string emailStatus = string.Empty;
            try
            {
                if (id == null)
                    return null;

                string message = string.Empty;

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
                int customerQuoteNo = CustomRepository.GetNextCustomerRefQuoteNo("EXV", customerReferenceNo, Convert.ToInt64(id));

                //int customerQuoteNo = 1;

                //if (customerQuotes != null)
                //{
                //    var lastCustQt = customerQuotes.OrderByDescending(x => x.CustomerQuoteNo).FirstOrDefault();
                //    if (lastCustQt != null)
                //        customerQuoteNo = (lastCustQt.CustomerQuoteNo ?? 0) + 1;
                //    else
                //        customerQuoteNo = 1;
                //}

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
                    QuoteAmount.CustomerQuoteNo = (int)customerQuoteNo;
                    QuoteAmount.QuoteSeqNo = QuoteSeqNo;
                    QuoteAmount.Company = SessionHelper.COMPANY_ID;
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
                    QuoteAmount.CustomerQuoteNo = (int)customerQuoteNo;
                    QuoteAmount.QuoteSeqNo = QuoteSeqNo;
                    QuoteAmount.Company = SessionHelper.COMPANY_ID;
                    string insert = _dbRepositoryQuoteAmount.Insert(QuoteAmount);
                }

                message = _dbRepositoryVehicle.Update(vehicle);

                //XML Generation and Make SOAP Request
                if (string.IsNullOrEmpty(message))
                {
                    //SP_GetXmlData_Result xmlResult = CustomRepository.GetXmlData(id);
                    SP_GetXmlData_Result xmlResult = CustomRepository.GetXmlData(id);
                    //sp_GetdataForEmailSending_Result quoteObj = CustomRepository.GetQuoteData(id, 1);
                    List<string> xmlFiles = new List<string>();
                    if (xmlResult != null)
                    {
                        if (xmlResult.FCL != 0)
                        {
                            var CustomerRefNo = _dbRepositoryQuoteAmount.GetEntities().Where(m => m.QuoteId == id && m.MoveType == "EXV" && m.ShippingType == "FCL").FirstOrDefault();
                            string file = string.Concat(CustomerRefNo.CustomerReferenceNo + "/" + CustomerRefNo.CustomerQuoteNo + "." + CustomerRefNo.QuoteSeqNo);
                            string path = await XMLHelper.GenerateXmlFCLFile(xmlResult, file);
                            ViewBag.ReferenceNo = string.Concat(CustomerRefNo.CustomerReferenceNo + "/" + CustomerRefNo.CustomerQuoteNo);
                            //XMLHelper.XmlAPICall(path, 0); //Default id = 0
                            xmlFiles.Add(path);
                            //GetCitiesByCountryAsync(path);
                            //Task task = new Task(() => emailStatus = XMLHelper.SendEmail(quoteObj, "User"));
                            //task.Start();
                            //XMLHelper.XmlAPICall(path,0); //Default id = 0
                        }
                        if (xmlResult.GPG != 0)
                        {
                            var CustomerRefNo = _dbRepositoryQuoteAmount.GetEntities().Where(m => m.QuoteId == id && m.MoveType == "EXV" && m.ShippingType == "GPG").FirstOrDefault();
                            string file = string.Concat(CustomerRefNo.CustomerReferenceNo + "/" + CustomerRefNo.CustomerQuoteNo + "." + CustomerRefNo.QuoteSeqNo);
                            string path = await XMLHelper.GenerateXmlGDPFile(xmlResult, file);
                            ViewBag.ReferenceNo = string.Concat(CustomerRefNo.CustomerReferenceNo + "/" + CustomerRefNo.CustomerQuoteNo);
                            //XMLHelper.XmlAPICall(path, 0); //Default id = 0
                            xmlFiles.Add(path);
                            //GetCitiesByCountryAsync(path);
                            //emailStatus = XMLHelper.SendEmail(quoteObj, "User");
                            //XMLHelper.XmlAPICall(path,0); //Default id = 0
                        }
                        //if (xmlFiles.Count > 0)
                        //{
                        //    Task task = new Task(() => SendXmlFiles(xmlFiles));
                        //    task.Start();
                        //}
                    }
                    return emailStatus;
                }
                return emailStatus;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                TempData[CustomEnums.NotifyType.Error.GetDescription()] = CommonHelper.GetErrorMessage(ex);
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

        //private void SendXmlFiles(List<string> xmlFiles)
        //{
        //    try
        //    {
        //        foreach (string xmlFile in xmlFiles)
        //        {
        //            XMLHelper.XmlAPICall(xmlFile, 0);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error(ex);
        //        TempData[CustomEnums.NotifyType.Error.GetDescription()] = CommonHelper.GetErrorMessage(ex);
        //    }
        //}
        #endregion
    }
}