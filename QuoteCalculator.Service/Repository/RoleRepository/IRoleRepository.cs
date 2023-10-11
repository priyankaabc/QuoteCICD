using QuoteCalculator.Service.Models;
using System.Collections.Generic;

namespace QuoteCalculator.Service.Repository.RoleRepository
{
    public interface IRoleRepository
    {
        List<RoleModel> GetAllRoles(DataTablePaginationModel model);
        List<RoleMenuMapModel> GetRoleById(int? roleId);
        int AddRole(List<RoleMenuMapModel> roleModel);
        int UpdateRole(List<RoleMenuMapModel> roleModel);
        int DeleteRole(int? roleId);
        int AddEditRole(RoleModel roleModel);
        List<RoleModel> GetRoleByRoleId(int? roleId);
        int ChangeStatus(int? roleId, bool IsActive);
    }
}
