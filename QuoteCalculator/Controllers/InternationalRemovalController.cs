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
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;

namespace QuoteCalculator.Controllers
{
    public class InternationalRemovalController : Controller
    {
        private static NLog.Logger logger = LogManager.GetCurrentClassLogger();
        #region private variables
        private readonly GenericRepository<tbl_InternationalRemoval> _dbRepositoryInternationalRemoval;
        private readonly GenericRepository<rates_destinations> _dbRepositoryCountry;
        private readonly GenericRepository<tbl_Title> _dbRepositorytitle;
        private readonly GenericRepository<cartons> _dbRepositorycartons;
        private readonly GenericRepository<uk> _dbRepositoryUKPostCode;
        private readonly GenericRepository<branch_postcode> _dbRepositoryUKBranchPostCode;
        private readonly GenericRepository<tbl_QuickQuoteItems> _dbRepositoryQuickQuoteItems;
        private readonly GenericRepository<tbl_AdditionalQuickQuoteItems> _dbRepositoryAdditionalQuoteItems;
        public readonly GenericRepository<tbl_RequestTracking> _dbRepositoryRequestTracking;
        public readonly GenericRepository<tbl_BlockIpOrEmail> _dbRepositoryBlockIpOrEmail;
        public readonly GenericRepository<tbl_WhiteListIp> _dbRepositoryWhiteListIp;
        public readonly GenericRepository<tbl_EmailTemplate> _dbRepositoryEmailTemplate;
        public readonly GenericRepository<tbl_QuoteAmount> _dbRepositoryQuoteAmount;
        public readonly GenericRepository<source> _dbRepositorySource;
        public readonly GenericRepository<tbl_CountryCode> _dbRepositoryCountryCode;
        public readonly GenericRepository<branch> _dbRepositoryBranch;
        public readonly GenericRepository<tbl_DaySchedule> _dbRepositoryDaySchedule;
        public readonly GenericRepository<tbl_GuideLink> _dbRepositoryGuideLink;
        private readonly GenericRepository<tbl_BaggageItem> _dbRepositoryMoveBaggage;
        #endregion

        #region Constructor
        public InternationalRemovalController()
        {
            _dbRepositoryInternationalRemoval = new GenericRepository<tbl_InternationalRemoval>();
            _dbRepositoryCountry = new GenericRepository<rates_destinations>();
            _dbRepositorytitle = new GenericRepository<tbl_Title>();
            _dbRepositorycartons = new GenericRepository<cartons>();
            _dbRepositoryUKPostCode = new GenericRepository<uk>();
            _dbRepositoryUKBranchPostCode = new GenericRepository<branch_postcode>();
            _dbRepositoryQuickQuoteItems = new GenericRepository<tbl_QuickQuoteItems>();
            _dbRepositoryAdditionalQuoteItems = new GenericRepository<tbl_AdditionalQuickQuoteItems>();
            _dbRepositoryRequestTracking = new GenericRepository<tbl_RequestTracking>();
            _dbRepositoryBlockIpOrEmail = new GenericRepository<tbl_BlockIpOrEmail>();
            _dbRepositoryWhiteListIp = new GenericRepository<tbl_WhiteListIp>();
            _dbRepositoryEmailTemplate = new GenericRepository<tbl_EmailTemplate>();
            _dbRepositoryQuoteAmount = new GenericRepository<tbl_QuoteAmount>();
            _dbRepositorySource = new GenericRepository<source>();
            _dbRepositoryCountryCode = new GenericRepository<tbl_CountryCode>();
            _dbRepositoryBranch = new GenericRepository<branch>();
            _dbRepositoryDaySchedule = new GenericRepository<tbl_DaySchedule>();
            _dbRepositoryGuideLink = new GenericRepository<tbl_GuideLink>();
            _dbRepositoryMoveBaggage = new GenericRepository<tbl_BaggageItem>();
        }
        #endregion

