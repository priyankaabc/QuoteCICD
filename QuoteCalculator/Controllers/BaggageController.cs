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
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace QuoteCalculator.Controllers
{
    public class BaggageController : Controller
    {
        private static NLog.Logger logger = LogManager.GetCurrentClassLogger();
        #region private variables
        private readonly GenericRepository<tbl_baggageQuote> _dbRepositoryBaggageQuote;
        private readonly GenericRepository<rates_destinations> _dbRepositoryCountry;
        private readonly GenericRepository<tbl_Title> _dbRepositorytitle;
        private readonly GenericRepository<cartons> _dbRepositorycartons;
        private readonly GenericRepository<tbl_BaggageItem> _dbRepositoryMoveBaggage;
        public readonly GenericRepository<tbl_RequestTracking> _dbRepositoryRequestTracking;
        public readonly GenericRepository<tbl_TransitionTimeLine> _dbRepositoryTransitionTimeLine;
        public readonly GenericRepository<tbl_BlockIpOrEmail> _dbRepositoryBlockIpOrEmail;
        public readonly GenericRepository<tbl_WhiteListIp> _dbRepositoryWhiteListIp;
        public readonly GenericRepository<tbl_EmailTemplate> _dbRepositoryEmailTemplate;
        public readonly GenericRepository<tbl_QuoteAmount> _dbRepositoryQuoteAmount;
        public readonly GenericRepository<tbl_SMSLog> _dbRepositorySMSLog;
        public readonly GenericRepository<source> _dbRepositorySource;
        public readonly GenericRepository<tbl_CountryCode> _dbRepositoryCountryCode;
        public readonly GenericRepository<user> _dbRepositoryUser;
        private readonly GenericRepository<uk> _dbRepositoryUKPostCode;
        private readonly GenericRepository<tbl_InternationalRemoval> _dbRepositoryInternationalRemoval;
        private readonly GenericRepository<tbl_QuickQuoteItems> _dbRepositoryQuickQuoteItems;
        private readonly GenericRepository<tbl_AdditionalQuickQuoteItems> _dbRepositoryAdditionalQuoteItems;
        #endregion

        #region Constructor
        public BaggageController()
        {
            _dbRepositoryBaggageQuote = new GenericRepository<tbl_baggageQuote>();
            _dbRepositoryCountry = new GenericRepository<rates_destinations>();
            _dbRepositorytitle = new GenericRepository<tbl_Title>();
            _dbRepositorycartons = new GenericRepository<cartons>();
            _dbRepositoryMoveBaggage = new GenericRepository<tbl_BaggageItem>();
            _dbRepositoryRequestTracking = new GenericRepository<tbl_RequestTracking>();
            _dbRepositoryTransitionTimeLine = new GenericRepository<tbl_TransitionTimeLine>();
            _dbRepositoryBlockIpOrEmail = new GenericRepository<tbl_BlockIpOrEmail>();
            _dbRepositoryWhiteListIp = new GenericRepository<tbl_WhiteListIp>();
            _dbRepositoryEmailTemplate = new GenericRepository<tbl_EmailTemplate>();
            _dbRepositoryQuoteAmount = new GenericRepository<tbl_QuoteAmount>();
            _dbRepositorySMSLog = new GenericRepository<tbl_SMSLog>();
            _dbRepositorySource = new GenericRepository<source>();
            _dbRepositoryCountryCode = new GenericRepository<tbl_CountryCode>();
            _dbRepositoryUser = new GenericRepository<user>();
            _dbRepositoryUKPostCode = new GenericRepository<uk>();
            _dbRepositoryInternationalRemoval = new GenericRepository<tbl_InternationalRemoval>();
            _dbRepositoryQuickQuoteItems = new GenericRepository<tbl_QuickQuoteItems>();
            _dbRepositoryAdditionalQuoteItems = new GenericRepository<tbl_AdditionalQuickQuoteItems>();
        }
        #endregion

        #region Method
        // GET: Baggage
        [HttpGet]
        public ActionResult Index(string countryCode, string baggageId, bool? isOldQuote)
        {
            tbl_baggageQuote model = new tbl_baggageQuote();
            try
            {
                if (baggageId == null) SessionHelper.newBaggageId = 0;
                else SessionHelper.newBaggageId = Convert.ToInt32(CommonHelper.Decode(baggageId));

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
                var id = 0;
                SessionHelper.isOldQuote = isOldQuote == true ? true : false;
                if (baggageId != null)
                {
                    id = Convert.ToInt32(CommonHelper.Decode(baggageId));
                }
                //tbl_baggageQuote model = new tbl_baggageQuote();
                //ViewBag.SelectedCountryList = _dbRepositoryCountry.GetEntities().Where(m => m.country_code == "UK").Select(m => new { m.country_code, m.country }).OrderBy(m => new { m.country }).ToList();            
                var fromCountryList = _dbRepositoryCountry.GetEntities().Where(m => m.country_code != " " && m.CompanyId == SessionHelper.COMPANY_ID).Select(m => new { m.country_code, m.country }).Distinct().ToList();
                fromCountryList.Insert(0, new { country_code = "UK", country = "UNITED KINGDOM" });
                ViewBag.FromCountryList = fromCountryList;
                var toCountryList = _dbRepositoryCountry.GetEntities().Where(m => m.bag_dest == 1 && m.CompanyId == SessionHelper.COMPANY_ID).Select(m => new { m.country_code, m.country }).Distinct().ToList();
                toCountryList.Insert(0, new { country_code = "US", country = "USA" });
                toCountryList.Insert(0, new { country_code = "UK", country = "UNITED KINGDOM" });
                toCountryList.Insert(0, new { country_code = "AE", country = "UNITED ARAB EMIRATES" });
                toCountryList.Insert(0, new { country_code = "TH", country = "THAILAND" });
                toCountryList.Insert(0, new { country_code = "ZA", country = "SOUTH AFRICA" });
                toCountryList.Insert(0, new { country_code = "SG", country = "SINGAPORE" });
                toCountryList.Insert(0, new { country_code = "NZ", country = "NEW ZEALAND" });
                toCountryList.Insert(0, new { country_code = "MT", country = "MALTA" });
                toCountryList.Insert(0, new { country_code = "MY", country = "MALAYSIA" });
                toCountryList.Insert(0, new { country_code = "IN", country = "INDIA" });
                toCountryList.Insert(0, new { country_code = "HK", country = "HONG KONG" });
                toCountryList.Insert(0, new { country_code = "CY", country = "CYPRUS" });
                toCountryList.Insert(0, new { country_code = "CA", country = "CANADA" });
                toCountryList.Insert(0, new { country_code = "AU", country = "AUSTRALIA" });

                ViewBag.CountryList = toCountryList;
                ViewBag.TitleList = _dbRepositorytitle.GetEntities().ToList().OrderBy(x => x.DisplayOrder);
                ViewBag.CountryCodeList = _dbRepositoryCountryCode.GetEntities().Distinct().ToList().OrderBy(m => m.CountryName);
                model.CountryCode = "+44";
                ViewBag.CountryId = countryCode;


                model.EstimatedMoveDate = DateTime.Now;
                if (id > 0)
                {
                    model = _dbRepositoryBaggageQuote.GetEntities().Where(m => m.Id == id).FirstOrDefault();
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
        public ActionResult Index(tbl_baggageQuote model)
        {
            try
            {
                SessionHelper.FromCountryCode = model.FromCountry;
                SessionHelper.ToCountryCode = model.ToCountry;
                SessionHelper.QuoteType = "2";
                model.Firstname = model.Firstname.First().ToString().ToUpper() + model.Firstname.Substring(1);
                model.Lastname = model.Lastname.First().ToString().ToUpper() + model.Lastname.Substring(1);
                model.Telephone = model.Telephone;
                if (model.FromCountry != "UK")
                {
                    model.SalesRep = "NG2";
                }
                //ViewBag.SelectedCountryList = _dbRepositoryCountry.GetEntities().Where(m => m.country == "UK").Select(m => new { m.country_code, m.country }).OrderBy(m => new { m.country }).ToList();
                ViewBag.CountryList = _dbRepositoryCountry.GetEntities().Where(m => m.bag_dest == 1 && m.CompanyId == SessionHelper.COMPANY_ID).Select(m => new { m.country_code, m.country }).Distinct().ToList();
                ViewBag.TitleList = _dbRepositorytitle.GetEntities().ToList();

                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                string message = string.Empty;
                if (model.FromCountry != "UK")
                    model.PostCode = null;
                if (model.ToCountry != "UK")
                    model.ToPostCode = "";

                if (model.PostCode != null && model.PostCode != string.Empty)
                {
                    string pc = model.PostCode.Replace(" ", "");
                    pc = pc.Substring(0, pc.Length - 3) + " " + pc.Substring(pc.Length - 3, 3);
                    var postCodeObj = _dbRepositoryUKPostCode.GetEntities().Where(m => m.zip == pc).FirstOrDefault();
                    model.PostCode = postCodeObj.zip;
                }
                if (model.ToPostCode != null && model.ToPostCode != string.Empty)
                {
                    string pc = model.ToPostCode.Replace(" ", "");
                    pc = pc.Substring(0, pc.Length - 3) + " " + pc.Substring(pc.Length - 3, 3);
                    var postCodeObj = _dbRepositoryUKPostCode.GetEntities().Where(m => m.zip == pc).FirstOrDefault();
                    model.ToPostCode = postCodeObj.zip;
                }

                model.EstimatedMoveDate = Convert.ToDateTime(model.EstimatedMoveDate);
                model.Company = SessionHelper.COMPANY_ID;
                //model.FromCountry = "United Kingdom";

                SP_GetXmlData_Result xmlResult;
                try
                {
                    model.CreatedDate = DateTime.Now;
                    model.NextExecutionDate = DateTime.Today.AddDays(7);

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
                        else if (SessionHelper.COMPANY_ID == 3)
                            companyName = "EI";

                        var sourceObj = _dbRepositorySource.GetEntities().Where(m => m.sr_apcompany == companyName && m.id == sourceId).FirstOrDefault();
                        if (sourceObj != null)
                        {
                            model.Sr_Code = sourceObj.sr_code;
                            model.Sr_Name = sourceObj.sr_name;
                        }
                    }

                    if (SessionHelper.isOldQuote == true)
                    {
                        message = _dbRepositoryBaggageQuote.Update(model);
                    }
                    else
                    {
                        model.Id = 0;
                        message = _dbRepositoryBaggageQuote.Insert(model);
                    }
                    SessionHelper.BaggageId = model.Id;
                    if (message == "")
                    {
                        string ip = Request.UserHostAddress;
                        tbl_RequestTracking requestTracking = new tbl_RequestTracking();
                        requestTracking.Baggage = model.Email;
                        requestTracking.IpAddress = ip;
                        requestTracking.IsMailSend = false;
                        requestTracking.CreatedDate = DateTime.Now;
                        _dbRepositoryRequestTracking.Insert(requestTracking);
                        SessionHelper.QuoteId = Convert.ToInt32(model.Id);
                        //xmlResult = CustomRepository.GenerateBaggageXml

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
                        var requestEmail = _dbRepositoryRequestTracking.GetEntities().Where(m => m.Baggage == model.Email && (DbFunctions.TruncateTime(m.CreatedDate.Value) == DateTime.Today)).ToList();
                        var requestIpEmail = _dbRepositoryRequestTracking.GetEntities().Where(m => m.Baggage == model.Email && m.IpAddress == ip && (DbFunctions.TruncateTime(m.CreatedDate.Value) == DateTime.Today)).ToList();
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
                                                        .Where(m => m.IpAddress == ip && m.Baggage == model.Email).Count();
                                var totalEmailCount = _dbRepositoryRequestTracking.GetEntities().Where(m => m.Baggage == model.Email).Count();
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

                                var totalEmailCount = _dbRepositoryRequestTracking.GetEntities().Where(m => m.Baggage == model.Email).Count();
                                var todayIpCount = _dbRepositoryRequestTracking.GetEntities()
                                     .Where(m => m.IpAddress == ip && (DbFunctions.TruncateTime(m.CreatedDate.Value) == DateTime.Today)).Count();
                                var totalemailcount = _dbRepositoryRequestTracking.GetEntities().Where(m => m.Baggage == model.Email).Count();
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

                            Task task = new Task(() => EmailHelper.SendAsyncEmail(null, "Quote guard - warning", bodyTemplate, "EmailBaggage_" + SessionHelper.COMPANY_ID, "DisplayBaggage", true));
                            task.Start();
                        }

                        var blockIpList = _dbRepositoryBlockIpOrEmail.GetEntities().Where(m => m.Ip == ip).ToList();
                        var blockEmailList = _dbRepositoryBlockIpOrEmail.GetEntities().Where(m => m.Email == model.Email).ToList();
                        //xmlResult = CustomRepository.GetXmlData(model.Id);
                        if (blockIpList.LastOrDefault() != null)
                        {
                            if (blockIpList.LastOrDefault().Status == 2)
                            {
                                if (blockEmailList.LastOrDefault() != null)
                                {
                                    if (blockEmailList.LastOrDefault().Status == 2)
                                    {
                                        //XMLHelper.GenerateBaggageXml(xmlResult);
                                        SessionHelper.TempSessionForMoveDetails = true;
                                        return RedirectToAction("MoveDetails", "Baggage");
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
                                        //XMLHelper.GenerateBaggageXml(xmlResult);
                                        SessionHelper.TempSessionForMoveDetails = true;
                                        return RedirectToAction("MoveDetails", "Baggage");
                                    }
                                }
                            }
                            else
                                return RedirectToAction("Success", "Vehicle");
                        }

                        //XMLHelper.GenerateBaggageXml(xmlResult);
                        SessionHelper.TempSessionForMoveDetails = true;
                        return RedirectToAction("MoveDetails", "Baggage");
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
            SessionHelper.TempSessionForMoveDetails = true;
            return RedirectToAction("MoveDetails", "Baggage");
        }
        [HttpGet]
        public ActionResult MoveDetails()
        {
            BaggageModel obj = new BaggageModel();
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

                var cartonList = _dbRepositorycartons.GetEntities().Where(m => m.company == SessionHelper.COMPANY_ID).OrderBy(m => m.displayorder).ToList();
                List<CartonsModel> cartonModelList = new List<CartonsModel>();
                List<Movebaggage> moveBaggaheList = new List<Movebaggage>();
                var quoteItemList = _dbRepositoryMoveBaggage.GetEntities().Where(m => m.QuoteId == SessionHelper.newBaggageId && m.QuoteId != 0 && m.CartonId != 0).ToList();
                var quoteItem = _dbRepositoryMoveBaggage.GetEntities().Where(m => m.QuoteId == SessionHelper.newBaggageId && m.QuoteId != 0 && m.CartonId == 0).ToList();

                for (int i = 0; i < cartonList.Count(); i++)
                {
                    CartonsModel cr = new CartonsModel();
                    Movebaggage mb = new Movebaggage();
                    //mb.Volume = 0;// cartonList[i].volume;

                    cr.id = cartonList[i].id;
                    cr.type = cartonList[i].type;
                    cr.display = cartonList[i].display;
                    cr.displayorder = cartonList[i].displayorder;
                    cr.standout = cartonList[i].standout;
                    cr.description = cartonList[i].description;
                    cr.moveware_description = cartonList[i].moveware_description;
                    cr.length = cartonList[i].length;
                    cr.breadth = cartonList[i].breadth;
                    cr.height = cartonList[i].height;
                    cr.volume = cartonList[i].volume;
                    cr.weight = cartonList[i].weight;
                    cr.filter = cartonList[i].filter;
                    cr.image = cartonList[i].image;
                    cr.title = cartonList[i].title;
                    cr.quantity = 0;

                    if (quoteItemList.Count() > 0)
                    {
                        for (int j = 0; j < quoteItemList.Count(); j++)
                        {
                            if (quoteItemList[j].CartonId == cr.id)
                            {
                                cr.quantity = quoteItemList[j].Quantity;
                                cr.UserVolume = quoteItemList[j].UserVolume;
                            }
                        }
                    }
                    cartonModelList.Add(cr);
                    moveBaggaheList.Add(mb);
                }
                for (int i = 0; i < quoteItem.Count(); i++)
                {
                    Movebaggage mb = new Movebaggage();
                    mb.description = quoteItem[i].Description;
                    mb.length = quoteItem[i].Length;
                    mb.breadth = quoteItem[i].Breadth;
                    mb.height = quoteItem[i].Height;
                    mb.quantity = quoteItem[i].Quantity;
                    mb.Volume = quoteItem[i].Volume;
                    mb.UserVolume = quoteItem[i].UserVolume;
                    moveBaggaheList[i] = mb;
                }
                obj.cartonList = cartonModelList;
                obj.moveList = moveBaggaheList;
                // obj.cartonList = result;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                TempData[CustomEnums.NotifyType.Error.GetDescription()] = CommonHelper.GetErrorMessage(ex);
            }
            return View(obj);
        }

        [HttpPost]
        public ActionResult MoveDetails(BaggageModel obj)
        {
            try
            {
                if (!SessionHelper.TempSessionForMoveDetails)
                {
                    return RedirectToAction("Index", "Baggage");
                }
                bool CartoonValidation = false;
                bool MoveListValidation = false;

                //if (!ModelState.IsValid)
                //{
                //    ModelState.AddModelError("dimension", "Invalid Login Credentials.");
                //    return View(obj);
                //}

                if (obj.cartonList.Count > 0)
                {
                    foreach (var i in obj.cartonList)
                    {
                        if (i.quantity > 0)
                        {
                            CartoonValidation = true;
                            break;
                        }

                    }
                }
                if (obj.moveList.Count > 0)
                {
                    foreach (var i in obj.moveList)
                    {
                        if (i.dimension != null || i.description != null || i.quantity > 0)
                        {
                            CartoonValidation = true;
                            break;
                        }
                    }
                }
                if (CartoonValidation == false && MoveListValidation == false)
                {
                    TempData["ErrorMessage"] = "Please select at least one item";
                    return View(obj);
                }

                if (CartoonValidation == true || MoveListValidation == true)
                {
                    foreach (var item in obj.moveList)
                    {
                        tbl_BaggageItem moveBaggage = new tbl_BaggageItem();
                        //string allstock = item.dimension;
                        //if (allstock != null)
                        //{
                        //if (item.quantity > 0)
                        //{
                        //string arrayStocks = allstock.ToLower();
                        //string[] arrayStock = arrayStocks.Split('x');
                        var length = item.length == 0 ? 1 : item.length;
                        var breath = item.breadth == 0 ? 1 : item.breadth;
                        var height = item.height == 0 ? 1 : item.height;
                        double ans = (length * breath * height) / 28316.8;
                        //var volume = String.Format("{0:0.0#}", ans);
                        moveBaggage.Length = item.length;
                        moveBaggage.Breadth = item.breadth;
                        moveBaggage.Height = item.height;
                        moveBaggage.Description = item.description;
                        moveBaggage.Volume = Math.Round(ans); //item.Volume;
                        moveBaggage.Quantity = item.quantity;
                        moveBaggage.UserVolume = item.UserVolume;
                        moveBaggage.Type = "ADDITIONAL";
                        moveBaggage.QuoteId = SessionHelper.QuoteId;
                        moveBaggage.Company = SessionHelper.COMPANY_ID;
                        _dbRepositoryMoveBaggage.Insert(moveBaggage);
                        //}
                        //}
                    }

                    foreach (var list in obj.cartonList)
                    {
                        if (list != null)
                        {
                            if (list.display == 1)
                            {
                                if (list.quantity > 0)
                                {
                                    tbl_BaggageItem moveBaggage = new tbl_BaggageItem();
                                    var baggageId = _dbRepositoryMoveBaggage.GetEntities().Where(m => m.QuoteId == SessionHelper.QuoteId && m.CartonId == list.id).Select(m => m.Id).FirstOrDefault();
                                    moveBaggage.Id = baggageId;
                                    moveBaggage.CartonId = list.id;
                                    moveBaggage.Type = list.type;
                                    moveBaggage.Description = list.description;
                                    moveBaggage.Length = list.length;
                                    moveBaggage.Breadth = list.breadth;
                                    moveBaggage.Height = list.height;
                                    moveBaggage.Volume = list.volume;
                                    moveBaggage.UserVolume = list.UserVolume == null ? 0 : list.UserVolume;
                                    moveBaggage.Quantity = list.quantity;
                                    moveBaggage.QuoteId = SessionHelper.QuoteId;
                                    moveBaggage.Groweight = list.weight;
                                    moveBaggage.MovewareDescription = list.moveware_description;
                                    moveBaggage.Company = SessionHelper.COMPANY_ID;
                                    if (baggageId > 0)
                                        _dbRepositoryMoveBaggage.Update(moveBaggage);
                                    else
                                        _dbRepositoryMoveBaggage.Insert(moveBaggage);
                                }
                            }
                        }
                    }
                }

                if (SessionHelper.COMPANY_ID == 1)
                {
                    int? TotalCuFt = 0;
                    int? TotalMoveListCuFt = 0;
                    if (obj.cartonList.Count > 0)
                    {
                        foreach (var i in obj.cartonList)
                        {
                            if (i.TotalCuft > 0)
                            {
                                TotalCuFt += i.TotalCuft;
                            }

                        }
                    }

                    if (obj.moveList.Count > 0)
                    {
                        foreach (var i in obj.moveList)
                        {
                            if (i.TotalCuft > 0)
                            {
                                TotalCuFt += i.TotalCuft;
                                TotalMoveListCuFt += i.TotalCuft;
                            }

                        }
                    }

                    if (TotalCuFt >= 120 || TotalMoveListCuFt > 20)
                    {
                        string message = string.Empty;
                        tbl_baggageQuote baggageModel = new tbl_baggageQuote();
                        tbl_InternationalRemoval removalModel = new tbl_InternationalRemoval();
                        baggageModel = _dbRepositoryBaggageQuote.GetEntities().Where(m => m.Id == SessionHelper.BaggageId).FirstOrDefault();
                        if (baggageModel.FromCountry == "UK")
                        {
                            removalModel.Id = 0;
                            removalModel.Firstname = baggageModel.Firstname;
                            removalModel.Lastname = baggageModel.Lastname;
                            removalModel.Email = baggageModel.Email;
                            removalModel.CountryCode = baggageModel.CountryCode;
                            removalModel.Telephone = baggageModel.Telephone;
                            removalModel.TitleId = (int)baggageModel.TitleId;
                            removalModel.ToCountryCode = baggageModel.ToCountry;
                            removalModel.PostCode = baggageModel.PostCode;
                            removalModel.CityName = baggageModel.CityName;
                            removalModel.EstimatedMoveDate = baggageModel.EstimatedMoveDate;
                            removalModel.IsConditionApply = baggageModel.IsConditionApply;
                            removalModel.BranchId = baggageModel.BranchId;
                            removalModel.Sr_Code = baggageModel.Sr_Code;
                            removalModel.Sr_Name = baggageModel.Sr_Name;
                            removalModel.CreatedDate = baggageModel.CreatedDate;
                            removalModel.Company = (int)baggageModel.Company;
                            removalModel.IsDelete = baggageModel.IsDelete;
                            removalModel.PostCode = baggageModel.PostCode;

                            message = _dbRepositoryInternationalRemoval.Insert(removalModel);
                            string msg1;
                            msg1 = _dbRepositoryBaggageQuote.Delete(SessionHelper.BaggageId);

                            if (message == "")
                            {                                
                                List<tbl_BaggageItem> baggageItemModel = _dbRepositoryMoveBaggage.GetEntities().Where(m => m.QuoteId == SessionHelper.BaggageId).ToList();
                                string dimensionStr = "";
                                for (var i = 0; i < baggageItemModel.Count; i++)
                                {
                                    dimensionStr += Convert.ToString(baggageItemModel[i].Quantity + " X " + baggageItemModel[i].Description + " (" + baggageItemModel[i].Volume + " cubic feet" + (baggageItemModel[i].Quantity > 1 ? " each)" : ")"));
                                    if (baggageItemModel[i].Length > 0 && baggageItemModel[i].Breadth > 0 && baggageItemModel[i].Height > 0)
                                    {
                                        dimensionStr += Convert.ToString(", " + baggageItemModel[i].Length + " X " + baggageItemModel[i].Breadth + " X " + baggageItemModel[i].Height + " Cms");
                                    }
                                    if (baggageItemModel[i].UserVolume > 0)
                                    {
                                        dimensionStr += Convert.ToString(", " + baggageItemModel[i].UserVolume + " Kgs");
                                    }
                                    dimensionStr += " | ";
                                }

                                string message1;
                                SessionHelper.InternationalRemovalId = removalModel.Id; //_dbRepositoryInternationalRemoval.GetEntities().FirstOrDefault().Id;
                                tbl_AdditionalQuickQuoteItems mainModel = new tbl_AdditionalQuickQuoteItems();

                                mainModel.Beds = null;
                                mainModel.Cuft = TotalCuFt.ToString();
                                mainModel.Ftcontainer = null;
                                mainModel.SpecialRequirements = dimensionStr.Remove(dimensionStr.Length - 1, 1);
                                mainModel.InternationalRemovalId = SessionHelper.InternationalRemovalId;
                                mainModel.QuickQuoteItemId = null; // 12;
                                message1 = _dbRepositoryAdditionalQuoteItems.Insert(mainModel);

                                if (message1 == "")
                                {
                                    return RedirectToAction("QuickOnlineQuoteDetail", "InternationalRemoval");
                                }
                                else
                                {
                                    return RedirectToAction("MoveDetails", "Baggage");
                                }
                            }
                        }                        
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                TempData[CustomEnums.NotifyType.Error.GetDescription()] = CommonHelper.GetErrorMessage(ex);
            }
            SessionHelper.TempSessionForMoveDetails = false;
            return RedirectToAction("Baggage", "Baggage", new { @quoteId = CommonHelper.Encode(SessionHelper.QuoteId.ToString()) });
        }

        [HttpGet]
        public async Task<ActionResult>Baggage(string quoteId, bool? isMyQuote)
        {
            BaggageModel obj = new BaggageModel();
            try
            {
                int companyId = SessionHelper.COMPANY_ID;
                int volume = 0;
                double cubicFeet = 0;
                var Id = Convert.ToInt32(CommonHelper.Decode(quoteId));
                List<tbl_BaggageItem> cartonObjList = _dbRepositoryMoveBaggage.GetEntities().Where(m => m.QuoteId == Id && m.Company == companyId).ToList();

                bool wasQuoteGeneratedBefore = (_dbRepositoryQuoteAmount.GetEntities().Where(m => m.QuoteId == Id && m.MoveType == "EXB" && m.Company == companyId).Count() > 0);
                SessionHelper.BaggageId = Id;

                List<Movebaggage> moveObjList = new List<Movebaggage>();
                var baggageItemList = cartonObjList.Where(m => m.CartonId == 0).ToList();

                for (int i = 0; i < baggageItemList.Count(); i++)
                {
                    Movebaggage moveBaggage = new Movebaggage();
                    moveBaggage.description = baggageItemList[i].Description;
                    moveBaggage.length = baggageItemList[i].Length;
                    moveBaggage.breadth = baggageItemList[i].Breadth;
                    moveBaggage.height = baggageItemList[i].Height;
                    moveBaggage.quantity = baggageItemList[i].Quantity;
                    moveBaggage.UserVolume = baggageItemList[i].UserVolume;
                    volume += baggageItemList[i].UserVolume ?? 0;
                    cubicFeet += (baggageItemList[i].Volume * baggageItemList[i].Quantity);
                    moveObjList.Add(moveBaggage);
                }

                List<CartonsModel> cartonList = new List<CartonsModel>();
                var cartonItemList = cartonObjList.Where(m => m.CartonId != 0).ToList();
                for (int i = 0; i < cartonItemList.Count(); i++)
                {
                    CartonsModel moveBaggage = new CartonsModel();
                    moveBaggage.description = cartonItemList[i].Description;
                    moveBaggage.length = cartonItemList[i].Length;
                    moveBaggage.breadth = cartonItemList[i].Breadth;
                    moveBaggage.height = cartonItemList[i].Height;
                    moveBaggage.quantity = cartonItemList[i].Quantity;
                    moveBaggage.volume = cartonItemList[i].Volume;
                    moveBaggage.UserVolume = cartonItemList[i].UserVolume;
                    moveBaggage.type = cartonItemList[i].Type;
                    volume += cartonItemList[i].UserVolume ?? 0;
                    cubicFeet += (cartonItemList[i].Volume * cartonItemList[i].Quantity);
                    cartonList.Add(moveBaggage);
                }

                obj.moveList = moveObjList;
                obj.cartonList = cartonList;

                var QuoteId = new SqlParameter
                {
                    ParameterName = "QuoteId",
                    DbType = DbType.Int64,
                    Value = Id
                };
                tbl_baggageQuote model = _dbRepositoryBaggageQuote.GetEntities().Where(m => m.Id == Id && m.Company == companyId).FirstOrDefault();

                quotesEntities entityObj = new quotesEntities();
                BaggageCalculationModel baggageObj = null;
                bool isDuplicateQuoteRef = false;
                if (isMyQuote == true)
                {
                    SessionHelper.FromCountryCode = model.FromCountry;
                    SessionHelper.ToCountryCode = model.ToCountry;
                    SessionHelper.QuoteType = "2";
                    baggageObj = new BaggageCalculationModel();
                    baggageObj.AirFreightToAirport = Convert.ToDecimal(model.AirFreightToAirportFinal);
                    baggageObj.AirFreightToDoor = Convert.ToDecimal(model.AirFreightToDoorFinal);
                    baggageObj.SeaFreight = Convert.ToDecimal(model.SeaFreightFinal);
                    baggageObj.Courier = Convert.ToDecimal(model.CourierFinal);
                    baggageObj.RoadFreightToDoor = Convert.ToDecimal(model.RoadFreightToDoorFinal);
                    baggageObj.CourierExpressToDoor = Convert.ToDecimal(model.CourierExpressToDoorFinal);
                }
                else
                {
                    baggageObj = new BaggageCalculationModel();
                    baggageObj = entityObj.Database.SqlQuery<BaggageCalculationModel>("SP_BaggegeCalculation @QuoteId", QuoteId).FirstOrDefault();
                    tbl_baggageQuote updatedModel = _dbRepositoryBaggageQuote.GetEntities().Where(m => m.Id == Id && m.Company == companyId).FirstOrDefault();

                    string refNo = (updatedModel == null) ? null : updatedModel.ReferenceNumber;
                    isDuplicateQuoteRef = _dbRepositoryBaggageQuote.GetEntities().Count(m => m.Id != Id && !string.IsNullOrEmpty(m.ReferenceNumber) && m.ReferenceNumber == refNo) > 0;
                }

                if (isMyQuote != true)
                {
                    tbl_EmailTemplate emailTemplateObj;
                    if (companyId == 1)
                    {
                        if (baggageObj.AirFreightToAirport == 0 && baggageObj.AirFreightToDoor == 0 && baggageObj.Courier == 0 && baggageObj.SeaFreight == 0 && baggageObj.RoadFreightToDoor == 0 && baggageObj.CourierExpressToDoor == 0)
                            emailTemplateObj = _dbRepositoryEmailTemplate.GetEntities().Where(m => m.ServiceId == 1018).FirstOrDefault();
                        else
                            emailTemplateObj = _dbRepositoryEmailTemplate.GetEntities().Where(m => m.ServiceId == 1004).FirstOrDefault();
                    }
                    else if (companyId == 2)
                    {
                        if (baggageObj.AirFreightToAirport == 0 && baggageObj.AirFreightToDoor == 0 && baggageObj.Courier == 0 && baggageObj.SeaFreight == 0 && baggageObj.RoadFreightToDoor == 0 && baggageObj.CourierExpressToDoor == 0)
                            emailTemplateObj = _dbRepositoryEmailTemplate.GetEntities().Where(m => m.ServiceId == 1026).FirstOrDefault();
                        else
                            emailTemplateObj = _dbRepositoryEmailTemplate.GetEntities().Where(m => m.ServiceId == 1009).FirstOrDefault();
                    }
                    else if (companyId == 3)
                    {
                        if (baggageObj.AirFreightToAirport == 0 && baggageObj.AirFreightToDoor == 0 && baggageObj.Courier == 0 && baggageObj.SeaFreight == 0 && baggageObj.RoadFreightToDoor == 0 && baggageObj.CourierExpressToDoor == 0)
                            emailTemplateObj = _dbRepositoryEmailTemplate.GetEntities().Where(m => m.ServiceId == 1022).FirstOrDefault();
                        else
                            emailTemplateObj = _dbRepositoryEmailTemplate.GetEntities().Where(m => m.ServiceId == 1021).FirstOrDefault();
                    }
                    else
                    {
                        emailTemplateObj = new tbl_EmailTemplate();
                        throw new Exception("Email template cannot be created for Company ID: " + companyId);
                    }
                    string bodyTemplate = string.Empty;
                    if (emailTemplateObj != null)
                    {
                        bodyTemplate = emailTemplateObj.HtmlContent;
                    }
                    string salesRepName = string.Empty;
                    string salesRepEmail = string.Empty;
                    if (!string.IsNullOrEmpty(model.SalesRep))
                    {
                        user userModel = _dbRepositoryUser.GetEntities().Where(m => m.SalesRepCode == model.SalesRep && m.CompanyId == companyId).FirstOrDefault();
                        if (userModel != null)
                        {
                            salesRepName = userModel.username;
                            salesRepEmail = userModel.email;
                        }
                        else
                        {
                            if (companyId == 1)
                            {
                                salesRepName = "Baggage Team";
                                salesRepEmail = "baggage@anglopacific.co.uk";
                            }
                            else if (companyId == 2)
                            {
                                salesRepName = "Pickfords Baggage Team";
                                salesRepEmail = "shipping@pickfords-baggage.co.uk";
                            }
                            else if (companyId == 3)
                            {
                                salesRepName = "Sales Team";
                                salesRepEmail = "sales@excess-international.com";
                            }
                        }
                    }
                    else
                    {
                        if (companyId == 1)
                        {
                            salesRepName = "Baggage Team";
                            salesRepEmail = "baggage@anglopacific.co.uk";
                        }
                        else if (companyId == 2)
                        {
                            salesRepName = "Pickfords Baggage Team";
                            salesRepEmail = "shipping@pickfords-baggage.co.uk";
                        }
                        else if (companyId == 3)
                        {
                            salesRepName = "Sales Team";
                            salesRepEmail = "sales@excess-international.com";
                        }
                    }
                    bodyTemplate = bodyTemplate.Replace("#salesRepName#", salesRepName);
                    bodyTemplate = bodyTemplate.Replace("#salesRepEmail#", salesRepEmail);
                    bodyTemplate = bodyTemplate.Replace("#CustName#", model.Firstname);
                    Task quoteTask = new Task(() => EmailHelper.SendAsyncEmail(model.Email, emailTemplateObj.Subject, bodyTemplate, "EmailBaggage_" + companyId, "", true));
                    quoteTask.Start();
                }

                obj.calculationLines = new List<BaggageCalculationLineModel>();
                SP_GetBaggageXmlData_Result xmlResult = CustomRepository.GetBaggageXmlData(Id);
                SP_GetCollectionDelivery_Result xmlColDelResult = CustomRepository.GetCollectionDeliveryData(Id);
                List<string> xmlFiles = new List<string>();

                if (baggageObj.AirFreightToAirport > 0)
                {
                    xmlResult.IxType = "DTDE";
                    decimal VolumetricsWeight = CustomRepository.GetVolumetricsWeight(Id, "AIR");
                    string Desc = VolumetricsWeight.ToString() + " Vol kgs" + (volume > 0 ? ("/" + volume + " Kgs gross") : "");
                    obj.calculationLines.Add(GetBaggageCalcLine("Air Freight To Airport", baggageObj.AirFreightToAirport.Value, Desc));
                    var CustomerRefNo = _dbRepositoryQuoteAmount.GetEntities().Where(m => m.QuoteId == Id && m.MoveType == "EXB" && m.ShippingType == "AirfreightToAirport" && m.Company == companyId).FirstOrDefault();
                    string file = string.Concat(CustomerRefNo.CustomerReferenceNo + "/" + CustomerRefNo.CustomerQuoteNo + "." + CustomerRefNo.QuoteSeqNo);
                    if (isMyQuote != true && !isDuplicateQuoteRef && !wasQuoteGeneratedBefore)
                    {
                        string AirFreightToAirport =await XMLHelper.GenerateBaggageXml(xmlResult, cartonObjList, Convert.ToDouble(baggageObj.AirFreightToAirport.Value), xmlColDelResult, file, "AIRTOPORT"); //TODO            
                        xmlFiles.Add(AirFreightToAirport);
                    }
                    ViewBag.ReferenceNo = string.Concat(CustomerRefNo.CustomerReferenceNo + "/" + CustomerRefNo.CustomerQuoteNo);
                }
                if (baggageObj.AirFreightToDoor > 0)
                {
                    xmlResult.IxType = "DOP";
                    decimal VolumetricsWeight = CustomRepository.GetVolumetricsWeight(Id, "AIR");
                    string Desc = VolumetricsWeight.ToString() + " Vol kgs" + (volume > 0 ? ("/" + volume + " Kgs gross") : "");
                    obj.calculationLines.Add(GetBaggageCalcLine("Air Freight To Door", baggageObj.AirFreightToDoor.Value, Desc));
                    var CustomerRefNo = _dbRepositoryQuoteAmount.GetEntities().Where(m => m.QuoteId == Id && m.MoveType == "EXB" && m.ShippingType == "AirfreightToDoor" && m.Company == companyId).FirstOrDefault();
                    string file = string.Concat(CustomerRefNo.CustomerReferenceNo + "/" + CustomerRefNo.CustomerQuoteNo + "." + CustomerRefNo.QuoteSeqNo);
                    if (isMyQuote != true && !isDuplicateQuoteRef && !wasQuoteGeneratedBefore)
                    {
                        string AirFreightToDoor =await XMLHelper.GenerateBaggageXml(xmlResult, cartonObjList, Convert.ToDouble(baggageObj.AirFreightToDoor.Value), xmlColDelResult, file, "AIRTODOOR"); //TODO            
                        xmlFiles.Add(AirFreightToDoor);
                    }
                    ViewBag.ReferenceNo = string.Concat(CustomerRefNo.CustomerReferenceNo + "/" + CustomerRefNo.CustomerQuoteNo);
                }
                if (baggageObj.Courier > 0)
                {
                    xmlResult.IxType = "DTDE";
                    decimal VolumetricsWeight = CustomRepository.GetVolumetricsWeight(Id, "COURIER");
                    string Desc = VolumetricsWeight.ToString() + " Vol kgs" + (volume > 0 ? ("/" + volume + " Kgs gross") : "");
                    obj.calculationLines.Add(GetBaggageCalcLine("Courier", baggageObj.Courier.Value, Desc));
                    var CustomerRefNo = _dbRepositoryQuoteAmount.GetEntities().Where(m => m.QuoteId == Id && m.MoveType == "EXB" && m.ShippingType == "Courier" && m.Company == companyId).FirstOrDefault();
                    string file = string.Concat(CustomerRefNo.CustomerReferenceNo + "/" + CustomerRefNo.CustomerQuoteNo + "." + CustomerRefNo.QuoteSeqNo);
                    var currentquoteObj = _dbRepositoryBaggageQuote.GetEntities().Where(m => m.Id == Id && m.Company == SessionHelper.COMPANY_ID).FirstOrDefault();
                    List<BaggageCostModel> _baggeCostModel = null;
                    if (currentquoteObj != null && (currentquoteObj.ToCountry == "UK"))
                    {
                        //considercostToXML = true;
                        _baggeCostModel = GetBaggageCostByQuoteId(Id, "Bag Imports UK");
                    }
                    else if (currentquoteObj != null && currentquoteObj.FromCountry != "UK" && currentquoteObj.ToCountry != "UK")
                    {
                        _baggeCostModel = GetBaggageCostByQuoteId(Id, "Bag C2C");
                    }
                    else
                    {
                        _baggeCostModel = GetBaggageCostByQuoteId(Id, "Curier Economy");
                    }
                    if (isMyQuote != true && !isDuplicateQuoteRef && !wasQuoteGeneratedBefore)
                    {
                        string Courier = await XMLHelper.GenerateBaggageXml(xmlResult, cartonObjList, Convert.ToDouble(baggageObj.Courier.Value), xmlColDelResult, file, "Courier", _baggeCostModel);
                        xmlFiles.Add(Courier);
                    }
                    ViewBag.ReferenceNo = string.Concat(CustomerRefNo.CustomerReferenceNo + "/" + CustomerRefNo.CustomerQuoteNo);
                }
                if (baggageObj.SeaFreight > 0)
                {
                    xmlResult.IxType = "DTDE";
                    string Desc = string.Concat(cubicFeet, " cubic feet");
                    obj.calculationLines.Add(GetBaggageCalcLine("Sea Freight", baggageObj.SeaFreight.Value, Desc));
                    var CustomerRefNo = _dbRepositoryQuoteAmount.GetEntities().Where(m => m.QuoteId == Id && m.MoveType == "EXB" && m.ShippingType == "SeaFreight" && m.Company == companyId).FirstOrDefault();
                    string file = string.Concat(CustomerRefNo.CustomerReferenceNo + "/" + CustomerRefNo.CustomerQuoteNo + "." + CustomerRefNo.QuoteSeqNo);
                    if (isMyQuote != true && !isDuplicateQuoteRef && !wasQuoteGeneratedBefore)
                    {
                        List<BaggageCostModel> _baggeCostModel = GetBaggageCostByQuoteId(Id, "Sea Freight");
                        string SeaFreight = await XMLHelper.GenerateBaggageXml(xmlResult, cartonObjList, Convert.ToDouble(baggageObj.SeaFreight.Value), xmlColDelResult, file, "Sea", _baggeCostModel); //TODO            
                        xmlFiles.Add(SeaFreight);
                    }
                    ViewBag.ReferenceNo = string.Concat(CustomerRefNo.CustomerReferenceNo + "/" + CustomerRefNo.CustomerQuoteNo);
                }
                if (baggageObj.RoadFreightToDoor > 0)
                {
                    xmlResult.IxType = "DTDE";
                    string Desc = string.Concat(cubicFeet, " cubic feet");
                    obj.calculationLines.Add(GetBaggageCalcLine("Road Freight To Door", baggageObj.RoadFreightToDoor.Value, Desc));
                    var CustomerRefNo = _dbRepositoryQuoteAmount.GetEntities().Where(m => m.QuoteId == Id && m.MoveType == "EXB" && m.ShippingType == "RoadfreightToDoor" && m.Company == companyId).FirstOrDefault();
                    string file = string.Concat(CustomerRefNo.CustomerReferenceNo + "/" + CustomerRefNo.CustomerQuoteNo + "." + CustomerRefNo.QuoteSeqNo);
                    if (isMyQuote != true && !isDuplicateQuoteRef && !wasQuoteGeneratedBefore)
                    {
                        string RoadFreightToDoor = await XMLHelper.GenerateBaggageXml(xmlResult, cartonObjList, Convert.ToDouble(baggageObj.RoadFreightToDoor.Value), xmlColDelResult, file, "Road"); //TODO            
                        xmlFiles.Add(RoadFreightToDoor);
                    }
                    ViewBag.ReferenceNo = string.Concat(CustomerRefNo.CustomerReferenceNo + "/" + CustomerRefNo.CustomerQuoteNo);
                }
                if (baggageObj.CourierExpressToDoor > 0)
                {
                    xmlResult.IxType = "DTDE";
                    decimal VolumetricsWeight = CustomRepository.GetVolumetricsWeight(Id, "COURIEREXPRESS");
                    string Desc = VolumetricsWeight.ToString() + " Vol kgs" + (volume > 0 ? ("/" + volume + " Kgs gross") : "");
                    obj.calculationLines.Add(GetBaggageCalcLine("Courier Express To Door", baggageObj.CourierExpressToDoor.Value, Desc));
                    var CustomerRefNo = _dbRepositoryQuoteAmount.GetEntities().Where(m => m.QuoteId == Id && m.MoveType == "EXB" && m.ShippingType == "CourierExpressToDoor" && m.Company == companyId).FirstOrDefault();
                    string file = string.Concat(CustomerRefNo.CustomerReferenceNo + "/" + CustomerRefNo.CustomerQuoteNo + "." + CustomerRefNo.QuoteSeqNo);
                    if (isMyQuote != true && !isDuplicateQuoteRef && !wasQuoteGeneratedBefore)
                    {
                        List<BaggageCostModel> _baggeCostModel = GetBaggageCostByQuoteId(Id, "Curier Express");
                        string CourierExpress = await XMLHelper.GenerateBaggageXml(xmlResult, cartonObjList, Convert.ToDouble(baggageObj.CourierExpressToDoor.Value), xmlColDelResult, file, "CourierExpress", _baggeCostModel); //TODO            
                        xmlFiles.Add(CourierExpress);
                    }
                    ViewBag.ReferenceNo = string.Concat(CustomerRefNo.CustomerReferenceNo + "/" + CustomerRefNo.CustomerQuoteNo);
                }

                //if (xmlFiles.Count > 0 && !wasQuoteGeneratedBefore && !isDuplicateQuoteRef)
                //{
                //    Task task = new Task(() => SendXmlFies(xmlFiles));
                //    task.Start();
                //}

                if (baggageObj.AirFreightToAirport == 0 && baggageObj.AirFreightToDoor == 0 && baggageObj.Courier == 0 && baggageObj.SeaFreight == 0 && baggageObj.RoadFreightToDoor == 0 && baggageObj.CourierExpressToDoor == 0)
                {
                    var CustomerRefNo = _dbRepositoryQuoteAmount.GetEntities().Where(m => m.QuoteId == Id && m.MoveType == "EXB" && m.ShippingType == "ENQUIRY" && m.Company == companyId).FirstOrDefault();
                    string file = string.Concat(CustomerRefNo.CustomerReferenceNo + "/" + CustomerRefNo.CustomerQuoteNo + "." + CustomerRefNo.QuoteSeqNo);
                    if (isMyQuote != true && !isDuplicateQuoteRef && !wasQuoteGeneratedBefore)
                    {
                        string Enquiry = await XMLHelper.GenerateBaggageEnquiryXml(xmlResult, cartonObjList, file); //TODO            
                        //Task task = new Task(() => XMLHelper.XmlAPICall(Enquiry, 0));
                        //task.Start();
                    }
                    ViewBag.ReferenceNo = string.Concat(CustomerRefNo.CustomerReferenceNo + "/" + CustomerRefNo.CustomerQuoteNo);
                }

                var baggageObject = _dbRepositoryBaggageQuote.GetEntities().Where(m => m.Id == Id).FirstOrDefault();
                if (baggageObject != null)
                {
                    ViewBag.isMethodSelected = false;
                    for (int i = 0; i < obj.calculationLines.Count; i++)
                    {
                        if (obj.calculationLines[i].DeliveryMethodName == "Air Freight To Airport")
                        {
                            if (baggageObject.AirFreightToAirport == true)
                            {
                                ViewBag.isMethodSelected = true;
                                obj.calculationLines[i].isSelected = true;
                            }
                        }
                        if (obj.calculationLines[i].DeliveryMethodName == "Air Freight To Door")
                        {
                            if (baggageObject.AirFreightToDoor == true)
                            {
                                ViewBag.isMethodSelected = true;
                                obj.calculationLines[i].isSelected = true;
                            }
                        }
                        if (obj.calculationLines[i].DeliveryMethodName == "Courier Economy To Door")
                        {
                            if (baggageObject.Courier == true)
                            {
                                ViewBag.isMethodSelected = true;
                                obj.calculationLines[i].isSelected = true;
                            }
                        }
                        if (obj.calculationLines[i].DeliveryMethodName == "Sea Freight To Door")
                        {
                            if (baggageObject.SeaFreight == true)
                            {
                                ViewBag.isMethodSelected = true;
                                obj.calculationLines[i].isSelected = true;
                            }
                        }
                        if (obj.calculationLines[i].DeliveryMethodName == "Road Freight To Door")
                        {
                            if (baggageObject.RoadFreightToDoor == true)
                            {
                                ViewBag.isMethodSelected = true;
                                obj.calculationLines[i].isSelected = true;
                            }
                        }
                        if (obj.calculationLines[i].DeliveryMethodName == "Courier Express To Door")
                        {
                            if (baggageObject.CourierExpressToDoor == true)
                            {
                                ViewBag.isMethodSelected = true;
                                obj.calculationLines[i].isSelected = true;
                            }
                        }
                    }
                }

                obj.calculationLines = obj.calculationLines.OrderBy(x => x.Amount).ToList();
                var shipping = _dbRepositoryBaggageQuote.GetEntities().Where(m => m.Id == Id && m.Company == companyId).FirstOrDefault();
                ViewBag.FromCountry = GetCountryName(shipping.FromCountry);
                ViewBag.FromCity = string.IsNullOrEmpty(shipping.FromCity) ? shipping.PostCode : shipping.FromCity;
                ViewBag.ToCountry = GetCountryName(shipping.ToCountry);
                ViewBag.ToCity = string.IsNullOrEmpty(shipping.CityName) ? shipping.ToPostCode : shipping.CityName;
                ViewBag.InternalNotes = string.IsNullOrEmpty(shipping.InternalNotes) ? null : shipping.InternalNotes;

                ViewBag.DeliveryCharge = xmlColDelResult.DeliveryCharge;
                ViewBag.CollectionCharge = xmlColDelResult.CollectionCharge;
                //Check condition in db
                var checkReq = _dbRepositoryBaggageQuote.GetEntities().Where(m => m.Telephone == shipping.Telephone && m.Company == companyId && m.CreatedDate >= DateTime.Today && shipping.ReferenceNumber.Substring(shipping.ReferenceNumber.Length - 2) == "/1" && m.ReferenceNumber.Substring(m.ReferenceNumber.Length - 2) == "/1").ToList();
                if (checkReq.Count == 1)
                {
                    //string DomainName = Request.Url.Host + (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port);
                    //string myQuoteUrl = DomainName + "/MyQuote";
                    string myQuoteUrl;
                    string SMSText;
                    if (companyId == 1)
                    {
                        myQuoteUrl = "https://quotes.anglopacific.co.uk/MyQuote";
                        SMSText = "Thank you for your online quote request. You can check your quotes here :" + myQuoteUrl + "' or call us on 0800 783 5322 to discuss. Anglo Pacific Shipping.";
                    }
                    else if (companyId == 2)
                    {
                        myQuoteUrl = "https://quotes.pickfords-baggage.co.uk/MyQuote";
                        SMSText = "Thank you for your online quote request. You can check your quotes here :" + myQuoteUrl + "' or call us on 08000 190 333 to discuss. Pickfords International Baggage Services.";
                    }
                    else if (companyId == 3)
                    {
                        myQuoteUrl = "https://quotes.excess-baggage.co.uk/MyQuote";
                        SMSText = "Thank you for your online quote request. You can check your quotes here: " + myQuoteUrl + "' or call us on (0)20 8324 2066 to discuss. Excess International Movers Ltd.";
                    }
                    else
                        throw new Exception("Company for Company Id: " + companyId + " Not Found.");


                    var mno = shipping.CountryCode + shipping.Telephone.TrimStart('0');
                    SMSHelper.SendSMS(companyId, mno, SMSText);
                    var newObj = new tbl_SMSLog();
                    newObj.QuoteId = shipping.Id.ToString();
                    newObj.Direction = "OUT_GOING";
                    newObj.SMSText = SMSText;
                    newObj.CratedDate = DateTime.Now;
                    _dbRepositorySMSLog.Insert(newObj);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                TempData[CustomEnums.NotifyType.Error.GetDescription()] = CommonHelper.GetErrorMessage(ex);
            }
            return View(obj);
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
                var BaggageCityList = obj.Database.SqlQuery<CityListModel>("GetCityForBaggae @CountryCode, @CompanyCode", countryCodeParameter, companyCodeParameter).ToList();
                return Json(BaggageCityList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                TempData[CustomEnums.NotifyType.Error.GetDescription()] = CommonHelper.GetErrorMessage(ex);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<string> BaggageQuoteCal(string colName, string quotePrice, int quoteNo = 0)
        {
            try
            {
                if (quoteNo == 0)
                {
                    quoteNo = SessionHelper.QuoteId;
                }
                colName = colName == "Courier Economy To Door" ? "Courier" : (colName == "Sea Freight To Door" ? "SeaFreight" : colName);
                quotePrice = quotePrice.Replace("£", "");
                string message = string.Empty;
                string MethodName = colName.Replace(" ", "");
                try
                {
                    if (!string.IsNullOrEmpty(MethodName))
                    {
                        string emailStatus = string.Empty;
                        tbl_baggageQuote model = _dbRepositoryBaggageQuote.GetEntities().Where(m => m.Id == quoteNo && m.Company == SessionHelper.COMPANY_ID).FirstOrDefault();
                        List<tbl_BaggageItem> cartonObjList = _dbRepositoryMoveBaggage.GetEntities().Where(m => m.QuoteId == quoteNo && m.Company == SessionHelper.COMPANY_ID).ToList();
                        SP_GetCollectionDelivery_Result xmlColDelResult = CustomRepository.GetCollectionDeliveryData(quoteNo);
                        if (model != null)
                        {
                            model.Price = Convert.ToDouble(quotePrice);

                            if (MethodName == "AirFreightToAirport" || MethodName == "AirfreightToAirport")
                            {
                                model.AirFreightToAirport = true;
                                model.AirFreightToDoor = false;
                                model.Courier = false;
                                model.SeaFreight = false;
                                model.RoadFreightToDoor = false;
                                model.CourierExpressToDoor = false;
                                message = _dbRepositoryBaggageQuote.Update(model);
                                SP_GetBaggageXmlData_Result xmlResult = CustomRepository.GetBaggageXmlData(quoteNo);
                                xmlResult.IxType = "DTDE";
                                var CustomerRefNo = _dbRepositoryQuoteAmount.GetEntities().Where(m => m.QuoteId == quoteNo && m.MoveType == "EXB" && m.ShippingType == "AirFreightToAirport" && m.Company == SessionHelper.COMPANY_ID).FirstOrDefault();
                                string file = string.Concat(CustomerRefNo.CustomerReferenceNo + "/" + CustomerRefNo.CustomerQuoteNo + "." + CustomerRefNo.QuoteSeqNo);
                                string AirFreightToAirport = await XMLHelper.GenerateBaggageXml(xmlResult, cartonObjList, model.Price.Value, xmlColDelResult, file, "AIRTOPORT");
                                //Task AF = new Task(() => XMLHelper.XmlAPICall(AirFreightToAirport, 0));
                                //AF.Start();
                                sp_GetdataForEmailSending_Result xmlEmailResult = CustomRepository.GetQuoteData(quoteNo, 2, "AirfreighToAirport");
                                xmlEmailResult.quoteName = (SessionHelper.COMPANY_ID == 1) ? "baggage" : (SessionHelper.COMPANY_ID == 2 ? "shipping" : "sales");
                                xmlEmailResult.ReferenceNo = file;
                                Task task = new Task(() => emailStatus = XMLHelper.SendEmail(xmlEmailResult, (SessionHelper.COMPANY_ID == 1) ? 3 : (SessionHelper.COMPANY_ID == 2) ? 1012 : 1024, AirFreightToAirport, file));
                                Task taskquote = new Task(() => emailStatus = XMLHelper.SendEmail(xmlEmailResult, (SessionHelper.COMPANY_ID == 1) ? 4 : (SessionHelper.COMPANY_ID == 2) ? 1011 : 1023, "",""));
                                task.Start();
                                taskquote.Start();
                            }
                            if (MethodName == "AirFreightToDoor" || MethodName == "AirfreightToDoor")
                            {
                                model.AirFreightToAirport = false;
                                model.AirFreightToDoor = true;
                                model.Courier = false;
                                model.SeaFreight = false;
                                model.RoadFreightToDoor = false;
                                model.CourierExpressToDoor = false;
                                message = _dbRepositoryBaggageQuote.Update(model);
                                SP_GetBaggageXmlData_Result xmlResult = CustomRepository.GetBaggageXmlData(quoteNo);
                                xmlResult.IxType = "DOP";
                                var CustomerRefNo = _dbRepositoryQuoteAmount.GetEntities().Where(m => m.QuoteId == quoteNo && m.MoveType == "EXB" && m.ShippingType == "AirFreightToDoor" && m.Company == SessionHelper.COMPANY_ID).FirstOrDefault();
                                string file = string.Concat(CustomerRefNo.CustomerReferenceNo + "/" + CustomerRefNo.CustomerQuoteNo + "." + CustomerRefNo.QuoteSeqNo);
                                string AirFreightToDoor = await XMLHelper.GenerateBaggageXml(xmlResult, cartonObjList, model.Price.Value, xmlColDelResult, file, "AIRTODOOR");
                                //Task AF = new Task(() => XMLHelper.XmlAPICall(AirFreightToDoor, 0));
                                //AF.Start();
                                sp_GetdataForEmailSending_Result xmlEmailResult = CustomRepository.GetQuoteData(quoteNo, 2, "AirfreightToDoor");
                                xmlEmailResult.quoteName = (SessionHelper.COMPANY_ID == 1) ? "baggage" : (SessionHelper.COMPANY_ID == 2 ? "shipping" : "sales");
                                xmlEmailResult.ReferenceNo = file;
                                Task task = new Task(() => emailStatus = XMLHelper.SendEmail(xmlEmailResult, (SessionHelper.COMPANY_ID == 1) ? 3 : (SessionHelper.COMPANY_ID == 2) ? 1012 : 1024, AirFreightToDoor, file));
                                Task taskquote = new Task(() => emailStatus = XMLHelper.SendEmail(xmlEmailResult, (SessionHelper.COMPANY_ID == 1) ? 4 : (SessionHelper.COMPANY_ID == 2) ? 1011 : 1023,"", ""));
                                task.Start();
                                taskquote.Start();
                            }
                            if (MethodName == "Courier")
                            {
                                model.AirFreightToAirport = false;
                                model.AirFreightToDoor = false;
                                model.Courier = true;
                                model.SeaFreight = false;
                                model.RoadFreightToDoor = false;
                                model.CourierExpressToDoor = false;
                                message = _dbRepositoryBaggageQuote.Update(model);
                                SP_GetBaggageXmlData_Result xmlResult = CustomRepository.GetBaggageXmlData(quoteNo);
                                xmlResult.IxType = "DTDE";
                                var CustomerRefNo = _dbRepositoryQuoteAmount.GetEntities().Where(m => m.QuoteId == quoteNo && m.MoveType == "EXB" && m.ShippingType == "Courier" && m.Company == SessionHelper.COMPANY_ID).FirstOrDefault();
                                string file = string.Concat(CustomerRefNo.CustomerReferenceNo + "/" + CustomerRefNo.CustomerQuoteNo + "." + CustomerRefNo.QuoteSeqNo);
                                string Courier = await XMLHelper.GenerateBaggageXml(xmlResult, cartonObjList, model.Price.Value, xmlColDelResult, file, "Courier");
                                //Task C = new Task(() => XMLHelper.XmlAPICall(Courier, 0));
                                //C.Start();
                                sp_GetdataForEmailSending_Result xmlEmailResult = CustomRepository.GetQuoteData(quoteNo, 2, "Courier");
                                xmlEmailResult.quoteName = (SessionHelper.COMPANY_ID == 1) ? "baggage" : (SessionHelper.COMPANY_ID == 2 ? "shipping" : "sales");
                                xmlEmailResult.ReferenceNo = file;
                                Task task = new Task(() => emailStatus = XMLHelper.SendEmail(xmlEmailResult, (SessionHelper.COMPANY_ID == 1) ? 3 : (SessionHelper.COMPANY_ID == 2) ? 1012 : 1024, Courier, file));
                                Task taskquote = new Task(() => emailStatus = XMLHelper.SendEmail(xmlEmailResult, (SessionHelper.COMPANY_ID == 1) ? 4 : (SessionHelper.COMPANY_ID == 2) ? 1011 : 1023, "",""));
                                task.Start();
                                taskquote.Start();
                            }
                            if (MethodName == "SeaFreight")
                            {
                                model.AirFreightToAirport = false;
                                model.AirFreightToDoor = false;
                                model.Courier = false;
                                model.SeaFreight = true;
                                model.RoadFreightToDoor = false;
                                model.CourierExpressToDoor = false;
                                message = _dbRepositoryBaggageQuote.Update(model);
                                SP_GetBaggageXmlData_Result xmlResult = CustomRepository.GetBaggageXmlData(quoteNo);
                                xmlResult.IxType = "DTDE";
                                var CustomerRefNo = _dbRepositoryQuoteAmount.GetEntities().Where(m => m.QuoteId == quoteNo && m.MoveType == "EXB" && m.ShippingType == "SeaFreight" && m.Company == SessionHelper.COMPANY_ID).FirstOrDefault();
                                string file = string.Concat(CustomerRefNo.CustomerReferenceNo + "/" + CustomerRefNo.CustomerQuoteNo + "." + CustomerRefNo.QuoteSeqNo);
                                string SeaFreight = await XMLHelper.GenerateBaggageXml(xmlResult, cartonObjList, model.Price.Value, xmlColDelResult, file, "Sea");
                                //Task SF = new Task(() => XMLHelper.XmlAPICall(SeaFreight, 0));
                                //SF.Start();
                                sp_GetdataForEmailSending_Result xmlEmailResult = CustomRepository.GetQuoteData(quoteNo, 2, "SeaFreight");
                                xmlEmailResult.quoteName = (SessionHelper.COMPANY_ID == 1) ? "baggage" : (SessionHelper.COMPANY_ID == 2 ? "shipping" : "sales");
                                xmlEmailResult.ReferenceNo = file;
                                Task task = new Task(() => emailStatus = XMLHelper.SendEmail(xmlEmailResult, (SessionHelper.COMPANY_ID == 1) ? 3 : (SessionHelper.COMPANY_ID == 2) ? 1012 : 1024, SeaFreight, file));
                                Task taskquote = new Task(() => emailStatus = XMLHelper.SendEmail(xmlEmailResult, (SessionHelper.COMPANY_ID == 1) ? 4 : (SessionHelper.COMPANY_ID == 2) ? 1011 : 1023, "",""));
                                task.Start();
                                taskquote.Start();
                            }
                            if (MethodName == "RoadFreightToDoor" || MethodName == "RoadfreightToDoor")
                            {
                                model.AirFreightToAirport = false;
                                model.AirFreightToDoor = false;
                                model.Courier = false;
                                model.SeaFreight = false;
                                model.RoadFreightToDoor = true;
                                model.CourierExpressToDoor = false;
                                message = _dbRepositoryBaggageQuote.Update(model);
                                SP_GetBaggageXmlData_Result xmlResult = CustomRepository.GetBaggageXmlData(quoteNo);
                                xmlResult.IxType = "DTDE";
                                var CustomerRefNo = _dbRepositoryQuoteAmount.GetEntities().Where(m => m.QuoteId == quoteNo && m.MoveType == "EXB" && m.ShippingType == "RoadfreightToDoor" && m.Company == SessionHelper.COMPANY_ID).FirstOrDefault();
                                string file = string.Concat(CustomerRefNo.CustomerReferenceNo + "/" + CustomerRefNo.CustomerQuoteNo + "." + CustomerRefNo.QuoteSeqNo);
                                string RoadFreightToDoor = await XMLHelper.GenerateBaggageXml(xmlResult, cartonObjList, model.Price.Value, xmlColDelResult, file, "Road");
                                //Task AF = new Task(() => XMLHelper.XmlAPICall(RoadFreightToDoor, 0));
                                //AF.Start();
                                sp_GetdataForEmailSending_Result xmlEmailResult = CustomRepository.GetQuoteData(quoteNo, 2, "RoadfreighToDoor");
                                xmlEmailResult.quoteName = (SessionHelper.COMPANY_ID == 1) ? "baggage" : (SessionHelper.COMPANY_ID == 2 ? "shipping" : "sales");
                                xmlEmailResult.ReferenceNo = file;
                                Task task = new Task(() => emailStatus = XMLHelper.SendEmail(xmlEmailResult, (SessionHelper.COMPANY_ID == 1) ? 3 : (SessionHelper.COMPANY_ID == 2) ? 1012 : 1024, RoadFreightToDoor, file));
                                Task taskquote = new Task(() => emailStatus = XMLHelper.SendEmail(xmlEmailResult, (SessionHelper.COMPANY_ID == 1) ? 4 : (SessionHelper.COMPANY_ID == 2) ? 1011 : 1023, "",""));
                                task.Start();
                                taskquote.Start();
                            }
                            if (MethodName == "CourierExpressToDoor")
                            {
                                model.AirFreightToAirport = false;
                                model.AirFreightToDoor = false;
                                model.Courier = false;
                                model.SeaFreight = false;
                                model.RoadFreightToDoor = false;
                                model.CourierExpressToDoor = true;
                                message = _dbRepositoryBaggageQuote.Update(model);
                                SP_GetBaggageXmlData_Result xmlResult = CustomRepository.GetBaggageXmlData(quoteNo);
                                xmlResult.IxType = "DTDE";
                                var CustomerRefNo = _dbRepositoryQuoteAmount.GetEntities().Where(m => m.QuoteId == quoteNo && m.MoveType == "EXB" && m.ShippingType == "CourierExpressToDoor" && m.Company == SessionHelper.COMPANY_ID).FirstOrDefault();
                                string file = string.Concat(CustomerRefNo.CustomerReferenceNo + "/" + CustomerRefNo.CustomerQuoteNo + "." + CustomerRefNo.QuoteSeqNo);
                                string CourierExpress = await XMLHelper.GenerateBaggageXml(xmlResult, cartonObjList, model.Price.Value, xmlColDelResult, file, "CourierExpress");
                                //Task C = new Task(() => XMLHelper.XmlAPICall(CourierExpress, 0));
                                //C.Start();
                                sp_GetdataForEmailSending_Result xmlEmailResult = CustomRepository.GetQuoteData(quoteNo, 2, "CourierExpress");
                                xmlEmailResult.quoteName = (SessionHelper.COMPANY_ID == 1) ? "baggage" : (SessionHelper.COMPANY_ID == 2 ? "shipping" : "sales");
                                xmlEmailResult.ReferenceNo = file;
                                Task task = new Task(() => emailStatus = XMLHelper.SendEmail(xmlEmailResult, (SessionHelper.COMPANY_ID == 1) ? 3 : (SessionHelper.COMPANY_ID == 2) ? 1012 : 1024, CourierExpress, file));
                                Task taskquote = new Task(() => emailStatus = XMLHelper.SendEmail(xmlEmailResult, (SessionHelper.COMPANY_ID == 1) ? 4 : (SessionHelper.COMPANY_ID == 2) ? 1011 : 1023, "",""));
                                task.Start();
                                taskquote.Start();
                            }
                        }
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
            catch (Exception ex)
            {
                logger.Error(ex);
                TempData[CustomEnums.NotifyType.Error.GetDescription()] = CommonHelper.GetErrorMessage(ex);
                return ex.Message;
            }
        }
        SP_GetXmlData_Result xmlResult;
        private BaggageCalculationLineModel GetBaggageCalcLine(string methodName, decimal amount, string desc)
        {
            try
            {
                string DeliveryMethodKey = methodName.Replace(" ", "");
                var timeLine = _dbRepositoryTransitionTimeLine.GetEntities().Where(m => m.DeliveryMethod == DeliveryMethodKey).FirstOrDefault();
                methodName = methodName == "Courier" ? "Courier Economy To Door" : (methodName == "Sea Freight" ? "Sea Freight To Door" : methodName);
                return new BaggageCalculationLineModel()
                {
                    DeliveryMethodName = methodName,
                    Amount = amount,
                    TransitionTime = timeLine == null ? "" : timeLine.TransitionTime,
                    CalcDescription = desc,
                    isSelected = false
                };
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                TempData[CustomEnums.NotifyType.Error.GetDescription()] = CommonHelper.GetErrorMessage(ex);
            }

            return new BaggageCalculationLineModel();
        }

        private string GetCountryName(string CountryCode)
        {
            string CountryName = string.Empty;
            try
            {
                if (CountryCode == "UK")
                {
                    CountryName = "UNITED KINGDOM";
                }
                else
                {
                    var Country = _dbRepositoryCountry.GetEntities().Where(m => m.country_code == CountryCode && m.CompanyId == SessionHelper.COMPANY_ID).FirstOrDefault();
                    CountryName = Country.country;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                TempData[CustomEnums.NotifyType.Error.GetDescription()] = CommonHelper.GetErrorMessage(ex);
            }
            return CountryName;
        }

        private List<BaggageCostModel> GetBaggageCostByQuoteId(int Id, string ShippingType)
        {
            List<BaggageCostModel> result = new List<BaggageCostModel>();
            try
            {
                quotesEntities entityObj = new quotesEntities();
                var QuoteId = new SqlParameter
                {
                    ParameterName = "QuoteId",
                    DbType = DbType.Int32,
                    Value = Id
                };
                var CostShippingType = new SqlParameter
                {
                    ParameterName = "ShippingType",
                    DbType = DbType.String,
                    Value = ShippingType
                };
                result = entityObj.Database.SqlQuery<BaggageCostModel>("SP_GetBaggageCostByQuoteId @QuoteId, @ShippingType", QuoteId, CostShippingType).ToList();
            }
            catch (Exception ex)
            {

            }
            return result;
        }
        #endregion
    }
}