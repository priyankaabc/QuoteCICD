using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuoteCalculatorAdmin.Common
{
    public class NotificationMessage
    {
        public string Message { get; set; }
        public CustomEnums.NotifyType Type { get; set; }

        public NotificationMessage(string msg, CustomEnums.NotifyType errorType)
        {
            Message = msg;
            Type = errorType;
        }
    }
}