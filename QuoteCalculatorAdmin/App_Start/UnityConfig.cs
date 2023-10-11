using System.Web.Mvc;
using Unity;
using Unity.Mvc5;
using QuoteCalculator.Service.Repository.UserRepository;
using QuoteCalculator.Service.Repository.RoleRepository;
using QuoteCalculator.Service.Repository.EmailRepository;
using QuoteCalculator.Service.Repository;
using QuoteCalculator.Service.Repository.HeadingContent;
using QuoteCalculator.Service.Repository.ChildContentMaster;
using QuoteCalculator.Service.Repository.OptOutRepository;
using QuoteCalculator.Service.Repository.VehicleRepository;
using QuoteCalculator.Service.Repository.CommonRepository;
using QuoteCalculator.Service.Repository.BaggageRepository;
using QuoteCalculator.Service.Repository.InternationalRemovalRepository;
using QuoteCalculator.Service.Repository.QuickQuoteItemsRepository;
using QuoteCalculator.Service.Repository.TradeQuote;
using QuoteCalculator.Service.Repository.ImportQuote;
using QuoteCalculator.Service.Repository.LoginRepository;
using QuoteCalculator.Service.Repository.HomeRepository;

namespace QuoteCalculatorAdmin
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();
            container.RegisterType<ILoginRepository, LoginRepository>();
            container.RegisterType<IUserRepository, UserRepository>();
            container.RegisterType<IRoleRepository, RoleRepository>();
            container.RegisterType<IEmailTemplateRepository, EmailTemplateRepository>();
            container.RegisterType<IHeadingContentRepository, HeadingContentRepository>();
            container.RegisterType<IQuotesListRepository, QuotesListRepository>();
            container.RegisterType<IChildContentMasterRepository, ChildContentMasterRepository>();
            container.RegisterType<IOptOutRepository, OptOutRepository>();
            container.RegisterType<IVehicleRepository, VehicleRepository>();
            container.RegisterType<ICommonRepository, CommonRepository>();
            container.RegisterType<IBaggageRepository, BaggageRepository>();
            container.RegisterType<IInternationalRemovalRepository, InternationalRemovalRepository>();
            container.RegisterType<IQuickQuoteItemsRepository, QuickQuoteItemsRepository>();
            container.RegisterType<ITradeQuoteRepository, TradeQuoteRepository>(); 
            container.RegisterType<IImportQuoteRepository, ImportQuoteRepository>(); 
            container.RegisterType<IHomeRepository, HomeRepository>(); 

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}