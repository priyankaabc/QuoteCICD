using NLog;
using QuoteCalculator.Common;
using QuoteCalculator.Data;
using QuoteCalculator.Data.Repository;
using QuoteCalculator.Helper;
using QuoteCalculator.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace QuoteCalculator.Controllers
{
    public class MyQuoteController : Controller
    {

        #region private variables
        private readonly GenericRepository<tbl_Vehicle> _dbRepositoryVehicle;
        private readonly GenericRepository<tbl_baggageQuote> _dbRepositoryBaggageQuote;
        private readonly GenericRepository<tbl_BaggageItem> _dbRepositoryMoveBaggage;
        private readonly GenericRepository<tbl_InternationalRemoval> _dbRepositoryInternationalRemoval;
        private readonly GenericRepository<tbl_AdditionalQuickQuoteItems> _dbRepositoryAdditionalQuoteItems;
        private readonly GenericRepository<tbl_AccessCode> _dbRepositoryAccessCode;
        public readonly GenericRepository<tbl_QuoteAmount> _dbRepositoryQuoteAmount;
        public readonly GenericRepository<tbl_EmailTemplate> _dbRepositoryEmailTemplate;

        quotesEntities quotesDBEntities = new quotesEntities();

        #endregion

        #region Constructor
        public MyQuoteController()
        {
            _dbRepositoryVehicle = new GenericRepository<tbl_Vehicle>();
            _dbRepositoryBaggageQuote = new GenericRepository<tbl_baggageQuote>();
            _dbRepositoryMoveBaggage = new GenericRepository<tbl_BaggageItem>();
            _dbRepositoryInternationalRemoval = new GenericRepository<tbl_InternationalRemoval>();
            _dbRepositoryAdditionalQuoteItems = new GenericRepository<tbl_AdditionalQuickQuoteItems>();
            _dbRepositoryAccessCode = new GenericRepository<tbl_AccessCode>();
            _dbRepositoryQuoteAmount = new GenericRepository<tbl_QuoteAmount>();
            _dbRepositoryEmailTemplate = new GenericRepository<tbl_EmailTemplate>();
        }
        #endregion

        private static NLog.Logger logger = LogManager.GetCurrentClassLogger();

        public ActionResult Index(int? baggageId)
        {
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
                if (baggageId != null)
                {
                    MyQuoteModel model = new MyQuoteModel();

                    var baggageQuote = _dbRepositoryBaggageQuote.GetEntities().Where(m => m.Id == baggageId).FirstOrDefault();
                    if (baggageQuote != null)
                    {
                        var myquoteObj = _dbRepositoryAccessCode.GetEntities().Where(m => m.Email == baggageQuote.Email).FirstOrDefault();
                        if (myquoteObj != null)
                        {
                            model.EmailAddres = myquoteObj.Email;
                            model.OTP = myquoteObj.AccessCode;
                            return View(model);
                        }
                        else
                        {
                            Random r = new Random();
                            string otp = r.Next(1000, 9999).ToString();
                            tbl_AccessCode accessCode = new tbl_AccessCode();
                            accessCode.Email = baggageQuote.Email;
                            accessCode.CreatedOn = DateTime.Now;
                            accessCode.AccessCode = otp;
                            _dbRepositoryAccessCode.Insert(accessCode);
                            model.EmailAddres = baggageQuote.Email;
                            model.OTP = otp;
                            return View(model);

                        }

                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                TempData[CustomEnums.NotifyType.Error.GetDescription()] = CommonHelper.GetErrorMessage(ex);
            }
            return View();
        }
        [HttpPost]
        public ActionResult Index(MyQuoteModel model)
        {
            try
            {
                model.OTP = model.OTP.Replace(" ", "");
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
                tbl_AccessCode accessCode = _dbRepositoryAccessCode.GetEntities().FirstOrDefault(x => x.Email.ToLower() == model.EmailAddres.ToLower() && x.Company == SessionHelper.COMPANY_ID);

                if ((accessCode != null && accessCode.Email.ToLower() == model.EmailAddres.ToLower() && accessCode.AccessCode == model.OTP) || (model.OTP == null))
                {
                    var firstDate = DateTime.Today.AddMonths(-6);
                    var vehicleList = _dbRepositoryVehicle.GetEntities().Where(m => m.Email.ToLower() == model.EmailAddres.ToLower() && m.IsDelete != true && m.CreatedDate >= firstDate && m.Company == SessionHelper.COMPANY_ID).OrderByDescending(m => m.CreatedDate).ToList();
                    List<VehicleQuoteInfoModel> vehicleQuoteList = new List<VehicleQuoteInfoModel>();
                    VehicleQuoteInfoModel vehicleObj = new VehicleQuoteInfoModel();
                    for (int i = 0; i < vehicleList.Count(); i++)
                    {
                        if (vehicleList[i].FCL > 0 || vehicleList[i].GPG > 0)
                        {
                            vehicleObj = new VehicleQuoteInfoModel();
                            vehicleObj.FromCountryName = vehicleList[i].FromCountryName;
                            vehicleObj.ToCountryName = vehicleList[i].ToCountryCode;
                            vehicleObj.CityName = vehicleList[i].CityName;
                            vehicleObj.Id = vehicleList[i].Id;
                            vehicleObj.PostCode = vehicleList[i].PostCode;
                            vehicleObj.VehicleMakeName = vehicleList[i].VehicleMakeName;
                            vehicleObj.VehicleModelName = vehicleList[i].VehicleModelName;
                            vehicleObj.CreatedDate = vehicleList[i].CreatedDate;
                            vehicleObj.IsFCL = vehicleList[i].IsFCL == true ? true : false;
                            vehicleObj.IsGRP = vehicleList[i].IsGRP == true ? true : false;
                            int quoteId = vehicleList[i].Id;
                            var tblQuoteAmounts = _dbRepositoryQuoteAmount.GetEntities().Where(x => x.QuoteId == quoteId && x.MoveType == "EXV" && (x.QuoteAmount ?? 0) > 0).ToList();

                            var gpgQuote = tblQuoteAmounts.FirstOrDefault(x => x.ShippingType == "GPG");
                            if (gpgQuote == null || (gpgQuote.QuoteAmount ?? 0) == 0)
                            {
                                //vehicleObj.IsGRP = false;
                                vehicleObj.GPG = 0;
                            }
                            else
                            {
                                vehicleObj.CustomerRefNo = string.Concat(gpgQuote.CustomerReferenceNo + "/" + gpgQuote.CustomerQuoteNo);
                                //vehicleObj.IsGRP = true;
                                vehicleObj.GPG = Convert.ToDouble(gpgQuote.QuoteAmount.Value);
                            }

                            var fclQuote = tblQuoteAmounts.FirstOrDefault(x => x.ShippingType == "FCL");
                            if (fclQuote == null || (fclQuote.QuoteAmount ?? 0) == 0)
                            {
                                //vehicleObj.IsFCL = false;
                                vehicleObj.FCL = 0;
                            }
                            else
                            {
                                vehicleObj.CustomerRefNo = string.Concat(fclQuote.CustomerReferenceNo + "/" + fclQuote.CustomerQuoteNo);
                                //vehicleObj.IsFCL = true;
                                vehicleObj.FCL = Convert.ToDouble(fclQuote.QuoteAmount.Value);
                            }
                            vehicleQuoteList.Add(vehicleObj);
                        }
                    }
                    QuoteInformationModel myquote = new QuoteInformationModel();
                    myquote.vehicleQuoteInfo = vehicleQuoteList;

                    var baggageList = _dbRepositoryBaggageQuote.GetEntities().Where(m => m.Email.ToLower() == model.EmailAddres.ToLower() && m.IsDelete != true && m.CreatedDate >= firstDate && m.Company == SessionHelper.COMPANY_ID).OrderByDescending(m => m.CreatedDate).ToList();
                    List<BaggageQuoteInfoModel> baggageQuoteList = new List<BaggageQuoteInfoModel>();


                    for (int i = 0; i < baggageList.Count(); i++)
                    {
                        if (baggageList[i].AirFreightToAirportFinal > 0 || baggageList[i].AirFreightToDoorFinal > 0 || baggageList[i].SeaFreightFinal > 0 || baggageList[i].CourierFinal > 0 || baggageList[i].RoadFreightToDoorFinal > 0 || baggageList[i].CourierExpressToDoorFinal > 0)
                        {
                            int volume = 0;
                            BaggageQuoteInfoModel baggageObj = new BaggageQuoteInfoModel();

                            baggageObj.Id = baggageList[i].Id;
                            baggageObj.PostCode = baggageList[i].PostCode;
                            baggageObj.FromCity = string.IsNullOrEmpty(baggageList[i].FromCity) ? baggageList[i].PostCode : baggageList[i].FromCity ;
                            baggageObj.FromCountry = baggageList[i].FromCountry;
                            baggageObj.ToPostCode = baggageList[i].ToPostCode;
                            baggageObj.ToCountry = baggageList[i].ToCountry;
                            baggageObj.CityName = string.IsNullOrEmpty(baggageList[i].CityName) ? baggageList[i].ToPostCode : baggageList[i].CityName;
                            baggageObj.CreatedDate = baggageList[i].CreatedDate;
                            baggageObj.AirFreightToAirport = baggageList[i].AirFreightToAirport == true ? true : false;
                            baggageObj.AirFreightToDoor = baggageList[i].AirFreightToDoor == true ? true : false;
                            baggageObj.Courier = baggageList[i].Courier == true ? true : false;
                            baggageObj.SeaFreight = baggageList[i].SeaFreight == true ? true : false;
                            baggageObj.RoadFreight = baggageList[i].RoadFreightToDoor == true ? true : false;
                            baggageObj.CourierExpress = baggageList[i].CourierExpressToDoor == true ? true : false;
                            baggageObj.isMethodSelected = false;
                            baggageObj.InternalNotes = string.IsNullOrEmpty(baggageList[i].InternalNotes) ? null : baggageList[i].InternalNotes;
                            if (baggageObj.AirFreightToAirport == true || baggageObj.AirFreightToDoor == true || baggageObj.Courier == true || baggageObj.SeaFreight == true || baggageObj.RoadFreight == true || baggageObj.CourierExpress == true)
                            {
                                baggageObj.isMethodSelected = true;
                            }

                            SP_GetCollectionDelivery_Result collectionDeliveryResult = CustomRepository.GetCollectionDeliveryData(baggageList[i].Id);
                            baggageObj.DeliveryCharge = Math.Round(Convert.ToDecimal(collectionDeliveryResult.DeliveryCharge), 2, MidpointRounding.ToEven);
                            baggageObj.CollectionCharge = Math.Round(Convert.ToDecimal(collectionDeliveryResult.CollectionCharge), 2, MidpointRounding.ToEven);

                            long id = Convert.ToInt64(baggageList[i].Id);
                            var cartonObjList = _dbRepositoryMoveBaggage.GetEntities().Where(m => m.QuoteId == id).ToList();

                            foreach (var cartonItem in cartonObjList)
                            {
                                string FullSizeCartonStr = "";
                                FullSizeCartonStr = Convert.ToString(cartonItem.Quantity + " X " + cartonItem.Description + " (" + cartonItem.Volume + " cubic feet" + (cartonItem.Quantity > 1 ? " each)" : ")"));
                                if (cartonItem.Length > 0 && cartonItem.Breadth > 0 && cartonItem.Height > 0)
                                {
                                    FullSizeCartonStr += Convert.ToString(", " + cartonItem.Length + " X " + cartonItem.Breadth + " X " + cartonItem.Height + " Cms");
                                }
                                if (cartonItem.UserVolume > 0)
                                {
                                    FullSizeCartonStr += Convert.ToString(", " + cartonItem.UserVolume + " Kgs");
                                }
                                baggageObj.CartonList += FullSizeCartonStr + System.Environment.NewLine;
                            }

                            if (cartonObjList != null && cartonObjList.Count > 0)
                                baggageObj.HasMainCartons = cartonObjList.Count(x => string.Compare(x.Type, "MAIN", true) == 0) > 0;

                            baggageObj.BaggagePriceList = new List<BaggageCalculationLineModel>();

                            quotesEntities entityObj = new quotesEntities();
                            var QuoteId = new SqlParameter
                            {
                                ParameterName = "QuoteId",
                                DbType = DbType.Int64,
                                Value = baggageList[i].Id
                            };

                            int quoteId = baggageList[i].Id;
                            var tblQuoteAmounts = _dbRepositoryQuoteAmount.GetEntities().Where(x => x.QuoteId == quoteId && x.MoveType == "EXB" && (x.QuoteAmount ?? 0) > 0).ToList();

                            if (tblQuoteAmounts.Count() == 0)
                                continue;
                            double cubicFeet = 0;
                            for (int j = 0; j < cartonObjList.Count(); j++)
                            {
                                volume += cartonObjList[j].UserVolume ?? 0;
                                cubicFeet += (cartonObjList[j].Volume * cartonObjList[j].Quantity);
                            }

                            foreach (var tblQuoteAmount in tblQuoteAmounts)
                            {
                                string Desc = string.Empty;
                                if (tblQuoteAmount.ShippingTypeDescription != "Sea Freight" && tblQuoteAmount.ShippingTypeDescription != "Road freight To Door")
                                {
                                    string deliveryType = tblQuoteAmount.ShippingTypeDescription == "Courier" ? "COURIER" : (tblQuoteAmount.ShippingTypeDescription == "Courier Express To Door" ? "COURIEREXPRESS" : "AIR");
                                    decimal VolumetricsWeight = CustomRepository.GetVolumetricsWeight(baggageList[i].Id, deliveryType);
                                    Desc = VolumetricsWeight.ToString() + " Vol kgs" + (volume > 0 ? ("/" + volume + " Kgs gross") : "");
                                }
                                else
                                {
                                    Desc = string.Concat(cubicFeet, " cubic feet");
                                }

                                baggageObj.BaggagePriceList.Add(new BaggageCalculationLineModel()
                                {
                                    DeliveryMethodName = tblQuoteAmount.ShippingTypeDescription == "Courier" ? "Courier Economy To Door" : (tblQuoteAmount.ShippingTypeDescription == "Sea Freight" ? "Sea Freight To Door" : tblQuoteAmount.ShippingTypeDescription),
                                    Amount = tblQuoteAmount.QuoteAmount ?? 0,
                                    TransitionTime = tblQuoteAmount.TransitionTime,
                                    CalcDescription = Desc
                                });
                                baggageObj.CustomerRefNo = string.Concat(tblQuoteAmount.CustomerReferenceNo + "/" + tblQuoteAmount.CustomerQuoteNo);
                                baggageObj.BaggagePriceList = baggageObj.BaggagePriceList.OrderBy(x => x.Amount).ToList();
                            }
                            baggageQuoteList.Add(baggageObj);
                        }
                    }
                    myquote.bagageQuoteInfo = baggageQuoteList;

                    var removalList = _dbRepositoryInternationalRemoval.GetEntities().Where(m => m.Email.ToLower() == model.EmailAddres.ToLower() && m.IsDelete != true && m.CreatedDate >= firstDate && m.Company == SessionHelper.COMPANY_ID).OrderByDescending(m => m.CreatedDate).ToList();
                    List<InternationalRemovalInfoModel> removalQuoteList = new List<InternationalRemovalInfoModel>();
                    InternationalRemovalInfoModel removalObj = new InternationalRemovalInfoModel();
                    for (int i = 0; i < removalList.Count(); i++)
                    {
                        int quoteId = removalList[i].Id;
                        var tblQuoteAmounts = _dbRepositoryQuoteAmount.GetEntities().Where(x => x.QuoteId == quoteId && x.MoveType == "EXR" && (x.QuoteAmount ?? 0) > 0).FirstOrDefault();
                        if (tblQuoteAmounts != null && tblQuoteAmounts.QuoteAmount > 0)
                        {
                            removalObj = new InternationalRemovalInfoModel();
                            removalObj.FromCountryName = removalList[i].FromCountryName;
                            removalObj.ToCountryName = removalList[i].ToCountryCode;
                            removalObj.CityName = removalList[i].CityName;
                            removalObj.Id = removalList[i].Id;
                            removalObj.PostCode = removalList[i].PostCode;
                            removalObj.CreatedDate = removalList[i].CreatedDate;


                            var additionalquoteObj = _dbRepositoryAdditionalQuoteItems.GetEntities().Where(m => m.InternationalRemovalId == quoteId).FirstOrDefault();
                            if (additionalquoteObj != null)
                            {
                                var strSpecialRequirements = additionalquoteObj.SpecialRequirements != null ? additionalquoteObj.SpecialRequirements.Split('|') : null;
                                removalObj.strSpecialRequirements = strSpecialRequirements;

                                removalObj.Description = Convert.ToString(additionalquoteObj.Beds != null ? additionalquoteObj.Beds + ", " : "" + additionalquoteObj.Cuft + " Cuft " + additionalquoteObj.Ftcontainer + " container");
                                removalObj.SpecialRequirements = additionalquoteObj.SpecialRequirements;
                            }

                            removalObj.CustomerRefNo = string.Concat(tblQuoteAmounts.CustomerReferenceNo + "/" + tblQuoteAmounts.CustomerQuoteNo);
                            removalObj.Amount = Convert.ToDecimal(tblQuoteAmounts.QuoteAmount.Value);
                            removalObj.Courier = tblQuoteAmounts.IsBooked == true ? true : false;
                            removalQuoteList.Add(removalObj);
                        }
                    }
                    myquote.removalQuoteInfo = removalQuoteList;

                    return PartialView("QuoteInfo", myquote);
                }
                else
                {
                    TempData[CustomEnums.NotifyType.Error.GetDescription()] = "Email and access code not match.";
                    return View();
                }
            }
            catch (Exception e)
            {
                logger.Error(e);
                TempData[CustomEnums.NotifyType.Error.GetDescription()] = CommonHelper.GetErrorMessage(e);
            }
            return View();
        }

        public JsonResult SendOTPEmail(string email)
        {
            try
            {


                //Generate new OTP
                Random r = new Random();
                string otp = r.Next(1000, 9999).ToString();
                tbl_AccessCode accessCodeEntry = _dbRepositoryAccessCode.GetEntities().FirstOrDefault(x => x.Email.ToLower() == email.ToLower() && x.Company == SessionHelper.COMPANY_ID);

                int companyId = SessionHelper.COMPANY_ID;
                QuoteType lastQuoteType = QuoteType.Baggage;
                //Send OTP Email
                //1. check if the email has any quotes using two quotes table... if no quote - validaiton error.
                var lastQuote = quotesDBEntities.vw_QuoteAmount.Where(x => x.Email == email && x.Company == companyId && x.ShippingType != "ENQUIRY").OrderByDescending(x => x.QuoteDate).FirstOrDefault();
                if (lastQuote == null)
                {
                    return Json(new { Message = "No quote available for this Email.", IsError = Convert.ToString((int)CustomEnums.NotifyType.Error) }, JsonRequestBehavior.AllowGet);
                }

                lastQuoteType = lastQuote.MoveType == "EXV" ? QuoteType.Vehicle : (lastQuote.MoveType == "EXB" ? QuoteType.Baggage : QuoteType.Removal);

                if (accessCodeEntry != null)
                {
                    accessCodeEntry.AccessCode = otp;
                    _dbRepositoryAccessCode.Update(accessCodeEntry);
                }
                else
                {
                    //2. If Existing - update OTP with above otherwise add new entry
                    accessCodeEntry = new tbl_AccessCode();
                    accessCodeEntry.Email = email;
                    accessCodeEntry.AccessCode = otp;
                    accessCodeEntry.CreatedOn = DateTime.Now;
                    accessCodeEntry.Company = companyId;
                    quotesDBEntities.tbl_AccessCode.Add(accessCodeEntry);
                }
                quotesDBEntities.SaveChanges();

                //3. TODO - Send email with OTP

                string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/";
                string toEmail = email;

                string bodyTemplate = null;
                string subject = null;

                if (companyId == 1)
                {
                    if (lastQuoteType == QuoteType.Vehicle)
                    {
                        var emailTemplateObj = _dbRepositoryEmailTemplate.GetEntities().Where(m => m.ServiceId == 1013).FirstOrDefault();
                        bodyTemplate = emailTemplateObj.HtmlContent;
                        subject = emailTemplateObj.Subject;
                    }
                    if (lastQuoteType == QuoteType.Baggage)
                    {
                        var emailTemplateObj = _dbRepositoryEmailTemplate.GetEntities().Where(m => m.ServiceId == 1014).FirstOrDefault();
                        bodyTemplate = emailTemplateObj.HtmlContent;
                        subject = emailTemplateObj.Subject;
                    }
                    if (lastQuoteType == QuoteType.Removal)
                    {
                        var emailTemplateObj = _dbRepositoryEmailTemplate.GetEntities().Where(m => m.ServiceId == 1016).FirstOrDefault();
                        bodyTemplate = emailTemplateObj.HtmlContent;
                        subject = emailTemplateObj.Subject;
                    }
                }
                else if (companyId == 2)
                {
                    var emailTemplateObj = _dbRepositoryEmailTemplate.GetEntities().Where(m => m.ServiceId == 1015).FirstOrDefault();
                    bodyTemplate = emailTemplateObj.HtmlContent;
                    subject = emailTemplateObj.Subject;
                }
                else if (companyId == 3)
                {
                    var emailTemplateObj = _dbRepositoryEmailTemplate.GetEntities().Where(m => m.ServiceId == 1025).FirstOrDefault();
                    bodyTemplate = emailTemplateObj.HtmlContent;
                    subject = emailTemplateObj.Subject;
                }
                else
                {
                    bodyTemplate = null;
                }

                bodyTemplate = bodyTemplate.Replace("#OTP#", otp);
                Task task = new Task(() => EmailHelper.SendAsyncEmail(toEmail, subject, bodyTemplate, "EmailFrom_" + companyId, "", true));
                task.Start();

                return Json(new { Message = "Email for access code has been sent. Please check your email.", IsSuccess = Convert.ToString((int)CustomEnums.NotifyType.Success) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                TempData[CustomEnums.NotifyType.Error.GetDescription()] = CommonHelper.GetErrorMessage(ex);
            }
            return Json(new { Message = "", IsSuccess = Convert.ToString((int)CustomEnums.NotifyType.Error) }, JsonRequestBehavior.AllowGet);

        }
    }
}