        #region Methods
        // GET: InternationalRemoval
        [HttpGet]
        public ActionResult Index(string countryCode, string InternationalRemovalId, bool? isOldQuote)
        {
            tbl_InternationalRemoval model = new tbl_InternationalRemoval();
            try
            {
                if (HttpContext.Request.Url.GetLeftPart(UriPartial.Authority) == System.Configuration.ConfigurationManager.AppSettings["PicfordUrl"])
                {
                    SessionHelper.COMPANY_ID = 2;
                    return RedirectToAction("Index", "Baggage");
                }
                else if (HttpContext.Request.Url.GetLeftPart(UriPartial.Authority) == System.Configuration.ConfigurationManager.AppSettings["ExcessUrl"])
                {
                    SessionHelper.COMPANY_ID = 3;
                    return RedirectToAction("Index", "Baggage");
                }
                else
                {
                    SessionHelper.COMPANY_ID = 1;
                    var id = 0;
                    SessionHelper.isOldQuote = isOldQuote == true ? true : false;
                    if (InternationalRemovalId != null)
                    {
                        id = Convert.ToInt32(CommonHelper.Decode(InternationalRemovalId));
                    }
                    ViewBag.SelectedCountryList = _dbRepositoryCountry.GetEntities().Where(m => m.country.Contains("United Kingdom")).Select(m => new { m.id, m.country }).ToList();
                    var countryList = _dbRepositoryCountry.GetEntities().Where(m => m.rem_dest == 1).Select(m => new { m.country_code, m.country }).Distinct().ToList();
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
                    ViewBag.CountryId = countryCode;
                    ViewBag.CountryCodeList = _dbRepositoryCountryCode.GetEntities().Distinct().OrderBy(m => m.CountryName).ToList();
                    model.CountryCode = "+44";
                    SessionHelper.InternationalRemovalId = Convert.ToInt32(null);
                    model.EstimatedMoveDate = DateTime.Now;
                    if (id > 0)
                    {
                        model = _dbRepositoryInternationalRemoval.GetEntities().Where(m => m.Id == id).FirstOrDefault();
                    }
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
        public ActionResult Index(tbl_InternationalRemoval model)
        {
            try
            {
                string message = string.Empty;
                SessionHelper.ToCountryCode = model.ToCountryCode;
                SessionHelper.QuoteType = "3";
                model.Firstname = model.Firstname.First().ToString().ToUpper() + model.Firstname.Substring(1);
                model.Lastname = model.Lastname.First().ToString().ToUpper() + model.Lastname.Substring(1);
                //if (model.Telephone.Substring(0, 1) != "0")
                //{
                //    model.Telephone = "0" + model.Telephone;
                //}
                model.Telephone = model.Telephone;
                model.CreatedDate = DateTime.Now;
                // var countryObj = _dbRepositoryCountry.GetEntities().Where(m => m.country == "United Kingdom").Select(m => new { m.id, m.country }).OrderBy(m => new { m.country }).FirstOrDefault();
                model.FromCountryName = "United Kingdom";  //countryObj.country;
                model.EstimatedMoveDate = Convert.ToDateTime(model.EstimatedMoveDate);
                model.Company = SessionHelper.COMPANY_ID;

                if (!ModelState.IsValid)
                {
                    ViewBag.SelectedCountryList = _dbRepositoryCountry.GetEntities().Where(m => m.country.Contains("United Kingdom")).Select(m => new { m.id, m.country }).ToList();
                    ViewBag.CountryList = _dbRepositoryCountry.GetEntities().Where(m => m.country != "United Kingdom").Select(m => new { m.country_code, m.country }).Distinct().ToList();
                    ViewBag.TitleList = _dbRepositorytitle.GetEntities().ToList();
                    return View(model);
                }

                try
                {
                    if (model.PostCode != null && model.PostCode != string.Empty)
                    {
                        string pc = model.PostCode.Replace(" ", "");
                        pc = pc.Substring(0, pc.Length - 3) + " " + pc.Substring(pc.Length - 3, 3);
                        var postCodeObj = _dbRepositoryUKPostCode.GetEntities().Where(m => m.zip == pc).FirstOrDefault();
                        model.PostCode = postCodeObj.zip;
                    }
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
                    if (SessionHelper.isOldQuote == true)
                    {
                        message = _dbRepositoryInternationalRemoval.Update(model);
                    }
                    else
                    {
                        if (Request.UrlReferrer.ToString().ToUpper().Contains("INVENTORY=VIDEO"))
                        {
                            model.HomeConsultationOrService = false;
                            model.HomeVideoSurvery = true;
                            model.QuickOnlineQuote = false;
                        }
                        model.Id = 0;
                        message = _dbRepositoryInternationalRemoval.Insert(model);
                    }
                    if (message == "")
                    {
                        string ip = Request.UserHostAddress;
                        tbl_RequestTracking requestTracking = new tbl_RequestTracking();
                        requestTracking.InternationalRemoval = model.Email;
                        requestTracking.IpAddress = ip;
                        requestTracking.IsMailSend = false;
                        requestTracking.CreatedDate = DateTime.Now;
                        _dbRepositoryRequestTracking.Insert(requestTracking);
                        SessionHelper.InternationalRemovalId = model.Id;

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
                        var requestEmail = _dbRepositoryRequestTracking.GetEntities().Where(m => m.InternationalRemoval == model.Email && (DbFunctions.TruncateTime(m.CreatedDate.Value) == DateTime.Today)).ToList();
                        var requestIpEmail = _dbRepositoryRequestTracking.GetEntities().Where(m => m.InternationalRemoval == model.Email && m.IpAddress == ip && (DbFunctions.TruncateTime(m.CreatedDate.Value) == DateTime.Today)).ToList();
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
                                                        .Where(m => m.IpAddress == ip && m.InternationalRemoval == model.Email).Count();
                                var totalEmailCount = _dbRepositoryRequestTracking.GetEntities().Where(m => m.InternationalRemoval == model.Email).Count();
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

                                var totalEmailCount = _dbRepositoryRequestTracking.GetEntities().Where(m => m.InternationalRemoval == model.Email).Count();
                                var todayIpCount = _dbRepositoryRequestTracking.GetEntities()
                                     .Where(m => m.IpAddress == ip && (DbFunctions.TruncateTime(m.CreatedDate.Value) == DateTime.Today)).Count();
                                var totalemailcount = _dbRepositoryRequestTracking.GetEntities().Where(m => m.InternationalRemoval == model.Email).Count();
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
                            Task task = new Task(() => EmailHelper.SendAsyncEmail(Obj.Email, "Quote guard - warning", bodyTemplate, "EmailRemovals", "DisplayRemoval", true));
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
                                        return Redirect("/InternationalRemoval/RemovalServiceSelection");
                                    }
                                    else { return RedirectToAction("Success", "Vehicle"); }
                                }
                            }
                            else
                                return RedirectToAction("Success", "Vehicle");
                        }
                        if (blockEmailList.LastOrDefault() != null)
                        {
                            if (blockEmailList.LastOrDefault().Status == 2)
                            {
                                if (blockIpList.LastOrDefault() != null)
                                {
                                    if (blockIpList.LastOrDefault().Status == 2)
                                    {
                                        return Redirect("/InternationalRemoval/RemovalServiceSelection");
                                    }
                                }
                            }
                            else
                                return RedirectToAction("Success", "Vehicle");
                        }

                        if (Request.UrlReferrer.ToString().ToUpper().Contains("INVENTORY=VIDEO"))
                        {
                            string Id = CommonHelper.Encode(SessionHelper.InternationalRemovalId.ToString());
                            return Redirect("/InternationalRemoval/HomeVideoSurvery?id=" + Id);
                        }

                        return Redirect("/InternationalRemoval/RemovalServiceSelection");
                    }

                    return Redirect("/InternationalRemoval/RemovalServiceSelection");
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    message = CommonHelper.GetErrorMessage(ex);
                    TempData[CustomEnums.NotifyType.Error.GetDescription()] = message;
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                TempData[CustomEnums.NotifyType.Error.GetDescription()] = CommonHelper.GetErrorMessage(ex);
                return View(model);
            }
        }

