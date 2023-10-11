using QuoteCalculator.Service.Models;
using System.Collections.Generic;

namespace QuoteCalculator.Service.Repository.HeadingContent
{
    public interface IHeadingContentRepository
    {
        List<HeadingContentModel> GetAllContents(DataTablePaginationModel model);
        HeadingContentModel GetContentById(int? userId);
        int AddContent(HeadingContentModel contentModel);
        int UpdateContent(HeadingContentModel contentModel);
        int DeleteContent(int? userId);
    }
}
