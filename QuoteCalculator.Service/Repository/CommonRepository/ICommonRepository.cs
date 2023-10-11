using QuoteCalculator.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuoteCalculator.Service.Repository.CommonRepository
{
    public interface ICommonRepository
    {
        List<CommonModel> GetList(string ListOf, int? companyId);
        List<CommonModel> GetServiceListCommon( bool IsTradeQuote);
        List<CommonModel> GetBranchByAgentId( int? AgentId);
        SP_GetCollectionDelivery_Result GetCollectionDelivery(long? quoteId, int? company);
    }
}
