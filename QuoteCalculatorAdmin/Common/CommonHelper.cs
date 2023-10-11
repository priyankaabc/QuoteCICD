using QuoteCalculatorAdmin.Data;
using QuoteCalculatorAdmin.Data.Repository;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Web;

namespace QuoteCalculatorAdmin.Common
{
    public class CommonHelper
    {
        public const string RegexEmail = @"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}";
        public const string RegexMobile = @"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$";
        public const string PleaseSelect = "---Select---";
        public const string DateFormat = "dd/MM/yyyy";
        public const string TimeFormate = "HH:mm";
        public static readonly ReadOnlyDictionary<string, object> CenterColumnStyle = new ReadOnlyDictionary<string, object>
            (new Dictionary<string, object> { { "align", "center" }, { "style", "text-align:center;vertical-align:middle !important;" } });
        public static readonly ReadOnlyDictionary<string, object> LeftColumnStyle = new ReadOnlyDictionary<string, object>
            (new Dictionary<string, object> { { "align", "left" }, { "style", "text-align:left;vertical-align:middle !important;" } });
        public const int PazeSize = 10;
        public static IEnumerable<int> PageSizes = new[] { 10, 20, 50, 100 };
        public static IEnumerable<int> SmallPageSizes = new[] { 5, 10, 20 };

        public static readonly ReadOnlyDictionary<string, object> ActionCenterColumnStyleWithCanEdit = new ReadOnlyDictionary<string, object>
            (new Dictionary<string, object> { { "align", "center" }, { "style", "text-align:center;vertical-align:middle !important;width:80px !important" }, { "width", "80px" } });

        public static readonly ReadOnlyDictionary<string, object> ActionCenterColumnStyleWithCanStatus = new ReadOnlyDictionary<string, object>
            (new Dictionary<string, object> { { "align", "center" }, { "style", "text-align:center;vertical-align:middle !important;width:120px !important" }, { "width", "120px" } });

        public static readonly ReadOnlyDictionary<string, object> StatusColumnStyle = new ReadOnlyDictionary<string, object>
            (new Dictionary<string, object> { { "align", "center" }, { "style", "text-align:center;vertical-align:middle !important;" }, { "width", "60px" } });

        public static readonly ReadOnlyDictionary<string, object> SmallColumnStyle = new ReadOnlyDictionary<string, object>
            (new Dictionary<string, object> { { "align", "center" }, { "style", "text-align:center;vertical-align:middle !important;" }, { "width", "30px" } });


        /// <summary>
        /// WeekOfYear
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static int WeekOfYear(DateTime date)
        {
            // var day = (int)CultureInfo.CurrentCulture.Calendar.GetDayOfWeek(date);
            //return CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(date.AddDays(4 - (day == 0 ? 7 : day)), CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            return CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(date, CultureInfo.CurrentCulture.DateTimeFormat.CalendarWeekRule, DayOfWeek.Monday);

        }

        /// <summary>
        /// GetFirstDateOfWeek
        /// </summary>
        /// <param name="dayInWeek"></param>
        /// <param name="firstDay"></param>
        /// <returns></returns>
        public static DateTime GetFirstDateOfWeek(DateTime dayInWeek, DayOfWeek firstDay)
        {
            var firstDayInWeek = dayInWeek.Date;
            while (firstDayInWeek.DayOfWeek != firstDay)
                firstDayInWeek = firstDayInWeek.AddDays(-1);

            return firstDayInWeek;
        }

        /// <summary>
        /// GetLastDateOfWeek
        /// </summary>
        /// <param name="dayInWeek"></param>
        /// <param name="firstDay"></param>
        /// <returns></returns>
        public static DateTime GetLastDateOfWeek(DateTime dayInWeek, DayOfWeek firstDay)
        {
            var lastDayInWeek = dayInWeek.Date;
            while (lastDayInWeek.DayOfWeek != firstDay)
                lastDayInWeek = lastDayInWeek.AddDays(1);

            return lastDayInWeek;
        }

        public static string GetErrorMessage(Exception ex, bool getStactRetrace = true)
        {
            try
            {
                var errorMessage = string.Empty;
                if (ex == null) return errorMessage;
                errorMessage = ex.Message;
                if (ex.InnerException != null)
                    errorMessage += Environment.NewLine + GetErrorMessage(ex.InnerException, getStactRetrace);
                if (getStactRetrace)
                    errorMessage += Environment.NewLine + ex.StackTrace;

                errorMessage = errorMessage.Replace("An error occurred while updating the entries. See the inner exception for details.", "");
                return errorMessage;
            }
            catch
            {
                return ex != null ? ex.Message : string.Empty;
            }
        }

        public static string GetDeleteErrorMessage(Exception ex, bool getStactRetrace = true)
        {
            try
            {
                var errorMessage = string.Empty;
                if (ex == null) return errorMessage;
                errorMessage = ex.Message;
                if (ex.InnerException != null)
                    errorMessage += Environment.NewLine + GetDeleteErrorMessage(ex.InnerException, getStactRetrace);
                if (getStactRetrace)
                    errorMessage += Environment.NewLine + ex.StackTrace;

                errorMessage = errorMessage.Replace("\"", " ");
                errorMessage = errorMessage.Replace("'", " ");
                return errorMessage;
            }
            catch
            {
                return ex != null ? ex.Message : string.Empty;
            }
        }

        public static string GetDeleteException(Exception exception)
        {
            string errorMessage = GetDeleteErrorMessage(exception, false);
            return errorMessage.Contains("The DELETE statement conflicted with the REFERENCE constraint") ? ParseDeleteMessage(errorMessage) : errorMessage;
        }

        private static string ParseDeleteMessage(string message)
        {
            try
            {
                const string str = "This record link to another record(s), you can not delete this record";
                return str;
            }
            catch
            {
                return message;
            }
        }

        public static List<tbl_Title> GetTitleList()
        {
            using (quotesEntities _dbContext = BaseContext.GetDbContext())
            {
                return _dbContext.tbl_Title.OrderBy(m => m.DisplayOrder).ToList();
            }
        }

        public static string Encode(string encodeMe)
        {
            byte[] encoded = System.Text.Encoding.UTF8.GetBytes(encodeMe);
            return Convert.ToBase64String(encoded);
        }

        public static string Decode(string decodeMe)
        {
            byte[] encoded = Convert.FromBase64String(decodeMe);
            return System.Text.Encoding.UTF8.GetString(encoded);
        }
    }
}