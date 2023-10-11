using QuoteCalculator.Service.Models;
using System.Collections.Generic;

namespace QuoteCalculator.Service.Repository
{
    public interface IQuotesListRepository
    {
        IEnumerable<serviceModel> getServiceList();
        IEnumerable<CountryModel> GetCountryList();
        IEnumerable<QuoteTypeModel> GetQuoteList();
        IEnumerable<CompanyModel> GetCompanyList();
    }
}
