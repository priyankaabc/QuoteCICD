using QuoteCalculator.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuoteCalculator.Service.Repository.EmailRepository
{
    public interface IEmailSettingsRepository
    {
        //List<EmailSettingsModel> GetEmailSettings(DataTablePaginationModel model);

        List<EmailSettingsModel> GetEmailSettings();

        string GetValueByKey(string key);
    }
}
