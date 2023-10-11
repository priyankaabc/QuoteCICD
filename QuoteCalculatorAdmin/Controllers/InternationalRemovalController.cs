using Kendo.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using QuoteCalculator.Service.Repository.InternationalRemovalRepository;
using QuoteCalculatorAdmin.Common;
using QuoteCalculatorAdmin.Data;
using QuoteCalculatorAdmin.Data.Repository;
using QuoteCalculatorAdmin.Helper;
using QuoteCalculator.Service.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using QuoteCalculator.Service.Repository.QuickQuoteItemsRepository;
using QuoteCalculator.Service.Repository.EmailRepository;
using QuoteCalculator.Service.Repository.CommonRepository;

namespace QuoteCalculatorAdmin.Controllers
{
    public class InternationalRemovalController : BaseController
    {
        #region private variables
        private readonly GenericRepository<tbl_AdditionalQuickQuoteItems> _dbRepositoryBaggageItems;
        private readonly GenericRepository<tbl_DaySchedule> _dbRepositoryDaySchedule;
        public readonly GenericRepository<tbl_CountryCode> _dbRepositoryCountryCode;
        private readonly GenericRepository<tbl_Title> _dbRepositorytitle;
        private readonly GenericRepository<rates_destinations> _dbRepositoryCountry;
        public readonly GenericRepository<tbl_EmailTemplate> _dbRepositoryEmailTemplate;
        public readonly GenericRepository<tbl_QuoteAmount> _dbRepositoryQuoteAmount;
        public readonly GenericRepository<tbl_GuideLink> _dbRepositoryGuideLink;
        private readonly IInternationalRemovalRepository _dbInternationalRepository;
        private readonly IQuickQuoteItemsRepository _dbQuickQuoteItemsRepository;
        private readonly IEmailTemplateRepository _dbEmailTemplateRepository;
        private readonly ICommonRepository _commonRepository;

        #endregion

        #region Constructor
        public InternationalRemovalController(IInternationalRemovalRepository dbInternationalRepository,
            IQuickQuoteItemsRepository dbQuickQuoteItemsRepository,
            IEmailTemplateRepository dbEmailTemplateRepository,
            ICommonRepository commonRepository)
        {
            _dbRepositoryBaggageItems = new GenericRepository<tbl_AdditionalQuickQuoteItems>();
            _dbRepositoryDaySchedule = new GenericRepository<tbl_DaySchedule>();
            _dbRepositoryCountryCode = new GenericRepository<tbl_CountryCode>();
            _dbRepositorytitle = new GenericRepository<tbl_Title>();
            _dbRepositoryCountry = new GenericRepository<rates_destinations>();
            _dbRepositoryEmailTemplate = new GenericRepository<tbl_EmailTemplate>();
            _dbRepositoryQuoteAmount = new GenericRepository<tbl_QuoteAmount>();
            _dbRepositoryGuideLink = new GenericRepository<tbl_GuideLink>();
            _dbInternationalRepository = dbInternationalRepository;
            _dbQuickQuoteItemsRepository = dbQuickQuoteItemsRepository;
            _dbEmailTemplateRepository = dbEmailTemplateRepository;
            _commonRepository = commonRepository;
        }
        #endregion

        #region Method
        public ActionResult Index()
        {
            return View();
        }

