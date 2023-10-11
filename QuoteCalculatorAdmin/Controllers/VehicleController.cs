using Kendo.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using QuoteCalculator.Service.Models;
using QuoteCalculator.Service.Repository.CommonRepository;
using QuoteCalculator.Service.Repository.VehicleRepository;
using QuoteCalculatorAdmin.Common;
using QuoteCalculatorAdmin.Data;
using QuoteCalculatorAdmin.Data.Repository;
//using QuoteCalculatorAdmin.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuoteCalculatorAdmin.Controllers
{
    public class VehicleController : BaseController
    {
        #region private variables
        private readonly GenericRepository<tbl_Vehicle> _dbRepository;
        private readonly GenericRepository<tbl_AccessCode> _dbRepositoryAccessCode;
        private readonly IVehicleRepository _vehicleRepository;
        private readonly ICommonRepository _commonRepository;
        #endregion

        #region Constructor
        public VehicleController(IVehicleRepository vehicleRepository, ICommonRepository commonRepository)
        {
            _dbRepository = new GenericRepository<tbl_Vehicle>();
            _dbRepositoryAccessCode = new GenericRepository<tbl_AccessCode>();
            _vehicleRepository = vehicleRepository;
            _commonRepository = commonRepository;
        }
        #endregion

        #region Method
        public ActionResult Index()
        {
          //  ViewBag.TitleList = CommonHelper.GetTitleList();
            return View();
        }
        
        //public ActionResult KendoRead([DataSourceRequest] DataSourceRequest request)
        public ActionResult GetVehicaldata(DatatableModel model)
        {            
            try
            {
                DataTablePaginationModel DtSearchModel = CommonController.GetDataTablePaginationModel(model);

                List<VehicleModel> dataList = _vehicleRepository.GetVehicleList(SessionHelper.CompanyId, DtSearchModel);

                DatatableResponseModel<VehicleModel> response = new DatatableResponseModel<VehicleModel>()
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

        public ActionResult Edit(int Id)
        {
            EditVehicleModel em = _vehicleRepository.GetVehicleById(Id);

            CommonController common = new CommonController(_commonRepository);

            ViewBag.TitleList = new SelectList(common.GetTitleList(), "Value", "Text"); //SelectionList.TitleList().Select(m => new { TitleId = m.Id, m.TitleName });
            ViewBag.BranchList = new SelectList(common.GetBranchListCommon(), "Value", "Text"); //SelectionList.BranchList().Select(m => new { m.br_id, m.br_branch, m.br_code });
            ViewBag.VehicleTypeList = new SelectList(common.GetVehicleTypeList(), "Value", "Text"); //SelectionList.VehicleTypeList().Select(m => new { m.Id, m.TypeName });
            return View(em);
        }

        public ActionResult Destroy(int Id)
        {
            try
            {
                var result = _vehicleRepository.DeleteVehicle(Id);
                if (result > 0)
                {
                    var data = new
                    {
                        result = true,
                        Message = "Delete Successfully!!!"
                    };
                    return Json(data);
                }
                else
                {
                    var data = new
                    {
                        result = false,
                        Message = "Error while deleting record"
                    };
                    return Json(data);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
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