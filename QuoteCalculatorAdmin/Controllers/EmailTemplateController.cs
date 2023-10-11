
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using QuoteCalculator.Service.Repository;
using QuoteCalculator.Service.Repository.EmailRepository;
using QuoteCalculatorAdmin.Common;
using QuoteCalculatorAdmin.Data;
using QuoteCalculatorAdmin.Data.Repository;
using QuoteCalculator.Service.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using QuoteCalculator.Service.Repository.CommonRepository;

namespace QuoteCalculatorAdmin.Controllers
{
    public class EmailTemplateController : BaseController
    {
        private readonly IEmailTemplateRepository _emailTemplateRepository;
        private readonly IQuotesListRepository _quotesListRepository;
        private readonly ICommonRepository _commonRepository;


        public EmailTemplateController(IEmailTemplateRepository emailTemplateRepository, IQuotesListRepository quotesListRepository, ICommonRepository commonRepository)
        {
            _emailTemplateRepository = emailTemplateRepository;
            _quotesListRepository = quotesListRepository;
            _commonRepository = commonRepository;
        }


        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult GetAllTemplatesList(DatatableModel model)
        {
            try
            {
                DataTablePaginationModel DtSearchModel = CommonController.GetDataTablePaginationModel(model);
                List<EmailTemplateModel> dataList = _emailTemplateRepository.GetAllTemplates(DtSearchModel);

                DatatableResponseModel<EmailTemplateModel> response = new DatatableResponseModel<EmailTemplateModel>()
                {
                    draw = model.draw,
                    recordsTotal = dataList != null && dataList.Count > 0 && dataList[0].TotalCount > 0 ? dataList[0].TotalCount ?? 0 : 0,
                    recordsFiltered = dataList != null && dataList.Count > 0 && dataList[0].TotalFilteredCount > 0 ? dataList[0].TotalFilteredCount ?? 0 : 0,
                    data = dataList
                };
                var jsonRes = Json(response, JsonRequestBehavior.AllowGet);
                jsonRes.MaxJsonLength = 50000000;
                return jsonRes;
            }
            catch (Exception ex)
            {

            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ManageTemplates(int templateId)
        {
            CommonController CommonController = new CommonController(_commonRepository);

            ViewBag.ServiceList = CommonController.getServiceList(); //_quotesListRepository.getServiceList();
            //ViewBag.ServiceList = _quotesListRepository.getServiceList();
            QuoteCalculator.Service.Models.EmailTemplateModel model = new QuoteCalculator.Service.Models.EmailTemplateModel();
            if (templateId > 0)
            {
                model = _emailTemplateRepository.GetTemplateById(templateId);
            }
            return View(model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ManageTemplates(QuoteCalculator.Service.Models.EmailTemplateModel templateModel)
        {
            try
            {
                int result = 0;
                templateModel.UserId = SessionHelper.UserId;
                if (templateModel.id > 0)
                {
                    result = _emailTemplateRepository.UpdateTemplate(templateModel);
                }
                else
                {
                    result = _emailTemplateRepository.AddTemplate(templateModel);
                }
                if (result > 0)
                {
                    TempData[CustomEnums.NotifyType.Success.GetDescription()] = "Email template saved successfully";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData[CustomEnums.NotifyType.Error.GetDescription()] = "Something went wrong!";
                    return View(templateModel);
                }
            }
            catch (Exception ex)
            {
                TempData[CustomEnums.NotifyType.Error.GetDescription()] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public ActionResult Destroy(int EmailTemplateId)
        {
            try
            {
                int result = _emailTemplateRepository.DeleteTemplate(EmailTemplateId);
                if (result > 0)
                {
                    return Json(new { IsSuccess = true, Message = "Email template deleted sucessfully" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { IsSuccess = false, Message = "Something went wrong!" }, JsonRequestBehavior.AllowGet);
        }

        //#region private variables

        //private readonly GenericRepository<tbl_EmailTemplate> _dbRepositoryEmailTemplate;
        //private readonly GenericRepository<tbl_Service> _dbRepositoryService;
        //#endregion
        //#region Constructor
        //public EmailTemplateController()
        //{
        //    _dbRepositoryEmailTemplate = new GenericRepository<tbl_EmailTemplate>();
        //    _dbRepositoryService = new GenericRepository<tbl_Service>();
        //}
        //#endregion

        //// GET: EmailTemplate
        //public ActionResult Index()
        //{            
        //    ViewBag.ServiceList = ServiceList().Select(m => new { m.Id, m.Name });

        //    return View();
        //}


        //public async Task<ActionResult> GetEmailtemplateList(DatatableModel model)
        //{
        //    try
        //    {
        //        quotesEntities entityObj = new quotesEntities();
        //        var searchString = "";
        //        if (model.search != null && !string.IsNullOrEmpty(model.search.value))
        //        {
        //            searchString = model.search.value;
        //        }
        //        var PageNumber = new SqlParameter
        //        {
        //            ParameterName = "PageNumber",
        //            DbType = DbType.Int32,
        //            Value = (model.start / model.length) + 1
        //        };
        //        var PageSize = new SqlParameter
        //        {
        //            ParameterName = "PageSize",
        //            DbType = DbType.Int32,
        //            Value = model.length
        //        };
        //        var SearchStr = new SqlParameter
        //        {
        //            ParameterName = "SearchStr",
        //            DbType = DbType.String,
        //            Value = searchString
        //        };
        //        var dataList = entityObj.Database.SqlQuery<EmailTemplateModel>("sp_GetEmailTemplateList @PageNumber,@PageSize,@SearchStr",
        //             PageNumber, PageSize, SearchStr).ToList();
        //        return Json(new
        //        {
        //            draw = model.draw,
        //            recordsTotal = dataList.Count > 0 ? (int)dataList.FirstOrDefault().TotalCount : 0,
        //            recordsFiltered = dataList.Count > 0 ? (int)dataList.FirstOrDefault().TotalCount : 0,
        //            data = dataList
        //        }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    return Json(null, JsonRequestBehavior.AllowGet);
        //}

        //// to get ServiceList
        //public static List<tbl_Service> ServiceList()
        //{
        //    using (quotesEntities _dbContext = BaseContext.GetDbContext())
        //    {
        //        return _dbContext.tbl_Service.OrderBy(x => x.Name).ToList();
        //    }
        //}
        //public ActionResult Read([DataSourceRequest] DataSourceRequest request)
        //{
        //    try
        //    {
        //        quotesEntities entityObj = new quotesEntities();
        //        var PageNumber = new SqlParameter
        //        {
        //            ParameterName = "PageNumber",
        //            DbType = DbType.Int32,
        //            Value = request.Page
        //        };
        //        var PageSize = new SqlParameter
        //        {
        //            ParameterName = "PageSize",
        //            DbType = DbType.Int32,
        //            Value = request.PageSize
        //        }; var dataList = entityObj.Database.SqlQuery<EmailTemplateModel>("sp_GetEmailTemplateListContent @PageNumber,@PageSize",
        //                  PageNumber, PageSize).ToList();

        //        DataSourceResult obj = new DataSourceResult();
        //        obj.Total = (dataList != null && dataList.Count > 0) ? (int)dataList[0].TotalCount : 0;
        //        obj.Data = dataList;

        //        return Json(obj, JsonRequestBehavior.AllowGet);

        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(CommonHelper.GetErrorMessage(ex));
        //    }
        //  //  return Json(_dbRepositoryEmailTemplate.GetEntities().ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        //}
        //public ActionResult AddEdit(int? id)
        //{
        //    try
        //    {
        //        tbl_EmailTemplate model = new tbl_EmailTemplate();
        //        if (id != null)
        //        {
        //            using (var context = BaseContext.GetDbContext())
        //            {
        //                var ServiceList = context.tbl_Service.Select(m => new { m.Id, m.Name }).OrderBy(m => new { m.Name }).ToList();
        //                ViewBag.ServiceList = ServiceList;
        //            }
        //            model = _dbRepositoryEmailTemplate.SelectById(id);
        //        }

        //    return View(model);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(CommonHelper.GetErrorMessage(ex));
        //    }
        //}


        //[HttpPost]
        //public ActionResult AddEdit(tbl_EmailTemplate model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }
        //    string message = string.Empty;
        //    try
        //    {
        //        var m = _dbRepositoryEmailTemplate.GetEntities().FirstOrDefault(x => x.ServiceId == model.ServiceId);
        //        if (model.Id > 0)
        //        {
        //            model.CreatedBy = m.CreatedBy;
        //            model.CreatedOn = m.CreatedOn;
        //            model.UpdateBy = SessionHelper.UserId;
        //            model.UpdateOn = DateTime.Now;

        //        }
        //        else
        //        {   model.CreatedBy = SessionHelper.UserId;
        //            model.CreatedOn = DateTime.Now;
        //        }

        //        message = (m != null ) ? _dbRepositoryEmailTemplate.Update(model) : _dbRepositoryEmailTemplate.Insert(model);
        //        message = ((m != null) && (model.Id > 0)) ?  "Email template updated sucessfully" : "Email template inserted sucessfully";
        //    }
        //    catch (Exception ex)
        //    {
        //        message = "Something went wrong";
        //        TempData[CustomEnums.NotifyType.Error.GetDescription()] = message;
        //    }
        //    TempData[CustomEnums.NotifyType.Success.GetDescription()] = message;
        //    return RedirectToAction("Index", "EmailTemplate");
        //}       

    }
}