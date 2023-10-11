using System.Collections.Generic;
using QuoteCalculator.Service.Models;


namespace QuoteCalculator.Service.Repository.ImportQuote
{
    public interface IImportQuoteRepository
    {
        List<ImportsQuoteModel> GetImportsQuoteList(DataTablePaginationModel model, int CompanyId);
        ImportsQuoteModel GetImportsQuoteById(int? Id);
        List<AddtionalCostModel> GetAdditionalCost();
        long UpdateImportQuote(ImportsQuoteModel model);
        int InsertUpdateCost(long ImportQuoteId, double TotalCost, double UpdatedCost);
        void ImportCalculateRate(long QuoteId);
        CommonModel GetBranchById(long POEId);
        List<DestinationDetails> GetImportConsigneeData(long? ImportQuoteId);
        ImportsQuoteModel GetImportQuoteXMLData(long? ImportQuoteId);
        List<AddtionalCostModel> GetImportCostData(long? ImportQuoteId);
        List<DestinationDetails> GetImportsDestinationById(long? Id);
        int DeleteDestinationDetails(int? CompanyId, int? DestinationId);
        List<AddtionalCostModel> GetAdditionalCostByDestId(long DestId);
        long InsertUpdateImportsDestination(DestinationDetails model, long? ImportQuoteId);
        int InsertAdditionalCost(ImportsQuoteAddCost model);
    }
}