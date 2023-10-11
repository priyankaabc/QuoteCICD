using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuoteCalculatorAdmin.Common;
using System.Data.SqlClient;
using System.Data;
using Kendo.Mvc.Extensions;
using System.Web.Script.Serialization;
using QuoteCalculatorAdmin.Helper;
using System.Threading.Tasks;
using Kendo.Mvc;
using QuoteCalculator.Service.Models;
using QuoteCalculator.Service.Repository.TradeQuote;
using QuoteCalculatorAdmin.Data;
using QuoteCalculator.Service.Repository.CommonRepository;

namespace QuoteCalculatorAdmin.Controllers
{
    public class TradeQuoteController : BaseController
    {
        private readonly ITradeQuoteRepository _TradeQuoteRepository;
        private readonly ICommonRepository _commonRepository;

        public TradeQuoteController(ITradeQuoteRepository TradeQuoteRepository, ICommonRepository commonRepository)
        {
            _TradeQuoteRepository = TradeQuoteRepository;
            _commonRepository = commonRepository;

        }
        // GET: TradeQuote
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetAgentList()
        {
            CommonController CommonObj = new CommonController(_commonRepository);
            var list = CommonObj.GetAgentList(true);
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult GetTradeQuoteList(DatatableModel model)
        {
            try
            {
                DataTablePaginationModel DtSearchModel = CommonController.GetDataTablePaginationModel(model);
                List<TradeQuoteModel> dataList = _TradeQuoteRepository.GetTradeQuoteList(DtSearchModel, SessionHelper.CompanyId);

                DatatableResponseModel<TradeQuoteModel> response = new DatatableResponseModel<TradeQuoteModel>()
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

            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }
  
        [HttpGet]
        public ActionResult AddEdit(int? Id)
         {
            CommonController CommonObj = new CommonController(_commonRepository);
            TradeQuoteModel model = new TradeQuoteModel();
            ViewBag.AgentList = CommonObj.GetAgentList(true);
            ViewBag.ServiceList = CommonObj.GetServiceListCommon(true);
            ViewBag.CountryList = CommonObj.GetCountryListCommon();
            ViewBag.BranchList=CommonObj.GetBranchListCommon();
            if (Id > 0)
            {
                model= _TradeQuoteRepository.GetTradeQuoteById(Id);
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult AddEdit(TradeQuoteModel model)
        {
            try
            {
                model.SalesRep = SessionHelper.SalesRepCode;
                model.Company = SessionHelper.CompanyId;
                model.UserId = SessionHelper.UserId;
                int QuoteId = _TradeQuoteRepository.UpdateTradeQuote(model);
                TempData[CustomEnums.NotifyType.Success.GetDescription()] = model.Id > 0 ? "Trade Quote Updated Successfully" : "Trade Quote Added Successfully";
                if (QuoteId > 0)
                {
                    //var paramTradeQuoteId = new SqlParameter
                    //{
                    //    ParameterName = "TradeQuoteId",
                    //    DbType = DbType.Int64,
                    //    Value = (object)QuoteId ?? DBNull.Value
                    //};
                    //entityObj.Database.ExecuteSqlCommand("SP_TradesCalculateRate @TradeQuoteId", paramTradeQuoteId);

                    _TradeQuoteRepository.TradesCalculateRate(QuoteId);
                }                
                return RedirectToAction("TradeQuote", "TradeQuote", new { @Id = QuoteId, @QuoteId = model.Id });
            }
            catch (Exception ex)
            {
                TempData[CustomEnums.NotifyType.Error.GetDescription()] = ex.Message;
                return RedirectToAction("Index", "TradeQuote");
            }
        }

        public ActionResult GetServiceList()
        {
            try
            {
                CommonController CommonObj = new CommonController(_commonRepository);
                var list = CommonObj.GetServiceListCommon(true);
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(CommonHelper.GetErrorMessage(ex));
            }
        }
        public ActionResult GetBranchById(int Id)
        {
            try
            {
                CommonController CommonObj = new CommonController(_commonRepository);
                IEnumerable<CommonModel> list = CommonObj.GetBranchByAgentId(Id);
                return Json(list, JsonRequestBehavior.AllowGet);

                
            }
            catch (Exception ex)
            {
                return Json(CommonHelper.GetErrorMessage(ex));
            }
        }
        public ActionResult GetDestCodeByCountryId(string countryCode)
        {
            try
            {
              
                List<TradeQuoteModel> dataList = _TradeQuoteRepository.GetDestCodeByCountryId(countryCode);
                return Json(dataList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(CommonHelper.GetErrorMessage(ex));
            }
        }

        [HttpGet]
        public ActionResult TradeQuote(int? Id, long? QuoteId)
        {
           TradeQuoteModel model = new TradeQuoteModel();
            
            try
            {

                if (Id > 0)
                {
                    model = _TradeQuoteRepository.GetTradeQuoteById(Id);
                    ViewBag.OldQuoteId = QuoteId;
                    //model.Type = model.AgentId == 0 ? "Customer" : "Agent";
                }

            }
            catch (Exception ex)
            {
                TempData[CustomEnums.NotifyType.Error.GetDescription()] = CommonHelper.GetErrorMessage(ex);
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult AddEditTradeQuote(TradeQuoteModel model)
        {
            try
            {
                quotesEntities entityObj = new quotesEntities();
                //int QuoteId = entityObj.Database.SqlQuery<int>("SP_InsertUpdateImportsQuotes @Id", Id).FirstOrDefault();
                TempData[CustomEnums.NotifyType.Success.GetDescription()] = model.Id > 0 ? "Trade Quote Added Successfully" : "Import Quote Updated Successfully";
                return RedirectToAction("Index", "TradeQuote");
            }
            catch (Exception ex)
            {
                TempData[CustomEnums.NotifyType.Error.GetDescription()] = ex.Message;
                return RedirectToAction("Index", "TradeQuote");
            }
        }

       
        public ActionResult GetAdditionalCost(long TraderQuoteId)
        {
            try
            {
                List<AddtionalCostModel> dataList = _TradeQuoteRepository.GetAdditionalCost(TraderQuoteId);
                DataSourceResult obj = new DataSourceResult();
                //obj.Total = (dataList != null && dataList.Count > 0) ? dataList[0].TotalCount : 0;
                obj.Data = dataList;
                return Json(obj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(CommonHelper.GetErrorMessage(ex));
            }
        }
        [HttpPost]
        public async Task<ActionResult> SaveTradeQuoteAddCost(string strData, decimal TotalPrice, decimal AdjustedPrice, decimal Profit, decimal AdjustedProfit, long TradeQuoteId)
        {
            try
            {
                var serializer = new JavaScriptSerializer();


                TradeQuoteAddCost tradeQuoteAddCost = new TradeQuoteAddCost();
                tradeQuoteAddCost.AddCostList= serializer.Deserialize<List<AddtionalCostModel>>(strData);
                tradeQuoteAddCost.CompanyId = SessionHelper.CompanyId;
                tradeQuoteAddCost.UserId = SessionHelper.UserId;
                tradeQuoteAddCost.TradeQuoteId = TradeQuoteId;
                tradeQuoteAddCost.TotalPrice = TotalPrice;
                tradeQuoteAddCost.AdjustedPrice = AdjustedPrice;
                tradeQuoteAddCost.Profit = Profit;
                tradeQuoteAddCost.AdjustedProfit = AdjustedProfit;
                DataTable dtCost = new DataTable("AdditionalCost");
                dtCost.Columns.Add("Type");
                dtCost.Columns.Add("Cost");

               
                //quotesEntities entityObj = new quotesEntities();
                int AddCostId = _TradeQuoteRepository.InsertAdditionalCost(tradeQuoteAddCost);
                //int AddCostId = entityObj.Database.SqlQuery<int>("SP_InsertAdditionalCost @CompanyId, @UserId, @TradeQuoteId, @ImportQuoteId, @TotalPrice, @AdjustedPrice, @Profit, @AdjustedProfit, @AdditionalCost",
                //    paraCompanyId, paraUserId, paraTradeQuoteId, paraImportQuoteId, paraTotalPrice, paraAdjustedPrice, paraProfit, paraAdjustedProfit, paramAdditionalCost).FirstOrDefault();

                //var paraTradeQuoteId1 = new SqlParameter
                //{
                //    ParameterName = "TradeQuoteId",
                //    DbType = DbType.Int64,
                //    Value = (object)TradeQuoteId ?? DBNull.Value
                //};
                TradeQuoteModel _TradeQuoteModel = new TradeQuoteModel();
                //_TradeQuoteModel = entityObj.Database.SqlQuery<TradeQuoteModel>("SP_GetTradeQuoteXMLData @TradeQuoteId", paraTradeQuoteId1).FirstOrDefault();
                _TradeQuoteModel = _TradeQuoteRepository.GetTradeQuoteXMLData(TradeQuoteId);

                //var paraTradeQuoteId2 = new SqlParameter
                //{
                //    ParameterName = "TradeQuoteId",
                //    DbType = DbType.Int64,
                //    Value = (object)TradeQuoteId ?? DBNull.Value
                //};
                List<AddtionalCostModel> _AdditionalCostModel = null;
                //      _AdditionalCostModel = entityObj.Database.SqlQuery<AddtionalCostModel>("SP_GetTradeCostData @TradeQuoteId", paraTradeQuoteId2).ToList();
                _AdditionalCostModel = _TradeQuoteRepository.GetTradeCostData(TradeQuoteId);
                string TradeQuote = await XMLHelper.GenerateTradeQuoteXml(_TradeQuoteModel, _AdditionalCostModel);
                return Json(new {Message = "Addtional cost saved successfully", IsSuccess = AddCostId > 0 ? true : false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.InnerException, IsSuccess = false }, JsonRequestBehavior.AllowGet);

            }
        }

        public ActionResult ThankYou(string ReferenceNo, string TotalPrice, string AdjustedPrice)
        {
            ViewBag.IsTradeQuote = true;
            ViewBag.TotalPrice = TotalPrice;
            ViewBag.AdjustedPrice = AdjustedPrice;
            ViewBag.ReferenceNo = ReferenceNo;
            return View("~/Views/ImportsQuote/ThankYou.cshtml");

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
    }
}