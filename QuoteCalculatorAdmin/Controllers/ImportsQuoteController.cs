using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuoteCalculatorAdmin.Data;

using QuoteCalculatorAdmin.Common;
using System.Data.SqlClient;
using System.Data;
using Kendo.Mvc.Extensions;
using System.Web.Script.Serialization;
using System.Threading.Tasks;
using QuoteCalculatorAdmin.Helper;
using Kendo.Mvc;
using QuoteCalculator.Service.Models;
using QuoteCalculator.Service.Repository.ImportQuote;
using QuoteCalculator.Service.Repository.CommonRepository;

namespace QuoteCalculatorAdmin.Controllers
{
    public class ImportsQuoteController : BaseController
    {
        private readonly IImportQuoteRepository _dbImportQuoteRepository;
        private readonly ICommonRepository _commonRepository;

        public ImportsQuoteController(IImportQuoteRepository dbImportQuoteRepository, ICommonRepository commonRepository)
        {
            _dbImportQuoteRepository = dbImportQuoteRepository;
            _commonRepository = commonRepository;
        }
        // GET: ImportsQuote
        public ActionResult Index()
        {
            return View();
        }
        //public ActionResult GetAgentList()
        //{
        //    try
        //    {
        //        quotesEntities entityObj = new quotesEntities();
        //        var list = entityObj.Database.SqlQuery<CommonDDLModel>("SP_GetImportAgentList").ToList();
        //        return Json(list, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(CommonHelper.GetErrorMessage(ex));
        //    }
        //}

       

