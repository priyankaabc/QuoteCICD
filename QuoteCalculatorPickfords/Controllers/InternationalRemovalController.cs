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
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace QuoteCalculatorPickfords.Controllers
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
        }
        #endregion

        #region Methods
        // GET: InternationalRemoval
        [HttpGet]
        public ActionResult Index(string countryCode)
        {
            //tbl_InternationalRemoval model = new tbl_InternationalRemoval();
            //ViewBag.SelectedCountryList = _dbRepositoryCountry.GetEntities().Where(m => m.country.Contains("United Kingdom")).Select(m => new { m.id, m.country }).OrderBy(m => new { m.country }).ToList();
            //var countryList = _dbRepositoryCountry.GetEntities().Where(m => m.rem_dest == 1).Select(m => new { m.country_code, m.country }).Distinct().ToList();
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
            //ViewBag.CountryId = countryCode;
            //SessionHelper.InternationalRemovalId = Convert.ToInt32(null);
            ////if (id != null && id > 0)
            ////{
            ////    model = _dbRepositoryInternationalRemoval.GetEntities().Where(m => m.Id == id).FirstOrDefault();
            ////    SessionHelper.InternationalRemovalId = model.Id;
            ////}
            return RedirectToAction("Index", "Baggage");
        }

        [HttpPost]
        public ActionResult Index(tbl_InternationalRemoval model)
        {
            string message = string.Empty;
            SessionHelper.ToCountryCode = model.ToCountryCode;
            SessionHelper.QuoteType = "3";
            if (model.Telephone.Substring(0, 1) != "0")
            {
                model.Telephone = "0" + model.Telephone;
            }
            model.CreatedDate = DateTime.Now;
            // var countryObj = _dbRepositoryCountry.GetEntities().Where(m => m.country == "United Kingdom").Select(m => new { m.id, m.country }).OrderBy(m => new { m.country }).FirstOrDefault();
            model.FromCountryName = "United Kingdom";  //countryObj.country;
            model.EstimatedMoveDate = Convert.ToDateTime(model.EstimatedMoveDate);
            model.Company = SessionHelper.COMPANY_ID;

            if (!ModelState.IsValid)
            {
                ViewBag.SelectedCountryList = _dbRepositoryCountry.GetEntities().Where(m => m.country.Contains("United Kingdom")).Select(m => new { m.id, m.country }).OrderBy(m => new { m.country }).ToList();
                ViewBag.CountryList = _dbRepositoryCountry.GetEntities().Where(m => m.country != "United Kingdom").Select(m => new { m.country_code, m.country }).Distinct().ToList();
                ViewBag.TitleList = _dbRepositorytitle.GetEntities().ToList();
                return View(model);
            }

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

                message = _dbRepositoryInternationalRemoval.Insert(model);
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
                    return Redirect("/InternationalRemoval/RemovalServiceSelection");


                    //var removalRequest = _dbRepositoryRequestTracking.GetEntities().Where(m => m.IpAddress == ip).ToList();
                    //if (removalRequest.Count() >= 10)
                    //{
                    //    var todayIpCount = _dbRepositoryRequestTracking.GetEntities()
                    //         .Where(m => m.IpAddress == ip && (DbFunctions.TruncateTime(m.CreatedDate.Value) == DateTime.Today)).Count();

                    //    string emailSuspendLink = "<a href='" + Url.Action("SetEmailRequest", "Vehicle", new RouteValueDictionary(new { key = "Email", Value = model.Email }), System.Web.HttpContext.Current.Request.Url.Scheme) + "'>" + "Suspend" + "</a>";
                    //    string emailOverrideLink = "<a href='" + "#" + "'>" + "Override" + "</a>";
                    //    string emailWarnLink = "<a href='#'> Warn </a>";

                    //    string ipSuspendLink = "<a href='" + Url.Action("SetEmailRequest", "Vehicle", new RouteValueDictionary(new { key = "Ip", Value = ip }), System.Web.HttpContext.Current.Request.Url.Scheme) + "'>" + "Suspend" + "</a>";
                    //    string ipOverrideLink = "<a href='" + "#" + "'>" + "Override" + "</a>";
                    //    string ipWarnLink = "<a href='#'> Warn </a>";

                    //    string bodyTemplate = System.IO.File.ReadAllText(Server.MapPath("~/Template/QuoteGuardWarning.html"));
                    //    var todayEmailCount = _dbRepositoryRequestTracking.GetEntities().Where(m => m.InternationalRemoval == model.Email && (DbFunctions.TruncateTime(m.CreatedDate.Value) == DateTime.Today)).Count();
                    //    var emailcount = _dbRepositoryRequestTracking.GetEntities().Where(m => m.InternationalRemoval == model.Email).Count();

                    //    bodyTemplate = bodyTemplate.Replace("[@emailAddress]", model.Email);
                    //    bodyTemplate = bodyTemplate.Replace("[@todayEmail]", todayEmailCount.ToString());
                    //    bodyTemplate = bodyTemplate.Replace("[@totalEmail]", emailcount.ToString());
                    //    bodyTemplate = bodyTemplate.Replace("[@totalIp]", removalRequest.Count().ToString());
                    //    bodyTemplate = bodyTemplate.Replace("[@todayIp]", todayIpCount.ToString());
                    //    bodyTemplate = bodyTemplate.Replace("[@emailStatusLink]", emailSuspendLink + " " + emailOverrideLink + " " + emailWarnLink);
                    //    bodyTemplate = bodyTemplate.Replace("[@ipStatusLink]", ipSuspendLink + " " + ipOverrideLink + " " + ipWarnLink);

                    //    Task task = new Task(() => EmailHelper.SendAsyncEmail(model.Email, "Quote guard - warning", bodyTemplate, true));
                    //    task.Start();
                    //    return RedirectToAction("Success", "Vehicle");
                    //}
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

        [HttpGet]
        public ActionResult RemovalServiceSelection()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RemovalServiceSelection(bool HomeConsultationOrService = false, bool HomeVideoSurvery = false, bool QuickOnlineQuote = false)
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

        [HttpGet]
        public ActionResult HomeConsultationOrService(string id)
        {
            var consultantId = Convert.ToInt32(CommonHelper.Decode(id));
            ViewBag.Id = consultantId;
            return View();
        }

        [HttpGet]
        public ActionResult HomeVideoSurvery(string id)
        {
            var QuoteId = Convert.ToInt32(CommonHelper.Decode(id));
            tbl_InternationalRemoval removal = _dbRepositoryInternationalRemoval.SelectById(QuoteId);
            var Title = _dbRepositorytitle.SelectById(removal.TitleId);
            var Country = _dbRepositoryCountry.GetEntities().Where(m => m.country_code == removal.ToCountryCode && m.CompanyId == SessionHelper.COMPANY_ID).FirstOrDefault();

            VideoSurveyModel model = new VideoSurveyModel();
            model.QuoteId = QuoteId;
            model.firstName = removal.Firstname;
            model.title = Title.TitleName.ToUpper();
            model.surName = removal.Lastname;
            model.email = removal.Email;
            model.mobileNumber = removal.Telephone;
            model.addressPickup = removal.FromCountryName.ToUpper();
            model.addressDropoff = Country.country;
            return View(model);
        }

        [HttpPost]
        public ActionResult SaveHomeVideoSurvey(int quoteId, long id, string appointmentTime)
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

        public ActionResult HomeVideoSurveyThankyou(int quoteId)
        {
            //var Id = Convert.ToInt32(CommonHelper.Decode(quoteId));
            tbl_InternationalRemoval model = _dbRepositoryInternationalRemoval.SelectById(quoteId);
            ViewBag.FirstName = model.Firstname;
            ViewBag.AppointmentTime = string.Format("{0:MMMM hh:mm tt}", model.VideoSurveyAppointmentTime);

           // tbl_InternationalRemoval model = _dbRepositoryInternationalRemoval.SelectById(id);
            string message = string.Empty;
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
            QuoteAmount.QuoteId = Convert.ToInt32(quoteId);
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

            SP_GetRemovalXmlData_Result xmlResult = CustomRepository.GetRemovalXmlData(quoteId);
            string file = string.Concat(customerReferenceNo + "/" + customerQuoteNo);
            //string Date = string.Format("{0:dd/MM/yyyy}", model.VideoSurveyAppointmentTime);
            //string Time = string.Format("{0:HH':'mm}", model.VideoSurveyAppointmentTime);
            string Removal = XMLHelper.GenerateHomeVideoSurveyXml(xmlResult,file,Convert.ToString(model.VideoSurveyAppointmentTime));
            Task task = new Task(() => XMLHelper.XmlAPICall(Removal, 0));
            task.Start();
            return View();
        }

        [HttpGet]
        public ActionResult QuickOnlineQuote(int? id)
        {
            QuickQuoteItemsModel model = new QuickQuoteItemsModel();
            model.InternationalRemovalId = Convert.ToInt32(id);
            model.items = _dbRepositoryQuickQuoteItems.GetEntities().OrderBy(m => m.DisplayOrder).ToList();
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
                    tbl_QuickQuoteItems item = _dbRepositoryQuickQuoteItems.SelectById(model.QuickQuoteItemId);
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
        public string HomeConsultationOrService(int? id, DateTime? value)
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
                string Removal = XMLHelper.GenerateHomeSurveyXml(xmlResult, file);
                Task task = new Task(() => XMLHelper.XmlAPICall(Removal, 0));
                task.Start();

                var emailTemplateObj = _dbRepositoryEmailTemplate.GetEntities().Where(m => m.ServiceId == 1005).FirstOrDefault();
                string bodyTemplate = string.Empty;
                if (emailTemplateObj != null)
                {
                    bodyTemplate = emailTemplateObj.HtmlContent;
                }
                bodyTemplate = bodyTemplate.Replace("#custName#", model.Firstname);
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

            var countryCodeParameter = new SqlParameter
            {
                ParameterName = "CountryCode",
                DbType = DbType.String,
                Value = countryCode
            };
            quotesEntities obj = new quotesEntities();
            var internationalRemovalCityList = obj.Database.SqlQuery<GetCityForInternationalRemoval_Result>("GetCityForInternationalRemoval @CountryCode", countryCodeParameter).ToList();
            return Json(internationalRemovalCityList, JsonRequestBehavior.AllowGet);
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

        public ActionResult ThankYou()
        {
            return View();
        }

        public ActionResult QuickOnlineQuoteDetail(int Id = 0, bool isMyQuote = false)
        {
            if (Id == 0)
            { 
                Id = SessionHelper.InternationalRemovalId;
            }
            var additionalquoteObj = _dbRepositoryAdditionalQuoteItems.GetEntities().Where(m => m.InternationalRemovalId == Id).ToList().LastOrDefault();
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
                tbl_InternationalRemoval model = _dbRepositoryInternationalRemoval.SelectById(Id);
                var emailTemplateObj = _dbRepositoryEmailTemplate.GetEntities().Where(m => m.ServiceId == 1005).FirstOrDefault();
                string bodyTemplate = string.Empty;
                if (emailTemplateObj != null)
                {
                    bodyTemplate = emailTemplateObj.HtmlContent;
                }
                bodyTemplate = bodyTemplate.Replace("#custName#", model.Firstname);
                Task quoteTask = new Task(() => EmailHelper.SendAsyncEmail(model.Email, emailTemplateObj.Subject, bodyTemplate, "EmailRemovals", "DisplayRemoval", true));
                quoteTask.Start();

                var QuoteId = new SqlParameter
                {
                    ParameterName = "QuoteId",
                    DbType = DbType.Int64,
                    Value = Id
                };

                quotesEntities obj = new quotesEntities();
                var result = obj.Database.SqlQuery<SP_RemovalCalculation_Result>("SP_RemovalCalculation @QuoteId", QuoteId).FirstOrDefault();
                if (result != null && result.FinalResult != null)
                {
                    SP_GetRemovalXmlData_Result xmlResult = CustomRepository.GetRemovalXmlData(Id);
                    var CustomerRefNo = _dbRepositoryQuoteAmount.GetEntities().Where(m => m.QuoteId == Id && m.MoveType == "EXR" && m.ShippingType == "Courier").FirstOrDefault();
                    string file = string.Concat(CustomerRefNo.CustomerReferenceNo + "/" + CustomerRefNo.CustomerQuoteNo + "." + CustomerRefNo.QuoteSeqNo);
                    string Removal = XMLHelper.GenerateRemovalXml(xmlResult, "Quote", file);
                    ViewBag.RemovalQuote = CustomerRefNo.QuoteAmount;
                    ViewBag.ReferenceNo = string.Concat(CustomerRefNo.CustomerReferenceNo + "/" + CustomerRefNo.CustomerQuoteNo);
                    Task task = new Task(() => XMLHelper.XmlAPICall(Removal, 0));
                    task.Start();
                }
                else
                {
                    SP_GetRemovalXmlData_Result xmlResult = CustomRepository.GetRemovalXmlData(Id);
                    var CustomerRefNo = _dbRepositoryQuoteAmount.GetEntities().Where(m => m.QuoteId == Id && m.MoveType == "EXR" && m.ShippingType == "ENQUIRY").FirstOrDefault();
                    string file = string.Concat(CustomerRefNo.CustomerReferenceNo + "/" + CustomerRefNo.CustomerQuoteNo);
                    string Removal = XMLHelper.GenerateRemovalEnquiryXml(xmlResult, "ENQUIRY", file);
                    ViewBag.RemovalQuote = CustomerRefNo.QuoteAmount;
                    ViewBag.ReferenceNo = string.Concat(CustomerRefNo.CustomerReferenceNo + "/" + CustomerRefNo.CustomerQuoteNo);
                    Task task = new Task(() => XMLHelper.XmlAPICall(Removal, 0));
                    task.Start();
                }
            }
            else
            {
                SessionHelper.ToCountryCode = internationalRemovalObj.ToCountryCode;
                SessionHelper.QuoteType = "3";
                var CustomerRefNo = _dbRepositoryQuoteAmount.GetEntities().Where(m => m.QuoteId == Id && m.MoveType == "EXR" && m.ShippingType == "Courier").FirstOrDefault();
                ViewBag.RemovalQuote = CustomerRefNo.QuoteAmount;
                ViewBag.ReferenceNo = string.Concat(CustomerRefNo.CustomerReferenceNo + "/" + CustomerRefNo.CustomerQuoteNo);
            }
            return View(additionalquoteObj);
        }

        [HttpPost]
        public string RemovalQuoteBook(int quoteId, string type)
        {
            string message = string.Empty;
            try
            {
                string emailStatus = string.Empty;
                var additionalquoteObj = _dbRepositoryAdditionalQuoteItems.GetEntities().Where(m => m.InternationalRemovalId == quoteId).ToList().LastOrDefault();
                var internationalRemovalObj = _dbRepositoryInternationalRemoval.GetEntities().Where(m => m.Id == quoteId).FirstOrDefault();
                SP_GetRemovalXmlData_Result xmlResult = CustomRepository.GetRemovalXmlData(quoteId);
                tbl_QuoteAmount QuoteAmount = _dbRepositoryQuoteAmount.GetEntities().Where(m => m.QuoteId == quoteId && m.MoveType == "EXR" && m.ShippingType == "Courier").FirstOrDefault();
                string file = string.Concat(QuoteAmount.CustomerReferenceNo + "/" + QuoteAmount.CustomerQuoteNo + "." + QuoteAmount.QuoteSeqNo);
                string Removal = XMLHelper.GenerateRemovalXml(xmlResult, "Book Now", file);
                Task taskR = new Task(() => XMLHelper.XmlAPICall(Removal, 0));
                taskR.Start();

                QuoteAmount.IsBooked = true;
                message = _dbRepositoryQuoteAmount.Update(QuoteAmount);

                sp_GetdataForEmailSending_Result xmlEmailResult = CustomRepository.GetQuoteData(SessionHelper.InternationalRemovalId, 3, "Courier");
                xmlEmailResult.quoteName = "removal";
                Task task = new Task(() => emailStatus = XMLHelper.SendEmail(xmlEmailResult, 5));
                Task taskquote = new Task(() => emailStatus = XMLHelper.SendEmail(xmlEmailResult, 6));
                task.Start();
                taskquote.Start();
                return message;
            }
            catch (Exception e)
            {
                logger.Error(e);
                return e.Message;
            }
        }

        private string GetCountryName(string CountryCode)
        {
            var Country = _dbRepositoryCountry.GetEntities().Where(m => m.country_code == CountryCode).FirstOrDefault();
            string CountryName = Country.country;
            return CountryName;
        }
        #endregion
    }
}