using QuoteCalculator.Service.Models;
using System.Collections.Generic;

namespace QuoteCalculator.Service.Repository.TradeQuote
{
    public interface ITradeQuoteRepository
    {
        List<TradeQuoteModel> GetTradeQuoteList(DataTablePaginationModel model,int CompanyId);
        List<AddtionalCostModel> GetAdditionalCost(long? TradeQuoteId);
        List<AddtionalCostModel> GetTradeCostData(long? TradeQuoteId);
        TradeQuoteModel GetTradeQuoteById(int? ID);
        TradeQuoteModel GetTradeQuoteXMLData(long? TradeQuoteId);
        int UpdateTradeQuote(TradeQuoteModel model);
        int InsertAdditionalCost(TradeQuoteAddCost model);
        void TradesCalculateRate(int? paramTradeQuoteId);
        List<TradeQuoteModel> GetDestCodeByCountryId(string CountryCode); 
    }
}
