using QuoteCalculator.Service.Models;
using System.Collections.Generic;

namespace QuoteCalculator.Service.Repository.UserRepository
{
    public interface IUserRepository
    {
        List<UserModel> GetAllUsers(DataTablePaginationModel model);
        UserModel GetUserById(int? userId);
        int AddUser(UserModel userModel);
        int UpdateUser(UserModel userModel);
        int DeleteUser(int? userId);
        int ChangeStatus(int? userId,bool IsActive);
    }

    
}