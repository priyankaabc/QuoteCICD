using QuoteCalculator.Service.Helper;
using QuoteCalculator.Service.Models;
using QuoteCalculator.Service.Repository.LoginRepository;
using QuoteCalculator.Service.Repository.UserRepository;
using QuoteCalculatorAdmin.Common;
using QuoteCalculatorAdmin.Data;
using QuoteCalculatorAdmin.Data.Repository;
using QuoteCalculatorAdmin.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace QuoteCalculatorAdmin.Controllers
{
    public class LoginController : Controller
    {
        #region private variables
        private readonly GenericRepository<user> _dbRepository;
        private readonly ILoginRepository  _loginRepository;
        private readonly IUserRepository _userRepository;

        private IFormsAuthenticationService FormsService { get; set; }
        #endregion

        #region Constructor
        public LoginController(ILoginRepository loginRepository, IUserRepository userRepository)
        {
            _dbRepository = new GenericRepository<user>();
            _loginRepository = loginRepository;
            _userRepository = userRepository;
        }
        #endregion

        #region Methods
        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            if (FormsService == null)
            {
                FormsService = new FormsAuthenticationService();
            }

            base.Initialize(requestContext);
        }

        public ActionResult SessionTimeout()
        {
            return View();
        }

        public ActionResult Index()
        {
           return View();
        }

        [HttpPost]
        public ActionResult Index(UserLoginModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View("Index", model);
                }
                model.Password = Security.Encrypt(Convert.ToString(model.Password));

                //user logedInUser = _dbRepository.GetEntities().FirstOrDefault(m => m.email == model.Email && m.password == model.Password && m.IsActive == true);
                QuoteCalculator.Service.Models.UserModel logedInUser = _loginRepository.UserLogin(model);

                if (logedInUser == null)
                {
                    ModelState.AddModelError("Email", "Invalid Login Credentials.");
                    return View(model);
                }
               
                
             SessionHelper.UserId = Convert.ToInt32(logedInUser.id);
                SessionHelper.WelcomeUser = logedInUser.username;
                SessionHelper.Email = logedInUser.email;
                SessionHelper.RoleId = logedInUser.RoleId;
                SessionHelper.CompanyId = Convert.ToInt32(logedInUser.CompanyId);
                SessionHelper.UserAccessPermissions = CustomRepository.UserAccessPermissions(SessionHelper.RoleId);
                SessionHelper.SalesRepCode = logedInUser.SalesRepCode;

                FormsService.SignIn(logedInUser.username, model.RememberMe);

                var result = SessionHelper.UserAccessPermissions.FirstOrDefault();
                if (result !=null && result.Controller == null)
                {
                    var data= SessionHelper.UserAccessPermissions.Where(m => m.ParentMenuId == null && m.Controller == null).OrderBy(m => m.DisplayOrder).FirstOrDefault();
                    var parentmenuid = data != null? data.MenuId : 0;
                    sp_UserAccessPermissions_Result menulist = SessionHelper.UserAccessPermissions.Where(m => m.ParentMenuId == parentmenuid).OrderBy(m => m.DisplayOrder).FirstOrDefault();
                    if (menulist != null)
                    {
                        return RedirectToAction(menulist.Action, menulist.Controller);
                    }
                    else
                    {
                        return View("Index", model);
                    }
                }
                else
                {
                    sp_UserAccessPermissions_Result menulist = SessionHelper.UserAccessPermissions.FirstOrDefault();
                    if (menulist != null)
                    {
                        return RedirectToAction(menulist.Action, menulist.Controller);
                    }
                    else
                    {
                        return View("Index", model);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData[CustomEnums.NotifyType.Error.GetDescription()] = ex.Message;

                return View("Index", model);
            }


        }

        public ActionResult Logout()
        {
            FormsService.SignOut();
            Session.Abandon();
            return RedirectToAction("Index", "Login");
        }

        public ActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ForgotPassword(string Email)
        {

            try
            {
                int companyId = SessionHelper.CompanyId;
                //user model = _dbRepository.GetEntities().FirstOrDefault(m => m.email == Email);
                QuoteCalculator.Service.Models.UserModel model = _loginRepository.ForgotPassword(Email);

                string token = Guid.NewGuid().ToString();
                if (model != null &&  model.id > 0)

                {
                    string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/";

                    string link = Url.Action("ResetPassword", "Login", new RouteValueDictionary(new { Token = token }), System.Web.HttpContext.Current.Request.Url.Scheme);

                    string resetLink = "<a href='" + link + "'>" + link + "</a>";
                    string toEmail = model.email;


                    string bodyTemplate = System.IO.File.ReadAllText(Server.MapPath("~/Template/ForgotPassword.html"));

                    bodyTemplate = bodyTemplate.Replace("[@link]", resetLink);
                    bodyTemplate = bodyTemplate.Replace("[@Mainlink]", link);
                    bodyTemplate = bodyTemplate.Replace("[@FooterLogo]", baseUrl + "Content/img/logo.png");

                    Task task = new Task(() => EmailHelper.SendAsyncEmail(companyId, toEmail, "ForgotPassword", bodyTemplate, "EmailUserName", "DisplayAngloPacific", true));
                    task.Start();


                    model.Token = token;
                    model.TokenExpiryDate = DateTime.Now.AddDays(1);
                    foreach (ModelState modelState in ViewData.ModelState.Values)
                    {
                        foreach (ModelError error in modelState.Errors)
                        {
                            var er = error;
                        }
                    }
                    //_dbRepository.Update(model);
                    _loginRepository.UpdateTokenDetails(model);

                    TempData[CustomEnums.NotifyType.Success.GetDescription()] = "Email For Reset Passsword has been sent. Please check your email.";
                    return RedirectToAction("Index");

                }
                else
                {
                    TempData[CustomEnums.NotifyType.Error.GetDescription()] = "Please enter valid Email Address.";

                     return RedirectToAction("Index", "Login");

                }
            }
            catch (Exception ex)
            {
                TempData[CustomEnums.NotifyType.Error.GetDescription()] =  ex.Message;

                return RedirectToAction("ForgotPassword", "Login");
            }
        }

        public ActionResult ResetPassword(string Token)
        {
            try
            {
                //user userModel = _dbRepository.GetEntities().FirstOrDefault(m => m.Token == Token);
                QuoteCalculator.Service.Models.UserModel userModel = _loginRepository.GetUserByToken(Token);

                if (userModel != null)
                {
                    if (userModel.id > 0 && userModel.TokenExpiryDate >= DateTime.Now)
                    {
                        ResetpasswordModel model = new ResetpasswordModel { Id = userModel.id };
                        return View(model);
                    }

                    if (userModel.TokenExpiryDate < DateTime.Now)
                    {
                        TempData["Message"] = "Token for reset password has been expired.please try agian to reset password.";
                        return RedirectToAction("Index", "Login");
                    }
                    TempData[CustomEnums.NotifyType.Error.GetDescription()] = "Something Went wrong. Please try again later.";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData[CustomEnums.NotifyType.Error.GetDescription()] = "Link has been expired";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData[CustomEnums.NotifyType.Error.GetDescription()] = ex.Message;

                return RedirectToAction("ForgotPassword", "Login");
            }

        }

        [HttpPost]
        public ActionResult ResetPassword(ResetpasswordModel userModel)
        {
            try
            {
                if (userModel.ConfirmPassword != userModel.Password)
                {
                    TempData[CustomEnums.NotifyType.Error.GetDescription()] = "Password and Confirm Password must be same.";
                    return RedirectToAction("Index", "Login");
                }

                //user model = _dbRepository.GetEntities().FirstOrDefault(m => m.id == userModel.Id);
                QuoteCalculator.Service.Models.UserModel model = _loginRepository.GetUserById(userModel.Id);

                if (model != null)
                {
                    model.Token = null;
                    model.TokenExpiryDate = null;
                    model.password = userModel.Password;
                    model.password = Security.Encrypt(Convert.ToString(userModel.Password));
                   int result= _userRepository.UpdateUser(model);
                    //_dbRepository.Update(model);
                }
                if (model != null && model.id > 0)
                {
                    TempData[CustomEnums.NotifyType.Success.GetDescription()] = "Password reset successful.";
                    return RedirectToAction("Index", "Login");
                }
                TempData[CustomEnums.NotifyType.Error.GetDescription()] = "Something went wrong. Please try again later.";
                return RedirectToAction("Index", "Login");
            }
            catch (Exception ex)
            {
                TempData[CustomEnums.NotifyType.Error.GetDescription()] = ex.Message;
                return RedirectToAction("Index", "Login");
            }
        }

        #endregion
        #region Form Authentication
        public class FormsAuthenticationService : IFormsAuthenticationService
          {
              public void SignIn(string userName, bool createPersistentCookie)
              {
                  if (string.IsNullOrEmpty(userName))
                  {
                      throw new ArgumentException(string.Empty, nameof(userName));
                  }
          
                  FormsAuthentication.SetAuthCookie(userName, createPersistentCookie);
              }
          
              public void SignOut()
              {
                  FormsAuthentication.SignOut();
              }
          }
          
          public interface IFormsAuthenticationService
          {
              void SignIn(string userName, bool createPersistentCookie);
          
              void SignOut();
          }
          #endregion
    }
}