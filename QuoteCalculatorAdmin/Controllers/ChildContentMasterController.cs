using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using QuoteCalculator.Service.Models;
using QuoteCalculator.Service.Repository.ChildContentMaster;
using QuoteCalculator.Service.Repository.CommonRepository;
using QuoteCalculatorAdmin.Common;
using QuoteCalculatorAdmin.Data;
using QuoteCalculatorAdmin.Data.Repository;
using QuoteCalculatorAdmin.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuoteCalculatorAdmin.Controllers
{
    public class ChildContentMasterController : BaseController
    {
        #region private variables
        private readonly IChildContentMasterRepository _contentRepository;
        private readonly GenericRepository<tbl_ChildContent> _dbChildContent;
        private readonly GenericRepository<tbl_HeadingContent> _dbHeadingContent;
        private readonly GenericRepository<rates_destinations> _dbRepositoryRateDestination;
        private readonly ICommonRepository _commonRepository;

        #endregion

        #region Constructor
        public ChildContentMasterController(IChildContentMasterRepository contentRepository, ICommonRepository commonRepository)
        {
            _contentRepository = contentRepository;
            _commonRepository = commonRepository;
            _dbChildContent = new GenericRepository<tbl_ChildContent>();
            _dbHeadingContent = new GenericRepository<tbl_HeadingContent>();
            _dbRepositoryRateDestination = new GenericRepository<rates_destinations>();
        }
        #endregion

        #region Method
        // GET: ContentMaster
        public ActionResult Index()
        {
            ViewBag.HedingContent = _dbHeadingContent.GetEntities();
            return View();
        }
        [HttpPost]
        public ActionResult GetContentList(DatatableModel model)
        {
            try
            {
                DataTablePaginationModel DtSearchModel = CommonController.GetDataTablePaginationModel(model);
                List<ContentMasterModel> dataList = _contentRepository.GetAllContents(DtSearchModel);

                DatatableResponseModel<ContentMasterModel> response = new DatatableResponseModel<ContentMasterModel>()
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
       
        //public ActionResult Read([DataSourceRequest] DataSourceRequest request)
        //{
        //    try
        //    {
                
        //        //var list = _dbChildContent.GetEntities().OrderBy(m => m.ChildContent).ToList();
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
        //        List<sp_GetAllChildContent_Result> list = CustomRepository.GetAllChildContent();
        //        return Json(list.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(CommonHelper.GetErrorMessage(ex));
        //    }
        //}
        [HttpPost]
        public ActionResult Save(ContentMasterModel model)
        {
            
            int result = 0;
            string message = string.Empty;
            try
            {

                if (model.ChildContentId == 0)
                {

                    model.CountryCode = string.Join<string>(",", model.CountryList);
                //    message = _dbChildContent.Insert(model);
                    result = _contentRepository.AddContent(model);
                }
                else
                {
                    model.CountryCode = string.Join<string>(",", model.CountryList);
                    //message = _dbChildContent.Update(model);
                    result = _contentRepository.UpdateContent(model);
                }

                if (result > 0)
                {
                    if(model.ChildContentId == 0)
                    {
                        return Json(new { Message = "Added Successfully" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { Message = "Updated Successfully" }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                message = CommonHelper.GetErrorMessage(ex);
            }

            return Json(new[] { model });

        }
        public ActionResult Destroy(int id)
        {
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
                return Json(new { Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Message = "Child Content Deleted.", IsError = Convert.ToString((int)CustomEnums.NotifyType.Success) }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ChildContentOpen(int? ChildContentId)
        {
            ContentMasterModel model = new ContentMasterModel();
            try
            {
                List<string> list = new List<string>();
                // model = _dbChildContent.GetEntities().Where(m => m.ChildContentId == ChildContentId).FirstOrDefault();
                CommonController CommonController = new CommonController(_commonRepository);
                IEnumerable<SelectListItem> CountryList = CommonController.GetCountryListCommon();
                ViewBag.CountryList = CommonController.GetCountryListCommon();
                ViewBag.HeadingContentList = CommonController.GetHeadingContentlList();
                if (ChildContentId != 0)
                {
                    model = _contentRepository.GetChildContentById(ChildContentId);
                }
                if (model.CountryCode != null)
                {
                    var array = model.CountryCode.Split(',');
                    foreach (var country in array)
                    {
                        list.Add(country.TrimStart().TrimEnd().ToString());
                    }

                }
                model.CountryList = list;
               
            }
            catch (Exception ex)
            {
                return Json(CommonHelper.GetErrorMessage(ex));
            }

            return PartialView("_ChildContent", model);
        }
        #endregion
    }
}