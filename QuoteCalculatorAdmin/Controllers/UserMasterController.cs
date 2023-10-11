using Kendo.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using QuoteCalculator.Service.Models;
using QuoteCalculator.Service.Repository.CommonRepository;
using QuoteCalculator.Service.Repository.UserRepository;
using QuoteCalculatorAdmin.Common;
using QuoteCalculatorAdmin.Data;
using QuoteCalculatorAdmin.Data.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace QuoteCalculatorAdmin.Controllers
{
    public class UserMasterController : BaseController
    {
        private readonly IUserRepository _userRepository;
        private readonly ICommonRepository _commonRepository;


        public UserMasterController(IUserRepository userRepository, ICommonRepository commonRepository)
        {
            this._userRepository = userRepository;
            _commonRepository = commonRepository;
        }

        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult GetUserList(DatatableModel model)
        {
            try
            {

                DataTablePaginationModel DtSearchModel = CommonController.GetDataTablePaginationModel(model);
                List<UserModel> dataList = _userRepository.GetAllUsers(DtSearchModel);
       
                DatatableResponseModel<UserModel> response = new DatatableResponseModel<UserModel>()
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
        public ActionResult ManageUsers(int UserId)
        {
           UserModel model = new UserModel();
            CommonController CommonController = new CommonController(_commonRepository);
         
            ViewBag.RoleList = CommonController.GetRoleList();
            ViewBag.CompanyList = CommonController.GetRolecompanyList();
            if (UserId > 0)
            {
                model = _userRepository.GetUserById(UserId);
                model.password = Security.Decrypt(model.password);
            }
            else
            {
                model.IsActive = true;
            }
            return View("~/Views/UserMaster/AddEditUser.cshtml", model);
        }

        [HttpPost]
        public ActionResult ManageUsers(UserModel model)
        {
            try
            {
                int result = 0;
                model.password = Security.Encrypt(model.password);
                if (model.id > 0)
                {
                    result = _userRepository.UpdateUser(model);
                }
                else
                {
                    result = _userRepository.AddUser(model);
                   
                }
                if (result > 0)
                {
                    TempData[CustomEnums.NotifyType.Success.GetDescription()] = "User saved successfully";
                }

                else if (result == -1)
                {
                    TempData[CustomEnums.NotifyType.Error.GetDescription()] = "Email Id Or Username already exist.";
                }
                else
                {
                    TempData[CustomEnums.NotifyType.Error.GetDescription()] = "Something went wrong";
                }
                return Redirect(nameof(Index));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //[HttpPost]
        //public ActionResult AddUpdateUser(QuoteCalculator.Service.Models.UserModel userModel)
        //{
        //    try
        //    {
        //        int result = 0;
        //        if (userModel.id > 0)
        //        {
        //            result = _userRepository.UpdateUser(userModel);
        //        }
        //        else
        //        {
        //            result = _userRepository.AddUser(userModel);
        //        }
        //        return Redirect(nameof(Index));
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { Message = ex.Message }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        [HttpPost]
        public ActionResult DeleteUser(int userId)
        {
            try
            {
                var result = _userRepository.DeleteUser(userId);
                if (result >= 0)
                {
                    return Json(new { Message = "Record Deleted" }, JsonRequestBehavior.AllowGet);
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
        [HttpPost]
        public ActionResult ChangeStatus(short id,bool IsActive)
        {
            try
            {
                int result = _userRepository.ChangeStatus(id, IsActive);
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
        //#region private variables       

        //private readonly GenericRepository<user> _dbRepositoryUser;

        //#endregion
        //#region Constructor
        //public UserMasterController()
        //{
        //    _dbRepositoryUser = new GenericRepository<user>();
        //}
        //#endregion

        //// GET: UserMaster
        //public ActionResult Index()
        //{
        //    ViewBag.RoleList = SelectionList.RoleList().Select(m => new { m.RoleId, m.RoleName });
        //    ViewBag.CompanyList = SelectionList.CompanyList().Select(m => new { m.Id, m.Code });
        //    return View();
        //}
        //[HttpPost]
        //public async Task<ActionResult> GetUserList(DatatableModel model)
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
        //        var dataList = entityObj.Database.SqlQuery<UserModel>("sp_GetUserList @PageNumber,@PageSize,@SearchStr",
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

        //public async Task<ActionResult> GetUserById(long userId)
        //{
        //    ViewBag.RoleList = SelectionList.RoleList();
        //    ViewBag.CompanyList = SelectionList.CompanyList();//.Select(m => new { m.Id, m.Code });
        //    var model = new UserModel();
        //    try
        //    {
        //        quotesEntities entityObj = new quotesEntities();
        //        var ParamuserId = new SqlParameter
        //        {
        //            ParameterName = "UserId",
        //            DbType = DbType.String,
        //            Value = userId
        //        };
        //        model = entityObj.Database.SqlQuery<UserModel>("sp_GetUserDetailById @UserId", ParamuserId).FirstOrDefault();
        //    }
        //    catch(Exception ex)
        //    {
        //        return Json(new { Message = "Something went wrong" }, JsonRequestBehavior.AllowGet);
        //    }
        //    return View("~/Views/UserMaster/AddEditUser.cshtml", model);
        //}
        //public ActionResult Read([DataSourceRequest] DataSourceRequest request)
        //{
        //    try
        //    {
        //        var allFilterDescriptors = new List<FilterDescriptor>();
        //        List<FilterDescriptor> filterList = RecurseFilterDescriptors(request.Filters, allFilterDescriptors);
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

        //        var email = new SqlParameter
        //        {
        //            ParameterName = "email",
        //            DbType = DbType.String,
        //            Value = ""
        //        };
        //        var username = new SqlParameter
        //        {
        //            ParameterName = "username",
        //            DbType = DbType.String,
        //            Value = ""
        //        };
        //        var RoleId = new SqlParameter
        //        {
        //            ParameterName = "RoleId",
        //            DbType = DbType.Int32,
        //            Value = 0
        //        };
        //        var CompanyId = new SqlParameter
        //        {
        //            ParameterName = "CompanyId",
        //            DbType = DbType.Int32,
        //            Value = 0
        //        };
        //        var SalesRepCode = new SqlParameter
        //        {
        //            ParameterName = "SalesRepCode",
        //            DbType = DbType.String,
        //            Value = ""
        //        };
        //        var RoleIdNotEqual = new SqlParameter
        //        {
        //            ParameterName = "RoleIdNotEqual",
        //            DbType = DbType.Int32,
        //            Value = 0
        //        };
        //        var CompanyIdNotEqual = new SqlParameter
        //        {
        //            ParameterName = "CompanyIdNotEqual",
        //            DbType = DbType.Int32,
        //            Value = 0
        //        };
        //        if (filterList != null && filterList.Count > 0)
        //        {
        //            foreach (var filteritem in filterList)
        //            {
        //                if (string.IsNullOrEmpty(filteritem.Value.ToString()) || string.IsNullOrEmpty(filteritem.Member))
        //                {
        //                    continue;
        //                }
        //                else if (filteritem.Member.ToLower() == "email")
        //                {
        //                    email.Value = filteritem.Value.ToString();
        //                }
        //                else if (filteritem.Member.ToLower() == "username")
        //                {
        //                    username.Value = filteritem.Value.ToString();
        //                }
        //                else if (filteritem.Operator.ToString().ToLower() == "isequalto"  && filteritem.Member.ToLower() == "roleid")
        //                {
        //                    RoleId.Value = filteritem.Value.ToString();
        //                }
        //                else if (filteritem.Operator.ToString().ToLower() == "isequalto" && filteritem.Member.ToLower() == "companyid")
        //                {
        //                    CompanyId.Value = filteritem.Value.ToString();
        //                }
        //                else if (filteritem.Member.ToLower() == "salesrepcode")
        //                {
        //                    SalesRepCode.Value = filteritem.Value.ToString();
        //                }

        //                else if(filteritem.Operator.ToString().ToLower() == "isnotequalto" && filteritem.Member.ToLower() == "roleid")
        //                {
        //                    RoleIdNotEqual.Value = filteritem.Value.ToString();
        //                }
        //                else if (filteritem.Operator.ToString().ToLower() == "isnotequalto" && filteritem.Member.ToLower() == "companyid")
        //                {
        //                    CompanyIdNotEqual.Value = filteritem.Value.ToString();
        //                }

        //            }

        //        }

        //        var dataList = entityObj.Database.SqlQuery<UserModel>("sp_GetUserListContent @email,@username,@RoleId,@CompanyId,@SalesRepCode,@RoleIdNotEqual,@CompanyIdNotEqual,@PageNumber,@PageSize",
        //            email, username, RoleId, CompanyId, SalesRepCode, RoleIdNotEqual, CompanyIdNotEqual, PageNumber, PageSize).ToList();




        //        //var list = _dbRepositoryUser.GetEntities().OrderByDescending(x => x.IsActive).ToList();
        //        foreach (var pass in dataList)
        //        {
        //            pass.password = Security.Decrypt(Convert.ToString(pass.password));
        //        }
        //        //DataSourceResult result = list.ToDataSourceResult(request);
        //        //return Json(result);
        //        DataSourceResult obj = new DataSourceResult();
        //        obj.Total = (dataList != null && dataList.Count > 0) ? (int)dataList[0].TotalCount : 0;
        //        obj.Data = dataList;

        //        return Json(obj, JsonRequestBehavior.AllowGet);
        //    }
        //    catch(Exception ex)
        //    {
        //        return Json(new { Message = "Something went wrong" }, JsonRequestBehavior.AllowGet);
        //    }

        //}

        //public ActionResult Destroy([DataSourceRequest] DataSourceRequest request, user model)
        //{
        //    string deleteMessage = string.Empty;

        //    try
        //    {
        //        _dbRepositoryUser.Delete(model.id);

        //        deleteMessage = "User deleted sucessfully";

        //    }
        //    catch (Exception ex)
        //    {
        //        deleteMessage = "Something went wrong";
        //    }
        //    return Json(new { Message = deleteMessage }, JsonRequestBehavior.AllowGet);
        //}


        //public ActionResult addEdit([DataSourceRequest] DataSourceRequest request, user model)
        //{
        //    string message = string.Empty;
        //    model.password = Security.Encrypt(Convert.ToString(model.password));
        //    if (model == null || !ModelState.IsValid)
        //    {
        //        return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        //    }

        //    try
        //    {
        //        if (model.id > 0)
        //        {
        //            _dbRepositoryUser.Update(model);
        //            message = "User updated sucessfully";
        //        }
        //        else
        //        {
        //            message = _dbRepositoryUser.Insert(model);
        //            message = "User inserted sucessfully";
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { Message = "Something went wrong" }, JsonRequestBehavior.AllowGet);
        //    }
        //    return Json(new { Message = message }, JsonRequestBehavior.AllowGet);

        //}
       
        //private List<FilterDescriptor> RecurseFilterDescriptors(IEnumerable<IFilterDescriptor> requestFilters, List<FilterDescriptor> allFilterDescriptors)
        //{
        //    if (requestFilters.Any())
        //    {
        //        foreach (var filterDescriptor in requestFilters)
        //        {
        //            if (filterDescriptor is FilterDescriptor)
        //            {
        //                allFilterDescriptors.Add((FilterDescriptor)filterDescriptor);
        //            }
        //            else if (filterDescriptor is CompositeFilterDescriptor)
        //            {
        //                RecurseFilterDescriptors(((CompositeFilterDescriptor)filterDescriptor).FilterDescriptors, allFilterDescriptors);
        //            }
        //        }
        //    }
        //    return allFilterDescriptors;
        //}
    }
}