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
        }
        #endregion

        #region Method
        // GET: Baggage
        [HttpGet]
        public ActionResult Index(string countryCode)
        {
            tbl_baggageQuote model = new tbl_baggageQuote();
            //ViewBag.SelectedCountryList = _dbRepositoryCountry.GetEntities().Where(m => m.country_code == "UK").Select(m => new { m.country_code, m.country }).OrderBy(m => new { m.country }).ToList();            
            var fromCountryList = _dbRepositoryCountry.GetEntities().Where(m => m.country_code != " " && m.CompanyId == SessionHelper.COMPANY_ID).Select(m => new { m.country_code, m.country }).Distinct().OrderBy(m => m.country).ToList();
            fromCountryList.Insert(0, new { country_code = "UK", country = "UNITED KINGDOM" });
            ViewBag.FromCountryList = fromCountryList;
            var toCountryList = _dbRepositoryCountry.GetEntities().Where(m => m.bag_dest == 1 && m.CompanyId == SessionHelper.COMPANY_ID).Select(m => new { m.country_code, m.country }).Distinct().OrderBy(m => m.country).ToList();
            toCountryList.Insert(0, new { country_code = "US", country = "USA" });
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
            toCountryList.Insert(0, new { country_code = "UK", country = "UNITED KINGDOM" });
            ViewBag.CountryList = toCountryList;
            ViewBag.TitleList = _dbRepositorytitle.GetEntities().ToList();
            ViewBag.CountryId = countryCode;
            return View(model);
        }
        [HttpPost]
        public ActionResult Index(tbl_baggageQuote model)
        {
            SessionHelper.ToCountryCode = model.ToCountry;
            SessionHelper.QuoteType = "2";
            //ViewBag.SelectedCountryList = _dbRepositoryCountry.GetEntities().Where(m => m.country == "UK").Select(m => new { m.country_code, m.country }).OrderBy(m => new { m.country }).ToList();
            ViewBag.CountryList = _dbRepositoryCountry.GetEntities().Where(m => m.bag_dest == 1 && m.CompanyId == SessionHelper.COMPANY_ID).Select(m => new { m.country_code, m.country }).OrderBy(m => new { m.country }).Distinct().ToList();
            ViewBag.TitleList = _dbRepositorytitle.GetEntities().ToList();

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            string message = string.Empty;
            model.EstimatedMoveDate = Convert.ToDateTime(model.EstimatedMoveDate);
            model.Company = SessionHelper.COMPANY_ID;
            //model.FromCountry = "United Kingdom";

            SP_GetXmlData_Result xmlResult;
            try
            {
                model.CreatedDate = DateTime.Now;
                model.NextExecutionDate = DateTime.Today.AddDays(7);

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
                message = _dbRepositoryBaggageQuote.Insert(model);
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
                        Task task = new Task(() => EmailHelper.SendAsyncEmail(null, "Quote guard - warning", bodyTemplate, "EmailBaggage", "DisplayBaggage", true));
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
                                    return RedirectToAction("MoveDetails", "Baggage");
                                }
                            }
                        }
                        else
                            return RedirectToAction("Success", "Vehicle");
                    }

                    //XMLHelper.GenerateBaggageXml(xmlResult);
                    return RedirectToAction("MoveDetails", "Baggage");
                }

            }
            catch (Exception ex)
            {
                logger.Error(ex);
                message = CommonHelper.GetErrorMessage(ex);
                TempData[CustomEnums.NotifyType.Error.GetDescription()] = message;
            }
            return RedirectToAction("MoveDetails", "Baggage");
        }
        [HttpGet]
        public ActionResult MoveDetails()
        {
            BaggageModel obj = new BaggageModel();
            var cartonList = _dbRepositorycartons.GetEntities().OrderBy(m => m.displayorder).ToList();
            List<CartonsModel> cartonModelList = new List<CartonsModel>();
            List<Movebaggage> moveBaggaheList = new List<Movebaggage>();

            for (int i = 0; i < cartonList.Count(); i++)
            {
                CartonsModel cr = new CartonsModel();
                Movebaggage mb = new Movebaggage();
                mb.Volume = cartonList[i].volume;

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
                cartonModelList.Add(cr);
                moveBaggaheList.Add(mb);

            }

            obj.cartonList = cartonModelList;
            obj.moveList = moveBaggaheList;
            // obj.cartonList = result;
            return View(obj);
        }

        [HttpPost]
        public ActionResult MoveDetails(BaggageModel obj)
        {
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
                    string allstock = item.dimension;
                    if (allstock != null)
                    {
                        if (item.quantity > 0)
                        {
                            string arrayStocks = allstock.ToLower();
                            string[] arrayStock = arrayStocks.Split('x');
                            moveBaggage.Length = Convert.ToInt32(arrayStock[0]);
                            moveBaggage.Breadth = Convert.ToInt32(arrayStock[1]);
                            moveBaggage.Height = Convert.ToInt32(arrayStock[2]);
                            moveBaggage.Description = item.description;
                            moveBaggage.Volume = item.Volume;
                            moveBaggage.Quantity = item.quantity;
                            moveBaggage.UserVolume = item.UserVolume;
                            moveBaggage.Type = "ADDITIONAL";
                            moveBaggage.QuoteId = SessionHelper.QuoteId;
                            moveBaggage.Company = SessionHelper.COMPANY_ID;
                            _dbRepositoryMoveBaggage.Insert(moveBaggage);
                        }
                    }
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
            return RedirectToAction("Baggage", "Baggage", new { @quoteId = CommonHelper.Encode(SessionHelper.QuoteId.ToString()) });
        }

        [HttpGet]
        public ActionResult Baggage(string quoteId, bool? isMyQuote)
        {
            int volume = 0;
            int cubicFeet = 0;
            var Id = Convert.ToInt32(CommonHelper.Decode(quoteId));
            List<tbl_BaggageItem> cartonObjList = _dbRepositoryMoveBaggage.GetEntities().Where(m => m.QuoteId == Id && m.Company == SessionHelper.COMPANY_ID).ToList();
            BaggageModel obj = new BaggageModel();

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
            tbl_baggageQuote model = _dbRepositoryBaggageQuote.GetEntities().Where(m => m.Id == Id && m.Company == SessionHelper.COMPANY_ID).FirstOrDefault();
            if (isMyQuote != true)
            {
                //tbl_baggageQuote model = _dbRepositoryBaggageQuote.SelectById(Id);
                var emailTemplateObj = _dbRepositoryEmailTemplate.GetEntities().Where(m => m.ServiceId == 1009).FirstOrDefault();
                string bodyTemplate = string.Empty;
                if (emailTemplateObj != null)
                {
                    bodyTemplate = emailTemplateObj.HtmlContent;
                }
                bodyTemplate = bodyTemplate.Replace("#CustName#", model.Firstname);
                Task quoteTask = new Task(() => EmailHelper.SendAsyncEmail(model.Email, emailTemplateObj.Subject, bodyTemplate, "EmailPickfords", "DisplayPickfords", true));
                quoteTask.Start();
            }
            quotesEntities entityObj = new quotesEntities();
            BaggageCalculationModel baggegeObj = null;
            if (isMyQuote == true)
            {
                SessionHelper.ToCountryCode = model.ToCountry;
                SessionHelper.QuoteType = "2";
                baggegeObj = new BaggageCalculationModel();
                baggegeObj.AirFreightToAirport = Convert.ToDecimal(model.AirFreightToAirportFinal);
                baggegeObj.AirFreightToDoor = Convert.ToDecimal(model.AirFreightToDoorFinal);
                baggegeObj.SeaFreight = Convert.ToDecimal(model.SeaFreightFinal);
                baggegeObj.Courier = Convert.ToDecimal(model.CourierFinal);
            }
            else
            {
                baggegeObj = new BaggageCalculationModel();
                baggegeObj = entityObj.Database.SqlQuery<BaggageCalculationModel>("SP_BaggegeCalculation @QuoteId", QuoteId).FirstOrDefault();
            }
            obj.calculationLines = new List<BaggageCalculationLineModel>();
            SP_GetBaggageXmlData_Result xmlResult = CustomRepository.GetBaggageXmlData(Id);
            SP_GetCollectionDelivery_Result xmlColDelResult = CustomRepository.GetCollectionDeliveryData(Id);
            List<string> xmlFiles = new List<string>();

            if (baggegeObj.AirFreightToAirport > 0)
            {
                xmlResult.IxType = "ToPort";
                decimal VolumetricsWeight = CustomRepository.GetVolumetricsWeight(Id, "AIR");
                string Desc = VolumetricsWeight.ToString() + " Vol kgs" + (volume > 0 ? ("/" + volume + " Kgs gross") : "");
                obj.calculationLines.Add(GetBaggageCalcLine("Air Freight To Airport", baggegeObj.AirFreightToAirport.Value, Desc));
                var CustomerRefNo = _dbRepositoryQuoteAmount.GetEntities().Where(m => m.QuoteId == Id && m.MoveType == "EXB" && m.ShippingType == "AirfreightToAirport" && m.Company == SessionHelper.COMPANY_ID).FirstOrDefault();
                string file = string.Concat(CustomerRefNo.CustomerReferenceNo + "/" + CustomerRefNo.CustomerQuoteNo + "." + CustomerRefNo.QuoteSeqNo);
                if (isMyQuote != true)
                {
                    string AirFreightToAirport = XMLHelper.GenerateBaggageXml(xmlResult, cartonObjList, Convert.ToDouble(baggegeObj.AirFreightToAirport.Value), xmlColDelResult, file, "AIR"); //TODO            
                    xmlFiles.Add(AirFreightToAirport);
                }
                ViewBag.ReferenceNo = string.Concat(CustomerRefNo.CustomerReferenceNo + "/" + CustomerRefNo.CustomerQuoteNo);
            }
            if (baggegeObj.AirFreightToDoor > 0)
            {
                xmlResult.IxType = "ToDoor";
                decimal VolumetricsWeight = CustomRepository.GetVolumetricsWeight(Id, "AIR");
                string Desc = VolumetricsWeight.ToString() + " Vol kgs" + (volume > 0 ? ("/" + volume + " Kgs gross") : "");
                obj.calculationLines.Add(GetBaggageCalcLine("Air Freight To Door", baggegeObj.AirFreightToDoor.Value, Desc));
                var CustomerRefNo = _dbRepositoryQuoteAmount.GetEntities().Where(m => m.QuoteId == Id && m.MoveType == "EXB" && m.ShippingType == "AirfreightToDoor" && m.Company == SessionHelper.COMPANY_ID).FirstOrDefault();
                string file = string.Concat(CustomerRefNo.CustomerReferenceNo + "/" + CustomerRefNo.CustomerQuoteNo + "." + CustomerRefNo.QuoteSeqNo);
                if (isMyQuote != true)
                {
                    string AirFreightToDoor = XMLHelper.GenerateBaggageXml(xmlResult, cartonObjList, Convert.ToDouble(baggegeObj.AirFreightToDoor.Value), xmlColDelResult, file, "AIR"); //TODO            
                    xmlFiles.Add(AirFreightToDoor);
                }
                ViewBag.ReferenceNo = string.Concat(CustomerRefNo.CustomerReferenceNo + "/" + CustomerRefNo.CustomerQuoteNo);
            }
            if (baggegeObj.Courier > 0)
            {
                xmlResult.IxType = "ToDoor";
                decimal VolumetricsWeight = CustomRepository.GetVolumetricsWeight(Id, "COURIER");
                string Desc = VolumetricsWeight.ToString() + " Vol kgs" + (volume > 0 ? ("/" + volume + " Kgs gross") : "");
                obj.calculationLines.Add(GetBaggageCalcLine("Courier", baggegeObj.Courier.Value, Desc));
                var CustomerRefNo = _dbRepositoryQuoteAmount.GetEntities().Where(m => m.QuoteId == Id && m.MoveType == "EXB" && m.ShippingType == "Courier" && m.Company == SessionHelper.COMPANY_ID).FirstOrDefault();
                string file = string.Concat(CustomerRefNo.CustomerReferenceNo + "/" + CustomerRefNo.CustomerQuoteNo + "." + CustomerRefNo.QuoteSeqNo);
                if (isMyQuote != true)
                {
                    string Courier = XMLHelper.GenerateBaggageXml(xmlResult, cartonObjList, Convert.ToDouble(baggegeObj.Courier.Value), xmlColDelResult, file, "Courier"); //TODO            
                    xmlFiles.Add(Courier);
                }
                ViewBag.ReferenceNo = string.Concat(CustomerRefNo.CustomerReferenceNo + "/" + CustomerRefNo.CustomerQuoteNo);
            }
            if (baggegeObj.SeaFreight > 0)
            {
                xmlResult.IxType = "ToDoor";
                string Desc = string.Concat(cubicFeet, " cubic feet");
                obj.calculationLines.Add(GetBaggageCalcLine("Sea Freight", baggegeObj.SeaFreight.Value, Desc));
                var CustomerRefNo = _dbRepositoryQuoteAmount.GetEntities().Where(m => m.QuoteId == Id && m.MoveType == "EXB" && m.ShippingType == "SeaFreight" && m.Company == SessionHelper.COMPANY_ID).FirstOrDefault();
                string file = string.Concat(CustomerRefNo.CustomerReferenceNo + "/" + CustomerRefNo.CustomerQuoteNo + "." + CustomerRefNo.QuoteSeqNo);
                if (isMyQuote != true)
                {
                    string SeaFreight = XMLHelper.GenerateBaggageXml(xmlResult, cartonObjList, Convert.ToDouble(baggegeObj.SeaFreight.Value), xmlColDelResult, file, "Sea"); //TODO            
                    xmlFiles.Add(SeaFreight);
                }
                ViewBag.ReferenceNo = string.Concat(CustomerRefNo.CustomerReferenceNo + "/" + CustomerRefNo.CustomerQuoteNo);
            }

            if(xmlFiles.Count > 0)
            {
                Task task = new Task(() => SendXmlFies(xmlFiles));
                task.Start();
            }

            if (baggegeObj.AirFreightToAirport == 0 && baggegeObj.AirFreightToDoor == 0 && baggegeObj.Courier == 0 && baggegeObj.SeaFreight == 0)
            {
                var CustomerRefNo = _dbRepositoryQuoteAmount.GetEntities().Where(m => m.QuoteId == Id && m.MoveType == "EXB" && m.ShippingType == "ENQUIRY" && m.Company == SessionHelper.COMPANY_ID).FirstOrDefault();
                string file = string.Concat(CustomerRefNo.CustomerReferenceNo + "/" + CustomerRefNo.CustomerQuoteNo + "." + CustomerRefNo.QuoteSeqNo);
                if (isMyQuote != true)
                {
                    string Enquiry = XMLHelper.GenerateBaggageEnquiryXml(xmlResult, cartonObjList, file); //TODO            
                    Task task = new Task(() => XMLHelper.XmlAPICall(Enquiry, 0));
                    task.Start();
                }
                ViewBag.ReferenceNo = string.Concat(CustomerRefNo.CustomerReferenceNo + "/" + CustomerRefNo.CustomerQuoteNo);
            }

            var baggageObj = _dbRepositoryBaggageQuote.GetEntities().Where(m => m.Id == Id).FirstOrDefault();
            if (baggageObj != null)
            {
                ViewBag.isMethodSelected = false;
                for (int i = 0; i < obj.calculationLines.Count; i++)
                {
                    if (obj.calculationLines[i].DeliveryMethodName == "Air Freight To Airport")
                    {
                        if (baggageObj.AirFreightToAirport == true)
                        {
                            ViewBag.isMethodSelected = true;
                            obj.calculationLines[i].isSelected = true;
                        }
                    }
                    if (obj.calculationLines[i].DeliveryMethodName == "Air Freight To Door")
                    {
                        if (baggageObj.AirFreightToDoor == true)
                        {
                            ViewBag.isMethodSelected = true;
                            obj.calculationLines[i].isSelected = true;
                        }
                    }
                    if (obj.calculationLines[i].DeliveryMethodName == "Courier")
                    {
                        if (baggageObj.Courier == true)
                        {
                            ViewBag.isMethodSelected = true;
                            obj.calculationLines[i].isSelected = true;
                        }
                    }
                    if (obj.calculationLines[i].DeliveryMethodName == "Sea Freight")
                    {
                        if (baggageObj.SeaFreight == true)
                        {
                            ViewBag.isMethodSelected = true;
                            obj.calculationLines[i].isSelected = true;
                        }
                    }
                }
            }

            obj.calculationLines = obj.calculationLines.OrderBy(x => x.Amount).ToList();
            //if (baggegeObj.SeaFreight <= 0 && baggegeObj.Courier <= 0 && baggegeObj.AirFreightToDoor <= 0 && baggegeObj.AirFreightToAirport <= 0)
            //{
            //    if (isMyQuote != true)
            //    {
            //        XMLHelper.GenerateBaggageXml(xmlResult, cartonObjList, 0, xmlColDelResult);
            //    }
            //}
            var shipping = _dbRepositoryBaggageQuote.GetEntities().Where(m => m.Id == Id && m.Company == SessionHelper.COMPANY_ID).FirstOrDefault();
            ViewBag.FromCountry = GetCountryName(shipping.FromCountry);
            ViewBag.FromCity = string.IsNullOrEmpty(shipping.FromCity) ? shipping.PostCode : shipping.FromCity;
            ViewBag.ToCountry = GetCountryName(shipping.ToCountry);
            ViewBag.ToCity = string.IsNullOrEmpty(shipping.CityName) ? shipping.ToPostCode : shipping.CityName;

            ViewBag.DeliveryCharge = xmlColDelResult.DeliveryCharge;
            ViewBag.CollectionCharge = xmlColDelResult.CollectionCharge;
            //Check condition in db
            var checkReq = _dbRepositoryBaggageQuote.GetEntities().Where(m => m.Telephone == shipping.Telephone && m.Company == SessionHelper.COMPANY_ID).ToList();
            if (checkReq.Count == 1)
            {
                string DomainName = Request.Url.Host + (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port);
                string myQuoteUrl = DomainName + "/MyQuote";
                string SMSText = "Thank you for your online quote request ref " + shipping.Id + ". We have emailed your quote but you can read " +
                    "it here '" + myQuoteUrl + "' or call us on 0800 783 5322 to discuss. Pickfords Shipping.";
                var mno = shipping.CountryCode + shipping.Telephone;
                SMSHelper.SendSMS(mno, SMSText);
                var newObj = new tbl_SMSLog();
                newObj.QuoteId = shipping.Id.ToString();
                newObj.Direction = "OUT_GOING";
                newObj.SMSText = SMSText;
                newObj.CratedDate = DateTime.Now;
                _dbRepositorySMSLog.Insert(newObj);
            }
            return View(obj);
        }

        private void SendXmlFies(List<string> xmlFiles)
        {
            foreach (string xmlFile in xmlFiles)
            {
                XMLHelper.XmlAPICall(xmlFile, 0);
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
            CityListModel model = new CityListModel();
            var vehicleCityList = obj.Database.SqlQuery<CityListModel>("GetCityForBaggae @CountryCode", countryCodeParameter).ToList();
            return Json(vehicleCityList, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public string BaggageQuoteCal(string colName, string quotePrice, int quoteNo = 0)
        {
            if (quoteNo == 0)
            {
                quoteNo = SessionHelper.QuoteId;
            }
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
                            message = _dbRepositoryBaggageQuote.Update(model);
                            SP_GetBaggageXmlData_Result xmlResult = CustomRepository.GetBaggageXmlData(quoteNo);
                            var CustomerRefNo = _dbRepositoryQuoteAmount.GetEntities().Where(m => m.QuoteId == quoteNo && m.MoveType == "EXB" && m.ShippingType == "AirFreightToAirport" && m.Company == SessionHelper.COMPANY_ID).FirstOrDefault();
                            string file = string.Concat(CustomerRefNo.CustomerReferenceNo + "/" + CustomerRefNo.CustomerQuoteNo + "." + CustomerRefNo.QuoteSeqNo);
                            string AirFreightToAirport = XMLHelper.GenerateBaggageXml(xmlResult, cartonObjList, model.Price.Value, xmlColDelResult, file, "AirFreightToAirport");
                            Task AF = new Task(() => XMLHelper.XmlAPICall(AirFreightToAirport, 0));
                            AF.Start();
                            sp_GetdataForEmailSending_Result xmlEmailResult = CustomRepository.GetQuoteData(quoteNo, 2, "AirfreighToAirport");
                            xmlEmailResult.quoteName = "baggage";
                            xmlEmailResult.ReferenceNo = file;
                            Task task = new Task(() => emailStatus = XMLHelper.SendEmail(xmlEmailResult, 3));
                            Task taskquote = new Task(() => emailStatus = XMLHelper.SendEmail(xmlEmailResult, 4));
                            task.Start();
                            taskquote.Start();
                        }
                        if (MethodName == "AirFreightToDoor" || MethodName == "AirfreightToDoor")
                        {
                            model.AirFreightToAirport = false;
                            model.AirFreightToDoor = true;
                            model.Courier = false;
                            model.SeaFreight = false;
                            message = _dbRepositoryBaggageQuote.Update(model);
                            SP_GetBaggageXmlData_Result xmlResult = CustomRepository.GetBaggageXmlData(quoteNo);
                            var CustomerRefNo = _dbRepositoryQuoteAmount.GetEntities().Where(m => m.QuoteId == quoteNo && m.MoveType == "EXB" && m.ShippingType == "AirFreightToDoor" && m.Company == SessionHelper.COMPANY_ID).FirstOrDefault();
                            string file = string.Concat(CustomerRefNo.CustomerReferenceNo + "/" + CustomerRefNo.CustomerQuoteNo + "." + CustomerRefNo.QuoteSeqNo);
                            string AirFreightToDoor = XMLHelper.GenerateBaggageXml(xmlResult, cartonObjList, model.Price.Value, xmlColDelResult, file, "AIR");
                            Task AF = new Task(() => XMLHelper.XmlAPICall(AirFreightToDoor, 0));
                            AF.Start();
                            sp_GetdataForEmailSending_Result xmlEmailResult = CustomRepository.GetQuoteData(quoteNo, 2, "AirfreightToDoor");
                            xmlEmailResult.quoteName = "baggage";
                            xmlEmailResult.ReferenceNo = file;
                            Task task = new Task(() => emailStatus = XMLHelper.SendEmail(xmlEmailResult, 3));
                            Task taskquote = new Task(() => emailStatus = XMLHelper.SendEmail(xmlEmailResult, 4));
                            task.Start();
                            taskquote.Start();
                        }
                        if (MethodName == "Courier")
                        {
                            model.AirFreightToAirport = false;
                            model.AirFreightToDoor = false;
                            model.Courier = true;
                            model.SeaFreight = false;
                            message = _dbRepositoryBaggageQuote.Update(model);
                            SP_GetBaggageXmlData_Result xmlResult = CustomRepository.GetBaggageXmlData(quoteNo);
                            var CustomerRefNo = _dbRepositoryQuoteAmount.GetEntities().Where(m => m.QuoteId == quoteNo && m.MoveType == "EXB" && m.ShippingType == "Courier" && m.Company == SessionHelper.COMPANY_ID).FirstOrDefault();
                            string file = string.Concat(CustomerRefNo.CustomerReferenceNo + "/" + CustomerRefNo.CustomerQuoteNo + "." + CustomerRefNo.QuoteSeqNo);
                            string Courier = XMLHelper.GenerateBaggageXml(xmlResult, cartonObjList, model.Price.Value, xmlColDelResult, file, "Courier");
                            Task C = new Task(() => XMLHelper.XmlAPICall(Courier, 0));
                            C.Start();
                            sp_GetdataForEmailSending_Result xmlEmailResult = CustomRepository.GetQuoteData(quoteNo, 2, "Courier");
                            xmlEmailResult.quoteName = "baggage";
                            xmlEmailResult.ReferenceNo = file;
                            Task task = new Task(() => emailStatus = XMLHelper.SendEmail(xmlEmailResult, 3));
                            Task taskquote = new Task(() => emailStatus = XMLHelper.SendEmail(xmlEmailResult, 4));
                            task.Start();
                            taskquote.Start();
                        }
                        if (MethodName == "SeaFreight")
                        {
                            model.AirFreightToAirport = false;
                            model.AirFreightToDoor = false;
                            model.Courier = false;
                            model.SeaFreight = true;
                            message = _dbRepositoryBaggageQuote.Update(model);
                            SP_GetBaggageXmlData_Result xmlResult = CustomRepository.GetBaggageXmlData(quoteNo);
                            var CustomerRefNo = _dbRepositoryQuoteAmount.GetEntities().Where(m => m.QuoteId == quoteNo && m.MoveType == "EXB" && m.ShippingType == "SeaFreight" && m.Company == SessionHelper.COMPANY_ID).FirstOrDefault();
                            string file = string.Concat(CustomerRefNo.CustomerReferenceNo + "/" + CustomerRefNo.CustomerQuoteNo + "." + CustomerRefNo.QuoteSeqNo);
                            string SeaFreight = XMLHelper.GenerateBaggageXml(xmlResult, cartonObjList, model.Price.Value, xmlColDelResult, file, "Sea");
                            Task SF = new Task(() => XMLHelper.XmlAPICall(SeaFreight, 0));
                            SF.Start();
                            sp_GetdataForEmailSending_Result xmlEmailResult = CustomRepository.GetQuoteData(quoteNo, 2, "SeaFreight");
                            xmlEmailResult.quoteName = "baggage";
                            xmlEmailResult.ReferenceNo = file;
                            Task task = new Task(() => emailStatus = XMLHelper.SendEmail(xmlEmailResult, 3));
                            Task taskquote = new Task(() => emailStatus = XMLHelper.SendEmail(xmlEmailResult, 4));
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
                return e.Message;
            }
        }
        SP_GetXmlData_Result xmlResult;
        private BaggageCalculationLineModel GetBaggageCalcLine(string methodName, decimal amount, string desc)
        {
            string DeliveryMethodKey = methodName.Replace(" ", "");
            var timeLine = _dbRepositoryTransitionTimeLine.GetEntities().Where(m => m.DeliveryMethod == DeliveryMethodKey).FirstOrDefault();
            return new BaggageCalculationLineModel()
            {
                DeliveryMethodName = methodName,
                Amount = amount,
                TransitionTime = timeLine == null ? "" : timeLine.TransitionTime,
                CalcDescription = desc,
                isSelected = false
            };
        }

        private string GetCountryName(string CountryCode)
        {
            string CountryName = string.Empty;
            if (CountryCode == "UK")
            {
                CountryName = "UNITED KINGDOM";
            }
            else
            {
                var Country = _dbRepositoryCountry.GetEntities().Where(m => m.country_code == CountryCode && m.CompanyId == SessionHelper.COMPANY_ID).FirstOrDefault();
                CountryName = Country == null ? "" : Country.country;
            }
            return CountryName;
        }

        #endregion
    }
}