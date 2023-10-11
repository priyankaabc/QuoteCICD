using Kendo.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using NLog;
using QuoteCalculator.Service.Models;
using QuoteCalculator.Service.Repository.BaggageRepository;
using QuoteCalculator.Service.Repository.CommonRepository;
using QuoteCalculatorAdmin.Common;
using QuoteCalculatorAdmin.Data;
using QuoteCalculatorAdmin.Data.Repository;
using QuoteCalculatorAdmin.Helper;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace QuoteCalculatorAdmin.Controllers
{
    public class BaggageController : BaseController
    {
        #region private variables

        private readonly GenericRepository<tbl_BaggageItem> _dbRepositoryBaggageItems;
        private readonly GenericRepository<cartons> _dbRepositorycartons;
        private readonly GenericRepository<tbl_BaggageItem> _dbRepositoryMoveBaggage;
        private readonly GenericRepository<tbl_baggageQuote> _dbRepositoryBaggageQuote;
        private readonly GenericRepository<tbl_SMSLog> _dbRepositorySMSLog;
        private readonly IBaggageRepository _baggageRepository;
        private readonly ICommonRepository _commonRepository;

        #endregion

        #region Constructor
        public BaggageController(IBaggageRepository baggageRepository, ICommonRepository commonRepository)
        {
            _dbRepositoryBaggageItems = new GenericRepository<tbl_BaggageItem>();
            _dbRepositorycartons = new GenericRepository<cartons>();
            _dbRepositoryMoveBaggage = new GenericRepository<tbl_BaggageItem>();
            _dbRepositoryBaggageQuote = new GenericRepository<tbl_baggageQuote>();            
            _dbRepositorySMSLog = new GenericRepository<tbl_SMSLog>();
            _baggageRepository = baggageRepository;
            _commonRepository = commonRepository;
        }
        #endregion

        #region Method
        public ActionResult Index()
        {
            try
            {
                //ViewBag.TitleList = CommonHelper.GetTitleList();
            }
            catch (Exception ex)
            {

                TempData[CustomEnums.NotifyType.Error.GetDescription()] = CommonHelper.GetErrorMessage(ex);
            }
            return View();
        }

        //public ActionResult KendoRead([DataSourceRequest] DataSourceRequest request)
        public ActionResult GetBaggageData(DatatableModel model)
        {
            try
            {

                DataTablePaginationModel DtSearchModel = CommonController.GetDataTablePaginationModel(model);

                List<BaggageListModel> dataList = _baggageRepository.GetAllBaggage(SessionHelper.CompanyId, DtSearchModel);

                DatatableResponseModel<BaggageListModel> response = new DatatableResponseModel<BaggageListModel>()
                {
                    draw = model.draw,
                    recordsTotal = dataList != null && dataList.Count > 0 && dataList[0].TotalCount > 0 ? dataList[0].TotalCount ?? 0 : 0,
                    recordsFiltered = dataList != null && dataList.Count > 0 && dataList[0].TotalFilteredCount > 0 ? dataList[0].TotalFilteredCount ?? 0 : 0,
                    data = dataList
                };


                return Json(response, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(CommonHelper.GetErrorMessage(ex));
            }
        }
        private List<FilterDescriptor> RecurseFilterDescriptors(IEnumerable<IFilterDescriptor> requestFilters, List<FilterDescriptor> allFilterDescriptors)
        {
            try
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

            }
            catch (Exception ex)
            {

                TempData[CustomEnums.NotifyType.Error.GetDescription()] = CommonHelper.GetErrorMessage(ex);
            }
            return allFilterDescriptors;
        }

        [HttpGet]
        public ActionResult AddEdit(int? Id)
        {
            BaggageModel obj = new BaggageModel();
            try
            {
                if (Id == null) { Id = 0; }

                //var cartonList = _dbRepositorycartons.GetEntities().Where(m => m.company == SessionHelper.CompanyId).OrderBy(m => m.displayorder).ToList();
                var cartonList = _baggageRepository.GetCartonListByCompanyId(SessionHelper.CompanyId);

                List<CartonsModel> cartonModelList = new List<CartonsModel>();
                List<Movebaggage> moveBaggaheList = new List<Movebaggage>();

                //var quoteObj = _dbRepositoryBaggageQuote.GetEntities().Where(m => m.Id == Id && m.Company == SessionHelper.CompanyId).FirstOrDefault();
                var quoteObj = _baggageRepository.GetBaggageDetailByIdAndCompanyId(Id, SessionHelper.CompanyId).FirstOrDefault();

                //var quoteItemList1 = _dbRepositoryBaggageItems.GetEntities().Where(m => m.QuoteId == Id && m.CartonId != 0 && m.QuoteId != 0).ToList();
                List<BaggageItemModel> quoteItemList = _baggageRepository.GetBaggageItemByQuoteId(Id).Where(m => m.CartonId != 0 && m.QuoteId != 0).ToList();

                //var quoteItem = _dbRepositoryBaggageItems.GetEntities().Where(m => m.QuoteId == Id && m.CartonId == 0 && m.QuoteId != 0).ToList();
                var quoteItem = _baggageRepository.GetBaggageItems(Id);

                CommonController CommonController = new CommonController(_commonRepository);

                IEnumerable<SelectListItem> CountryList = CommonController.GetCountryListCommon();

                //ViewBag.TitleList = CommonHelper.GetTitleList();
                ViewBag.TitleList = new SelectList(_commonRepository.GetList("Title", null), "Value", "Text"); //SelectionList.TitleList().Select(m => new { TitleId = m.Id, m.TitleName });
                ViewBag.CountryList = new SelectList(CountryList, "Value", "Text");  //CountryList;

                ViewBag.SalesRepCodeList = new SelectList(_commonRepository.GetList("SalesRepDropdown", SessionHelper.CompanyId), "Value", "Text");//CommonController.GetSalesRepCodeListCommon();
                ViewBag.CountryCodeList = new SelectList(_commonRepository.GetList("countryCode", null), "Value", "Text"); //CommonController.GetCountryCodeListCommon();

                var QuoteId = new SqlParameter
                {
                    ParameterName = "QuoteId",
                    DbType = DbType.Int64,
                    Value = Id
                };


                quotesEntities entityObj = new quotesEntities();
                //var quoteamountList = entityObj.Database.SqlQuery<int>("SP_GetQuoteIdList @QuoteId", QuoteId).ToList();
                var quoteamountList = _baggageRepository.GetQuoteIdList(Id);

                List<QuoteAmountList> quoteAmtList = new List<QuoteAmountList>();

                quoteAmtList = quoteamountList.ToList();
                obj.quoteAmountList = quoteAmtList.ToList();

                //if (quoteamountList != null)
                //{
                //    for (int i = 0; i < quoteamountList.Count(); i++)
                //    {
                //        QuoteAmountList quoteAmount = new QuoteAmountList();
                //        quoteAmount.QuoteId = quoteamountList[i];
                //        quoteAmtList.Add(quoteAmount);
                //    }
                //    obj.quoteAmountList = quoteAmtList;
                //}

                TempData[CustomEnums.NotifyType.Error.GetDescription()] = "";

                obj.baggageQuote = new baggageQuote();
                obj.baggageQuote.SalesRep = SessionHelper.SalesRepCode;
                if (quoteObj != null)
                {
                    TempData["isEdit"] = "0";
                    obj.baggageQuote.Id = quoteObj.Id;
                    obj.baggageQuote.TitleId = quoteObj.TitleId;
                    obj.baggageQuote.Firstname = quoteObj.Firstname;
                    obj.baggageQuote.Lastname = quoteObj.Lastname;
                    obj.baggageQuote.Email = quoteObj.Email;
                    obj.baggageQuote.CountryCode = quoteObj.CountryCode;
                    obj.baggageQuote.Telephone = quoteObj.Telephone;
                    obj.baggageQuote.FromCountry = quoteObj.FromCountry;
                    obj.baggageQuote.FromCity = quoteObj.FromCity;
                    obj.baggageQuote.PostCode = quoteObj.PostCode;
                    obj.baggageQuote.EstimatedMoveDate = quoteObj.EstimatedMoveDate;
                    obj.baggageQuote.BranchId = quoteObj.BranchId;
                    obj.baggageQuote.ToCountry = quoteObj.ToCountry;
                    obj.baggageQuote.CityName = quoteObj.CityName;
                    obj.baggageQuote.ToPostCode = quoteObj.ToPostCode;
                    obj.baggageQuote.InternalNotes = string.IsNullOrEmpty(quoteObj.InternalNotes) ? null : quoteObj.InternalNotes;
                    obj.baggageQuote.SalesRep = string.IsNullOrEmpty(quoteObj.SalesRep) ? SessionHelper.SalesRepCode : quoteObj.SalesRep;
                    obj.baggageQuote.IsInquiry = quoteObj.IsInquiry;
                    obj.baggageQuote.ReferenceNumber = quoteObj.ReferenceNumber;
                }
                if (cartonList != null)
                {
                    for (int i = 0; i < cartonList.Count(); i++)
                    {
                        QuoteCalculator.Service.Models.CartonsModel cr = new QuoteCalculator.Service.Models.CartonsModel();
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
                        cr.title = cartonList[i].title;
                        if (cartonList[i].image == null)
                        {
                            cr.image = cartonList[i].image;
                        }
                        else { cr.image = cartonList[i].image.Replace("images", "img"); }
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
                }
                if (quoteItem != null)
                {
                    for (int i = 0; i < quoteItem.Count(); i++)
                    {
                        Movebaggage mb = new Movebaggage();
                        mb.Id = quoteItem[i].Id;
                        mb.description = quoteItem[i].Description;
                        mb.length = quoteItem[i].Length;
                        mb.breadth = quoteItem[i].Breadth;
                        mb.height = quoteItem[i].Height;
                        mb.quantity = quoteItem[i].Quantity;
                        mb.Volume = quoteItem[i].Volume;
                        mb.UserVolume = quoteItem[i].UserVolume;
                        moveBaggaheList[i] = mb;
                    }
                }
                obj.cartonList = cartonModelList;
                obj.moveList = moveBaggaheList;
                SessionHelper.BaggageQuoteId = obj.baggageQuote.Id;
                BaggageQuoteInfoModel modelObj = new BaggageQuoteInfoModel();
                if (Id != null)
                {
                    // modelObj = _dbRepository.SelectById(Id); 
                    modelObj = _baggageRepository.GetBaggageQuoteById(Id);
                }
            }
            catch (Exception ex)
            {
                TempData[CustomEnums.NotifyType.Error.GetDescription()] = CommonHelper.GetErrorMessage(ex);
            }
            return View(obj);
        }

        [HttpPost]
        public ActionResult AddEdit(BaggageModel model, bool? isEdit, bool? isInquiry)
        {
            try
            {
                model.baggageQuote.Id = 0;

                if (isEdit != null)
                {
                    TempData["isEdit"] = "1";
                }

                //if (model.baggageQuote.FromCountry != "UK")
                //{
                //    model.baggageQuote.SalesRep = "NG2";
                //}

                SessionHelper.ToCountryCode = model.baggageQuote.ToCountry;
                bool CartoonValidation = false;
                bool MoveListValidation = false;

                if (model.cartonList != null && model.cartonList.Count > 0)
                {
                    foreach (var i in model.cartonList)
                    {
                        if (i.quantity > 0)
                        {
                            CartoonValidation = true;
                            break;
                        }
                    }
                }
                if (model.moveList != null && model.moveList.Count > 0)
                {
                    foreach (var i in model.moveList)
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
                    CommonController CommonController = new CommonController(_commonRepository);
                    IEnumerable<SelectListItem> CountryList = CommonController.GetCountryListCommon();
                    ViewBag.TitleList = new SelectList(_commonRepository.GetList("Title", null), "Value", "Text");
                    ViewBag.CountryList = new SelectList(CountryList, "Value", "Text");
                    ViewBag.SalesRepCodeList = new SelectList(_commonRepository.GetList("SalesRepDropdown", SessionHelper.CompanyId), "Value", "Text");
                    ViewBag.CountryCodeList = new SelectList(_commonRepository.GetList("countryCode", null), "Value", "Text");

                    TempData[CustomEnums.NotifyType.Error.GetDescription()] = "Please select at least one item";
                    return View(model);
                }
                SessionHelper.BaggageQuoteId = model.baggageQuote.Id;
                var newId = 0;
                var QuoteObj = new BaggageQuoteInfoModel();
                if (model.baggageQuote != null)
                {
                    if (model.baggageQuote.PostCode != null && model.baggageQuote.PostCode != string.Empty)
                    {
                        string pc = model.baggageQuote.PostCode.Replace(" ", "");
                        pc = pc.Substring(0, pc.Length - 3) + " " + pc.Substring(pc.Length - 3, 3);
                        var postCodeObj = _baggageRepository.GetListFromUk(pc)?.FirstOrDefault();
                        string postCode = postCodeObj?.zip ?? "";
                        model.baggageQuote.PostCode = postCode;
                    }
                    if (model.baggageQuote.ToPostCode != null && model.baggageQuote.ToPostCode != string.Empty)
                    {
                        string pc = model.baggageQuote.ToPostCode.Replace(" ", "");
                        pc = pc.Substring(0, pc.Length - 3) + " " + pc.Substring(pc.Length - 3, 3);

                        //var postCodeObj = _dbRepositoryUKPostCode.GetEntities().Where(m => m.zip == pc).FirstOrDefault();
                        var postCodeObj = _baggageRepository.GetListFromUk(pc).FirstOrDefault();

                        model.baggageQuote.ToPostCode = postCodeObj.zip;
                    }

                    QuoteObj.IsConditionApply = true;
                    QuoteObj.TitleId = model.baggageQuote.TitleId;
                    QuoteObj.Firstname = string.IsNullOrEmpty(model.baggageQuote.Firstname) ? "" : model.baggageQuote.Firstname.First().ToString().ToUpper() + model.baggageQuote.Firstname.Substring(1);
                    QuoteObj.Lastname = string.IsNullOrEmpty(model.baggageQuote.Lastname) ? "" : model.baggageQuote.Lastname.First().ToString().ToUpper() + model.baggageQuote.Lastname.Substring(1);
                    QuoteObj.Email = model.baggageQuote.Email;
                    QuoteObj.CountryCode = model.baggageQuote.CountryCode;
                    QuoteObj.Telephone = string.IsNullOrEmpty(model.baggageQuote.Telephone) ? "" : model.baggageQuote.Telephone;
                    QuoteObj.FromCountry = model.baggageQuote.FromCountry;
                    QuoteObj.ToCountry = model.baggageQuote.ToCountry;
                    QuoteObj.PostCode = model.baggageQuote.PostCode;
                    QuoteObj.CityName = model.baggageQuote.CityName;
                    QuoteObj.EstimatedMoveDate = Convert.ToDateTime(model.baggageQuote.EstimatedMoveDate);
                    if (model.baggageQuote.BranchId == null)
                        QuoteObj.BranchId = GetBranchId(string.IsNullOrEmpty(model.baggageQuote.PostCode) ? model.baggageQuote.ToPostCode : model.baggageQuote.PostCode);
                    else
                        QuoteObj.BranchId = model.baggageQuote.BranchId;
                    QuoteObj.FromCity = model.baggageQuote.FromCity;
                    QuoteObj.ToPostCode = model.baggageQuote.ToPostCode;
                    QuoteObj.CreatedDate = DateTime.Now;
                    QuoteObj.NextExecutionDate = DateTime.Today.AddDays(7);
                    QuoteObj.InternalNotes = string.IsNullOrEmpty(model.baggageQuote.InternalNotes) ? null : model.baggageQuote.InternalNotes;
                    QuoteObj.SalesRep = model.baggageQuote.SalesRep ?? SessionHelper.SalesRepCode;
                    QuoteObj.Company = SessionHelper.CompanyId;
                    QuoteObj.IsInquiry = isInquiry == true;
                    if (model.baggageQuote.IsInquiry == true) { QuoteObj.IsInquiry = true; }

                    if (model.baggageQuote.Id > 0)
                    {
                        QuoteObj.Id = model.baggageQuote.Id;
                        if (TempData["isEdit"] != null)
                        {
                            if (TempData["isEdit"].ToString() == "1")
                            {
                                {
                                    newId = _baggageRepository.InsertUpdateBaggageQuote(QuoteObj);
                                }
                            }
                            else
                            {
                                newId = _baggageRepository.InsertUpdateBaggageQuote(QuoteObj);
                            }
                        }
                        else
                        {
                            newId = _baggageRepository.InsertUpdateBaggageQuote(QuoteObj);
                        }
                    }
                    else
                    {
                        newId = _baggageRepository.InsertUpdateBaggageQuote(QuoteObj);
                        QuoteObj.Id = newId;

                    }
                    SessionHelper.BaggageQuoteId = QuoteObj.Id;
                    SessionHelper.QuoteType = "2";
                }


                if (CartoonValidation == true || MoveListValidation == true)
                {
                    foreach (var item in model.moveList)
                    {
                        BaggageItemModel moveBaggage = new BaggageItemModel();
                        int length = item.length == 0 ? 1 : item.length;
                        int breath = item.breadth == 0 ? 1 : item.breadth;
                        int height = item.height == 0 ? 1 : item.height;
                        double ans = (length * breath * height) / 28316.8;
                        moveBaggage.Id = item.Id;
                        moveBaggage.Length = item.length;
                        moveBaggage.Breadth = item.breadth;
                        moveBaggage.Height = item.height;
                        moveBaggage.Description = item.description;
                        moveBaggage.Volume = Math.Ceiling(ans); // item.Volume;
                        moveBaggage.Quantity = item.quantity;
                        moveBaggage.UserVolume = item.UserVolume;
                        moveBaggage.Type = "ADDITIONAL";
                        moveBaggage.QuoteId = QuoteObj.Id;
                        moveBaggage.Company = SessionHelper.CompanyId;
                        int newItemId = 0;
                        if (model.baggageQuote.Id > 0 && item.quantity > 0)
                            if (TempData["isEdit"] != null)
                            {
                                if (TempData["isEdit"].ToString() == "1")
                                {
                                    if (item.description != null && item.quantity != 0)
                                        newItemId = _baggageRepository.InsertUpdateBaggageItem(moveBaggage);
                                }

                            }
                            else
                            {
                                if (item.description != null && item.quantity != 0)
                                    newItemId = _baggageRepository.InsertUpdateBaggageItem(moveBaggage);
                            }
                        else
                        {
                            if(item.description != null && item.quantity!=0)
                            newItemId = _baggageRepository.InsertUpdateBaggageItem(moveBaggage);
                        }
                    }

                    foreach (var list in model.cartonList)
                    {
                        if (list != null)
                        {
                            if (list.display == 1)
                            {
                                if (list.quantity > 0)
                                {
                                    BaggageItemModel moveBaggage = new BaggageItemModel();
                                    var baggageItems = _baggageRepository.GetBaggageItemByIds(model.baggageQuote.Id, list.id);
                                    long baggageId;

                                    if (baggageItems.Count == 0)
                                    {
                                        baggageId = 0;
                                    }
                                    else
                                    {
                                        baggageId = Convert.ToInt64(baggageItems.First().Id);
                                    }
                                    //_dbRepositoryBaggageItems.GetEntities().Where(m => m.QuoteId == model.baggageQuote.Id && m.CartonId == list.id && m.QuoteId != 0).Select(m => m.Id).FirstOrDefault();
                                    moveBaggage.Id = baggageId;
                                    moveBaggage.CartonId = list.id;
                                    moveBaggage.QuoteId = model.baggageQuote.Id;
                                    moveBaggage.Type = list.type;
                                    moveBaggage.Description = list.description;
                                    moveBaggage.Length = list.length;
                                    moveBaggage.Breadth = list.breadth;
                                    moveBaggage.Height = list.height;
                                    moveBaggage.Volume = list.volume;
                                    moveBaggage.UserVolume = list.UserVolume == null ? 0 : list.UserVolume;
                                    moveBaggage.Quantity = list.quantity;
                                    moveBaggage.QuoteId = QuoteObj.Id;
                                    moveBaggage.Groweight = list.weight;
                                    moveBaggage.MovewareDescription = list.moveware_description;
                                    moveBaggage.Company = SessionHelper.CompanyId;

                                    var newItemId = 0;
                                    if (model.baggageQuote.Id > 0)
                                    {
                                        if (TempData["isEdit"] != null)
                                        {
                                            if (TempData["isEdit"].ToString() == "1")
                                            {
                                                newItemId = _baggageRepository.InsertUpdateBaggageItem(moveBaggage);
                                            }
                                        }
                                        else
                                        {
                                            newItemId = _baggageRepository.InsertUpdateBaggageItem(moveBaggage);
                                        }
                                    }
                                    else
                                    {
                                        newItemId = _baggageRepository.InsertUpdateBaggageItem(moveBaggage);
                                    }
                                }
                            }
                        }
                    }
                }

                //No need to remove items as we are always adding new quotes
                /*
                var itemIdList = _dbRepositoryBaggageItems.GetEntities().Where(m => m.QuoteId == QuoteObj.Id && m.CartonId == 0).Select(m => m.Id).ToList();
                var selectedIdList = _dbRepositoryBaggageItems.GetEntities().Where(m => m.QuoteId == QuoteObj.Id && m.CartonId != 0).Select(m => m.Id).ToList();
                //var selectedIdList = model.moveList.Where(m => m.Id != 0).Select(m => m.Id).ToList();
                var removedIdList = itemIdList.Except(selectedIdList).ToList();
                if (selectedIdList.Count() > 0)
                {
                    for (int i = 0; i < removedIdList.Count(); i++)
                    {
                        _dbRepositoryBaggageItems.Delete(removedIdList[i]);
                    }
                }
                */

                model.baggageQuote.Id = QuoteObj.Id;
                TempData[CustomEnums.NotifyType.Success.GetDescription()] = "Quote Added Successfully";
                if (isInquiry == true)
                {
                    return Json(new { data = SessionHelper.BaggageQuoteId });
                }
                //if (isEdit != null)
                //{
                //    //return RedirectToAction("AddEdit", new { @Id = SessionHelper.BaggageQuoteId });
                //    return View(model);
                //}

            }
            catch (Exception ex)
            {
                TempData[CustomEnums.NotifyType.Error.GetDescription()] = CommonHelper.GetErrorMessage(ex);
            }
            //return Json(new { result = CommonHelper.Encode(SessionHelper.BaggageQuoteId.ToString()) }, JsonRequestBehavior.AllowGet);
            return RedirectToAction("Baggage", "Baggage", new { @baggageId = CommonHelper.Encode(SessionHelper.BaggageQuoteId.ToString()) });
        }

        [HttpGet]
        public async Task<ActionResult> Baggage(string baggageId)
        {
            BaggageModel obj = new BaggageModel();

            try
            {
                int companyId = SessionHelper.CompanyId;
                int volume = 0;
                double cubicFeet = 0;
                var Id = Convert.ToInt32(CommonHelper.Decode(baggageId));

                //List<tbl_BaggageItem> cartonObjList1 = _dbRepositoryMoveBaggage.GetEntities().Where(m => m.QuoteId == Id && m.QuoteId != 0).ToList();
                List<BaggageItemModel> cartonObjList = _baggageRepository.GetMoveBaggageListByQuoteId(Id);
                //bool wasQuoteGeneratedBefore = (_dbRepositoryQuoteAmount.GetEntities().Where(m => m.QuoteId == Id && m.MoveType == "EXB" && m.Company == companyId).Count() > 0);
                bool wasQuoteGeneratedBefore = (_baggageRepository.GetQuoteAmountByShippingType(Id, "EXB", null).Where(x => x.Company == companyId).Count() > 0);

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
                    moveBaggage.id = cartonItemList[i].Id;
                    moveBaggage.description = cartonItemList[i].Description;
                    moveBaggage.length = cartonItemList[i].Length;
                    moveBaggage.breadth = cartonItemList[i].Breadth;
                    moveBaggage.height = cartonItemList[i].Height;
                    moveBaggage.quantity = cartonItemList[i].Quantity;
                    moveBaggage.UserVolume = cartonItemList[i].UserVolume;
                    moveBaggage.volume = cartonItemList[i].Volume;
                    moveBaggage.type = cartonItemList[i].Type;

                    volume += cartonItemList[i].UserVolume ?? 0;
                    cubicFeet += (cartonItemList[i].Volume * cartonItemList[i].Quantity);
                    cartonList.Add(moveBaggage);
                }

                obj.moveList = moveObjList;
                obj.cartonList = cartonList;


                quotesEntities entityObj = new quotesEntities();
                BaggageCalculationModel baggageObj = _baggageRepository.BaggageCalculation(Convert.ToInt64(Id));
                bool isDuplicateQuoteRef = false;
                BaggageQuoteInfoModel updatedModel = _baggageRepository.GetBaggageDetailByIdAndCompanyId(Id, SessionHelper.CompanyId).FirstOrDefault();
                string refNo = (updatedModel == null) ? null : updatedModel.ReferenceNumber;
                //         isDuplicateQuoteRef = _dbRepositoryBaggageQuote.GetEntities().Any(m => m.Id != Id && m.ReferenceNumber != null && m.ReferenceNumber == refNo && m.IsInquiry == false);
                isDuplicateQuoteRef = _baggageRepository.checkDuplicateQuoteRef(Id, refNo).Where(x => x.IsInquiry == false).Count() > 0;

                obj.calculationLines = new List<BaggageCalculationLineModel>();
                //  SP_GetBaggageXmlData_Result xmlResult1 = CustomRepository.GetBaggageXmlData(Id, companyId);
                BaggageXmlData xmlResult = _baggageRepository.GetBaggageXmlData(Id, companyId);
                //  SP_GetCollectionDelivery_Result xmlColDelResult1 = CustomRepository.GetCollectionDeliveryData(Id, companyId);
                CollectionDelivery xmlColDelResult = _baggageRepository.GetCollectionDeliveryData(Id, companyId);
                List<string> xmlFiles = new List<string>();

                if (baggageObj.AirFreightToAirport > 0)
                {
                    xmlResult.IxType = "DTDE";
                    //    decimal VolumetricsWeight1 = CustomRepository.GetVolumetricsWeight(Id, "AIR");
                    decimal VolumetricsWeight = _baggageRepository.GetVolumetricsWeight(Id, "AIR", companyId);
                    string Desc = VolumetricsWeight.ToString() + " Vol kgs" + (volume > 0 ? ("/" + volume + " Kgs gross") : "");
                    obj.calculationLines.Add(GetBaggageCalcLine("Air Freight To Airport", baggageObj.AirFreightToAirport.Value, Desc));
                    var CustomerRefNo = _baggageRepository.GetQuoteAmountByShippingType(Id, "EXB", "AirfreightToAirport").FirstOrDefault();
                    //var CustomerRefNo = _dbRepositoryQuoteAmount.GetEntities().Where(m => m.QuoteId == Id && m.MoveType == "EXB" && m.ShippingType == "AirfreightToAirport").FirstOrDefault();
                    //string file = CustomerRefNo.CustomerReferenceNo + "/" + CustomerRefNo.CustomerQuoteNo + "." + CustomerRefNo.QuoteSeqNo;
                    string file = System.IO.Path.Combine(Convert.ToString(CustomerRefNo.CustomerReferenceNo), CustomerRefNo.CustomerQuoteNo + "." + CustomerRefNo.QuoteSeqNo);
                    //string file = "AirFreightToAirport";
                    if (!isDuplicateQuoteRef && !wasQuoteGeneratedBefore)
                    {
                        string AirFreightToAirport = await XMLHelper.GenerateBaggageXml(xmlResult, cartonObjList, Convert.ToDouble(baggageObj.AirFreightToAirport.Value), xmlColDelResult, file, "AIRTOPORT", null, companyId); //TODO            
                        xmlFiles.Add(AirFreightToAirport);
                    }
                    ViewBag.ReferenceNo = string.Concat(CustomerRefNo.CustomerReferenceNo + "/" + CustomerRefNo.CustomerQuoteNo);
                }
                if (baggageObj.AirFreightToDoor > 0)
                {
                    xmlResult.IxType = "DOP";
                    // decimal VolumetricsWeight1 = CustomRepository.GetVolumetricsWeight(Id, "AIR");
                    decimal VolumetricsWeight = _baggageRepository.GetVolumetricsWeight(Id, "AIR", companyId);
                    string Desc = VolumetricsWeight.ToString() + " Vol kgs" + (volume > 0 ? ("/" + volume + " Kgs gross") : "");
                    obj.calculationLines.Add(GetBaggageCalcLine("Air Freight To Door", baggageObj.AirFreightToDoor.Value, Desc));
                    var CustomerRefNo = _baggageRepository.GetQuoteAmountByShippingType(Id, "EXB", "AirfreightToDoor").FirstOrDefault();
                    //var CustomerRefNo = _dbRepositoryQuoteAmount.GetEntities().Where(m => m.QuoteId == Id && m.MoveType == "EXB" && m.ShippingType == "AirfreightToDoor").FirstOrDefault();
                    //string file = CustomerRefNo.CustomerReferenceNo + "/" + CustomerRefNo.CustomerQuoteNo + "." + CustomerRefNo.QuoteSeqNo;
                    string file = System.IO.Path.Combine(Convert.ToString(CustomerRefNo.CustomerReferenceNo), CustomerRefNo.CustomerQuoteNo + "." + CustomerRefNo.QuoteSeqNo);
                    //string file = "AirFreightToDoor";
                    if (!isDuplicateQuoteRef && !wasQuoteGeneratedBefore)
                    {
                        string AirFreightToDoor = await XMLHelper.GenerateBaggageXml(xmlResult, cartonObjList, Convert.ToDouble(baggageObj.AirFreightToDoor.Value), xmlColDelResult, file, "AIRTODOOR", null, companyId); //TODO            
                        xmlFiles.Add(AirFreightToDoor);
                    }
                    ViewBag.ReferenceNo = string.Concat(CustomerRefNo.CustomerReferenceNo + "/" + CustomerRefNo.CustomerQuoteNo);
                }
                if (baggageObj.Courier > 0)
                {
                    xmlResult.IxType = "DTDE";
                    //   decimal VolumetricsWeight1 = CustomRepository.GetVolumetricsWeight(Id, "COURIER");
                    decimal VolumetricsWeight = _baggageRepository.GetVolumetricsWeight(Id, "COURIER", companyId);
                    string Desc = VolumetricsWeight.ToString() + " Vol kgs" + (volume > 0 ? ("/" + volume + " Kgs gross") : "");
                    // obj.calculationLines.Add(GetBaggageCalcLine("Courier", baggageObj.Courier.Value, Desc));
                    var calculationLines = GetBaggageCalcLine("Courier", baggageObj.Courier.Value, Desc);
                    var currentquoteObj = _baggageRepository.GetBaggageDetailByIdAndCompanyId(Id, SessionHelper.CompanyId).FirstOrDefault();
                    var ShippingType = "";
                    if (currentquoteObj.FromCountry == "UK")
                        ShippingType = "Courier";
                    else
                        ShippingType = "CourierExpressToDoor";
                    var CustomerRefNo = _baggageRepository.GetQuoteAmountByShippingType(Id, "EXB", ShippingType).FirstOrDefault();
                    //var CustomerRefNo = _dbRepositoryQuoteAmount.GetEntities().Where(m => m.QuoteId == Id && m.MoveType == "EXB" && m.ShippingType == "Courier").FirstOrDefault();
                    if (CustomerRefNo != null)
                    {
                        string file = CustomerRefNo.CustomerReferenceNo + "/" + CustomerRefNo.CustomerQuoteNo + "." + CustomerRefNo.QuoteSeqNo;
                        //string file = "Courier";
                        //bool considercostToXML = false;
                        //var currentquoteObj = _dbRepositoryBaggageQuote.GetEntities().Where(m => m.Id == Id && m.Company == SessionHelper.CompanyId).FirstOrDefault();
                        List<BaggageCostModel> _baggeCostModel = null;
                        if (currentquoteObj != null && (currentquoteObj.ToCountry == "UK"))
                        {
                            calculationLines.DeliveryMethodName = "Courier Express To Door";
                            calculationLines.TransitionTime = "3 to 5 working days";
                            obj.calculationLines.Add(calculationLines);
                            //considercostToXML = true;
                            _baggeCostModel = _baggageRepository.GetBaggageCostByQuoteId(Id, "Bag Imports UK");
                        }
                        else if (currentquoteObj != null && currentquoteObj.FromCountry != "UK" && currentquoteObj.ToCountry != "UK")
                        {
                            calculationLines.DeliveryMethodName = "Courier Express To Door";
                            calculationLines.TransitionTime = "3 to 5 working days";
                            obj.calculationLines.Add(calculationLines);
                            _baggeCostModel = _baggageRepository.GetBaggageCostByQuoteId(Id, "Bag C2C");
                        }
                        else
                        {
                            obj.calculationLines.Add(calculationLines);
                            _baggeCostModel = _baggageRepository.GetBaggageCostByQuoteId(Id, "Curier Economy");
                        }
                        if (!isDuplicateQuoteRef && !wasQuoteGeneratedBefore)
                        {
                            string Courier = await XMLHelper.GenerateBaggageXml(xmlResult, cartonObjList, Convert.ToDouble(baggageObj.Courier.Value), xmlColDelResult, file, "Courier", _baggeCostModel, companyId); //TODO            
                            xmlFiles.Add(Courier);
                        }
                        ViewBag.ReferenceNo = string.Concat(CustomerRefNo.CustomerReferenceNo + "/" + CustomerRefNo.CustomerQuoteNo);
                    }
                }
                if (baggageObj.SeaFreight > 0)
                {
                    xmlResult.IxType = "DTDE";
                    string Desc = string.Concat(cubicFeet, " cubic feet");
                    obj.calculationLines.Add(GetBaggageCalcLine("Sea Freight", baggageObj.SeaFreight.Value, Desc));
                    var CustomerRefNo = _baggageRepository.GetQuoteAmountByShippingType(Id, "EXB", "SeaFreight").FirstOrDefault();
                    //var CustomerRefNo = _dbRepositoryQuoteAmount.GetEntities().Where(m => m.QuoteId == Id && m.MoveType == "EXB" && m.ShippingType == "SeaFreight").FirstOrDefault();
                    if (CustomerRefNo != null)
                    {
                        //string file = CustomerRefNo.CustomerReferenceNo + "/" + CustomerRefNo.CustomerQuoteNo + "." + CustomerRefNo.QuoteSeqNo;
                        string file = System.IO.Path.Combine(Convert.ToString(CustomerRefNo.CustomerReferenceNo), CustomerRefNo.CustomerQuoteNo + "." + CustomerRefNo.QuoteSeqNo);
                        //string file = "SeaFreight";
                        if (!isDuplicateQuoteRef && !wasQuoteGeneratedBefore)
                        {
                            List<BaggageCostModel> _baggeCostModel = _baggageRepository.GetBaggageCostByQuoteId(Id, "Sea Freight");
                            string SeaFreight = await XMLHelper.GenerateBaggageXml(xmlResult, cartonObjList, Convert.ToDouble(baggageObj.SeaFreight.Value), xmlColDelResult, file, "Sea", _baggeCostModel, companyId); //TODO            
                            xmlFiles.Add(SeaFreight);
                        }
                        ViewBag.ReferenceNo = string.Concat(CustomerRefNo.CustomerReferenceNo + "/" + CustomerRefNo.CustomerQuoteNo);
                    }
                }
                if (baggageObj.RoadFreightToDoor > 0)
                {
                    xmlResult.IxType = "DTDE";
                    string Desc = string.Concat(cubicFeet, " cubic feet");
                    obj.calculationLines.Add(GetBaggageCalcLine("Road Freight To Door", baggageObj.RoadFreightToDoor.Value, Desc));
                    var CustomerRefNo = _baggageRepository.GetQuoteAmountByShippingType(Id, "EXB", "RoadfreightToDoor").FirstOrDefault();
                    //var CustomerRefNo = _dbRepositoryQuoteAmount.GetEntities().Where(m => m.QuoteId == Id && m.MoveType == "EXB" && m.ShippingType == "RoadfreightToDoor").FirstOrDefault();
                    //string file = CustomerRefNo.CustomerReferenceNo + "/" + CustomerRefNo.CustomerQuoteNo + "." + CustomerRefNo.QuoteSeqNo;
                    string file = System.IO.Path.Combine(Convert.ToString(CustomerRefNo.CustomerReferenceNo), CustomerRefNo.CustomerQuoteNo + "." + CustomerRefNo.QuoteSeqNo);
                    if (!isDuplicateQuoteRef && !wasQuoteGeneratedBefore)
                    {
                        string RoadFreightToDoor = await XMLHelper.GenerateBaggageXml(xmlResult, cartonObjList, Convert.ToDouble(baggageObj.RoadFreightToDoor.Value), xmlColDelResult, file, "Road", null, companyId); //TODO            
                        xmlFiles.Add(RoadFreightToDoor);
                    }
                    ViewBag.ReferenceNo = string.Concat(CustomerRefNo.CustomerReferenceNo + "/" + CustomerRefNo.CustomerQuoteNo);
                }
                if (baggageObj.CourierExpressToDoor > 0)
                {
                    xmlResult.IxType = "DTDE";
                    //decimal VolumetricsWeight1 = CustomRepository.GetVolumetricsWeight(Id, "COURIEREXPRESS");
                    decimal VolumetricsWeight = _baggageRepository.GetVolumetricsWeight(Id, "COURIEREXPRESS", companyId);
                    string Desc = VolumetricsWeight.ToString() + " Vol kgs" + (volume > 0 ? ("/" + volume + " Kgs gross") : "");
                    obj.calculationLines.Add(GetBaggageCalcLine("Courier Express To Door", baggageObj.CourierExpressToDoor.Value, Desc));
                    var CustomerRefNo = _baggageRepository.GetQuoteAmountByShippingType(Id, "EXB", "CourierExpressToDoor").Where(x => x.Company == companyId).FirstOrDefault();
                    //var CustomerRefNo = _dbRepositoryQuoteAmount.GetEntities().Where(m => m.QuoteId == Id && m.MoveType == "EXB" && m.ShippingType == "CourierExpressToDoor" && m.Company == companyId).FirstOrDefault();
                    //string file = CustomerRefNo.CustomerReferenceNo + "/" + CustomerRefNo.CustomerQuoteNo + "." + CustomerRefNo.QuoteSeqNo;
                    string file = System.IO.Path.Combine(Convert.ToString(CustomerRefNo.CustomerReferenceNo), CustomerRefNo.CustomerQuoteNo + "." + CustomerRefNo.QuoteSeqNo);
                    if (!isDuplicateQuoteRef && !wasQuoteGeneratedBefore)
                    {
                        List<BaggageCostModel> _baggeCostModel = _baggageRepository.GetBaggageCostByQuoteId(Id, "Curier Express");
                        string CourierExpress = await XMLHelper.GenerateBaggageXml(xmlResult, cartonObjList, Convert.ToDouble(baggageObj.CourierExpressToDoor.Value), xmlColDelResult, file, "CourierExpress", _baggeCostModel, companyId); //TODO            
                        xmlFiles.Add(CourierExpress);
                    }
                    ViewBag.ReferenceNo = string.Concat(CustomerRefNo.CustomerReferenceNo + "/" + CustomerRefNo.CustomerQuoteNo);
                }

                //if (xmlFiles.Count > 0 && !wasQuoteGeneratedBefore)
                //{
                //    Task task = new Task(() => SendXmlFies(xmlFiles));
                //    task.Start();
                //}


                //tbl_baggageQuote model = _dbRepositoryBaggageQuote.SelectById(Id);
                BaggageQuoteInfoModel model = _baggageRepository.GetBaggageQuoteById(Id);
                EmailTemplateModel emailTemplateObj;
                if (baggageObj.AirFreightToAirport == 0 && baggageObj.AirFreightToDoor == 0 && baggageObj.Courier == 0 && baggageObj.SeaFreight == 0 && baggageObj.RoadFreightToDoor == 0 && baggageObj.CourierExpressToDoor == 0)
                {
                    if (companyId == 1)
                        //emailTemplateObj = _dbRepositoryEmailTemplate.GetEntities().Where(m => m.ServiceId == 1018).FirstOrDefault();
                        emailTemplateObj = _baggageRepository.GetEmailTemplateByServiceId(1018).FirstOrDefault();
                    else if (companyId == 2)
                        emailTemplateObj = _baggageRepository.GetEmailTemplateByServiceId(1026).FirstOrDefault();
                    else if (companyId == 3)
                        emailTemplateObj = _baggageRepository.GetEmailTemplateByServiceId(1022).FirstOrDefault();
                    else
                        emailTemplateObj = null;
                }
                else
                {
                    // emailTemplateObj = _dbRepositoryEmailTemplate.GetEntities().Where(m => m.ServiceId == (companyId == 1 ? 1004 : (companyId == 2 ? 1009 : 1021))).FirstOrDefault();
                    emailTemplateObj = _baggageRepository.GetEmailTemplateByServiceId((companyId == 1 ? 1004 : (companyId == 2 ? 1009 : 1021))).FirstOrDefault();
                }

                string bodyTemplate = string.Empty;
                if (emailTemplateObj != null)
                {
                    bodyTemplate = emailTemplateObj.HtmlContent;
                }
                string salesRepName = string.Empty;
                string salesRepEmail = string.Empty;
                string displayName = string.Empty;
                if (!string.IsNullOrEmpty(model.SalesRep))
                {
                    //user userModel = _dbRepositoryUser.GetEntities().Where(m => m.SalesRepCode == model.SalesRep && m.CompanyId == companyId).FirstOrDefault();
                    UserModel userModel = _baggageRepository.GetUsersBySalesRepCode(model.SalesRep, companyId).FirstOrDefault();
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
                        displayName = "DisplayBaggage";
                    }
                    else if (companyId == 2)
                    {
                        salesRepName = "Pickfords Baggage Team";
                        salesRepEmail = "shipping@pickfords-baggage.co.uk";
                        displayName = "DisplayPickfords";
                    }
                    else if (companyId == 3)
                    {
                        salesRepName = "Sales Team";
                        salesRepEmail = "sales@excess-international.com";
                        displayName = "DisplayExcessBaggage";
                    }
                }
                bodyTemplate = bodyTemplate.Replace("#salesRepName#", salesRepName);
                bodyTemplate = bodyTemplate.Replace("#salesRepEmail#", salesRepEmail);
                bodyTemplate = bodyTemplate.Replace("#CustName#", model.Firstname);
                if (!wasQuoteGeneratedBefore)
                {
                    Task quoteTask = new Task(() => EmailHelper.SendAsyncEmail(companyId, model.Email, emailTemplateObj.Subject, bodyTemplate, "EmailBaggage_" + companyId, displayName, true));
                    quoteTask.Start();
                }
                //  var baggageObject = _dbRepositoryBaggageQuote.GetEntities().Where(m => m.Id == Id).FirstOrDefault();
                var baggageObject = _baggageRepository.GetBaggageQuoteById(Id);

                if (baggageObj != null)
                {
                    SessionHelper.QuoteType = "2";
                    SessionHelper.FromCountryCode = baggageObject.FromCountry;
                    SessionHelper.ToCountryCode = baggageObject.ToCountry;
                    ViewBag.InternalNotes = string.IsNullOrEmpty(baggageObject.InternalNotes) ? null : baggageObject.InternalNotes;
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

                //var shipping = _dbRepositoryBaggageQuote.GetEntities().Where(m => m.Id == Id).FirstOrDefault();
                var shipping = _baggageRepository.GetBaggageQuoteById(Id);
                ViewBag.FromCountry = shipping.FromCountry;
                ViewBag.FromCity = string.IsNullOrEmpty(shipping.FromCity) ? shipping.PostCode : shipping.FromCity;
                ViewBag.ToCountry = shipping.ToCountry;
                ViewBag.ToCity = string.IsNullOrEmpty(shipping.CityName) ? shipping.ToPostCode : shipping.CityName;
                ViewBag.DeliveryCharge = xmlColDelResult.DeliveryCharge;
                ViewBag.CollectionCharge = xmlColDelResult.CollectionCharge;
                obj.baggageQuote = new baggageQuote();
                obj.baggageQuote.Id = Id;
                obj.baggageQuote.Email = model.Email;
                obj.baggageQuote.Company = model.Company ?? 0;
                ViewBag.CompanyShortCode = (companyId == 1 ? "AP" : (companyId == 2 ? "PF" : "EI"));
                ViewBag.VolumeCount = obj.moveList.Sum(m => m.UserVolume) + obj.cartonList.Sum(m => m.UserVolume);

                //var checkReq = _dbRepositoryBaggageQuote.GetEntities().Where(m => m.Telephone == shipping.Telephone && m.Company == companyId && m.CreatedDate >= DateTime.Today && shipping.ReferenceNumber.Substring(shipping.ReferenceNumber.Length - 2) == "/1" && m.ReferenceNumber.Substring(m.ReferenceNumber.Length - 2) == "/1").ToList();
                var checkReq = _baggageRepository.GetallBaggageQuote(companyId).Where(m => m.Telephone == shipping.Telephone && m.CreatedDate >= DateTime.Today && shipping.ReferenceNumber.Substring(shipping.ReferenceNumber.Length - 2) == "/1" && m.ReferenceNumber.Substring(m.ReferenceNumber.Length - 2) == "/1").ToList();
                if (checkReq.Count == 1 && !wasQuoteGeneratedBefore)
                {
                    //string DomainName = Request.Url.Host + (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port);
                    //string myQuoteUrl = DomainName + "/MyQuote";
                    string myQuoteUrl;
                    string SMSText;
                    if (companyId == 1)
                    {
                        myQuoteUrl = "https://quotes.anglopacific.co.uk/MyQuote";
                        SMSText = "Thank you for your online quote request. You can check your quotes here: " + myQuoteUrl + "' or call us on 0800 783 5322 to discuss. Anglo Pacific Shipping.";
                    }
                    else if (companyId == 2)
                    {
                        myQuoteUrl = "https://quotes.pickfords-baggage.co.uk/MyQuote";
                        SMSText = "Thank you for your online quote request. You can check your quotes here: " + myQuoteUrl + "' or call us on 08000 190 333 to discuss. Pickfords International Baggage Services.";
                    }
                    else if (companyId == 3)
                    {
                        myQuoteUrl = "https://quotes.excess-international-baggage.com/MyQuote";
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
                TempData[CustomEnums.NotifyType.Error.GetDescription()] = CommonHelper.GetErrorMessage(ex);
            }
            return View(obj);
        }

        //private void SendXmlFies(List<string> xmlFiles)
        //{
        //    foreach (string xmlFile in xmlFiles)
        //    {
        //        XMLHelper.XmlAPICall(xmlFile, 0);
        //    }
        //}
        //private List<BaggageCostModel> GetBaggageCostByQuoteId(int Id, string ShippingType)
        //{
        //    List<BaggageCostModel> result = new List<BaggageCostModel>();
        //    try
        //    {
        //        quotesEntities entityObj = new quotesEntities();
        //        var QuoteId = new SqlParameter
        //        {
        //            ParameterName = "QuoteId",
        //            DbType = DbType.Int32,
        //            Value = Id
        //        };
        //        var CostShippingType = new SqlParameter
        //        {
        //            ParameterName = "ShippingType",
        //            DbType = DbType.String,
        //            Value = ShippingType
        //        };
        //        result = entityObj.Database.SqlQuery<BaggageCostModel>("SP_GetBaggageCostByQuoteId @QuoteId,@ShippingType", QuoteId, CostShippingType).ToList();
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    return result;
        //}

        public ActionResult KendoItemRead([DataSourceRequest] DataSourceRequest request)
        {
            try
            {
                BaggageModel obj = new BaggageModel();
                //var cartonList = _dbRepositorycartons.GetEntities().Where(m => m.company == SessionHelper.CompanyId).OrderBy(m => m.displayorder).ToList();
                var cartonList = _baggageRepository.GetCartonListByCompanyId(SessionHelper.CompanyId);
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
                    cr.quantity = 0;
                    cartonModelList.Add(cr);
                    moveBaggaheList.Add(mb);
                }

                obj.cartonList = cartonModelList;
                obj.moveList = moveBaggaheList;
                if (!request.Sorts.Any())
                {
                    request.Sorts.Add(new SortDescriptor("Id", ListSortDirection.Ascending));
                }
                return Json(obj);
            }
            catch (Exception ex)
            {
                return Json(CommonHelper.GetErrorMessage(ex));
            }
        }

        public JsonResult GetCityListByCountryId(string countryCode)
        {
            try
            {
                var vehicleCityList = _baggageRepository.GetCityListByCountryId(countryCode, 1);
                return Json(vehicleCityList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(CommonHelper.GetErrorMessage(ex));
            }

        }

        [HttpPost]
        public async Task<string> BaggageQuoteCal(string colName, string quotePrice, int quoteNo = 0)
        {
            string message = string.Empty;
            int GenId = 0;
            colName = colName == "Courier Economy To Door" ? "Courier" : (colName == "Sea Freight To Door" ? "SeaFreight" : colName);
            string MethodName = colName.Replace(" ", "");
            int companyId = SessionHelper.CompanyId;
            try
            {
                if (!string.IsNullOrEmpty(MethodName))
                {
                    string emailStatus = string.Empty;
                    //tbl_baggageQuote model = _dbRepositoryBaggageQuote.SelectById(quoteNo);
                    BaggageQuoteInfoModel model = _baggageRepository.GetBaggageQuoteById(quoteNo);

                    //List<tbl_BaggageItem> cartonObjList1 = _dbRepositoryMoveBaggage.GetEntities().Where(m => m.QuoteId == quoteNo).ToList();
                    List<BaggageItemModel> cartonObjList = _baggageRepository.GetMoveBaggageListByQuoteId(quoteNo);
                    //  SP_GetCollectionDelivery_Result xmlColDelResult = CustomRepository.GetCollectionDeliveryData(quoteNo, companyId);
                    CollectionDelivery xmlColDelResult = _baggageRepository.GetCollectionDeliveryData(quoteNo, companyId);
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
                            // message = _dbRepositoryBaggageQuote.Update(model);
                            GenId = _baggageRepository.UpdateBaggageQuote(model);
                            //SP_GetBaggageXmlData_Result xmlResult = CustomRepository.GetBaggageXmlData(quoteNo, companyId);
                            BaggageXmlData xmlResult = _baggageRepository.GetBaggageXmlData(quoteNo, companyId);
                            xmlResult.IxType = "DTDE";
                            //var CustomerRefNo = _dbRepositoryQuoteAmount.GetEntities().Where(m => m.QuoteId == quoteNo && m.MoveType == "EXB" && m.ShippingType == "AirFreightToAirport").FirstOrDefault();
                            var CustomerRefNo = _baggageRepository.GetQuoteAmountByShippingType(quoteNo, "EXB", "AirfreightToAirport")?.FirstOrDefault();

                            string customerReferenceNo = CustomerRefNo?.CustomerReferenceNo.ToString() ?? "";
                            string customerQuoteNo = CustomerRefNo?.CustomerQuoteNo.ToString() ?? "";
                            string quoteSeqNo = CustomerRefNo?.QuoteSeqNo.ToString() ?? "";

                            string file = customerReferenceNo + "/" + customerQuoteNo + "." + quoteSeqNo;

                            string AirFreightToAirport = await XMLHelper.GenerateBaggageXml(xmlResult, cartonObjList, model.Price.Value, xmlColDelResult, file, "AirFreightToAirport", null, companyId);
                            //Task AF = new Task(() => XMLHelper.XmlAPICall(AirFreightToAirport, 0));
                            //AF.Start();
                            sp_GetdataForEmailSending_Result xmlEmailResult = CustomRepository.GetQuoteData(quoteNo, 2, "AirfreighToAirport");
                            xmlEmailResult.quoteName = companyId == 1 ? "baggage" : (companyId == 2) ? "shipping" : "sales";
                            xmlEmailResult.ReferenceNo = file;
                            Task task = new Task(() => emailStatus = XMLHelper.SendEmail(companyId, xmlEmailResult, (companyId == 1) ? 3 : (companyId == 2) ? 1012 : 1024, AirFreightToAirport, file));
                            //if(SessionHelper.CompanyId == 1)
                            Task taskquote = new Task(() => emailStatus = XMLHelper.SendEmail(companyId, xmlEmailResult, companyId == 1 ? 4 : (companyId == 2) ? 1011 : 1023, "", ""));
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
                            // message = _dbRepositoryBaggageQuote.Update(model);
                            GenId = _baggageRepository.UpdateBaggageQuote(model);
                            //SP_GetBaggageXmlData_Result xmlResult = CustomRepository.GetBaggageXmlData(quoteNo, companyId);
                            BaggageXmlData xmlResult = _baggageRepository.GetBaggageXmlData(quoteNo, companyId);
                            xmlResult.IxType = "DOP";
                            //var CustomerRefNo = _dbRepositoryQuoteAmount.GetEntities().Where(m => m.QuoteId == quoteNo && m.MoveType == "EXB" && m.ShippingType == "AirFreightToDoor").FirstOrDefault();


                            var CustomerRefNo = _baggageRepository.GetQuoteAmountByShippingType(quoteNo, "EXB", "AirFreightToDoor")?.FirstOrDefault();

                            string file = "";
                            if (CustomerRefNo != null)
                            {
                                //file = CustomerRefNo.CustomerReferenceNo + "/" + CustomerRefNo.CustomerQuoteNo + "." + CustomerRefNo.QuoteSeqNo;
                                file = System.IO.Path.Combine(Convert.ToString(CustomerRefNo.CustomerReferenceNo), CustomerRefNo.CustomerQuoteNo + "." + CustomerRefNo.QuoteSeqNo);
                            }

                            string AirFreightToDoor = await XMLHelper.GenerateBaggageXml(xmlResult, cartonObjList, model.Price.Value, xmlColDelResult, file, "AIR", null, companyId);
                            //Task AF = new Task(() => XMLHelper.XmlAPICall(AirFreightToDoor, 0));
                            //AF.Start();
                            sp_GetdataForEmailSending_Result xmlEmailResult = CustomRepository.GetQuoteData(quoteNo, 2, "AirfreightToDoor");
                            xmlEmailResult.quoteName = companyId == 1 ? "baggage" : (companyId == 2 ? "shipping" : "sales");
                            xmlEmailResult.ReferenceNo = file;
                            Task task = new Task(() => emailStatus = XMLHelper.SendEmail(companyId, xmlEmailResult, (companyId == 1) ? 3 : (companyId == 2) ? 1012 : 1024, AirFreightToDoor, file));
                            Task taskquote = new Task(() => emailStatus = XMLHelper.SendEmail(companyId, xmlEmailResult, companyId == 1 ? 4 : (companyId == 2) ? 1011 : 1023, "", ""));
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
                            GenId = _baggageRepository.UpdateBaggageQuote(model);
                            // message = _dbRepositoryBaggageQuote.Update(model);
                            //SP_GetBaggageXmlData_Result xmlResult = CustomRepository.GetBaggageXmlData(quoteNo, companyId);
                            BaggageXmlData xmlResult = _baggageRepository.GetBaggageXmlData(quoteNo, companyId);
                            xmlResult.IxType = "DTDE";
                            //  var CustomerRefNo = _dbRepositoryQuoteAmount.GetEntities().Where(m => m.QuoteId == quoteNo && m.MoveType == "EXB" && m.ShippingType == "Courier").FirstOrDefault();
                            var CustomerRefNo = _baggageRepository.GetQuoteAmountByShippingType(quoteNo, "EXB", "Courier")?.FirstOrDefault();

                            string file = "";
                            if (CustomerRefNo != null)
                            {
                                //file = CustomerRefNo.CustomerReferenceNo + "/" + CustomerRefNo.CustomerQuoteNo + "." + CustomerRefNo.QuoteSeqNo;
                                file = System.IO.Path.Combine(Convert.ToString(CustomerRefNo.CustomerReferenceNo), CustomerRefNo.CustomerQuoteNo + "." + CustomerRefNo.QuoteSeqNo);
                            }
                            string Courier = await XMLHelper.GenerateBaggageXml(xmlResult, cartonObjList, model.Price.Value, xmlColDelResult, file, "Courier", null, companyId);
                            //Task C = new Task(() => XMLHelper.XmlAPICall(Courier, 0));
                            //C.Start();
                            sp_GetdataForEmailSending_Result xmlEmailResult = CustomRepository.GetQuoteData(quoteNo, 2, "Courier");
                            xmlEmailResult.quoteName = companyId == 1 ? "baggage" : (companyId == 2 ? "shipping" : "sales");
                            xmlEmailResult.ReferenceNo = file;
                            Task task = new Task(() => emailStatus = XMLHelper.SendEmail(companyId, xmlEmailResult, (companyId == 1) ? 3 : (companyId == 2) ? 1012 : 1024, Courier, file));
                            Task taskquote = new Task(() => emailStatus = XMLHelper.SendEmail(companyId, xmlEmailResult, companyId == 1 ? 4 : (companyId == 2) ? 1011 : 1023, "", ""));
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
                            GenId = _baggageRepository.UpdateBaggageQuote(model);
                            // message = _dbRepositoryBaggageQuote.Update(model);
                            //SP_GetBaggageXmlData_Result xmlResult = CustomRepository.GetBaggageXmlData(quoteNo, companyId);
                            BaggageXmlData xmlResult = _baggageRepository.GetBaggageXmlData(quoteNo, companyId);
                            xmlResult.IxType = "DTDE";
                            //var CustomerRefNo = _dbRepositoryQuoteAmount.GetEntities().Where(m => m.QuoteId == quoteNo && m.MoveType == "EXB" && m.ShippingType == "SeaFreight").FirstOrDefault();
                            var CustomerRefNo = _baggageRepository.GetQuoteAmountByShippingType(quoteNo, "EXB", "SeaFreight")?.FirstOrDefault();

                            string file = "";
                            if (CustomerRefNo != null)
                            {
                                //file = CustomerRefNo.CustomerReferenceNo + "/" + CustomerRefNo.CustomerQuoteNo + "." + CustomerRefNo.QuoteSeqNo;
                                file = System.IO.Path.Combine(Convert.ToString(CustomerRefNo.CustomerReferenceNo), CustomerRefNo.CustomerQuoteNo + "." + CustomerRefNo.QuoteSeqNo);
                            }
                            string SeaFreight = await XMLHelper.GenerateBaggageXml(xmlResult, cartonObjList, model.Price.Value, xmlColDelResult, file, "Sea", null, companyId);
                            //Task SF = new Task(() => XMLHelper.XmlAPICall(SeaFreight, 0));
                            //SF.Start();
                            sp_GetdataForEmailSending_Result xmlEmailResult = CustomRepository.GetQuoteData(quoteNo, 2, "SeaFreight");
                            xmlEmailResult.quoteName = companyId == 1 ? "baggage" : (companyId == 2 ? "shipping" : "sales");
                            xmlEmailResult.ReferenceNo = file;
                            Task task = new Task(() => emailStatus = XMLHelper.SendEmail(companyId, xmlEmailResult, (companyId == 1) ? 3 : (companyId == 2) ? 1012 : 1024, SeaFreight, file));
                            Task taskquote = new Task(() => emailStatus = XMLHelper.SendEmail(companyId, xmlEmailResult, companyId == 1 ? 4 : (companyId == 2) ? 1011 : 1023, "", ""));
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
                            GenId = _baggageRepository.UpdateBaggageQuote(model);
                            // message = _dbRepositoryBaggageQuote.Update(model);
                            //SP_GetBaggageXmlData_Result xmlResult = CustomRepository.GetBaggageXmlData(quoteNo, companyId);
                            BaggageXmlData xmlResult = _baggageRepository.GetBaggageXmlData(quoteNo, companyId);
                            var CustomerRefNo = _baggageRepository.GetQuoteAmountByShippingType(quoteNo, "EXB", "RoadFreightToDoor")?.FirstOrDefault();
                            //   var CustomerRefNo = _dbRepositoryQuoteAmount.GetEntities().Where(m => m.QuoteId == quoteNo && m.MoveType == "EXB" && m.ShippingType == "RoadFreightToDoor").FirstOrDefault();

                            string file = "";
                            if (CustomerRefNo != null)
                            {
                                //file = CustomerRefNo.CustomerReferenceNo + "/" + CustomerRefNo.CustomerQuoteNo + "." + CustomerRefNo.QuoteSeqNo;
                                file = System.IO.Path.Combine(Convert.ToString(CustomerRefNo.CustomerReferenceNo), CustomerRefNo.CustomerQuoteNo + "." + CustomerRefNo.QuoteSeqNo);
                            }
                            string RoadFreightToDoor = await XMLHelper.GenerateBaggageXml(xmlResult, cartonObjList, model.Price.Value, xmlColDelResult, file, "Road", null, companyId);
                            //Task AF = new Task(() => XMLHelper.XmlAPICall(RoadFreightToDoor, 0));
                            //AF.Start();
                            sp_GetdataForEmailSending_Result xmlEmailResult = CustomRepository.GetQuoteData(quoteNo, 2, "RoadfreightToDoor");
                            xmlEmailResult.quoteName = companyId == 1 ? "baggage" : (companyId == 2 ? "shipping" : "sales");
                            xmlEmailResult.ReferenceNo = file;
                            Task task = new Task(() => emailStatus = XMLHelper.SendEmail(companyId, xmlEmailResult, (companyId == 1) ? 3 : (companyId == 2) ? 1012 : 1024, RoadFreightToDoor, file));
                            Task taskquote = new Task(() => emailStatus = XMLHelper.SendEmail(companyId, xmlEmailResult, companyId == 1 ? 4 : (companyId == 2) ? 1011 : 1023, "", ""));
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
                            GenId = _baggageRepository.UpdateBaggageQuote(model);
                            // message = _dbRepositoryBaggageQuote.Update(model);
                            //SP_GetBaggageXmlData_Result xmlResult = CustomRepository.GetBaggageXmlData(quoteNo, companyId);
                            BaggageXmlData xmlResult = _baggageRepository.GetBaggageXmlData(quoteNo, companyId);
                            xmlResult.IxType = "DTDE";
                            //var CustomerRefNo = _baggageRepository.GetQuoteAmountByShippingType(quoteNo, "EXB", "CourierExpressToDoor")?.Where(m => m.Company == companyId).FirstOrDefault();
                            var CustomerRefNo = _baggageRepository.GetQuoteAmountByShippingType(quoteNo, "EXB", "CourierExpressToDoor")?.FirstOrDefault(m => m.Company == companyId);
                            //var CustomerRefNo = _dbRepositoryQuoteAmount.GetEntities().Where(m => m.QuoteId == quoteNo && m.MoveType == "EXB" && m.ShippingType == "CourierExpressToDoor" && m.Company == companyId).FirstOrDefault();
                            string file = "";
                            if (CustomerRefNo != null)
                            {
                                //file = CustomerRefNo.CustomerReferenceNo + "/" + CustomerRefNo.CustomerQuoteNo + "." + CustomerRefNo.QuoteSeqNo;
                                file = System.IO.Path.Combine(Convert.ToString(CustomerRefNo.CustomerReferenceNo), CustomerRefNo.CustomerQuoteNo + "." + CustomerRefNo.QuoteSeqNo);
                            }
                            string CourierExpress = await XMLHelper.GenerateBaggageXml(xmlResult, cartonObjList, model.Price.Value, xmlColDelResult, file, "CourierExpress", null, companyId);
                            //Task C = new Task(() => XMLHelper.XmlAPICall(CourierExpress, 0));
                            //C.Start();
                            sp_GetdataForEmailSending_Result xmlEmailResult = CustomRepository.GetQuoteData(quoteNo, 2, "CourierExpress");
                            xmlEmailResult.quoteName = (companyId == 1) ? "baggage" : (companyId == 2 ? "shipping" : "sales");
                            xmlEmailResult.ReferenceNo = file;
                            Task task = new Task(() => emailStatus = XMLHelper.SendEmail(companyId, xmlEmailResult, (companyId == 1) ? 3 : (companyId == 2) ? 1012 : 1024, CourierExpress, file));
                            Task taskquote = new Task(() => emailStatus = XMLHelper.SendEmail(companyId, xmlEmailResult, companyId == 1 ? 4 : (companyId == 2) ? 1011 : 1023, "", ""));
                            task.Start();
                            taskquote.Start();
                        }
                    }
                }
                if (GenId > 0)
                {
                    message = "Save changes successfully";
                }

                return message;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
        SP_GetXmlData_Result xmlResult;

        public JsonResult CheckValidPostCode(string postCode)
        {
            int? branchId = GetBranchId(postCode);
            if (branchId.HasValue)
                return Json(branchId, JsonRequestBehavior.AllowGet);

            else
                return Json(false, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ShowQuoteDetails()
        {
            BaggageModel obj = new BaggageModel();

            try
            {
                int volume = 0;
                double cubicFeet = 0;
                // List<tbl_BaggageItem> cartonObjList1 = _dbRepositoryMoveBaggage.GetEntities().Where(m => m.QuoteId == SessionHelper.BaggageQuoteId).ToList();
                List<BaggageItemModel> cartonObjList = _baggageRepository.GetBaggageItemByQuoteId(SessionHelper.BaggageQuoteId);

                List<Movebaggage> moveObjList = new List<Movebaggage>();
                var baggageItemList = cartonObjList.Where(m => m.CartonId == 0).ToList();

                for (int i = 0; i < baggageItemList.Count(); i++)
                {
                    volume += baggageItemList[i].UserVolume ?? 0;
                    cubicFeet += (baggageItemList[i].Volume * baggageItemList[i].Quantity);
                }

                List<CartonsModel> cartonList = new List<CartonsModel>();
                var cartonItemList = cartonObjList.Where(m => m.CartonId != 0).ToList();
                for (int i = 0; i < cartonItemList.Count(); i++)
                {
                    volume += cartonItemList[i].UserVolume ?? 0;
                    cubicFeet += (cartonItemList[i].Volume * cartonItemList[i].Quantity);
                }

                obj.calculationLines = new List<BaggageCalculationLineModel>();
                //var QuoteId = new SqlParameter
                //{
                //    ParameterName = "QuoteId",
                //    DbType = DbType.Int64,
                //    Value = SessionHelper.BaggageQuoteId
                //};
                quotesEntities entityObj = new quotesEntities();
                // BaggageCalculationModel baggageObj = entityObj.Database.SqlQuery<BaggageCalculationModel>("SP_BaggegeCalculation @QuoteId", QuoteId).FirstOrDefault();
                BaggageCalculationModel baggageObj = _baggageRepository.BaggageCalculation(SessionHelper.BaggageQuoteId);

                if (baggageObj.AirFreightToAirport > 0)
                {
                    //  decimal VolumetricsWeight1 = CustomRepository.GetVolumetricsWeight(SessionHelper.BaggageQuoteId, "AIR");
                    decimal VolumetricsWeight = _baggageRepository.GetVolumetricsWeight(SessionHelper.BaggageQuoteId, "AIR", SessionHelper.CompanyId);
                    string Desc = VolumetricsWeight.ToString() + " Vol kgs" + (volume > 0 ? ("/" + volume + " Kgs gross") : "");
                    obj.calculationLines.Add(GetBaggageCalcLine("Air Freight To Airport", baggageObj.AirFreightToAirport.Value, Desc));
                }
                if (baggageObj.AirFreightToDoor > 0)
                {
                    //  decimal VolumetricsWeight = CustomRepository.GetVolumetricsWeight(SessionHelper.BaggageQuoteId, "AIR");
                    decimal VolumetricsWeight = _baggageRepository.GetVolumetricsWeight(SessionHelper.BaggageQuoteId, "AIR", SessionHelper.CompanyId);
                    string Desc = VolumetricsWeight.ToString() + " Vol kgs" + (volume > 0 ? ("/" + volume + " Kgs gross") : "");
                    obj.calculationLines.Add(GetBaggageCalcLine("Air Freight To Door", baggageObj.AirFreightToDoor.Value, Desc));
                }
                if (baggageObj.Courier > 0)
                {
                    //decimal VolumetricsWeight = CustomRepository.GetVolumetricsWeight(SessionHelper.BaggageQuoteId, "COURIER");
                    decimal VolumetricsWeight = _baggageRepository.GetVolumetricsWeight(SessionHelper.BaggageQuoteId, "COURIER", SessionHelper.CompanyId);
                    string Desc = VolumetricsWeight.ToString() + " Vol kgs" + (volume > 0 ? ("/" + volume + " Kgs gross") : "");
                    obj.calculationLines.Add(GetBaggageCalcLine("Courier", baggageObj.Courier.Value, Desc));
                }
                if (baggageObj.SeaFreight > 0)
                {
                    string Desc = string.Concat(cubicFeet, " cubic feet");
                    obj.calculationLines.Add(GetBaggageCalcLine("Sea Freight", baggageObj.SeaFreight.Value, Desc));
                }
                if (baggageObj.RoadFreightToDoor > 0)
                {
                    string Desc = string.Concat(cubicFeet, " cubic feet");
                    obj.calculationLines.Add(GetBaggageCalcLine("Road Freight To Door", baggageObj.RoadFreightToDoor.Value, Desc));
                }
                if (baggageObj.CourierExpressToDoor > 0)
                {
                    //decimal VolumetricsWeight = CustomRepository.GetVolumetricsWeight(SessionHelper.BaggageQuoteId, "COURIEREXPRESS");
                    decimal VolumetricsWeight = _baggageRepository.GetVolumetricsWeight(SessionHelper.BaggageQuoteId, "COURIEREXPRESS", SessionHelper.CompanyId);
                    string Desc = VolumetricsWeight.ToString() + " Vol kgs" + (volume > 0 ? ("/" + volume + " Kgs gross") : "");
                    obj.calculationLines.Add(GetBaggageCalcLine("Courier Express To Door", baggageObj.CourierExpressToDoor.Value, Desc));
                }

                obj.calculationLines = obj.calculationLines.OrderBy(m => m.Amount).ToList();
                //SP_GetCollectionDelivery_Result xmlColDelResult = CustomRepository.GetCollectionDeliveryData(SessionHelper.BaggageQuoteId, SessionHelper.CompanyId);
                CollectionDelivery xmlColDelResult = _baggageRepository.GetCollectionDeliveryData(SessionHelper.BaggageQuoteId, SessionHelper.CompanyId);
                //var shipping1 = _dbRepositoryBaggageQuote.GetEntities().Where(m => m.Id == SessionHelper.BaggageQuoteId).FirstOrDefault();
                var shipping = _baggageRepository.GetBaggageQuoteById(SessionHelper.BaggageQuoteId);
                ViewBag.FromCountry = shipping.FromCountry;
                ViewBag.FromCity = string.IsNullOrEmpty(shipping.FromCity) ? shipping.PostCode : shipping.FromCity;

                ViewBag.ToCountry = shipping.ToCountry;
                ViewBag.ToCity = string.IsNullOrEmpty(shipping.CityName) ? shipping.ToPostCode : shipping.CityName;

                ViewBag.DeliveryCharge = xmlColDelResult.DeliveryCharge;
                ViewBag.CollectionCharge = xmlColDelResult.CollectionCharge;

                if (cartonObjList != null && cartonObjList.Count > 0)
                    ViewBag.HasMainCarton = cartonObjList.Count(x => string.Compare(x.Type, "MAIN", true) == 0) > 0;
            }
            catch (Exception ex)
            {
                TempData[CustomEnums.NotifyType.Error.GetDescription()] = CommonHelper.GetErrorMessage(ex);
            }

            return PartialView("_ShowBaggageQuoteDetails", obj);
        }

        private BaggageCalculationLineModel GetBaggageCalcLine(string methodName, decimal amount, string desc)
        {
            string DeliveryMethodKey = methodName.Replace(" ", "");
            var timeLine = _baggageRepository.GetTransitionTimeLine(DeliveryMethodKey);
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

        public ActionResult Destroy(int? Id)
        {
            string message = string.Empty;

            try
            {
                var result = _baggageRepository.DeleteBaggage(Id);

                //tbl_baggageQuote model = _dbRepository.SelectById(Id);
                //model.IsDelete = true;
                //message = _dbRepository.Update(model);
            }
            catch (Exception ex)
            {
                message = CommonHelper.GetErrorMessage(ex);
            }
            return Json(new { Message = "Quote Deleted.", IsError = Convert.ToString((int)CustomEnums.NotifyType.Error) }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ShowPrevioueNextQuote(int Id, string isPrevOrNext)
        {
            try
            {
                var QuoteId = new SqlParameter
                {
                    ParameterName = "QuoteId",
                    DbType = DbType.Int64,
                    Value = Id
                };
                quotesEntities entityObj = new quotesEntities();
                // var quoteamountList1 = entityObj.Database.SqlQuery<int>("SP_GetQuoteIdList @QuoteId", QuoteId).ToList();
                var quoteamountList = _baggageRepository.GetQuoteIdList(Id);
                var currentIndex = quoteamountList.FindIndex(x => x.QuoteId == Id);

                if (isPrevOrNext == "Next")
                {
                    if (quoteamountList.Count() - 1 > currentIndex)
                        Id = quoteamountList[currentIndex - 1].QuoteId;
                }
                if (isPrevOrNext == "Previous")
                {
                    if (quoteamountList.Count() - 1 >= currentIndex)
                    {
                        if (currentIndex != 0)
                            Id = quoteamountList[currentIndex - 1].QuoteId;
                    }
                }
            }
            catch (Exception ex)
            {
                TempData[CustomEnums.NotifyType.Error.GetDescription()] = CommonHelper.GetErrorMessage(ex);
            }
            return RedirectToAction("AddEdit", new { @Id = Id });
        }

        public ActionResult ViewMyQuote(int Id, string email)
        {
            QuoteInformationModel myquote = new QuoteInformationModel();
            List<BaggageQuoteInfoModel> bqi = new List<BaggageQuoteInfoModel>();

            try
            {
                //SessionHelper.BaggageQuoteId = Id;
                //var firstDate = DateTime.Today.AddMonths(-6);

                //var baggageList = _dbRepositoryBaggageQuote.GetEntities().Where(m => m.Email == email && m.IsDelete != true && m.CreatedDate >= firstDate && m.IsInquiry != true && m.Company == SessionHelper.CompanyId).OrderByDescending(m => m.CreatedDate).ToList();

                //List<BaggageQuoteInfoModel> baggageQuoteList = new List<BaggageQuoteInfoModel>();
                //int volume = 0;

                //for (int i = 0; i < baggageList.Count(); i++)
                //{
                //    if (baggageList[i].AirFreightToAirportFinal > 0 || baggageList[i].AirFreightToDoorFinal > 0 || baggageList[i].SeaFreightFinal > 0 || baggageList[i].CourierFinal > 0 || baggageList[i].RoadFreightToDoorFinal > 0 || baggageList[i].CourierExpressToDoorFinal > 0)
                //    {
                //        BaggageQuoteInfoModel baggageObj = new BaggageQuoteInfoModel();

                //        baggageObj.Id = baggageList[i].Id;
                //        baggageObj.PostCode = baggageList[i].PostCode;
                //        baggageObj.FromCity = baggageList[i].FromCity;
                //        baggageObj.FromCountry = baggageList[i].FromCountry;
                //        baggageObj.ToPostCode = baggageList[i].ToPostCode;
                //        baggageObj.ToCountry = baggageList[i].ToCountry;
                //        baggageObj.CityName = baggageList[i].CityName;
                //        baggageObj.CreatedDate = baggageList[i].CreatedDate;
                //        baggageObj.AirFreightToAirport = baggageList[i].AirFreightToAirport == true ? true : false;
                //        baggageObj.AirFreightToDoor = baggageList[i].AirFreightToDoor == true ? true : false;
                //        baggageObj.Courier = baggageList[i].Courier == true ? true : false;
                //        baggageObj.SeaFreight = baggageList[i].SeaFreight == true ? true : false;
                //        baggageObj.RoadFreightToDoor = baggageList[i].RoadFreightToDoor == true ? true : false;
                //        baggageObj.CourierExpressToDoor = baggageList[i].CourierExpressToDoor == true ? true : false;
                //        baggageObj.InternalNotes = string.IsNullOrEmpty(baggageList[i].InternalNotes) ? null : baggageList[i].InternalNotes;
                //        baggageObj.isMethodSelected = false;
                //        if (baggageObj.AirFreightToAirport == true || baggageObj.AirFreightToDoor == true || baggageObj.Courier == true || baggageObj.SeaFreight == true || baggageObj.RoadFreightToDoor == true || baggageObj.CourierExpressToDoor == true)
                //        {
                //            baggageObj.isMethodSelected = true;
                //        }

                //        ViewBag.FromCity = string.IsNullOrEmpty(baggageObj.FromCity) ? baggageObj.PostCode : baggageObj.FromCity;
                //        ViewBag.ToCity = string.IsNullOrEmpty(baggageObj.CityName) ? baggageObj.ToPostCode : baggageObj.CityName;

                //        QuoteCalculator.Service.Models.SP_GetCollectionDelivery_Result collectionDeliveryResult = _commonRepository.GetCollectionDelivery(baggageList[i].Id, SessionHelper.CompanyId); //CustomRepository.GetCollectionDeliveryData(baggageList[i].Id);

                //        baggageObj.DeliveryCharge = Math.Round(Convert.ToDecimal(collectionDeliveryResult.DeliveryCharge), 2, MidpointRounding.ToEven);
                //        baggageObj.CollectionCharge = Math.Round(Convert.ToDecimal(collectionDeliveryResult.CollectionCharge), 2, MidpointRounding.ToEven);


                //        long id = Convert.ToInt64(baggageList[i].Id);

                //        var cartonObjList = _dbRepositoryMoveBaggage.GetEntities().Where(m => m.QuoteId == id).ToList();

                //        foreach (var cartonItem in cartonObjList)
                //        {
                //            string FullSizeCartonStr = "";
                //            FullSizeCartonStr = Convert.ToString(cartonItem.Quantity + " X " + cartonItem.Description + " (" + cartonItem.Volume + " cubic feet" + (cartonItem.Quantity > 1 ? " each)" : ")"));
                //            if (cartonItem.Length > 0 && cartonItem.Breadth > 0 && cartonItem.Height > 0)
                //            {
                //                FullSizeCartonStr += Convert.ToString(", " + cartonItem.Length + " X " + cartonItem.Breadth + " X " + cartonItem.Height + " Cms");
                //            }
                //            if (cartonItem.UserVolume > 0)
                //            {
                //                FullSizeCartonStr += Convert.ToString(", " + cartonItem.UserVolume + " Kgs");
                //            }
                //            baggageObj.CartonList += FullSizeCartonStr + System.Environment.NewLine;
                //        }
                //        if (cartonObjList != null && cartonObjList.Count > 0)
                //            baggageObj.HasMainCartons = cartonObjList.Count(x => string.Compare(x.Type, "MAIN", true) == 0) > 0;

                //        baggageObj.BaggagePriceList = new List<BaggageCalculationLineModel>();

                //        quotesEntities entityObj = new quotesEntities();
                //        var QuoteId = new SqlParameter
                //        {
                //            ParameterName = "QuoteId",
                //            DbType = DbType.Int64,
                //            Value = baggageList[i].Id
                //        };

                //        int quoteId = baggageList[i].Id;
                //        var tblQuoteAmounts = _dbRepositoryQuoteAmount.GetEntities().Where(x => x.QuoteId == quoteId && x.MoveType == "EXB" && (x.QuoteAmount ?? 0) > 0).ToList();

                //        if (tblQuoteAmounts.Count() == 0)
                //            continue;
                //        double cubicFeet = 0;
                //        volume = 0;
                //        for (int j = 0; j < cartonObjList.Count(); j++)
                //        {
                //            volume += cartonObjList[j].UserVolume ?? 0;
                //            cubicFeet += (cartonObjList[j].Volume * cartonObjList[j].Quantity);
                //        }

                //        foreach (var tblQuoteAmount in tblQuoteAmounts)
                //        {
                //            string Desc = string.Empty;
                //            if (tblQuoteAmount.ShippingTypeDescription != "Sea Freight" && tblQuoteAmount.ShippingTypeDescription != "Road freight To Door")
                //            {
                //                string deliveryType = tblQuoteAmount.ShippingTypeDescription == "Courier" ? "COURIER" : (tblQuoteAmount.ShippingTypeDescription == "Courier Express To Door" ? "COURIEREXPRESS" : "AIR");
                //                decimal VolumetricsWeight = CustomRepository.GetVolumetricsWeight(baggageList[i].Id, deliveryType);
                //                Desc = VolumetricsWeight.ToString() + " Vol kgs" + (volume > 0 ? ("/" + volume + " Kgs gross") : "");
                //            }
                //            else
                //            {
                //                Desc = string.Concat(cubicFeet, " cubic feet");
                //            }

                //            baggageObj.BaggagePriceList.Add(new BaggageCalculationLineModel()
                //            {
                //                DeliveryMethodName = tblQuoteAmount.ShippingTypeDescription == "Courier" ? "Courier Economy To Door" : (tblQuoteAmount.ShippingTypeDescription == "Sea Freight" ? "Sea Freight To Door" : tblQuoteAmount.ShippingTypeDescription),
                //                Amount = tblQuoteAmount.QuoteAmount ?? 0,
                //                TransitionTime = tblQuoteAmount.TransitionTime,
                //                CalcDescription = Desc
                //            });
                //            baggageObj.CustomerRefNo = string.Concat(tblQuoteAmount.CustomerReferenceNo + "/" + tblQuoteAmount.CustomerQuoteNo);
                //        }
                //        baggageObj.BaggagePriceList = baggageObj.BaggagePriceList.OrderBy(m => m.Amount).ToList();

                //        if (baggageQuoteList.Count(x => x.CustomerRefNo == baggageObj.CustomerRefNo) == 0)
                //            baggageQuoteList.Add(baggageObj);
                //    }
                //}

                bqi = _baggageRepository.ViewMyQuote(Id, email, SessionHelper.CompanyId);

                myquote.bagageQuoteInfo = bqi;
            }
            catch (Exception ex)
            {
                TempData[CustomEnums.NotifyType.Error.GetDescription()] = CommonHelper.GetErrorMessage(ex);
            }
            return PartialView("QuoteInfo", myquote);
        }
        public ActionResult GetTermsAndConditions(int quoteId, string FromCountryCode, string ToCountryCode)
        {
            SessionHelper.BaggageQuoteId = quoteId;
            SessionHelper.QuoteType = "2";
            SessionHelper.FromCountryCode = FromCountryCode;
            SessionHelper.ToCountryCode = ToCountryCode;
            return RedirectToAction("Baggage", new { @baggageId = CommonHelper.Encode(quoteId.ToString()) });
        }

        public int? GetBranchId(string postCode)
        {
            try
            {
                if (string.IsNullOrEmpty(postCode))
                    return null;

                var code = postCode;
                ViewBag.BranchPostCode = 0;
                //string pc = postCode.Replace(" ", "");

                //var postCodeObj = _dbRepositoryUKPostCode.GetEntities().Where(m => m.zip == pc).FirstOrDefault();
                var postCodeObj = _baggageRepository.GetListFromUk(postCode).FirstOrDefault();

                // var postCodeObj = _dbRepositoryUKPostCode.GetEntities().Where(m => m.zip.Replace(" ", "") == postCode.Replace(" ", "")).FirstOrDefault();
                if (postCodeObj != null)
                {
                    // postCode = postCode.Replace(" ", "");
                    int postCodeLength = postCode.Length;
                    var zipCode = postCode.Substring(0, postCodeLength - 3);
                    for (int i = 4; i >= 2; i--)
                    {
                        var splitedCode = postCode.Substring(0, i);
                        //var branchPostCode = _dbRepositoryUKBranchPostCode.GetEntities().Where(m => m.postcode == splitedCode).FirstOrDefault();
                        var branchPostCode = _baggageRepository.GetListFromUKBranchPostCode(splitedCode).FirstOrDefault();
                        if (branchPostCode != null)
                        {
                            if (branchPostCode.vehicle_branch_id != 0)
                            {
                                ViewBag.BranchPostCode = branchPostCode.vehicle_branch_id;
                                return (int)branchPostCode.baggage_branch_id;
                            }
                        }
                    }
                }
                return null;
            }

            catch (Exception ex)
            {
                TempData[CustomEnums.NotifyType.Error.GetDescription()] = CommonHelper.GetErrorMessage(ex);
                return null;
            }
        }

        [HttpPost]
        public ActionResult UpdateBookStatus(int quoteId, string referenceNo, string deliveryMethod)
        {
            try
            {
                //  tbl_baggageQuote baggageQuoteData1 = _dbRepositoryBaggageQuote.GetEntities().Where(m => m.ReferenceNumber == referenceNo && m.Id == quoteId).FirstOrDefault();
                BaggageQuoteInfoModel baggageQuoteData = _baggageRepository.checkDuplicateQuoteRef(quoteId, referenceNo).FirstOrDefault(); ;
                if (baggageQuoteData != null)
                {
                    if (deliveryMethod == "Courier")
                    {
                        baggageQuoteData.Courier = false;
                    }
                    else if (deliveryMethod == "AirFreightToDoor")
                    {
                        baggageQuoteData.AirFreightToDoor = false;
                    }
                    else if (deliveryMethod == "AirFreightToAirport")
                    {
                        baggageQuoteData.AirFreightToAirport = false;
                    }
                    else if (deliveryMethod == "SeaFreight")
                    {
                        baggageQuoteData.SeaFreight = false;
                    }
                    else if (deliveryMethod == "RoadFreightToDoor")
                    {
                        baggageQuoteData.RoadFreightToDoor = false;
                    }
                    else if (deliveryMethod == "CourierExpressToDoor")
                    {
                        baggageQuoteData.CourierExpressToDoor = false;
                    }

                    //_dbRepositoryBaggageQuote.Update(baggageQuoteData);
                    _baggageRepository.UpdateBaggageQuote(baggageQuoteData);
                }

                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(CommonHelper.GetErrorMessage(ex));
            }
        }

        #endregion
    }
}