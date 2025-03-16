using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DataCapture
{
    internal class Excelhelper
    {
        private static string SanitizeSheetName(string sheetName)
        {
            if (string.IsNullOrWhiteSpace(sheetName))
                return "Sheet1"; // Default if empty

            // Remove invalid characters
            string sanitized = new string(sheetName.Where(c => !"/\\?*[]".Contains(c)).ToArray());

            // Trim to 31 characters (Excel limit)
            return sanitized.Length > 31 ? sanitized.Substring(0, 31) : sanitized;
        }
        /// <summary>
        /// Creates an Excel file with a specified sheet name.
        /// </summary>
        public static void CreateExcelFile(string filePath)
        {
            using (var workbook = new XLWorkbook())
            {
                // ClosedXML requires at least one worksheet to save a workbook.
                var tempSheet = workbook.Worksheets.Add("1");

                // Save the file, overwriting if it already exists.
                workbook.SaveAs(filePath);
            }
        }

        /// <summary>
        /// Ensures the sheet name is unique within the workbook.
        /// </summary>
        private static string GetUniqueSheetName(XLWorkbook workbook, string sheetName)
        {
            string baseName = sheetName;
            int counter = 1;

            while (workbook.Worksheets.Any(ws => ws.Name == sheetName))
            {
                sheetName = baseName.Length > 28 ? baseName.Substring(0, 28) + $"_{counter}" : baseName + $"_{counter}";
                counter++;
            }

            return sheetName;
        }

        /// <summary>
        /// Writes a list of values to an Excel sheet.
        /// </summary>
        public static void WriteListToExcel(string filePath, string sheetName, List<string> data)
        {
            sheetName = SanitizeSheetName(sheetName);
            if (!File.Exists(filePath))
            {
                CreateExcelFile(filePath);
            }

            using (var workbook = new XLWorkbook(filePath))
            {
                var worksheet = workbook.Worksheets.Worksheet(sheetName);

                for (int i = 0; i < data.Count; i++)
                {
                    worksheet.Cell(i + 1, 1).Value = data[i];
                }

                workbook.Save();
            }
        }

        /// <summary>
        /// Writes a dictionary (key-value pairs) to an Excel sheet.
        /// </summary>
        public static void WriteDictionaryToExcel(string filePath, string sheetName, Dictionary<string, string> data)
        {
            sheetName = SanitizeSheetName(sheetName);

            if (!File.Exists(filePath))
            {
                CreateExcelFile(filePath);
            }

            using (var workbook = new XLWorkbook(filePath))
            {
                sheetName = SanitizeSheetName(sheetName);
                sheetName = GetUniqueSheetName(workbook, sheetName);
                // Check if the sheet already exists; if not, create it
                var worksheet = workbook.Worksheets.FirstOrDefault(ws => ws.Name == sheetName)
                                ?? workbook.Worksheets.Add(sheetName);

                int row = 1;

                foreach (var pair in data)
                {
                    worksheet.Cell(row, 1).Value = pair.Key;
                    worksheet.Cell(row, 2).Value = pair.Value;
                    row++;
                }

                workbook.Save();
            }
        }
    }
}