        //public ActionResult KendoRead([DataSourceRequest] DataSourceRequest request)
        public ActionResult GetInternationRemovalList(DatatableModel model)
        {
            try
            {
                DataTablePaginationModel DtSearchModel = CommonController.GetDataTablePaginationModel(model);
                List<InternationalRemovalModel> Result = _dbInternationalRepository.GetInternationRemovalList(DtSearchModel, SessionHelper.CompanyId);
                DatatableResponseModel<InternationalRemovalModel> response = new DatatableResponseModel<InternationalRemovalModel>()
                {
                    draw = model.draw,
                    recordsTotal = Result != null && Result.Count > 0 && Result[0].TotalCount > 0 ? Result[0].TotalCount ?? 0 : 0,
                    recordsFiltered = Result != null && Result.Count > 0 && Result[0].TotalFilteredCount > 0 ? Result[0].TotalFilteredCount ?? 0 : 0,
                    data = Result
                };
                return Json(response, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(CommonHelper.GetErrorMessage(ex));
            }
        }

        public ActionResult Edit(int Id)
        {
            InternationalRemovalModel model = new InternationalRemovalModel();
            try
            {

                CommonController CommonController = new CommonController(_commonRepository);
                IEnumerable<SelectListItem> CountryList = CommonController.GetCountryListCommon();
                ViewBag.CountryList = CountryList;
                ViewBag.TitleList = _commonRepository.GetList("Title", null); //CommonHelper.GetTitleList();
                ViewBag.CountryCodeList = CommonController.GetCountryCodeListCommon();
                ViewBag.SalesRepCodeList = CommonController.GetSalesRepCodeListCommon();
                ViewBag.DayScheduleList = _commonRepository.GetList("DaySchedule", null); //_dbRepositoryDaySchedule.GetEntities().ToList();

                model.FromCountryName = "United Kingdom";
                model.SelectionType = "Quick Online Quote";
                model.CountryCode = "+44";
                model.SalesRepCode = SessionHelper.SalesRepCode;
                if (Id > 0)
                {
                    model = _dbInternationalRepository.GetInternationalRemovalById(Id);
                
                }
                if (model != null)
                {
                    model.SelectionType = model.HomeConsultationOrService == true ? "Home Consultation" : model.HomeVideoSurvery == true ? "Home Video Survey" : "Quick Online Quote";

                    List<QuickQuoteItemsModel> Items = _dbQuickQuoteItemsRepository.GetQuickQuoteItems(0, SessionHelper.CompanyId); //.Where(m => m.company == SessionHelper.CompanyId).OrderBy(m => m.DisplayOrder).ToList();
                    List<QuickQuoteItemsModel> quoteItemList = new List<QuickQuoteItemsModel>();
                    if (Items != null)
                    {
                        for (int i = 0; i < Items.Count(); i++)
                        {
                            QuickQuoteItemsModel cr = new QuickQuoteItemsModel
                            {
                                ItemId = Items[i].ItemId,
                                Title = Items[i].Title,
                                Cuft = Items[i].Cuft,
                                DisplayOrder = Items[i].DisplayOrder
                            };
                            if (Items[i].image == null)
                            {
                                cr.image = Items[i].image;
                            }
                            else { cr.image = Items[i].image.Replace("images", "img"); }
                            cr.Ftcontainer = Items[i].Ftcontainer;
                            quoteItemList.Add(cr);
                        }
                    }
                    model.items = quoteItemList;
                }
            }
            catch (Exception ex)
            {
                TempData[CustomEnums.NotifyType.Error.GetDescription()] = ex.Message;
            }
            return View(model);
        }

        public ActionResult KendoItemRead([DataSourceRequest] DataSourceRequest request, int? InternationalRemovalId)
        {
            try
            {
                if (!request.Sorts.Any())
                {
                    request.Sorts.Add(new SortDescriptor("ItemId", ListSortDirection.Ascending));
                }
                return Json(_dbRepositoryBaggageItems.GetEntities().Where(m => m.InternationalRemovalId == InternationalRemovalId).ToDataSourceResult(request));
            }
            catch (Exception ex)
            {
                return Json(CommonHelper.GetErrorMessage(ex));
            }
        }

       
        [HttpPost]
        public ActionResult DeleteInternationalRemoval(int? Id)
        {
            try
            {
                int result = _dbInternationalRepository.DeleteInternationalRemoval(Id);
                if (result > 0)
                {
                    var data = new
                    {
                        result = true,
                        Message = "Delete Successfully!!!"
                    };
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var data = new
                    {
                        result = false,
                        Message = "Error while deleting record"
                    };
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(CommonHelper.GetErrorMessage(ex));
            }
        }

       
        [HttpPost]
        public async Task<ActionResult> AddEdit(InternationalRemovalModel model, bool? isInquiry)
        {
            try
            {
                model.HomeConsultationOrService = model.SelectionType == "Home Consultation";
                model.HomeVideoSurvery = model.SelectionType == "Home Video Survey";
                model.QuickOnlineQuote = model.SelectionType == "Quick Online Quote";


                model.CreatedDate = DateTime.Now;
                model.EstimatedMoveDate = Convert.ToDateTime(model.EstimatedMoveDate);
                model.Company = SessionHelper.CompanyId;
                model.IsInquiry = isInquiry;
                int result = _dbInternationalRepository.UpdateInternationalRemoval(model);
                var CompanyId = SessionHelper.CompanyId;
                int removalId = model.Id > 0 ? model.Id ?? 0 : result;


                if (model.SelectionType == "Quick Online Quote")
                {
                    model.CreatedDate = DateTime.Now;
                    model.EstimatedMoveDate = Convert.ToDateTime(model.EstimatedMoveDate);


                    AdditionalQuickQuoteItemsModel AdditionalQuickQuote = new AdditionalQuickQuoteItemsModel();
                    AdditionalQuickQuoteItemsModel additionalquoteObj = _dbQuickQuoteItemsRepository.GetAdditionalQuickQuoteItems().Where(m => m.InternationalRemovalId == result).ToList().FirstOrDefault();

                    if (additionalquoteObj == null)
                    {
                        var Items = _dbQuickQuoteItemsRepository.GetQuickQuoteItemsById(model.QuickQuoteItemId, CompanyId);
                        AdditionalQuickQuote.Beds = Items.Title;
                        AdditionalQuickQuote.InternationalRemovalId = result;
                        AdditionalQuickQuote.QuickQuoteItemId = model.QuickQuoteItemId ?? 0;
                        AdditionalQuickQuote.Cuft = Items.Cuft;
                        AdditionalQuickQuote.SpecialRequirements = model.SpecialRequirements;
                         _dbQuickQuoteItemsRepository.AddAdditionalQuickQuote(AdditionalQuickQuote, CompanyId);
                    }
                    else
                    {
                         _dbQuickQuoteItemsRepository.UpdateAdditionalQuickQuote(additionalquoteObj, CompanyId);
                    }
                    if (isInquiry == true)
                    {
                        return Json(new { data = result });
                    }
                    return RedirectToAction("QuickOnlineQuoteDetail", "InternationalRemoval", new { @Id = result });

                }
                else if (model.SelectionType == "Home Video Survey")
                {
                    long customerReferenceNo = CustomRepository.GetNextCustomerRefNo("EXR", model.Email, model.ToCountryCode, model.CityName);
                    var customerQuotes = _dbQuickQuoteItemsRepository.GetQuoteAmount().Where(x => x.CustomerReferenceNo == customerReferenceNo);
                    int customerQuoteNo = 1;

                    if (customerQuotes != null)
                    {
                        QuoteAmountModel lastCustQt = customerQuotes.OrderByDescending(x => x.CustomerQuoteNo).FirstOrDefault();
                        if (lastCustQt != null)
                            customerQuoteNo = lastCustQt.CustomerQuoteNo;
                        else
                            customerQuoteNo = 1;
                    }

                    QuoteAmountModel QuoteAmount = new QuoteAmountModel
                    {
                        QuoteId = Convert.ToInt32(result),
                        MoveType = "EXR",
                        QuoteAmount = 0,
                        ShippingType = "HomeVideoSurvey",
                        ShippingTypeDescription = "Free Home Video Survey",
                        TransitionTime = string.Empty,
                        CreatedOn = DateTime.Now,
                        CustomerReferenceNo = ((int)customerReferenceNo),
                        CustomerQuoteNo = customerQuoteNo,
                        QuoteSeqNo = 0,
                        Company = SessionHelper.CompanyId
                    };
                    _dbQuickQuoteItemsRepository.UpdateQuoteAmount(QuoteAmount);

                    SP_GetRemovalXmlData_Result xmlResult = CustomRepository.GetRemovalXmlData(result);
                    if (xmlResult != null)
                    {
                        string file = string.Concat(customerReferenceNo + "/" + customerQuoteNo);
                        await XMLHelper.GenerateHomeVideoSurveyXml(xmlResult, file, model.VideoSurveyAppointmentTime, SessionHelper.CompanyId);
                    }

                    return RedirectToAction("ThankYou", "InternationalRemoval", new { @survey = "VideoSurvey" });
                }
                else
                {
                    long customerReferenceNo = CustomRepository.GetNextCustomerRefNo("EXR", model.Email, model.ToCountryCode, model.CityName);
                    var customerQuotes = _dbQuickQuoteItemsRepository.GetQuoteAmount().Where(x => x.CustomerReferenceNo == customerReferenceNo);
                    int customerQuoteNo = 1;

                    if (customerQuotes != null)
                    {
                        QuoteAmountModel lastCustQt = customerQuotes.OrderByDescending(x => x.CustomerQuoteNo).FirstOrDefault();
                        if (lastCustQt != null)
                            customerQuoteNo = lastCustQt.CustomerQuoteNo;
                        else
                            customerQuoteNo = 1;
                    }

                    QuoteAmountModel QuoteAmount = new QuoteAmountModel
                    {
                        QuoteId = Convert.ToInt32(result),
                        MoveType = "EXR",
                        QuoteAmount = 0,
                        ShippingType = "HomeSurvey",
                        ShippingTypeDescription = "Free Home Survey",
                        TransitionTime = "",
                        CreatedOn = System.DateTime.Now,
                        CustomerReferenceNo = ((int)customerReferenceNo),
                        CustomerQuoteNo = customerQuoteNo,
                        QuoteSeqNo = 0,
                        Company = SessionHelper.CompanyId
                    };
                     _dbQuickQuoteItemsRepository.UpdateQuoteAmount(QuoteAmount);

                    SP_GetRemovalXmlData_Result xmlResult = CustomRepository.GetRemovalXmlData(result);
                    string file = string.Concat(customerReferenceNo + "/" + customerQuoteNo);
                     await XMLHelper.GenerateHomeSurveyXml(xmlResult, file, SessionHelper.CompanyId);
                    return RedirectToAction("ThankYou", "InternationalRemoval", new { @survey = "HomeSurvey" });
                }
            }
            catch (Exception ex)
            {
                TempData[CustomEnums.NotifyType.Error.GetDescription()] = ex.Message;
                return RedirectToAction("Index", "InternationalRemoval");
            }
        }

        public async Task<ActionResult> QuickOnlineQuoteDetail(int Id, bool IsQuoteIfo = false, bool IsInquiry = false)
        {
            AdditionalQuickQuoteItemsModel additionalquoteObj = new AdditionalQuickQuoteItemsModel();
            try
            {
                var CompanyId = SessionHelper.CompanyId;
                additionalquoteObj = _dbQuickQuoteItemsRepository.GetAdditionalQuickQuoteItems().Where(m => m.InternationalRemovalId == Id).ToList().LastOrDefault();
                if (additionalquoteObj != null)
                {
                    var SpecialRequirements = additionalquoteObj.SpecialRequirements != null ? additionalquoteObj.SpecialRequirements.Split('|') : null;//additionalquoteObj.SpecialRequirements.Split('|');
                    additionalquoteObj.strSpecialRequirements = SpecialRequirements;
                }
                var internationalRemovalObj = _dbInternationalRepository.GetAllInternationalRemoval().Where(m => m.Id == Id).FirstOrDefault();
                if (internationalRemovalObj != null)
                {
                    ViewBag.PostCode = internationalRemovalObj.PostCode;
                    ViewBag.FromCountry = internationalRemovalObj.FromCountryName;
                    ViewBag.ToCity = internationalRemovalObj.CityName;
                    ViewBag.ToCountry = GetCountryName(internationalRemovalObj.ToCountryCode);
                    ViewBag.Email = internationalRemovalObj.Email;

                    SessionHelper.ToCountryCode = internationalRemovalObj.ToCountryCode;
                    SessionHelper.QuoteType = "3";
                }

                EmailTemplateModel emailTemplateObj;
                int QuickQuoteItemId = Id;
                DataTablePaginationModel DtModel = new DataTablePaginationModel();
                int result = !IsQuoteIfo || IsInquiry ? _dbInternationalRepository.GetInternationalRemovalCalculation(QuickQuoteItemId) : 1;
                if (result > 0)
                {
                    var CustomerRefNo = _dbQuickQuoteItemsRepository.GetQuoteAmount().Where(m => m.QuoteId == Id && m.MoveType == "EXR").FirstOrDefault();
                    if (CustomerRefNo != null)
                    {
                        ViewBag.RemovalQuote = CustomerRefNo.QuoteAmount;
                        ViewBag.ReferenceNo = string.Concat(CustomerRefNo.CustomerReferenceNo + "/" + CustomerRefNo.CustomerQuoteNo);
                        ViewBag.IsBooked = CustomerRefNo.IsBooked;
                    }
                    emailTemplateObj = _dbEmailTemplateRepository.GetAllTemplates(DtModel).Where(m => m.ServiceId == 1005).FirstOrDefault();

                    if (!IsQuoteIfo && CustomerRefNo != null)
                    {
                        SP_GetRemovalXmlData_Result xmlResult = CustomRepository.GetRemovalXmlData(Id);
                        string file = string.Concat(CustomerRefNo.CustomerReferenceNo + "/" + CustomerRefNo.CustomerQuoteNo + "." + CustomerRefNo.QuoteSeqNo);
                        if (xmlResult != null)
                        {
                            string Removal = await XMLHelper.GenerateRemovalXml(xmlResult, "Quote", file, CompanyId);
                        }
                    }
                }
                else
                {
                    var CustomerRefNo = _dbQuickQuoteItemsRepository.GetQuoteAmount().Where(m => m.QuoteId == Id && m.MoveType == "EXR" && m.ShippingType == "ENQUIRY").FirstOrDefault();
                    if (CustomerRefNo != null)
                    {
                        ViewBag.RemovalQuote = CustomerRefNo.QuoteAmount;
                        ViewBag.ReferenceNo = string.Concat(CustomerRefNo.CustomerReferenceNo + "/" + CustomerRefNo.CustomerQuoteNo);
                        ViewBag.IsBooked = CustomerRefNo.IsBooked;
                    }
                    emailTemplateObj = _dbEmailTemplateRepository.GetAllTemplates(DtModel).Where(m => m.ServiceId == 1019).FirstOrDefault();

                    if (!IsQuoteIfo && CustomerRefNo != null)
                    {
                        SP_GetRemovalXmlData_Result xmlResult = CustomRepository.GetRemovalXmlData(Id);
                        string file = string.Concat(CustomerRefNo.CustomerReferenceNo + "/" + CustomerRefNo.CustomerQuoteNo);
                        if (xmlResult != null)
                        {
                            string Removal = await XMLHelper.GenerateRemovalEnquiryXml(xmlResult, "ENQUIRY", file, CompanyId);
                        }
                    }
                }

                InternationalRemovalModel model = _dbInternationalRepository.GetInternationalRemovalById(Id);

                if (!IsQuoteIfo)
                {
                    string bodyTemplate = string.Empty;
                    if (emailTemplateObj != null)
                    {
                        bodyTemplate = emailTemplateObj.HtmlContent;
                    }

                    bodyTemplate = bodyTemplate.Replace("#CustName#", model.Firstname);
                    var GuidLink = _dbQuickQuoteItemsRepository.GetGuideLink().Where(m => m.CountryCode == model.ToCountryCode && (m.CityName == model.CityName || m.CityName == null)).FirstOrDefault();
                    if (GuidLink != null)
                        bodyTemplate = bodyTemplate.Replace("#Guide#", "(excluding incidental costs noted in our exclusions and detailed in our helpful <b><a target='_blank' href='" + GuidLink.RemovalURL + "'>Guide</a></b>)");
                    else
                        bodyTemplate = bodyTemplate.Replace("#Guide#", "(excluding incidental costs noted in our exclusions)");

                    Task quoteTask = new Task(() => EmailHelper.SendAsyncEmail(CompanyId, model.Email, emailTemplateObj.Subject, bodyTemplate, "EmailRemovals", "DisplayRemoval", true));
                    quoteTask.Start();
                }
              RemovalQuoteCalculationModel RemovalCalculationData = _dbInternationalRepository.GetRemovalQuoteCalculationById(Id);
                ViewBag.Vehicle = RemovalCalculationData.Vehicle;
                ViewBag.Labour = RemovalCalculationData.Labour;
                ViewBag.PackingMaterials = RemovalCalculationData.PackingMaterials;
                ViewBag.SeaFreight = RemovalCalculationData.SeaFreight;
                ViewBag.ReceivingHandling = RemovalCalculationData.ReceivingHandling;
                ViewBag.DestinationCharges = RemovalCalculationData.DestinationCharges;
                ViewBag.OriginCost = RemovalCalculationData.OriginCost;
                ViewBag.OriginMarkup = RemovalCalculationData.OriginMarkup;
                ViewBag.Total = RemovalCalculationData.Total;

                if (IsInquiry)
                {
                    return PartialView("_ShowRemovalQuoteDetails", additionalquoteObj);
                }
                return View(additionalquoteObj);
            }
            catch (Exception ex)
            {
                TempData[CustomEnums.NotifyType.Error.GetDescription()] = CommonHelper.GetErrorMessage(ex);
            }
            return View(additionalquoteObj);
        }

        //[HttpPost]
        //public async Task<string> RemovalQuoteBook(int quoteId, string type)
        //{
        //    string message = string.Empty;
        //    try
        //    {
        //        try
        //        {
        //            var CompanyId = SessionHelper.CompanyId;
        //            string emailStatus = string.Empty;
        //            var additionalquoteObj = _dbRepositoryAdditionalQuoteItems.GetEntities().Where(m => m.InternationalRemovalId == quoteId).ToList().LastOrDefault();
        //            var internationalRemovalObj = _dbRepositoryInternationalRemoval.GetEntities().Where(m => m.Id == quoteId).FirstOrDefault();
        //            SP_GetRemovalXmlData_Result xmlResult = CustomRepository.GetRemovalXmlData(quoteId);
        //            tbl_QuoteAmount QuoteAmount = _dbRepositoryQuoteAmount.GetEntities().Where(m => m.QuoteId == quoteId && m.MoveType == "EXR" && m.ShippingType == "Courier").FirstOrDefault();
        //            string file = string.Concat(QuoteAmount.CustomerReferenceNo + "/" + QuoteAmount.CustomerQuoteNo + "." + QuoteAmount.QuoteSeqNo);
        //            string Removal = await XMLHelper.GenerateRemovalXml(xmlResult, "Book Now", file, CompanyId);
        //            //Task taskR = new Task(() => XMLHelper.XmlAPICall(Removal, 0));
        //            //taskR.Start();

        //            QuoteAmount.IsBooked = true;
        //            message = _dbRepositoryQuoteAmount.Update(QuoteAmount);

        //            sp_GetdataForEmailSending_Result xmlEmailResult = CustomRepository.GetQuoteData(quoteId, 3, "Courier");
        //            xmlEmailResult.quoteName = "removals";
        //            Task task = new Task(() => emailStatus = XMLHelper.SendEmail(CompanyId, xmlEmailResult, 5, Removal, file));
        //            Task taskquote = new Task(() => emailStatus = XMLHelper.SendEmail(CompanyId, xmlEmailResult, 6, "", ""));
        //            task.Start();
        //            taskquote.Start();
        //            return message;
        //        }
        //        catch (Exception e)
        //        {
        //            TempData[CustomEnums.NotifyType.Error.GetDescription()] = CommonHelper.GetErrorMessage(e);
        //            return e.Message;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData[CustomEnums.NotifyType.Error.GetDescription()] = CommonHelper.GetErrorMessage(ex);
        //        return message;
        //    }
        //}

        [HttpPost]
        public async Task<string> RemovalQuoteBook(int quoteId, string type)
        {
            string message = string.Empty;
            try
            {
                QuoteAmountModel QuoteAmount = new QuoteAmountModel();
                try
                {
                    var CompanyId = SessionHelper.CompanyId;
                    string emailStatus = string.Empty;
                    var additionalquoteObj = _dbQuickQuoteItemsRepository.GetAdditionalQuickQuoteItems().Where(m => m.InternationalRemovalId == quoteId).ToList().LastOrDefault();
                    var internationalRemovalObj = _dbInternationalRepository.GetAllInternationalRemoval().Where(m => m.Id == quoteId).FirstOrDefault();
                    SP_GetRemovalXmlData_Result xmlResult = CustomRepository.GetRemovalXmlData(quoteId);
                    QuoteAmount = _dbQuickQuoteItemsRepository.GetQuoteAmountByQuoteId(quoteId, "EXR", "Courier");
                   
                    QuoteAmount.IsBooked = true;
                    message = _dbQuickQuoteItemsRepository.UpdateQuoteAmount(QuoteAmount).ToString();
                    if (xmlResult != null)
                    {
                        string file = string.Concat(QuoteAmount.CustomerReferenceNo + "/" + QuoteAmount.CustomerQuoteNo + "." + QuoteAmount.QuoteSeqNo);
                        string Removal = await XMLHelper.GenerateRemovalXml(xmlResult, "Book Now", file, CompanyId);

                        sp_GetdataForEmailSending_Result xmlEmailResult = CustomRepository.GetQuoteData(quoteId, 3, "Courier");
                        xmlEmailResult.quoteName = "removals";
                        Task task = new Task(() => emailStatus = XMLHelper.SendEmail(CompanyId, xmlEmailResult, 5, Removal, file));
                        Task taskquote = new Task(() => emailStatus = XMLHelper.SendEmail(CompanyId, xmlEmailResult, 6, "", ""));

                        task.Start();
                        taskquote.Start();
                    }
                    return message;
                }
                catch (Exception e)
                {
                    TempData[CustomEnums.NotifyType.Error.GetDescription()] = CommonHelper.GetErrorMessage(e);
                    return e.Message;
                }
            }
            catch (Exception ex)
            {
                TempData[CustomEnums.NotifyType.Error.GetDescription()] = CommonHelper.GetErrorMessage(ex);
                return message;
            }
        }

        public ActionResult ThankYou(string survey)
        {
            try
            {
                if (survey == "HomeSurvey")
                {
                    ViewBag.IsQuickQuote = false;
                    ViewBag.Survey = "Free home survey";
                    ViewBag.ThankYouMessage = "Thank you for your enquiry. We will contact you to arrange a suitable date and time for a free home survey and quotation without obligation.";
                }
                else if (survey == "VideoSurvey")
                {
                    ViewBag.IsQuickQuote = false;
                    ViewBag.Survey = "Free video survey";
                    ViewBag.ThankYouMessage = "Thank you for your enquiry. We will contact you to arrange a suitable date and time for a free video survey and quotation without obligation.";
                }
                else
                {
                    ViewBag.IsQuickQuote = true;
                }
            }
            catch (Exception ex)
            {
                TempData[CustomEnums.NotifyType.Error.GetDescription()] = CommonHelper.GetErrorMessage(ex);
            }
            return View();
        }

        public ActionResult GetTitleList()
        {
            using (var context = BaseContext.GetDbContext())
            {
                var TitleList = _dbRepositorytitle.GetEntities().OrderBy(x => x.DisplayOrder).ToList();
                return Json(TitleList, JsonRequestBehavior.AllowGet);
            }
        }

      
        public ActionResult GetDayScheduleList()
        {
            using (var context = BaseContext.GetDbContext())
            {
                try
                {
                    var DayScheduleList = _dbRepositoryDaySchedule.GetEntities().ToList();
                    return Json(DayScheduleList, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    return Json(CommonHelper.GetErrorMessage(ex));
                }
            }
        }

        private string GetCountryName(string CountryCode)
        {
            string CountryName = string.Empty;
            try
            {
                var Country = _dbRepositoryCountry.GetEntities().Where(m => m.country_code == CountryCode).FirstOrDefault();
                if (Country != null)
                {
                    CountryName = Country.country;
                }
            }
            catch (Exception ex)
            {
                TempData[CustomEnums.NotifyType.Error.GetDescription()] = CommonHelper.GetErrorMessage(ex);
            }
            return CountryName;
        }

        public ActionResult ViewMyQuote(int? Id, string email)
        {
            QuoteInformationModel myquote = new QuoteInformationModel();

            try
            {
                var firstDate = DateTime.Today.AddMonths(-6);
                var removalList = _dbInternationalRepository.GetAllInternationalRemoval().Where(m => m.Email == email && m.IsDelete != true && m.CreatedDate >= firstDate && m.QuickOnlineQuote == true && m.Company == SessionHelper.CompanyId).OrderByDescending(m => m.CreatedDate).ToList();
                List<InternatioalRemovalQuoteInfoModel> removalQuoteList = new List<InternatioalRemovalQuoteInfoModel>();

                var CustomerRefNo = _dbQuickQuoteItemsRepository.GetQuoteAmount().ToList();

                for (int i = 0; i < removalList.Count(); i++)
                {
                    long id = Convert.ToInt64(removalList[i].Id);
                    var additionalquoteObj = _dbQuickQuoteItemsRepository.GetAdditionalQuickQuoteItems().Where(m => m.InternationalRemovalId == id).ToList().LastOrDefault();

                    InternatioalRemovalQuoteInfoModel removalObj = new InternatioalRemovalQuoteInfoModel();

                    removalObj.Id = removalList[i].Id ?? 0;
                    removalObj.PostCode = removalList[i].PostCode;
                    removalObj.FromCountryName = removalList[i].FromCountryName;
                    removalObj.CityName = removalList[i].CityName;
                    removalObj.ToCountryCode = removalList[i].ToCountryCode;
                    removalObj.CountryCode = removalList[i].CountryCode;
                    removalObj.CreatedDate = removalList[i].CreatedDate;
                    removalObj.Telephone = removalList[i].Telephone;
                    removalObj.TitleId = removalList[i].TitleId ?? 0;
                    removalObj.EstimatedMoveDate = removalList[i].EstimatedMoveDate;
                    removalObj.ToCountryName = GetCountryName(removalList[i].ToCountryCode);
                    RemovalQuoteCalculationModel RemovalCalculationData = new RemovalQuoteCalculationModel();
                    if (additionalquoteObj != null)
                    {
                        removalObj.Beds = additionalquoteObj.Beds;
                        removalObj.Cuft = additionalquoteObj.Cuft;
                        removalObj.Ftcontainer = additionalquoteObj.Ftcontainer;
                        removalObj.SpecialRequirements = additionalquoteObj.SpecialRequirements;

                        RemovalCalculationData = _dbInternationalRepository.GetRemovalQuoteCalculationById(removalList[i].Id);
                    }
                    ViewBag.FromCountry = string.IsNullOrEmpty(removalObj.FromCountryName) ? removalObj.PostCode : removalObj.FromCountryName;
                    ViewBag.ToCity = string.IsNullOrEmpty(removalObj.CityName) ? removalObj.ToCountryCode : removalObj.CityName;

                    removalObj.Vehicle = RemovalCalculationData != null ? RemovalCalculationData.Vehicle : 0;
                    removalObj.Labour = RemovalCalculationData != null ? RemovalCalculationData.Labour : 0;
                    removalObj.PackingMaterials = RemovalCalculationData != null ? RemovalCalculationData.PackingMaterials : 0;
                    removalObj.SeaFreight = RemovalCalculationData != null ? RemovalCalculationData.SeaFreight : 0;
                    removalObj.ReceivingHandling = RemovalCalculationData != null ? RemovalCalculationData.ReceivingHandling : 0;
                    removalObj.DestinationCharges = RemovalCalculationData != null ? RemovalCalculationData.DestinationCharges : 0;
                    removalObj.OriginCost = RemovalCalculationData != null ? RemovalCalculationData.OriginCost : 0;
                    removalObj.OriginMarkup = RemovalCalculationData != null ? RemovalCalculationData.OriginMarkup : 0;
                    removalObj.Total = RemovalCalculationData != null ? RemovalCalculationData.Total : 0;

                    var objCustomerRefNo = CustomerRefNo.Where(m => m.QuoteId == id && m.MoveType == "EXR").FirstOrDefault();
                    removalObj.ReferenceNo = objCustomerRefNo != null ? string.Concat(objCustomerRefNo.CustomerReferenceNo + "/" + objCustomerRefNo.CustomerQuoteNo) : "";
                    removalObj.IsBooked = objCustomerRefNo != null ? objCustomerRefNo.IsBooked : false;
                    removalQuoteList.Add(removalObj);
                }
                myquote.internationremovalQuoteInfo = removalQuoteList;
            }
            catch (Exception ex)
            {
                TempData[CustomEnums.NotifyType.Error.GetDescription()] = CommonHelper.GetErrorMessage(ex);

            }
            return PartialView("RemovalQuoteInfo", myquote);
        }

        [HttpPost]
        public ActionResult UpdateBookStatus(int quoteId)
        {
            QuoteAmountModel removalQuoteData = _dbQuickQuoteItemsRepository.GetQuoteAmount().Where(m => m.QuoteId == quoteId && m.MoveType == "EXR").FirstOrDefault();
            if (removalQuoteData != null)
            {
                removalQuoteData.IsBooked = false;
                _dbQuickQuoteItemsRepository.UpdateQuoteAmount(removalQuoteData);
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        private List<FilterDescriptor> RecurseFilterDescriptors(IEnumerable<IFilterDescriptor> requestFilters, List<FilterDescriptor> allFilterDescriptors)
        {
            if (requestFilters.Any())
            {
                foreach (var filterDescriptor in requestFilters)
                {
                    if (filterDescriptor is FilterDescriptor)
                    {
                        allFilterDescriptors.Add((FilterDescriptor)filterDescriptor);
                    }
                    else if (filterDescriptor is CompositeFilterDescriptor)
                    {
                        RecurseFilterDescriptors(((CompositeFilterDescriptor)filterDescriptor).FilterDescriptors, allFilterDescriptors);
                    }
                }
            }
            return allFilterDescriptors;
        }

        #endregion
    }
}