using QuoteCalculator.Service.Models;
using System.Collections.Generic;

namespace QuoteCalculator.Service.Repository.EmailRepository
{
    public interface IEmailTemplateRepository
    {
        List<EmailTemplateModel> GetAllTemplates(DataTablePaginationModel model);
        EmailTemplateModel GetTemplateById(int? templateId);
        int AddTemplate(EmailTemplateModel templateModel);
        int UpdateTemplate(EmailTemplateModel templateModel);
        int DeleteTemplate(int? templateId);
    }
}
