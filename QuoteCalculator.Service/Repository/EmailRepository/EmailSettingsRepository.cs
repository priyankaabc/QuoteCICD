using Dapper;
using QuoteCalculator.Service.Common;
using QuoteCalculator.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuoteCalculator.Service.Repository.EmailRepository
{
    public class EmailSettingsRepository : BaseRepository, IEmailSettingsRepository
    {
        public List<EmailSettingsModel> GetEmailSettings()
        {
            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@prmtable", "EmailSettings");
            var EmailSettingsList = GetAll<EmailSettingsModel>("sp_Get_list", parameter);
            return EmailSettingsList;
        }

        public string GetValueByKey(string key)
        {
            var result = "";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@key", key);
            var emailSettings = Get<EmailSettingsModel>("SP_GetValueByKey", parameters);
            result = emailSettings.Value;
            return result;
        }
    }
}
