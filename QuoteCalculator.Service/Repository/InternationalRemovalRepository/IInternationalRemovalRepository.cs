using System.Collections.Generic;
using QuoteCalculator.Service.Models;

namespace QuoteCalculator.Service.Repository.InternationalRemovalRepository
{
    public interface IInternationalRemovalRepository
    {
        List<InternationalRemovalModel> GetInternationRemovalList(DataTablePaginationModel model, int CompanyId);
        InternationalRemovalModel GetInternationalRemovalById(int Id);
        int DeleteInternationalRemoval(int? Id);
        int UpdateInternationalRemoval(InternationalRemovalModel InternationalRemovalModel);
        IEnumerable<InternationalRemovalModel> GetAllInternationalRemoval();
        int GetInternationalRemovalCalculation(int? QuickQuoteItemId);
        RemovalQuoteCalculationModel GetRemovalQuoteCalculationById(int? QuoteId);
    }
}