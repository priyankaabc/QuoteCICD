using NLog;
using QuoteCalculator.Common;
using QuoteCalculator.Data;
using QuoteCalculator.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuoteCalculator.Controllers
{
    public class CommonController : Controller
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public ActionResult GetTitleList()
        {
            try
            {
                using (var context = BaseContext.GetDbContext())
                {
                    var list = context.tbl_Title.Select(m => new { m.Id, m.TitleName }).OrderBy(m => new { m.TitleName }).ToList();
                    return Json(list, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                TempData[CustomEnums.NotifyType.Error.GetDescription()] = CommonHelper.GetErrorMessage(ex);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }
        //public ActionResult GetCountryList()
        //{
        //    using (var context = BaseContext.GetDbContext())
        //    {
        //        var list = context.tbl_Country.Select(m => new { m.Id, m.CountryName }).OrderBy(m => new { m.CountryName }).ToList();
        //        return Json(list, JsonRequestBehavior.AllowGet);
        //    }
        //}

        //public ActionResult GetCascadingCityList(string CountryId)
        //{
        //    var List = CustomRepository.GetCascadingCityList(CountryId);
        //    return Json(List, JsonRequestBehavior.AllowGet);
        //}


        //public ActionResult GetVehicleTypeList()
        //{
        //    using (var context = BaseContext.GetDbContext())
        //    {
        //        var list = context.tbl_VehicleType.Select(m => new { m.Id, m.TypeName }).OrderBy(m => new { m.TypeName }).ToList();
        //        return Json(list, JsonRequestBehavior.AllowGet);
        //    }
        //}
        //public ActionResult GetVehicleMakeList()
        //{
        //    using (var context = BaseContext.GetDbContext())
        //    {
        //        var list = context.tbl_VehicleMake.Select(m => new { m.Id, m.MakeName }).OrderBy(m => new { m.MakeName }).ToList();
        //        return Json(list, JsonRequestBehavior.AllowGet);
        //    }
        //}
        //public ActionResult GetVehicleModelList()
        //{
        //    using (var context = BaseContext.GetDbContext())
        //    {
        //        var list = context.tbl_VehicleModel.Select(m => new { m.Id, m.ModelName }).OrderBy(m => new { m.ModelName }).ToList();
        //        return Json(list, JsonRequestBehavior.AllowGet);
        //    }
        //}

        [HttpGet]
        public ActionResult QuoteDetails(string countryCode, string quoteType)
        {
            List<SP_GetQuoteContents_Result> Details = new List<SP_GetQuoteContents_Result>();
            try
            {
                Details = CustomRepository.GetQuoteContents(SessionHelper.FromCountryCode, countryCode, quoteType).OrderBy(m => m.DisplayOrder).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                TempData[CustomEnums.NotifyType.Error.GetDescription()] = CommonHelper.GetErrorMessage(ex);
            }
            return PartialView("_QuoteContentView", Details);
        }
    }
}