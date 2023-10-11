using Kendo.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Newtonsoft.Json;
using QuoteCalculator.Service.Models;
using QuoteCalculator.Service.Repository.RoleRepository;
using QuoteCalculatorAdmin.Data;
using QuoteCalculatorAdmin.Data.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

namespace QuoteCalculatorAdmin.Controllers
{
    public class RoleController : BaseController
    {

        private readonly IRoleRepository _roleRepository;

        public RoleController(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult GetRoleList(DatatableModel model)
        {
            try
            {
                DataTablePaginationModel DtSearchModel = CommonController.GetDataTablePaginationModel(model);
                List<RoleModel> dataList = _roleRepository.GetAllRoles(DtSearchModel);

                DatatableResponseModel<RoleModel> response = new DatatableResponseModel<RoleModel>()
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
                return Json(new { Message = ex.Message }, JsonRequestBehavior.AllowGet);

            }
         
        }
        public ActionResult ManageRole(int roleId)
        {
            List<QuoteCalculator.Service.Models.RoleMenuMapModel> model = new List<QuoteCalculator.Service.Models.RoleMenuMapModel>();
            if (roleId > 0)
            {
                model = _roleRepository.GetRoleById(roleId);
            }
            return View("~/Views/Role/ManageRole.cshtml", model);
        }

        [HttpPost]
        public ActionResult ManageRole(List<QuoteCalculator.Service.Models.RoleMenuMapModel> model)
        {
            try
            {
                if (model != null)
                {
                    var data = model.FirstOrDefault();
                    if (data != null)
                    {
                        if (data.RoleId > 0)
                        {
                            _roleRepository.UpdateRole(model);
                        }
                        else
                        {
                            _roleRepository.AddRole(model);
                        }
                    }
                }
                return Redirect(nameof(Index));
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult AddEdit(RoleModel roleModel)
        {
            int result = 0;
            result = _roleRepository.AddEditRole(roleModel);
            return Json(new { result = result }, JsonRequestBehavior.AllowGet);
   
        }

        [HttpPost]
        public ActionResult DeleteRole(int RoleId)
        {
            try
            {
                int result = _roleRepository.DeleteRole(RoleId);
                if (result >= 0)
                {
                    return Json(new { IsSuccess = true, Message = "Role Deleted Successfully." }, JsonRequestBehavior.AllowGet);
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
        }
        [HttpPost]
        public ActionResult ChangeStatus(short id, bool IsActive)
        {
            try
            {
                int result = _roleRepository.ChangeStatus(id, IsActive);
                if (result >= 0)
                {
                    return Json(new { Message = "Status Updated" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Message = "Something went wrong" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult AddEdit(int? Id)
        {
            RoleModel roleModel = new RoleModel();
            List<RoleModel> role = _roleRepository.GetRoleByRoleId(Id);

            if (role.Any())
            {
                roleModel = role.FirstOrDefault();
            }
            else
            {
                roleModel.RoleId = 0;
                roleModel.RoleName = string.Empty;
                roleModel.IsActive = false;
            }

            return View("~/Views/Role/AddEdit.cshtml", roleModel);
        }
        
    }
}