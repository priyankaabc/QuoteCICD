using Dapper;
using QuoteCalculator.Service.Common;
using QuoteCalculator.Service.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace QuoteCalculator.Service.Repository.RoleRepository
{
    public class RoleRepository : BaseRepository, IRoleRepository
    {
        public int AddRole(List<RoleMenuMapModel> roleModel)
        {
            try
            {
                foreach (var item in roleModel)
                {
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@RoleMenuMapId", item.RoleMenuMapId);
                    parameters.Add("@roleId", item.RoleId);
                    parameters.Add("@menuId", item.MenuId);
                    parameters.Add("@isView", item.IsView);
                    parameters.Add("@IsInsert", item.IsInsert);
                    parameters.Add("@isEdit", item.IsEdit);
                    parameters.Add("@isDelete", item.IsDelete);
                    parameters.Add("@IsChangeStatus", item.IsChangeStatus);
                    ExecuteStoredProcedure("sp_tbl_Role_addEdit", parameters);
                }
                return 1;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int DeleteRole(int? roleId)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@RoleId", roleId);
            int result = ExecuteStoredProcedure("sp_tbl_Role_Delete", parameters);
            return result;
        }

        public int ChangeStatus(int? roleId, bool IsActive)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", roleId);
            parameters.Add("@IsActive", !IsActive);
            int result = ExecuteStoredProcedure("sp_changerole_Status", parameters);
            return result;
        }
        public List<RoleModel> GetAllRoles(DataTablePaginationModel model)
        {
            try
            {
              
                List<RoleModel> datalist;
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@PageNumber", model.DtPageNumber);
                parameters.Add("@PageSize", model.DtPageSize);
                parameters.Add("@StrSearch", model.DtSearch != null ? model.DtSearch.Trim() : "");
                parameters.Add("@SortColumn", model.DtSortColumn);
                parameters.Add("@SortOrder", model.DtSortOrder);
                datalist = GetAll<RoleModel>("sp_GetRoleAllListContent", parameters);
                return datalist;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<RoleModel> GetRoleByRoleId(int? roleId)
        {
            int roles = Convert.ToInt32(roleId);
            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@RoleId", roles);
            var role = GetAll<RoleModel>("SP_GetRoleByRoleId", parameter);
            return role;
        }
        public List<RoleMenuMapModel> GetRoleById(int? roleId)
        {
            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@RoleId", roleId);
            var role = GetAll<RoleMenuMapModel>("sp_GetRoleById", parameter);
            return role;
        }

        public int AddEditRole(RoleModel roleModel)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@RoleId", roleModel.RoleId);
            parameters.Add("@RoleName", roleModel.RoleName);
            parameters.Add("@IsActive", roleModel.IsActive);
            var x = Get<RoleModel>("SP_InsertUpdateRole", parameters);
            return x.RoleId;
        }

        public int UpdateRole(List<RoleMenuMapModel> roleModel)
        {
            try
            {
                foreach (var item in roleModel)
                {
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@RoleMenuMapId", item.RoleMenuMapId);
                    parameters.Add("@roleId", item.RoleId);
                    parameters.Add("@menuId", item.MenuId);
                    parameters.Add("@isView", item.IsView);
                    parameters.Add("@IsInsert", item.IsInsert);
                    parameters.Add("@isEdit", item.IsEdit);
                    parameters.Add("@isDelete", item.IsDelete);
                    parameters.Add("@IsChangeStatus", item.IsChangeStatus);
                    ExecuteStoredProcedure("sp_tbl_Role_addEdit", parameters);
                }
                return 1;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
