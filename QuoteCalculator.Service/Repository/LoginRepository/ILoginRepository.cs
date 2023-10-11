using QuoteCalculator.Service.Models;


namespace QuoteCalculator.Service.Repository.LoginRepository
{
    public interface ILoginRepository
    {
        UserModel UserLogin(UserLoginModel model);
        UserModel ForgotPassword(string email);
        int UpdateTokenDetails(UserModel userModel);
        UserModel GetUserByToken(string Token);
        UserModel GetUserById(long userId);
    }
}
