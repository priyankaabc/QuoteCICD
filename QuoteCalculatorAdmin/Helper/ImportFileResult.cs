using QuoteCalculatorAdmin.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuoteCalculatorAdmin.Helper
{
    public class ImportFileResult
    {
        public bool IsSuccess { get; set; }
        public string ReturnMessage { get; set; }
        public string ImportLog { get; set; }
        public int ImportRecordsCount { get; set; }

        public ImportFileResult(bool isSuccess) : this(isSuccess, null, null, 0)
        {
        }
        public ImportFileResult(bool isSuccess, string returnMessage) : this(isSuccess, returnMessage, null,0)
        {
        }
        public ImportFileResult(bool isSuccess, string returnMessage, string importLog):this(isSuccess,returnMessage, importLog,0)
        {

        }
        public ImportFileResult(bool isSuccess, string returnMessage, string importLog, int importRecordsCount)
        {
            IsSuccess = isSuccess;
            ReturnMessage = returnMessage;
            ImportLog = importLog;
            ImportRecordsCount = importRecordsCount;
        }

        public string NotificationDescription
        {
            get
            {
                if (IsSuccess)
                    return CustomEnums.NotifyType.Success.GetDescription();
                else
                    return CustomEnums.NotifyType.Error.GetDescription();
            }
        }
    }
}