        [HttpGet]
        public ActionResult RemovalServiceSelection()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RemovalServiceSelection(bool HomeConsultationOrService = false, bool HomeVideoSurvery = false, bool QuickOnlineQuote = false)
        {
            try
            {
                string message = string.Empty;
                tbl_InternationalRemoval model = _dbRepositoryInternationalRemoval.SelectById(SessionHelper.InternationalRemovalId);
                if (SessionHelper.InternationalRemovalId == 0)
                {
                    //  ModelState.AddModelError("ErrorMessage", "Please fill first step");
                    TempData[CustomEnums.NotifyType.Error.GetDescription()] = "Please fill first step";
                    return RedirectToAction("Index", "InternationalRemoval", model);
                }
                if (HomeConsultationOrService == false && HomeVideoSurvery == false && QuickOnlineQuote == false)
                {
                    ModelState.AddModelError("ErrorMessage", "Please select atleast one");
                    return View(model);
                }
                try
                {
                    model.HomeConsultationOrService = HomeConsultationOrService;
                    model.HomeVideoSurvery = HomeVideoSurvery;
                    model.QuickOnlineQuote = QuickOnlineQuote;
                    message = _dbRepositoryInternationalRemoval.Update(model);
                    string msg = "Save successfully";
                    if (HomeConsultationOrService == true && HomeVideoSurvery == false && QuickOnlineQuote == false)
                    {
                        return Json(new { Message = msg, QuoteId = CommonHelper.Encode(model.Id.ToString()), SurveyType = 1, JsonRequestBehavior.AllowGet });
                        //return RedirectToAction("HomeConsultationOrService", "InternationalRemoval", new { id = model.Id });
                    }
                    if (HomeVideoSurvery == true && HomeConsultationOrService == false && QuickOnlineQuote == false)
                    {
                        return Json(new { Message = msg, QuoteId = CommonHelper.Encode(model.Id.ToString()), SurveyType = 2, JsonRequestBehavior.AllowGet });
                        //return Redirect("/InternationalRemoval/HomeVideoSurvery");
                    }
                    else
                    {
                        return Json(new { Message = msg, QuoteId = CommonHelper.Encode(model.Id.ToString()), SurveyType = 3, JsonRequestBehavior.AllowGet });
                        //return RedirectToAction("QuickOnlineQuote", "InternationalRemoval", new { id = model.Id });
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    //message = "Something went wrong";
                    TempData[CustomEnums.NotifyType.Error.GetDescription()] = ex.Message;
                    return Redirect("/InternationalRemoval/RemovalServiceSelection");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                TempData[CustomEnums.NotifyType.Error.GetDescription()] = CommonHelper.GetErrorMessage(ex);
                return Redirect("/InternationalRemoval/RemovalServiceSelection");
            }
        }

        [HttpGet]
        public ActionResult HomeConsultationOrService(string id)
        {
            try
            {
                var consultantId = Convert.ToInt32(CommonHelper.Decode(id));
                ViewBag.Id = consultantId;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                TempData[CustomEnums.NotifyType.Error.GetDescription()] = CommonHelper.GetErrorMessage(ex);
            }
            return View();
        }

        [HttpGet]
        public ActionResult HomeVideoSurvery(string id)
        {
            VideoSurveyModel model = new VideoSurveyModel();
            try
            {
                ViewBag.DayScheduleList = _dbRepositoryDaySchedule.GetEntities().ToList();
                var QuoteId = Convert.ToInt32(CommonHelper.Decode(id));
                tbl_InternationalRemoval removal = _dbRepositoryInternationalRemoval.SelectById(QuoteId);
                var Title = _dbRepositorytitle.SelectById(removal.TitleId);
                var Country = _dbRepositoryCountry.GetEntities().Where(m => m.country_code == removal.ToCountryCode && m.CompanyId == SessionHelper.COMPANY_ID).FirstOrDefault();

                model.QuoteId = QuoteId;
                model.firstName = removal.Firstname;
                model.title = Title.TitleName.ToUpper();
                model.surName = removal.Lastname;
                model.email = removal.Email;
                model.mobileNumber = removal.Telephone;
                model.addressPickup = removal.FromCountryName.ToUpper();
                model.addressDropoff = Country.country;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                TempData[CustomEnums.NotifyType.Error.GetDescription()] = CommonHelper.GetErrorMessage(ex);
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult SaveHomeVideoSurvey(int quoteId, long id, string appointmentTime)
        {
            try
            {
                tbl_InternationalRemoval model = _dbRepositoryInternationalRemoval.GetEntities().Where(m => m.Id == quoteId && m.Company == SessionHelper.COMPANY_ID).FirstOrDefault();
                if (model != null)
                {
                    model.VideoSurveyId = id;
                    model.VideoSurveyAppointmentTime = Convert.ToDateTime(appointmentTime);
                    string message = _dbRepositoryInternationalRemoval.Update(model);

                    return Json(new { Message = "Save Successfully.", IsSuccess = Convert.ToString((int)CustomEnums.NotifyType.Success) }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Message = "Something wents wrong.", IsError = Convert.ToString((int)CustomEnums.NotifyType.Error) }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                TempData[CustomEnums.NotifyType.Error.GetDescription()] = CommonHelper.GetErrorMessage(ex);
            }
            return Json(new { Message = "Something wents wrong.", IsError = Convert.ToString((int)CustomEnums.NotifyType.Error) }, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> HomeVideoSurveyThankyou(string dayScheduleId, string appointmentTime)
        {
            try
            {
                string message = string.Empty;
                tbl_InternationalRemoval model = _dbRepositoryInternationalRemoval.SelectById(SessionHelper.InternationalRemovalId);
                if (model != null)
                {
                    model.dayScheduleId = Convert.ToInt32(dayScheduleId);
                    model.VideoSurveyAppointmentTime = Convert.ToDateTime(appointmentTime);

                    message = _dbRepositoryInternationalRemoval.Update(model);
                }
                ViewBag.FirstName = model.Firstname;
                ViewBag.AppointmentTime = string.Format("{0:dd MMMM hh:mm tt}", model.VideoSurveyAppointmentTime);

                long customerReferenceNo = CustomRepository.GetNextCustomerRefNo("EXR", model.Email, model.ToCountryCode, model.CityName);
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

                tbl_QuoteAmount QuoteAmount = new tbl_QuoteAmount();
                QuoteAmount = new tbl_QuoteAmount();
                QuoteAmount.QuoteId = Convert.ToInt32(SessionHelper.InternationalRemovalId);
                QuoteAmount.MoveType = "EXR";
                QuoteAmount.QuoteAmount = 0;
                QuoteAmount.ShippingType = "HomeVideoSurvey";
                QuoteAmount.ShippingTypeDescription = "Free Home Video Survey";
                QuoteAmount.TransitionTime = "";
                QuoteAmount.CreatedOn = System.DateTime.Now;
                QuoteAmount.CustomerReferenceNo = customerReferenceNo;
                QuoteAmount.CustomerQuoteNo = customerQuoteNo;
                QuoteAmount.QuoteSeqNo = 0;
                QuoteAmount.Company = SessionHelper.COMPANY_ID;
                string insert = _dbRepositoryQuoteAmount.Insert(QuoteAmount);

                SP_GetRemovalXmlData_Result xmlResult = CustomRepository.GetRemovalXmlData(SessionHelper.InternationalRemovalId);
                string file = string.Concat(customerReferenceNo + "/" + customerQuoteNo);
                //string Date = string.Format("{0:dd/MM/yyyy}", model.VideoSurveyAppointmentTime);
                //string Time = string.Format("{0:HH':'mm}", model.VideoSurveyAppointmentTime);
                string Removal = await XMLHelper.GenerateHomeVideoSurveyXml(xmlResult, file, model.VideoSurveyAppointmentTime);
                //Task task = new Task(() => XMLHelper.XmlAPICall(Removal, 0));
                //task.Start();
                if (message == "")
                {
                    return Json(new { Message = "Save Successfully.", IsSuccess = Convert.ToString((int)CustomEnums.NotifyType.Success) }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Message = "Something wents wrong.", IsError = Convert.ToString((int)CustomEnums.NotifyType.Error) }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                TempData[CustomEnums.NotifyType.Error.GetDescription()] = CommonHelper.GetErrorMessage(ex);
            }
            return Json(new { Message = "Something wents wrong.", IsError = Convert.ToString((int)CustomEnums.NotifyType.Error) }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult QuickOnlineQuote(int? id)
        {
            QuickQuoteItemsModel model = new QuickQuoteItemsModel();
            try
            {
                if (HttpContext.Request.Url.GetLeftPart(UriPartial.Authority) == System.Configuration.ConfigurationManager.AppSettings["AngloUrl"])
                {
                    SessionHelper.COMPANY_ID = 1;
                }
                else if (HttpContext.Request.Url.GetLeftPart(UriPartial.Authority) == System.Configuration.ConfigurationManager.AppSettings["PicfordUrl"])
                {
                    SessionHelper.COMPANY_ID = 2;
                }
                else if (HttpContext.Request.Url.GetLeftPart(UriPartial.Authority) == System.Configuration.ConfigurationManager.AppSettings["ExcessUrl"])
                {
                    SessionHelper.COMPANY_ID = 3;
                }
                model.InternationalRemovalId = Convert.ToInt32(id);
                model.items = _dbRepositoryQuickQuoteItems.GetEntities().Where(m => m.company == SessionHelper.COMPANY_ID).OrderBy(m => m.DisplayOrder).ToList();

            }
            catch (Exception ex)
            {
                logger.Error(ex);
                TempData[CustomEnums.NotifyType.Error.GetDescription()] = CommonHelper.GetErrorMessage(ex);
            }
            return View(model);
        }

        [HttpPost]
        public string QuickOnlineQuote(QuickQuoteItemsModel model)
        {
            try
            {
                tbl_AdditionalQuickQuoteItems mainModel = new tbl_AdditionalQuickQuoteItems();
                string message = string.Empty;
                if (model.QuickQuoteItemId != 0)
                {
                    tbl_QuickQuoteItems item = _dbRepositoryQuickQuoteItems.GetEntities().Where(m => m.ItemId == model.QuickQuoteItemId && m.company == SessionHelper.COMPANY_ID).FirstOrDefault();
                    mainModel.Beds = item.Title;
                    mainModel.Cuft = item.Cuft;
                    mainModel.Ftcontainer = item.Ftcontainer;
                    mainModel.SpecialRequirements = model.SpecialRequirements;
                    mainModel.InternationalRemovalId = SessionHelper.InternationalRemovalId;
                    mainModel.QuickQuoteItemId = model.QuickQuoteItemId;
                    message = _dbRepositoryAdditionalQuoteItems.Insert(mainModel);
                    return message;
                }
                else
                {
                    mainModel.Beds = model.Beds;
                    mainModel.Cuft = model.Cuft;
                    //mainModel.Ftcontainer = model.Ftcontainer;
                    mainModel.SpecialRequirements = model.SpecialRequirements;
                    mainModel.InternationalRemovalId = SessionHelper.InternationalRemovalId;
                    mainModel.QuickQuoteItemId = null; //To avoid foreign key constraint
                    message = _dbRepositoryAdditionalQuoteItems.Insert(mainModel);
                    return message;
                }

            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return CommonHelper.GetErrorMessage(ex, false);
            }
        }

        [HttpPost]
        public async Task<string> HomeConsultationOrService(int? id, DateTime? value)
        {
            try
            {
                tbl_InternationalRemoval model = _dbRepositoryInternationalRemoval.SelectById(id);
                string message = string.Empty;
                if (model.HomeConsultationOrService == true)
                {
                    //model.HomeConsultationDateTime = DateTime.Parse(value);
                    model.HomeConsultationDateTime = value;
                    message = _dbRepositoryInternationalRemoval.Update(model);
                }
                long customerReferenceNo = CustomRepository.GetNextCustomerRefNo("EXR", model.Email, model.ToCountryCode, model.CityName);
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

                tbl_QuoteAmount QuoteAmount = new tbl_QuoteAmount();
                QuoteAmount = new tbl_QuoteAmount();
                QuoteAmount.QuoteId = Convert.ToInt32(id);
                QuoteAmount.MoveType = "EXR";
                QuoteAmount.QuoteAmount = 0;
                QuoteAmount.ShippingType = "HomeSurvey";
                QuoteAmount.ShippingTypeDescription = "Free Home Survey";
                QuoteAmount.TransitionTime = "";
                QuoteAmount.CreatedOn = System.DateTime.Now;
                QuoteAmount.CustomerReferenceNo = customerReferenceNo;
                QuoteAmount.CustomerQuoteNo = customerQuoteNo;
                QuoteAmount.QuoteSeqNo = 0;
                QuoteAmount.Company = SessionHelper.COMPANY_ID;
                string insert = _dbRepositoryQuoteAmount.Insert(QuoteAmount);

                SP_GetRemovalXmlData_Result xmlResult = CustomRepository.GetRemovalXmlData(id);
                string file = string.Concat(customerReferenceNo + "/" + customerQuoteNo);
                string Removal = await XMLHelper.GenerateHomeSurveyXml(xmlResult, file);
                //Task task = new Task(() => XMLHelper.XmlAPICall(Removal, 0));
                //task.Start();

                var emailTemplateObj = _dbRepositoryEmailTemplate.GetEntities().Where(m => m.ServiceId == 1020).FirstOrDefault();
                string bodyTemplate = string.Empty;
                if (emailTemplateObj != null)
                {
                    bodyTemplate = emailTemplateObj.HtmlContent;
                }
                bodyTemplate = bodyTemplate.Replace("#CustName#", model.Firstname);
                Task quoteTask = new Task(() => EmailHelper.SendAsyncEmail(model.Email, emailTemplateObj.Subject, bodyTemplate, "EmailRemovals", "DisplayRemoval", true));
                quoteTask.Start();
                return message;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return CommonHelper.GetErrorMessage(ex, false);
            }
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
                var internationalRemovalCityList = obj.Database.SqlQuery<GetCityForInternationalRemoval_Result>("GetCityForInternationalRemoval @CountryCode, @CompanyCode", countryCodeParameter, companyCodeParameter).ToList();
                return Json(internationalRemovalCityList, JsonRequestBehavior.AllowGet);
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
                                var branchDetail = _dbRepositoryBranch.GetEntities().Where(m => m.br_id == branchPostCode.vehicle_branch_id).FirstOrDefault();
                                var data = new
                                {
                                    branchId = branchPostCode.vehicle_branch_id,
                                    brPostcode = branchDetail.br_postcode
                                };
                                ViewBag.brPostcode = branchDetail.br_postcode;
                                ViewBag.BranchPostCode = branchPostCode.vehicle_branch_id;
                                return Json(data, JsonRequestBehavior.AllowGet);
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

        public ActionResult ThankYou(string survey)
        {
            try
            {
                if (survey == "Home")
                {
                    ViewBag.Survey = "Free home survey";
                    ViewBag.ThankYouMessage = "Thank you for your enquiry. We will contact you to arrange a suitable date and time for a free home survey and quotation without obligation.";
                }
                else
                {
                    ViewBag.Survey = "Free video survey";
                    ViewBag.ThankYouMessage = "Thank you for your enquiry. We will contact you to arrange a suitable date and time for a free video survey and quotation without obligation.";
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                TempData[CustomEnums.NotifyType.Error.GetDescription()] = CommonHelper.GetErrorMessage(ex);
            }
            return View();
        }

        public async Task<ActionResult> QuickOnlineQuoteDetail(int Id = 0, bool isMyQuote = false)
        {
            tbl_AdditionalQuickQuoteItems additionalquoteObj = new tbl_AdditionalQuickQuoteItems();
            try
            {                
                if (Id == 0)
                {
                    Id = SessionHelper.InternationalRemovalId;
                }
                additionalquoteObj = _dbRepositoryAdditionalQuoteItems.GetEntities().Where(m => m.InternationalRemovalId == Id).ToList().LastOrDefault();

                var SpecialRequirements = additionalquoteObj.SpecialRequirements != null ? additionalquoteObj.SpecialRequirements.Split('|') : null;//additionalquoteObj.SpecialRequirements.Split('|');
                additionalquoteObj.strSpecialRequirements = SpecialRequirements;

                var internationalRemovalObj = _dbRepositoryInternationalRemoval.GetEntities().Where(m => m.Id == Id).FirstOrDefault();
                if (internationalRemovalObj != null)
                {
                    ViewBag.PostCode = internationalRemovalObj.PostCode;
                    ViewBag.FromCountry = internationalRemovalObj.FromCountryName;
                    ViewBag.ToCity = internationalRemovalObj.CityName;
                    ViewBag.ToCountry = GetCountryName(internationalRemovalObj.ToCountryCode);
                }

                if (isMyQuote != true)
                {
                    var QuoteId = new SqlParameter
                    {
                        ParameterName = "QuoteId",
                        DbType = DbType.Int64,
                        Value = Id
                    };
                    tbl_EmailTemplate emailTemplateObj;
                    quotesEntities obj = new quotesEntities();
                    var result = obj.Database.SqlQuery<SP_RemovalCalculation_Result>("SP_RemovalCalculation @QuoteId", QuoteId).FirstOrDefault();
                    if (result != null && result.FinalResult != null && result.FinalResult > 0)
                    {
                        emailTemplateObj = _dbRepositoryEmailTemplate.GetEntities().Where(m => m.ServiceId == 1005).FirstOrDefault();
                        SP_GetRemovalXmlData_Result xmlResult = CustomRepository.GetRemovalXmlData(Id);
                        var CustomerRefNo = _dbRepositoryQuoteAmount.GetEntities().Where(m => m.QuoteId == Id && m.MoveType == "EXR").FirstOrDefault();
                        string file = string.Concat(CustomerRefNo.CustomerReferenceNo + "/" + CustomerRefNo.CustomerQuoteNo + "." + CustomerRefNo.QuoteSeqNo);
                        string Removal = await XMLHelper.GenerateRemovalXml(xmlResult, "Quote", file);
                        ViewBag.RemovalQuote = CustomerRefNo.QuoteAmount;
                        ViewBag.ReferenceNo = string.Concat(CustomerRefNo.CustomerReferenceNo + "/" + CustomerRefNo.CustomerQuoteNo);
                        //Task task = new Task(() => XMLHelper.XmlAPICall(Removal, 0));
                        //task.Start();
                    }
                    else
                    {
                        emailTemplateObj = _dbRepositoryEmailTemplate.GetEntities().Where(m => m.ServiceId == 1019).FirstOrDefault();
                        SP_GetRemovalXmlData_Result xmlResult = CustomRepository.GetRemovalXmlData(Id);
                        var CustomerRefNo = _dbRepositoryQuoteAmount.GetEntities().Where(m => m.QuoteId == Id && m.MoveType == "EXR" && m.ShippingType == "ENQUIRY").FirstOrDefault();
                        string file = string.Concat(CustomerRefNo.CustomerReferenceNo + "/" + CustomerRefNo.CustomerQuoteNo);
                        string Removal = await XMLHelper.GenerateRemovalEnquiryXml(xmlResult, "ENQUIRY", file);
                        ViewBag.RemovalQuote = CustomerRefNo.QuoteAmount;
                        ViewBag.ReferenceNo = string.Concat(CustomerRefNo.CustomerReferenceNo + "/" + CustomerRefNo.CustomerQuoteNo);
                        //Task task = new Task(() => XMLHelper.XmlAPICall(Removal, 0));
                        //task.Start();
                    }

                    tbl_InternationalRemoval model = _dbRepositoryInternationalRemoval.SelectById(Id);

                    string bodyTemplate = string.Empty;
                    if (emailTemplateObj != null)
                    {
                        bodyTemplate = emailTemplateObj.HtmlContent;
                    }

                    bodyTemplate = bodyTemplate.Replace("#CustName#", model.Firstname);
                    var GuidLink = _dbRepositoryGuideLink.GetEntities().Where(m => m.CountryCode == model.ToCountryCode && (m.CityName == model.CityName || m.CityName == null)).FirstOrDefault();
                    if (GuidLink != null)
                        bodyTemplate = bodyTemplate.Replace("#Guide#", "(excluding incidental costs noted in our exclusions and detailed in our helpful <b><a target='_blank' href='" + GuidLink.RemovalURL + "'>Guide</a></b>)");
                    else
                        bodyTemplate = bodyTemplate.Replace("#Guide#", "(excluding incidental costs noted in our exclusions)");

                    Task quoteTask = new Task(() => EmailHelper.SendAsyncEmail(model.Email, emailTemplateObj.Subject, bodyTemplate, "EmailRemovals", "DisplayRemoval", true));
                    quoteTask.Start();
                }
                else
                {
                    SessionHelper.ToCountryCode = internationalRemovalObj.ToCountryCode;
                    SessionHelper.QuoteType = "3";
                    var CustomerRefNo = _dbRepositoryQuoteAmount.GetEntities().Where(m => m.QuoteId == Id && m.MoveType == "EXR" && m.ShippingType == "Courier").FirstOrDefault();
                    ViewBag.RemovalQuote = CustomerRefNo.QuoteAmount;
                    ViewBag.ReferenceNo = string.Concat(CustomerRefNo.CustomerReferenceNo + "/" + CustomerRefNo.CustomerQuoteNo);
                }
                SessionHelper.InternationalRemovalId = Id;
                return View(additionalquoteObj);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                TempData[CustomEnums.NotifyType.Error.GetDescription()] = CommonHelper.GetErrorMessage(ex);
            }
            return View(additionalquoteObj);
        }

        [HttpPost]
        public async Task<string> RemovalQuoteBook(int quoteId, string type)
        {
            string message = string.Empty;
            try
            {
                try
                {
                    string emailStatus = string.Empty;
                    var additionalquoteObj = _dbRepositoryAdditionalQuoteItems.GetEntities().Where(m => m.InternationalRemovalId == quoteId).ToList().LastOrDefault();
                    var internationalRemovalObj = _dbRepositoryInternationalRemoval.GetEntities().Where(m => m.Id == quoteId).FirstOrDefault();
                    SP_GetRemovalXmlData_Result xmlResult = CustomRepository.GetRemovalXmlData(quoteId);
                    tbl_QuoteAmount QuoteAmount = _dbRepositoryQuoteAmount.GetEntities().Where(m => m.QuoteId == quoteId && m.MoveType == "EXR" && m.ShippingType == "Courier").FirstOrDefault();
                    string file = string.Concat(QuoteAmount.CustomerReferenceNo + "/" + QuoteAmount.CustomerQuoteNo + "." + QuoteAmount.QuoteSeqNo);
                    string Removal = await XMLHelper.GenerateRemovalXml(xmlResult, "Book Now", file);
                    //Task taskR = new Task(() => XMLHelper.XmlAPICall(Removal, 0));
                    //taskR.Start();

                    QuoteAmount.IsBooked = true;
                    message = _dbRepositoryQuoteAmount.Update(QuoteAmount);

                    sp_GetdataForEmailSending_Result xmlEmailResult = CustomRepository.GetQuoteData(SessionHelper.InternationalRemovalId, 3, "Courier");
                    xmlEmailResult.quoteName = "removals";
                    Task task = new Task(() => emailStatus = XMLHelper.SendEmail(xmlEmailResult, 5,"",""));
                    Task taskquote = new Task(() => emailStatus = XMLHelper.SendEmail(xmlEmailResult, 6,"",""));
                    task.Start();
                    taskquote.Start();
                    return message;
                }
                catch (Exception e)
                {
                    logger.Error(e);
                    TempData[CustomEnums.NotifyType.Error.GetDescription()] = CommonHelper.GetErrorMessage(e);
                    return e.Message;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                TempData[CustomEnums.NotifyType.Error.GetDescription()] = CommonHelper.GetErrorMessage(ex);
                return message;
            }
        }

        private string GetCountryName(string CountryCode)
        {
            string CountryName = string.Empty;
            try
            {
                var Country = _dbRepositoryCountry.GetEntities().Where(m => m.country_code == CountryCode).FirstOrDefault();
                CountryName = Country.country;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                TempData[CustomEnums.NotifyType.Error.GetDescription()] = CommonHelper.GetErrorMessage(ex);
            }
            return CountryName;
        }
        #endregion
    }
}