using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using QuoteCalculator.Service.Models;
using QuoteCalculator.Service.Repository;
using QuoteCalculator.Service.Repository.CommonRepository;
using QuoteCalculator.Service.Repository.HeadingContent;
using QuoteCalculatorAdmin.Common;
using QuoteCalculatorAdmin.Data;
using QuoteCalculatorAdmin.Data.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuoteCalculatorAdmin.Controllers
{
    public class ContentMasterController : BaseController
    {
        private readonly IHeadingContentRepository _contentRepository;
        private readonly IQuotesListRepository _repositoy;
        private readonly GenericRepository<tbl_HeadingContent> _dbHeadingContent;
        private readonly ICommonRepository _commonRepository;

        public ContentMasterController(IHeadingContentRepository contentRepository, IQuotesListRepository repositoy, ICommonRepository commonRepository)
        {
            _contentRepository = contentRepository;
            _repositoy = repositoy;
            _commonRepository = commonRepository;
        }

        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult GetContentList(DatatableModel model)
        {
            try
            {
                DataTablePaginationModel DtSearchModel = CommonController.GetDataTablePaginationModel(model);
                List<HeadingContentModel> dataList = _contentRepository.GetAllContents(DtSearchModel);

                DatatableResponseModel<HeadingContentModel> response = new DatatableResponseModel<HeadingContentModel>()
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
                TempData[CustomEnums.NotifyType.Error.GetDescription()] = ex.Message;
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ManageContent(int contentId)
        {
            QuoteCalculator.Service.Models.HeadingContentModel model = new QuoteCalculator.Service.Models.HeadingContentModel();
            try
            {
                CommonController CommonController = new CommonController(_commonRepository);
                ViewBag.CountryList = CommonController.GetCountryListCommon();
                ViewBag.CompanyList = CommonController.GetCompanyList();
                ViewBag.QuoteTypeList = CommonController.GetQuoteList();

                if (contentId > 0)
                {
                    model = _contentRepository.GetContentById(contentId);
                    if (!string.IsNullOrEmpty(model.FromCountryCode))
                    {
                        model.FromCountryList = model.FromCountryCode.Split(',').ToList();
                    }
                    if (!string.IsNullOrEmpty(model.CountryCode))
                    {
                        model.CountryList = model.CountryCode.Split(',').ToList();
                    }
                    if (!string.IsNullOrEmpty(model.QuoteType))
                    {
                        model.QuoteTypeList = model.QuoteType.Split(',').ToList();
                    }
                    if (!string.IsNullOrEmpty(model.Company))
                    {
                        model.CompanyList = model.Company.Split(',').ToList();
                    }
                }                
            }
            catch (Exception ex)
            {
                TempData[CustomEnums.NotifyType.Error.GetDescription()] = ex.Message;
            }
            return View("~/Views/ContentMaster/ManageContent.cshtml", model);
        }

        [HttpPost]
        public ActionResult ManageContent(QuoteCalculator.Service.Models.HeadingContentModel model)
        {
            try
            {
                int result = 0;
                if (model.FromCountryList != null)
                {
                    model.FromCountryCode = string.Join<string>(",", model.FromCountryList);
                }
                if (model.CountryList != null)
                {
                    model.CountryCode = string.Join<string>(",", model.CountryList);
                }
                if (model.QuoteTypeList != null)
                {
                    model.QuoteType = string.Join<string>(",", model.QuoteTypeList);
                }
                if (model.CompanyList != null)
                {
                    model.Company = string.Join<string>(",", model.CompanyList);
                }
                if (model.HeadingContentId > 0)
                {
                    result = _contentRepository.UpdateContent(model);
                }
                else
                {
                    result = _contentRepository.AddContent(model);
                }
                if (result > 0)
                {
                    return Json(new { Success = true, Message = "Header content saved successfully." }, JsonRequestBehavior.AllowGet);
                    //return RedirectToAction(nameof(Index));
                }
                else
                {
                    return Json(new { Success = false, Message = "Something went wrong!" }, JsonRequestBehavior.AllowGet);
                    //return View("~/Views/ContentMaster/ManageContent.cshtml", model);
                }
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }            
        }
        public ActionResult Destroy(int id)
        {
            string deleteMessage = string.Empty;

            try
            {
                int result = _contentRepository.DeleteContent(id);
                if (result >= 0)
                {
                    return Json(new { IsSuccess = true, Message = "Record Deleted Successfully." }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { IsSuccess = false, Message = "Something went wrong" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                TempData[CustomEnums.NotifyType.Error.GetDescription()] = ex.Message;
            }
            return Json(new { Message = "HeadingContent Deleted.", IsError = Convert.ToString((int)CustomEnums.NotifyType.Error) }, JsonRequestBehavior.AllowGet);
        }
        //#region private variables


        //private readonly GenericRepository<rates_destinations> _dbRepositoryRateDestination;
        //private readonly GenericRepository<tbl_QuoteType> _dbRepositoryQuoteType;
        //#endregion

        //#region Constructor
        //public ContentMasterController()
        //{
        //    _dbHeadingContent = new GenericRepository<tbl_HeadingContent>();
        //    _dbRepositoryRateDestination = new GenericRepository<rates_destinations>();
        //    _dbRepositoryQuoteType = new GenericRepository<tbl_QuoteType>();
        //}
        //#endregion

        //#region Method
        //// GET: ContentMaster
        //public ActionResult Index()
        //{
        //    return View();
        //}
        //public ActionResult Read([DataSourceRequest] DataSourceRequest request)
        //{
        //    try
        //    {

        //        //var list = _dbHeadingContent.GetEntities().OrderBy(m => m.HeadingContent).ToList();
        //        //var len = list.Count();
        //        //for (int i = 0; i < len; i++)
        //        //{
        //        //    List<string> country = new List<string>();

        //        //    var str = list[i].CountryCode.ToString();
        //        //    var array = str.Split(',');
        //        //    foreach (var coun in array)
        //        //    {
        //        //        var countryName = _dbRepositoryRateDestination.GetEntities().Where(m => m.country_code == coun).Select(m => m.country).FirstOrDefault();
        //        //        country.Add(countryName);
        //        //    }

        //        //    list[i].CountryCode = string.Join<string>(",", country);
        //        //}
        //        //for (int j = 0; j < len; j++)
        //        //{
        //        //    List<string> quote = new List<string>();
        //        //    var str = list[j].QuoteType.ToString();
        //        //    var arr = str.Split(',');
        //        //    foreach (var quot in arr)
        //        //    {
        //        //        int id = Convert.ToInt32(quot);
        //        //        var quoteType = _dbRepositoryQuoteType.GetEntities().Where(m => m.Id == id).Select(m => m.QuoteType).FirstOrDefault();
        //        //        quote.Add(quoteType);
        //        //    }
        //        //    list[j].QuoteType = string.Join<string>(",", quote);
        //        //}
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
        //        };
        //        var dataList = entityObj.Database.SqlQuery<HeadingContentModel>("sp_GetAllHeadingContent_p @PageNumber,@PageSize",
        //            PageNumber, PageSize).ToList();

        //        DataSourceResult obj = new DataSourceResult();
        //        obj.Total = (dataList != null && dataList.Count > 0) ? (int)dataList[0].TotalCount : 0;
        //        obj.Data = dataList;

        //        return Json(obj, JsonRequestBehavior.AllowGet);

        //        //List<sp_GetAllHeadingContent_Result> list =  CustomRepository.GetAllHeadingContent();
        //        //return Json(list.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(CommonHelper.GetErrorMessage(ex));
        //    }
        //}
        //[HttpPost]
        //public ActionResult Save([DataSourceRequest] DataSourceRequest request, tbl_HeadingContent model)
        //{
        //    string message = string.Empty;
        //    try
        //    {
        //        model.FromCountryCode = string.Join<string>(",", model.FromCountryList);
        //        model.CountryCode = string.Join<string>(",", model.CountryList);
        //        model.QuoteType = string.Join<string>(",", model.QuoteTypeList);
        //        model.Company = string.Join<string>(",", model.CompanyList);
        //        if (model.HeadingContentId == 0)
        //        {
        //            message = _dbHeadingContent.Insert(model);
        //        }
        //        else
        //        {
        //            message = _dbHeadingContent.Update(model);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        message = CommonHelper.GetErrorMessage(ex);
        //    }

        //    return Json(new[] { model }.ToDataSourceResult(request, ModelState));

        //}
       
        //public ActionResult HeadingContentOpen(int? HeadingContentId)
        //{
        //    try
        //    {
        //        tbl_HeadingContent model1 = new tbl_HeadingContent();
        //        QuoteCalculator.Service.Models.HeadingContentModel model = new QuoteCalculator.Service.Models.HeadingContentModel();

        //        if (HeadingContentId != 0)
        //        {
        //            List<string> fromCountryList = new List<string>();
        //            List<string> toCountryList = new List<string>();
        //            CommonController CommonController = new CommonController();
        //            ViewBag.CountryList = CommonController.GetCountryListCommon();
        //            ViewBag.CompanyList = _repositoy.GetCompanyList();
        //            ViewBag.QuoteTypeList = _repositoy.GetQuoteList();

        //            if (HeadingContentId > 0)
        //            {
        //                model = _contentRepository.GetContentById(HeadingContentId);
        //            }
        //            //    model = _dbHeadingContent.GetEntities().Where(m => m.HeadingContentId == HeadingContentId).FirstOrDefault();
        //            //model = CustomRepository.GetEditHeadingContent(HeadingUniqueId);
        //            if (model.FromCountryCode != null)
        //            {
        //                var fromCountryArray = model.FromCountryCode.Split(',');
        //                foreach (var fromCountry in fromCountryArray)
        //                {
        //                    fromCountryList.Add(fromCountry.TrimStart().TrimEnd().ToString());
        //                }
        //            }
        //            if (model.CountryCode != null)
        //            {
        //                var toCountryArray = model.CountryCode.Split(',');
        //                foreach (var toCountry in toCountryArray)
        //                {
        //                    toCountryList.Add(toCountry.TrimStart().TrimEnd().ToString());
        //                }
        //            }
        //            model.FromCountryList = fromCountryList;
        //            model.CountryList = toCountryList;

        //            List<string> quoteList = new List<string>();
        //            var arr = model.QuoteType.Split(',');
        //            foreach (var quot in arr)
        //            {
        //                quoteList.Add(quot.TrimStart().TrimEnd().ToString());
        //            }
        //            model.QuoteTypeList = quoteList;

        //            List<string> companyList = new List<string>();
        //            var comarr = model.Company.Split(',');
        //            foreach (var com in comarr)
        //            {
        //                companyList.Add(com.TrimStart().TrimEnd().ToString());
        //            }
        //            model.CompanyList = companyList;
        //        }


        //        return PartialView("_HeadingContent", model);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(CommonHelper.GetErrorMessage(ex));
        //    }
        //}
        //#endregion
    }
}