using Dapper;
using QuoteCalculator.Service.Common;
using QuoteCalculator.Service.Models;

namespace QuoteCalculator.Service.Repository.LoginRepository
{
    public class LoginRepository : BaseRepository, ILoginRepository
    {
        public UserModel UserLogin(UserLoginModel model)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@email", model.Email);
            parameters.Add("@password", model.Password);
            UserModel result = Get<UserModel>("SP_UserLogin", parameters);
            return result;
        }
        public UserModel ForgotPassword(string email)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@email", email);
            UserModel result = Get<UserModel>("SP_ForgotPassword", parameters);
            return result;
        }
        public int UpdateTokenDetails(UserModel userModel)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", userModel.id);
            parameters.Add("@Token", userModel.Token);
            parameters.Add("@TokenExpiryDate", userModel.TokenExpiryDate);
            
            int result = ExecuteStoredProcedure("sp_update_Forgotpwd_Detail", parameters);
            return result;
        }

        public UserModel GetUserByToken(string Token)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Token", Token);

            UserModel result = Get<UserModel>("SP_GetUserByToken", parameters);
            return result;
        }

        public UserModel GetUserById(long userId)
        {
            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@Id", userId);

            var user = Get<UserModel>("SP_GetUserBy_Id", parameter);
            return user;
        }
    }
}
