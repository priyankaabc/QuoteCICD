using Dapper;
using QuoteCalculator.Service.Common;
using QuoteCalculator.Service.Models;
using System.Collections.Generic;

namespace QuoteCalculator.Service.Repository
{
    public class QuotesListRepository : BaseRepository, IQuotesListRepository
    {
        public IEnumerable<CompanyModel> GetCompanyList()
        {
            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@prmtable", "company");
            var companyList  = GetAll<CompanyModel>("sp_Get_list", parameter);
            return companyList;
        }

        public IEnumerable<CountryModel> GetCountryList()
        {
            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@prmtable", "country");
            var countryList = GetAll<CountryModel>("sp_Get_list", parameter);
            return countryList;
        }

        public IEnumerable<QuoteTypeModel> GetQuoteList()
        {
            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@prmtable", "Quote");
            var quoteTypeList = GetAll<QuoteTypeModel>("sp_Get_list", parameter);
            return quoteTypeList;
        }

        public IEnumerable<serviceModel> getServiceList()
        {
            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@prmtable", "service");
            var serviceList = GetAll<serviceModel>("sp_Get_list", parameter);
            return serviceList;
        }

    }
}
