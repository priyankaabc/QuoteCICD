using QuoteCalculatorPickfords.Common;
using QuoteCalculatorPickfords.Data;
using QuoteCalculatorPickfords.Data.Repository;
using QuoteCalculatorPickfords.Helper;
using QuoteCalculatorPickfords.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace QuoteCalculatorPickfords.Controllers
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
        }
        #endregion


        public ActionResult Index(int? baggageId)
        {
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
            return View();
        }
        [HttpPost]
        public ActionResult Index(MyQuoteModel model)
        {
            tbl_AccessCode accessCode = _dbRepositoryAccessCode.GetEntities().FirstOrDefault(x => x.Email.ToLower() == model.EmailAddres.ToLower() && x.Company == SessionHelper.COMPANY_ID);

            if ((accessCode != null && accessCode.Email == model.EmailAddres && accessCode.AccessCode == model.OTP) || (model.OTP == null))
            {
                var firstDate = DateTime.Today.AddMonths(-6);
                //var vehicleList = _dbRepositoryVehicle.GetEntities().Where(m => m.Email == model.EmailAddres && m.CreatedDate >= firstDate).OrderByDescending(m=>m.CreatedDate).ToList();
                //List<VehicleQuoteInfoModel> vehicleQuoteList = new List<VehicleQuoteInfoModel>();
                //VehicleQuoteInfoModel vehicleObj = new VehicleQuoteInfoModel();
                //for (int i = 0; i < vehicleList.Count(); i++)
                //{
                //    if (vehicleList[i].FCL > 0 || vehicleList[i].GPG > 0)
                //    {
                //        vehicleObj = new VehicleQuoteInfoModel();
                //        vehicleObj.FromCountryName = vehicleList[i].FromCountryName;
                //        vehicleObj.ToCountryName = vehicleList[i].ToCountryCode;
                //        vehicleObj.CityName = vehicleList[i].CityName;
                //        vehicleObj.Id = vehicleList[i].Id;
                //        vehicleObj.PostCode = vehicleList[i].PostCode;
                //        vehicleObj.VehicleMakeName = vehicleList[i].VehicleMakeName;
                //        vehicleObj.VehicleModelName = vehicleList[i].VehicleModelName;
                //        vehicleObj.CreatedDate = vehicleList[i].CreatedDate;
                //        vehicleObj.IsFCL = vehicleList[i].IsFCL == true ? true : false;
                //        vehicleObj.IsGRP = vehicleList[i].IsGRP == true ? true : false;
                //        int quoteId = vehicleList[i].Id;
                //        var tblQuoteAmounts = _dbRepositoryQuoteAmount.GetEntities().Where(x => x.QuoteId == quoteId && x.MoveType == "EXV" && (x.QuoteAmount ?? 0) > 0).ToList();

                //        var gpgQuote = tblQuoteAmounts.FirstOrDefault(x => x.ShippingType == "GPG");
                //        if (gpgQuote == null || (gpgQuote.QuoteAmount ?? 0) == 0)
                //        {
                //            //vehicleObj.IsGRP = false;
                //            vehicleObj.GPG = 0;
                //        }
                //        else
                //        {
                //            vehicleObj.CustomerRefNo = string.Concat(gpgQuote.CustomerReferenceNo + "/" + gpgQuote.CustomerQuoteNo);
                //            //vehicleObj.IsGRP = true;
                //            vehicleObj.GPG = Convert.ToDouble(gpgQuote.QuoteAmount.Value);
                //        }

                //        var fclQuote = tblQuoteAmounts.FirstOrDefault(x => x.ShippingType == "FCL");
                //        if (fclQuote == null || (fclQuote.QuoteAmount ?? 0) == 0)
                //        {
                //            //vehicleObj.IsFCL = false;
                //            vehicleObj.FCL = 0;
                //        }
                //        else
                //        {
                //            vehicleObj.CustomerRefNo = string.Concat(fclQuote.CustomerReferenceNo + "/" + fclQuote.CustomerQuoteNo);
                //            //vehicleObj.IsFCL = true;
                //            vehicleObj.FCL = Convert.ToDouble(fclQuote.QuoteAmount.Value);
                //        }
                //        vehicleQuoteList.Add(vehicleObj);
                //    }
                //}
                QuoteInformationModel myquote = new QuoteInformationModel();
                //myquote.vehicleQuoteInfo = vehicleQuoteList;

                var baggageList = _dbRepositoryBaggageQuote.GetEntities().Where(m => m.Email == model.EmailAddres && m.IsDelete != true && m.CreatedDate >= firstDate && m.Company == SessionHelper.COMPANY_ID).OrderByDescending(m => m.CreatedDate).ToList();
                List<BaggageQuoteInfoModel> baggageQuoteList = new List<BaggageQuoteInfoModel>();
                

                for (int i = 0; i < baggageList.Count(); i++)
                {
                    if (baggageList[i].AirFreightToAirportFinal > 0 || baggageList[i].AirFreightToDoorFinal > 0 || baggageList[i].SeaFreightFinal > 0 || baggageList[i].CourierFinal > 0)
                    {
                        BaggageQuoteInfoModel baggageObj = new BaggageQuoteInfoModel();

                        baggageObj.Id = baggageList[i].Id;
                        baggageObj.PostCode = baggageList[i].PostCode;
                        baggageObj.FromCity = baggageList[i].FromCity;
                        baggageObj.FromCountry = baggageList[i].FromCountry;
                        baggageObj.ToPostCode = baggageList[i].ToPostCode;
                        baggageObj.ToCountry = baggageList[i].ToCountry;
                        baggageObj.CityName = baggageList[i].CityName;
                        baggageObj.CreatedDate = baggageList[i].CreatedDate;
                        baggageObj.AirFreightToAirport = baggageList[i].AirFreightToAirport == true ? true : false;
                        baggageObj.AirFreightToDoor = baggageList[i].AirFreightToDoor == true ? true : false;
                        baggageObj.Courier = baggageList[i].Courier == true ? true : false;
                        baggageObj.SeaFreight = baggageList[i].SeaFreight == true ? true : false;
                        baggageObj.isMethodSelected = false;
                        if (baggageObj.AirFreightToAirport == true || baggageObj.AirFreightToDoor == true || baggageObj.Courier == true || baggageObj.SeaFreight == true)
                        {
                            baggageObj.isMethodSelected = true;
                        }

                        ViewBag.FromCity = string.IsNullOrEmpty(baggageObj.FromCity) ? baggageObj.PostCode : baggageObj.FromCity;
                        ViewBag.ToCity = string.IsNullOrEmpty(baggageObj.CityName) ? baggageObj.ToPostCode : baggageObj.CityName;

                        SP_GetCollectionDelivery_Result collectionDeliveryResult = CustomRepository.GetCollectionDeliveryData(baggageList[i].Id);
                        ViewBag.DeliveryCharge = Math.Round(Convert.ToDecimal(collectionDeliveryResult.DeliveryCharge), 2, MidpointRounding.ToEven);
                        ViewBag.CollectionCharge = Math.Round(Convert.ToDecimal(collectionDeliveryResult.CollectionCharge), 2, MidpointRounding.ToEven);

                        long id = Convert.ToInt64(baggageList[i].Id);
                        var cartonObjList = _dbRepositoryMoveBaggage.GetEntities().Where(m => m.QuoteId == id).ToList();

                        baggageObj.CartonList = string.Join(", ", cartonObjList.Select(x => (x.Quantity + " " + x.Description)).ToList());

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
                        int volume = 0;
                        int cubicFeet = 0;
                        for (int j = 0; j < cartonObjList.Count(); j++)
                        {
                            volume += cartonObjList[j].UserVolume ?? 0;
                            cubicFeet += (cartonObjList[j].Volume * cartonObjList[j].Quantity);
                        }

                        foreach (var tblQuoteAmount in tblQuoteAmounts)
                        {
                            string Desc = string.Empty;
                            if (tblQuoteAmount.ShippingTypeDescription != "Sea Freight")
                            {
                                string deliveryType = tblQuoteAmount.ShippingTypeDescription == "Courier" ? "COURIER" : "AIR";
                                decimal VolumetricsWeight = CustomRepository.GetVolumetricsWeight(baggageList[i].Id, deliveryType);
                                Desc = VolumetricsWeight.ToString() + " Vol kgs" + (volume > 0 ? ("/" + volume + " Kgs gross") : "");
                            }
                            else
                            {
                                Desc = string.Concat(cubicFeet, " cubic feet");
                            }

                            baggageObj.BaggagePriceList.Add(new BaggageCalculationLineModel()
                            {
                                DeliveryMethodName = tblQuoteAmount.ShippingTypeDescription,
                                Amount = tblQuoteAmount.QuoteAmount ?? 0,
                                TransitionTime = tblQuoteAmount.TransitionTime,
                                CalcDescription = Desc
                            });
                            baggageObj.CustomerRefNo = string.Concat(tblQuoteAmount.CustomerReferenceNo + "/" + tblQuoteAmount.CustomerQuoteNo);
                        }
                        baggageQuoteList.Add(baggageObj);
                    }
                }
                myquote.bagageQuoteInfo = baggageQuoteList;

                //var removalList = _dbRepositoryInternationalRemoval.GetEntities().Where(m => m.Email == model.EmailAddres && m.CreatedDate >= firstDate).OrderByDescending(m => m.CreatedDate).ToList();
                //List<InternationalRemovalInfoModel> removalQuoteList = new List<InternationalRemovalInfoModel>();
                //InternationalRemovalInfoModel removalObj = new InternationalRemovalInfoModel();
                //for (int i = 0; i < removalList.Count(); i++)
                //{
                //    int quoteId = removalList[i].Id;
                //    var tblQuoteAmounts = _dbRepositoryQuoteAmount.GetEntities().Where(x => x.QuoteId == quoteId && x.MoveType == "EXR" && (x.QuoteAmount ?? 0) > 0).FirstOrDefault();
                //    if (tblQuoteAmounts != null && tblQuoteAmounts.QuoteAmount > 0)
                //    {
                //        removalObj = new InternationalRemovalInfoModel();
                //        removalObj.FromCountryName = removalList[i].FromCountryName;
                //        removalObj.ToCountryName = removalList[i].ToCountryCode;
                //        removalObj.CityName = removalList[i].CityName;
                //        removalObj.Id = removalList[i].Id;
                //        removalObj.PostCode = removalList[i].PostCode;
                //        removalObj.CreatedDate = removalList[i].CreatedDate;


                //        var additionalquoteObj = _dbRepositoryAdditionalQuoteItems.GetEntities().Where(m => m.InternationalRemovalId == quoteId).FirstOrDefault();
                //        if (additionalquoteObj != null)
                //        {
                //            removalObj.Description = Convert.ToString(additionalquoteObj.Beds + ", " + additionalquoteObj.Cuft + " Cuft " + additionalquoteObj.Ftcontainer + "container");
                //            removalObj.SpecialRequirements = additionalquoteObj.SpecialRequirements;
                //        }

                //        removalObj.CustomerRefNo = string.Concat(tblQuoteAmounts.CustomerReferenceNo + "/" + tblQuoteAmounts.CustomerQuoteNo);
                //        removalObj.Amount = Convert.ToDecimal(tblQuoteAmounts.QuoteAmount.Value);
                //        removalObj.Courier = tblQuoteAmounts.IsBooked == true ? true : false;
                //        removalQuoteList.Add(removalObj);
                //    }
                //}
                //myquote.removalQuoteInfo = removalQuoteList;

                return PartialView("QuoteInfo", myquote);
            }
            else
            {
                TempData[CustomEnums.NotifyType.Error.GetDescription()] = "Email and access code not match.";
                return View();
            }
        }

        public JsonResult SendOTPEmail(string email)
        {
            //Generate new OTP
            Random r = new Random();
            string otp = r.Next(1000, 9999).ToString();
            tbl_AccessCode accessCodeEntry = _dbRepositoryAccessCode.GetEntities().FirstOrDefault(x => x.Email.ToLower() == email.ToLower() && x.Company == SessionHelper.COMPANY_ID);

            if (accessCodeEntry != null)
            {
                accessCodeEntry.AccessCode = otp;
                _dbRepositoryAccessCode.Update(accessCodeEntry);
            }
            else
            {
                //Send OTP Email
                //1. check if the email has any quotes using two quotes table... if no quote - validaiton error.
                //int vehicleList = _dbRepositoryVehicle.GetEntities().Where(m => m.Email == email).Count();
                int baggageList = _dbRepositoryBaggageQuote.GetEntities().Where(m => m.Email == email && m.IsDelete != true && m.Company == SessionHelper.COMPANY_ID).Count();
                //int removalList = _dbRepositoryInternationalRemoval.GetEntities().Where(m => m.Email == email).Count();

                //if (vehicleList == 0 && baggageList == 0 && removalList == 0)
                if (baggageList == 0)
                {
                    return Json(new { Message = "No quote available for this Email.", IsError = Convert.ToString((int)CustomEnums.NotifyType.Error) }, JsonRequestBehavior.AllowGet);
                }

                //2. If Existing - update OTP with above otherwise add new entry
                accessCodeEntry = new tbl_AccessCode();
                accessCodeEntry.Email = email;
                accessCodeEntry.AccessCode = otp;
                accessCodeEntry.CreatedOn = DateTime.Now;
                accessCodeEntry.Company = SessionHelper.COMPANY_ID;
                quotesDBEntities.tbl_AccessCode.Add(accessCodeEntry);
            }
            quotesDBEntities.SaveChanges();

            //3. TODO - Send email with OTP

            string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/";
            string toEmail = email;
            string bodyTemplate = System.IO.File.ReadAllText(Server.MapPath("~/Template/AccessCodeOTP.html"));
            bodyTemplate = bodyTemplate.Replace("[@OTP]", otp);
            bodyTemplate = bodyTemplate.Replace("[@href]", baseUrl + "MyQuote");

            Task task = new Task(() => EmailHelper.SendAsyncEmail(toEmail, "Pickfords Access Code", bodyTemplate, "EmailFrom", "", true));
            task.Start();

            return Json(new { Message = "Email for access code has been sent. Please check your email.", IsSuccess = Convert.ToString((int)CustomEnums.NotifyType.Success) }, JsonRequestBehavior.AllowGet);
        }
    }
}