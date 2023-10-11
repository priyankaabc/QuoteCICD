using QuoteCalculator.Service.Models;
using System.Collections.Generic;

namespace QuoteCalculator.Service.Repository.ChildContentMaster
{
    public interface IChildContentMasterRepository
    {
        List<ContentMasterModel> GetAllContents(DataTablePaginationModel model);
        ContentMasterModel GetChildContentById(int? userId);
        int AddContent(ContentMasterModel contentModel);
        int UpdateContent(ContentMasterModel contentModel);
        int DeleteContent(int? userId);
    }
}