        [HttpPost]
        public ActionResult GetImportsQuoteList(DatatableModel model)
        {
            try
            {
                DataTablePaginationModel DtSearchModel = CommonController.GetDataTablePaginationModel(model);
                List<ImportsQuoteModel> Result = _dbImportQuoteRepository.GetImportsQuoteList(DtSearchModel, SessionHelper.CompanyId);
                DatatableResponseModel<ImportsQuoteModel> response = new DatatableResponseModel<ImportsQuoteModel>()
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
        [HttpGet]
        public ActionResult AddEdit(int? Id, string op, bool ShowDestDetail = false)
        {
            try
            {
                CommonController CommonObj = new CommonController(_commonRepository);
                ViewBag.AgentList = CommonObj.GetAgentList(false);
                ViewBag.CountryList = CommonObj.GetCountryListImportQuoteCommon();
                ViewBag.POEList = CommonObj.GetPOEList();
                ViewBag.BranchList = CommonObj.GetBranchListCommon();
                ViewBag.ServiceList = CommonObj.GetServiceListCommon(false);
                ViewBag.ContainerSizeList = CommonObj.GetContainerSizeList();
                ImportsQuoteModel model = new ImportsQuoteModel
                {
                    Type = "Customer"
                };
                if (Id > 0)
                {
                    quotesEntities entityObj = new quotesEntities();
                    ImportsQuoteModel list = _dbImportQuoteRepository.GetImportsQuoteById(Id);
                    model = list;
                    model.Type = model.AgentId == 0 ? "Customer" : "Agent";
                    List<AddtionalCostModel> Additionallist = _dbImportQuoteRepository.GetAdditionalCost();
                    model.AdditinalCostList = Additionallist;
                    model.ShowDestDetail = ShowDestDetail;
                }
                return View(model);
            }
            catch (Exception ex)
            {
                return Json(CommonHelper.GetErrorMessage(ex));
            }

        }
        [HttpPost]
        public ActionResult AddEdit(ImportsQuoteModel model, bool? isEdit)
        {
            try
            {
                model.SalesRep = SessionHelper.SalesRepCode;
                model.Company = SessionHelper.CompanyId;
                model.UserId = SessionHelper.UserId;
                //quotesEntities entityObj = new quotesEntities();
                //long QuoteId = entityObj.Database.SqlQuery<long>("SP_InsertUpdateImportsQuotes @Id, @AgentId, @CustomerName, @OriginCountry, @OriginTown, @POEId, @ServiceId, @ContainerSizeId, @Company, @Branch, @SalesRep, @CreatedBy,@Operation,@ShowDestDetail,@Note", Id, AgentId, CustomerName, OriginCountry, OriginTown, POEId, ServiceId, ContainerSizeId, Company, Branch, SalesRep, CreatedBy, Operation, ShowDestDetail, Note).FirstOrDefault();
                long QuoteId = _dbImportQuoteRepository.UpdateImportQuote(model);
                TempData[CustomEnums.NotifyType.Success.GetDescription()] = model.Id > 0 ? "Import Quote Updated Successfully" : "Import Quote Added Successfully";
                if (QuoteId > 0)
                {
                    
                    var paramImportQuoteId = new SqlParameter
                    {
                        ParameterName = "ImportQuoteId",
                        DbType = DbType.Int64,
                        Value = (object)QuoteId ?? DBNull.Value
                    };
                    _dbImportQuoteRepository.ImportCalculateRate(QuoteId);
                    //entityObj.Database.ExecuteSqlCommand("SP_ImportCalculateRate @ImportQuoteId", paramImportQuoteId);
                }

                if (model.Id > 0 && model.ShowDestDetail)
                {
                    return RedirectToAction("ImportQuote", "ImportsQuote", new { @Id = QuoteId, @op = model.Operation });
                }
                else
                {
                    return RedirectToAction("AddEdit", "ImportsQuote", new { @Id = QuoteId, @op = model.Operation, @ShowDestDetail = true });
                }

            }
            catch (Exception ex)
            {
                TempData[CustomEnums.NotifyType.Error.GetDescription()] = ex.Message;
                return RedirectToAction("Index", "ImportsQuote");
            }
        }

        //public ActionResult GetPOEList()
        //{
        //    try
        //    {
        //        quotesEntities entityObj = new quotesEntities();
        //        var list = entityObj.Database.SqlQuery<CommonDDLModel>("SP_GetPOEList").ToList();
        //        return Json(list, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(CommonHelper.GetErrorMessage(ex));
        //    }

        //}

        //public ActionResult GetServiceList()
        //{
        //    try
        //    {
        //        bool IsTradeQuote = false;
        //        quotesEntities entityObj = new quotesEntities();
        //        var paramIsTrade = new SqlParameter
        //        {
        //            ParameterName = "IsTradeQuote",
        //            DbType = DbType.Boolean,
        //            Value = IsTradeQuote
        //        };
        //        var list = entityObj.Database.SqlQuery<CommonDDLModel>("SP_GetServiceList @IsTradeQuote", paramIsTrade).ToList();
        //        return Json(list, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(CommonHelper.GetErrorMessage(ex));
        //    }
        //}
        //public ActionResult GetVehicleList()
        //{
        //    try
        //    {
        //        quotesEntities entityObj = new quotesEntities();
        //        var list = entityObj.Database.SqlQuery<CommonDDLModel>("SP_GetVehicleList").ToList();
        //        return Json(list, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(CommonHelper.GetErrorMessage(ex));
        //    }
        //}
        //public ActionResult GetContainerSizeList()
        //{
        //    try
        //    {
        //        quotesEntities entityObj = new quotesEntities();
        //        var list = entityObj.Database.SqlQuery<CommonDDLModel>("SP_GetContainerSizeList").ToList();
        //        return Json(list, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(CommonHelper.GetErrorMessage(ex));
        //    }
        //}
        public ActionResult GetBranchById(int Id)
        {
            try
            {
                CommonModel list = _dbImportQuoteRepository.GetBranchById(Id);
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(CommonHelper.GetErrorMessage(ex));
            }
        }

        //public ActionResult GetBranchList()
        //{
        //    try
        //    {
        //        quotesEntities entityObj = new quotesEntities();
        //        var list = entityObj.Database.SqlQuery<CommonDDLModel>("SP_GetBranchList").ToList();
        //        return Json(list, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(CommonHelper.GetErrorMessage(ex));
        //    }
        //}

        [HttpGet]
        public ActionResult ImportQuote(int? Id, string op)
        {
            ImportsQuoteModel model = new ImportsQuoteModel();
            try
            {
                if (Id > 0)
                {
              
                    quotesEntities entityObj = new quotesEntities();
                    ImportsQuoteModel Detail = _dbImportQuoteRepository.GetImportsQuoteById(Id);
                    //ImportsQuoteModel Detail = entityObj.Database.SqlQuery<ImportsQuoteModel>("SP_GetImportsQuotesById @Id", ImportId).FirstOrDefault();
                    model = Detail;
                    model.Type = model.AgentId == 0 ? "Customer" : "Agent";
                    model.Rate = model.Rate == null ? 0 : model.Rate;
                    model.TotalCost = model.TotalCost == null ? 0 : model.TotalCost;
                }
            }
            catch (Exception ex)
            {
                TempData[CustomEnums.NotifyType.Error.GetDescription()] = ex.Message;
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult AddEditImportQuote(ImportsQuoteModel model)
        {
            try
            {
                quotesEntities entityObj = new quotesEntities();
                //int QuoteId = entityObj.Database.SqlQuery<int>("SP_InsertUpdateImportsQuotes @Id", Id).FirstOrDefault();
                TempData[CustomEnums.NotifyType.Success.GetDescription()] = model.Id > 0 ? "Import Quote Updated Successfully" : "Import Quote Added Successfully";
                return RedirectToAction("Index", "ImportsQuote");
            }
            catch (Exception ex)
            {
                TempData[CustomEnums.NotifyType.Error.GetDescription()] = ex.Message;
                return RedirectToAction("Index", "ImportsQuote");
            }
        }

        public ActionResult GetAdditionalCost([DataSourceRequest] DataSourceRequest request, long ImportQuoteId)
        {
            try
            {
                quotesEntities entityObj = new quotesEntities();
                var list = _dbImportQuoteRepository.GetAdditionalCost();
                return Json(list.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(CommonHelper.GetErrorMessage(ex));
            }
        }

        [HttpPost]
        public async Task<ActionResult> SaveImportQuoteAddCost(double TotalCost, double UpdatedCost, long ImportQuoteId)
        {
            try
            {
             
                quotesEntities entityObj = new quotesEntities();
                
                //int AddCostId = entityObj.Database.SqlQuery<int>("SP_InsertAdditionalCost @CompanyId, @UserId, @TradeQuoteId, @ImportQuoteId,@AdditionalCost", paraCompanyId, paraUserId, paraTradeQuoteId, paraImportQuoteId, paramAdditionalCost).FirstOrDefault();
                //int CostId = entityObj.Database.SqlQuery<int>("SP_InsertUpdateCost @TotalCost, @UpdatedCost, @ImportQuoteId", paraTotalCost, paraUpdatedCost, paraImportQuoteId).FirstOrDefault();
                int CostId = _dbImportQuoteRepository.InsertUpdateCost(ImportQuoteId,TotalCost, UpdatedCost);
                string Company = SessionHelper.CompanyId == 1 ? "AP" : SessionHelper.CompanyId == 2 ? "PF" : "EI";

                ImportsQuoteModel _ImportQuoteModel = new ImportsQuoteModel();
                quotesEntities entityObject = new quotesEntities();
                _ImportQuoteModel = _dbImportQuoteRepository.GetImportQuoteXMLData(ImportQuoteId);

                List<DestinationDetails> _ConsigneeModel = null;
                quotesEntities entityObject1 = new quotesEntities();
                var paraImportQuoteId2 = new SqlParameter
                {
                    ParameterName = "ImportQuoteId",
                    DbType = DbType.Int64,
                    Value = (object)ImportQuoteId ?? DBNull.Value
                };
                _ConsigneeModel = _dbImportQuoteRepository.GetImportConsigneeData(ImportQuoteId);

                List<AddtionalCostModel> _AdditionalCostModel = null;
               
                _AdditionalCostModel = _dbImportQuoteRepository.GetImportCostData(ImportQuoteId);
                string TradeQuote = await XMLHelper.GenerateImportQuoteXml(_ImportQuoteModel, _ConsigneeModel, _AdditionalCostModel);
                return Json(new { Message = "Updated cost saved successfully", IsSuccess = CostId > 0, ReferenceNo = _ImportQuoteModel != null ?_ImportQuoteModel.RefNo : string.Empty }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.InnerException, IsSuccess = false }, JsonRequestBehavior.AllowGet);

            }
        }

        public ActionResult ThankYou(string ReferenceNo)
        {
            if (!string.IsNullOrEmpty(ReferenceNo))
                ViewBag.ReferenceNo = ReferenceNo;

            return View();
        }

        public ActionResult GetDestinationDetails([DataSourceRequest] DataSourceRequest request, int ImportQuoteId)
        {
            try
            {
                var model = new DestinationDetails();
                var list = _dbImportQuoteRepository.GetImportsDestinationById(ImportQuoteId);
                DataSourceResult response = new DataSourceResult
                {
                    Total = list != null ? list.Count : 0,
                    Data = list
                };
                return Json(response, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(CommonHelper.GetErrorMessage(ex));
            }
        }

        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult DestinationDetailsUpdate([DataSourceRequest] DataSourceRequest request, DestinationDetails destinatinDetails, long ImportQuoteId, string StrvehicleId)
        
        [HttpPost]
        public ActionResult DestinationDetailsUpdate(DestinationDetails destinatinDetails, long ImportQuoteId, string StrvehicleId)
        {
            try
            {
                long destId = 0;
                //if (destinatinDetails != null && ModelState.IsValid)
                if (destinatinDetails != null)
                {

                    //quotesEntities entityObj = new quotesEntities();
                    //destId = entityObj.Database.SqlQuery<long>("SP_InsertUpdateImportsDestination @Id, @DestAddress1, @DestAddress2, @DestPostcode, @ConsigneeName, @Kgs, @VehicleId, @CollectFromBranch, @ImportQuoteId", id, DestAddress1, DestAddress2, DestPostcode, ConsigneeName, Kgs, VehicleId, CollectFromBranch, QuoteId).FirstOrDefault();
                    if (!string.IsNullOrEmpty(StrvehicleId))
                    {
                        destinatinDetails.VehicleId = Convert.ToInt32(StrvehicleId);
                    }
                    
                    destId = _dbImportQuoteRepository.InsertUpdateImportsDestination(destinatinDetails, ImportQuoteId);
                }

                //return Json(new[] { destinatinDetails }.ToDataSourceResult(request, ModelState));
                return Json(new { Success = destId > 0, Message = destId > 0 ? "Assignee Saved Successfully" : "Something Went Wrong!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(CommonHelper.GetErrorMessage(ex));
            }
        }

        public ActionResult AddtionalCostView(AddtionalCostModel objModel)
        {
            //AddtionalCostModel objModel = new AddtionalCostModel();
            return PartialView("AdditionalCostView", objModel);
        }

        public ActionResult GetAdditionalCostByDestId(string DestId)
        {
            try
            {
                List<AddtionalCostModel> list = _dbImportQuoteRepository.GetAdditionalCostByDestId(Convert.ToInt64(DestId));
                DataSourceResult obj = new DataSourceResult
                {
                    Total = list != null ? list.Count : 0,
                    Data = list
                };
                return Json(obj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(CommonHelper.GetErrorMessage(ex));
            }
        }

        [HttpPost]
        public ActionResult SaveQuoteAddCost(string strData, long DestinationId)
        {
            try
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                ImportsQuoteAddCost Importmodel = new ImportsQuoteAddCost();
                Importmodel.AddCostList = serializer.Deserialize<List<AddtionalCostModel>>(strData);
                Importmodel.DestinationId = DestinationId;

                DataTable dtCost = new DataTable("AdditionalCost");
                dtCost.Columns.Add("Type");
                dtCost.Columns.Add("Cost");

                //foreach (ImportsQuoteAddCost model in AddCostList)
                //{
                //    DataRow dtrow = dtCost.NewRow();
                //    dtrow["Type"] = model.Type;
                //    dtrow["Cost"] = model.Cost;
                //    dtCost.Rows.Add(dtrow);
                //}

                //var paraCompanyId = new SqlParameter
                //{
                //    ParameterName = "CompanyId",
                //    DbType = DbType.Int32,
                //    Value = (object)SessionHelper.CompanyId ?? DBNull.Value
                //};
                //var paraDestinationId = new SqlParameter
                //{
                //    ParameterName = "DestinationId",
                //    DbType = DbType.Int64,
                //    Value = (object)DestinationId ?? DBNull.Value
                //};

                //var paramAdditionalCost = new SqlParameter
                //{
                //    ParameterName = "AdditionalCost",
                //    SqlDbType = SqlDbType.Structured,
                //    Value = dtCost,
                //    TypeName = "dbo.AdditionalCost"
                //};
                int AddCostId = _dbImportQuoteRepository.InsertAdditionalCost(Importmodel);
                //quotesEntities entityObj = new quotesEntities();
                //int AddCostId = entityObj.Database.SqlQuery<int>("SP_InsertAdditionalCostForDestination @CompanyId, @DestinationId,@AdditionalCost", paraCompanyId, paraDestinationId, paramAdditionalCost).FirstOrDefault();
                return Json(new { Message = "Addtional cost saved successfully", IsSuccess = AddCostId > 0 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.InnerException, IsSuccess = false }, JsonRequestBehavior.AllowGet);

            }
        }

        [AcceptVerbs("Post")]
        public ActionResult DestinationDetailsDelete(DestinationDetails model)
        {
            try
            {
                if (model != null)
                {
                    int AddCostId = _dbImportQuoteRepository.DeleteDestinationDetails(SessionHelper.CompanyId, model.Id);
                    return Json(new { Success = AddCostId > 0, Message = AddCostId > 0 ? "Assignee Deleted Successfully" : "Something Went Wrong!" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(CommonHelper.GetErrorMessage(ex));
            }
            return Json(new { Success = false, Message = "Something Went Wrong!" }, JsonRequestBehavior.AllowGet);
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

        [HttpPost]
        public ActionResult GetAssigneeDetails(DestinationDetails model)
        {
            //if (model.Id > 0)
            //{

            //}
            CommonController Common = new CommonController(_commonRepository);
            ViewBag.ConsignmentList = Common.GetVehicleList();
            return PartialView("~/Views/ImportsQuote/AddUpdateConsignee.cshtml", model);
        }
    }
}