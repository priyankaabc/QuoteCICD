using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuoteCalculatorAdmin.Helper
{
    public static class ExcelHelper
    {
        /// <summary>
        /// Check if excel file exists and it has correct excel sheet name
        /// </summary>
        /// <param name="file">Excel File</param>
        /// <param name="sheetName">Excel Worksheet name to validate</param>
        /// <returns></returns>
        public static ImportFileResult ValidateExcelFile(HttpPostedFileBase file, string sheetName)
        {
            if (file == null || file.ContentLength <= 0 || string.IsNullOrEmpty(file.FileName))
                return new ImportFileResult(false, "Flie not found.");

            using (var package = new ExcelPackage(file.InputStream))
            {
                if (package.Workbook == null || package.Workbook.Worksheets == null)
                    return new ImportFileResult(false, "Invalid Excel File - No workbook can be found.");
                if (package.Workbook.Worksheets.Count(x => x.Name.ToLower() == sheetName.ToLower()) == 0)
                    return new ImportFileResult(false, "The Excel file does no have a sheet with name: " + sheetName);
            }

            return new ImportFileResult(true, null);
        }

        /// <summary>
        /// Check if the excel file has all expected columns
        /// </summary>
        /// <param name="sheet">Excel worksheet</param>
        /// <param name="headers">all exceptd header text. The mehtod will check them in the exact sequence.</param>
        /// <returns></returns>
        public static ImportFileResult ValidateColumnHeaders(ExcelWorksheet sheet, List<string> headers)
        {
            //Check if sheet has enough rows and columms...
            if (sheet.Dimension.End.Row < 1)
                return new ImportFileResult(false, "No rows found to import.");

            if (sheet.Dimension.End.Column < headers.Count)
                return new ImportFileResult(false, "The file does no have all required columns [" + headers.Count + "].");

            for (int i = 0; i < headers.Count; i++)
            {
                string colName = GetStr(sheet, 1, i + 1);
                if (string.Compare(headers[i], colName, true) != 0)
                    return new ImportFileResult(false, "Unexpected column at index: " + (i + 1)
                            + ". Expected Column: [" + headers[i] + "]. Actual Column: [" + colName + "].");
            }

            return new ImportFileResult(true);
        }

        /// <summary>
        /// Get string from the excel cell
        /// </summary>
        /// <param name="sheet">Excel Sheet Name</param>
        /// <param name="rowNo">Cell Row No (1 based index)</param>
        /// <param name="colNo">Cell Column No (1 based index)</param>
        /// <param name="emptyIfNull">return empty string or null if the cell has no value</param>
        /// <returns></returns>
        public static string GetStr(ExcelWorksheet sheet, int rowNo, int colNo, bool emptyIfNull = true)
        {
            if (sheet == null)
                throw new ArgumentNullException("sheet");
            if (sheet.Dimension.End.Row < rowNo)
                throw new ArgumentException("Sheet does not have enough rows to return value");
            if (sheet.Dimension.End.Column < colNo)
                throw new ArgumentException("Sheet does not have enough columns to return value");
            object val = sheet.Cells[rowNo, colNo].Value;
            if (val != null)
                return val.ToString();
            return emptyIfNull ? "" : null;
        }

        /// <summary>
        /// Get value and convert to decimal. This will throw an exception if the cell does not have valid Decimal
        /// </summary>
        /// <param name="sheet">Excel Work sheet</param>
        /// <param name="rowNo">Cell Row No (1 based index)</param>
        /// <param name="colNo">Cell Column No (1 based index)</param>
        /// <returns></returns>
        public static decimal GetDecimal(ExcelWorksheet sheet, int rowNo, int colNo)
        {
            if (HasDecimal(sheet, rowNo, colNo))
                return Convert.ToDecimal(sheet.Cells[rowNo, colNo].Value);

            throw new Exception("Invalid Decimal Value. Column: " + colNo + ". Row: " + rowNo + ". Value: " + sheet.Cells[rowNo, colNo].Value);
        }

        /// <summary>
        /// Check if the cell has Valid Decimal value
        /// </summary>
        /// <param name="sheet">Excel Work sheet</param>
        /// <param name="rowNo">Cell Row No (1 based index)</param>
        /// <param name="colNo">Cell Column No (1 based index)</param>
        /// <returns></returns>
        public static bool HasDecimal(ExcelWorksheet sheet, int rowNo, int colNo)
        {
            if (sheet == null)
                throw new ArgumentNullException("sheet");
            if (sheet.Dimension.End.Row < rowNo)
                throw new ArgumentException("Sheet does not have enough rows to return value");
            if (sheet.Dimension.End.Column < colNo)
                throw new ArgumentException("Sheet does not have enough columns to return value");
            object objVal = sheet.Cells[rowNo, colNo].Value;

            return objVal != null && decimal.TryParse(objVal.ToString(), out decimal decimalVal);
        }
    }
}