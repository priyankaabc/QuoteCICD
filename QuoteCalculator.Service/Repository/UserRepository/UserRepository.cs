using Dapper;
using QuoteCalculator.Service.Common;
using QuoteCalculator.Service.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace QuoteCalculator.Service.Repository.UserRepository
{
    public class UserRepository : BaseRepository, IUserRepository
    {

        public int AddUser(UserModel userModel)
        {
            return addupdateUser(userModel);
        }

        public int DeleteUser(int? userId)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", userId);
            int result = ExecuteStoredProcedure("sp_quotes_user_Delete", parameters);
            return result;
        }

        public int ChangeStatus(int? userId,bool IsActive)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", userId);
            parameters.Add("@IsActive", !IsActive);
            int result = ExecuteStoredProcedure("sp_changeuser_Status", parameters);
            return result;
        }
        
        public List<UserModel> GetAllUsers(DataTablePaginationModel model)
         {

            try
            {
                List<UserModel> datalist;
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@PageNumber", model.DtPageNumber);
                parameters.Add("@PageSize", model.DtPageSize);
                parameters.Add("@StrSearch", model.DtSearch != null ? model.DtSearch.Trim():"");
                parameters.Add("@SortColumn", model.DtSortColumn);
                parameters.Add("@SortOrder", model.DtSortOrder);
                datalist = GetAll<UserModel>("sp_GetUserList", parameters);
                return datalist;

            }
            catch(Exception ex)
            {
                throw ex;
            }
           
        }

        public UserModel GetUserById(int? userId)
        {
            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@prmId", userId);

            var user = Get<UserModel>("sp_GetUserById", parameter);
            return user;
        }

        public int UpdateUser(UserModel userModel)
        {
            return addupdateUser(userModel);
        }

        public int addupdateUser(UserModel userModel)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", userModel.id);
            parameters.Add("@UserName", userModel.username);
            parameters.Add("@email", userModel.email);
            parameters.Add("@RoleId", userModel.RoleId);
            parameters.Add("@companyId", userModel.CompanyId);
            parameters.Add("@RepCode", userModel.SalesRepCode);
            parameters.Add("@password", userModel.password);
            parameters.Add("@IsActive", userModel.IsActive);
            int result = ExecuteStoredProcedure("sp_quotes_user_addEdit", parameters);
            return result;
        }
    }
}
