using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using QuoteCalculator.Service.Models;
using QuoteCalculator.Service.Repository.OptOutRepository;
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
    public class OptOutController : Controller
    {
        #region private variables       

        private readonly GenericRepository<tbl_OptOut> _dbRepositoryOptOut;
        private readonly IOptOutRepository _dbOptOutRepository;

        #endregion
        #region Constructor
        public OptOutController(IOptOutRepository dbOptOutRepository)
        {
            _dbRepositoryOptOut = new GenericRepository<tbl_OptOut>();
            _dbOptOutRepository = dbOptOutRepository;
        }
        #endregion
        // GET: OptOut
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetoptoutList(DatatableModel model)
        {
            try
            {
                DataTablePaginationModel DtSearchModel = CommonController.GetDataTablePaginationModel(model);

                List<OptOutModel> Result = _dbOptOutRepository.GetOptOutList(DtSearchModel);
                DatatableResponseModel<OptOutModel> response = new DatatableResponseModel<OptOutModel>()
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
        //public ActionResult KendoRead([DataSourceRequest] DataSourceRequest request)
        //{
        //    try
        //    {
        //        quotesEntities obj = new quotesEntities();
        //        var OptOutList = obj.Database.SqlQuery<OptOutResult>("SP_OptOutResult").ToList();
        //        return Json(OptOutList.ToDataSourceResult(request));
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(CommonHelper.GetErrorMessage(ex));
        //    }
        //}
        [HttpGet]
        public ActionResult ChangeStatus(string Email)
        {
            try
            {
                long Result = _dbOptOutRepository.ChangeStatus(Email, SessionHelper.UserId);
                if (Result > 0)
                {
                    return Json(new { IsSuccess = true, Message = "Opted Out Successfully" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { IsSuccess = false, Message = "Something Went Wrong!" }, JsonRequestBehavior.AllowGet);

                //var CheckEmail = _dbRepositoryOptOut.GetEntities().Where(m => m.Email == Email).ToList();
                //if (CheckEmail.Count > 0)
                //{
                //    var emailParameter = new SqlParameter
                //    {
                //        ParameterName = "email",
                //        DbType = DbType.String,
                //        Value = Email
                //    };
                //    quotesEntities obj = new quotesEntities();
                //    CityListModel model = new CityListModel();
                //    int result = obj.Database.SqlQuery<int>("SP_SetOptIn @email", emailParameter).FirstOrDefault();
                //    if (result > 0)
                //    {
                //        return Json(new { IsSuccess = true, Message = "Opted In Successfully" }, JsonRequestBehavior.AllowGet);
                //    }
                //    else
                //    {
                //        return Json(new { IsSuccess = false, Message = result.ToString() }, JsonRequestBehavior.AllowGet);
                //    }
                //}
                //else
                //{
                //    tbl_OptOut obj = new tbl_OptOut();
                //    obj.CreatedBy = SessionHelper.UserId;
                //    obj.Email = Email;
                //    obj.MovewareCopied = DateTime.Now;
                //    obj.CreatedDate = DateTime.Now;
                //    string message = _dbRepositoryOptOut.Insert(obj);
                //    return Json(new { IsSuccess = true, Message = "Opted Out Successfully" }, JsonRequestBehavior.AllowGet);
                //}
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}