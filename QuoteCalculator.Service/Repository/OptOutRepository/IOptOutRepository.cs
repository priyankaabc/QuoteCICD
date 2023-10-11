using QuoteCalculator.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuoteCalculator.Service.Repository.OptOutRepository
{
    public interface IOptOutRepository
    {
        List<OptOutModel> GetOptOutList(DataTablePaginationModel model);
        long ChangeStatus(string Email, int LoggedInUserId);
    }
}
