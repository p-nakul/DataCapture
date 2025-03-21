using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using DataCapture.Models;

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
                //sheetName = GetUniqueSheetName(workbook, sheetName);
                // Check if the sheet already exists; if not, create it
                var worksheet = workbook.Worksheets.FirstOrDefault(ws => ws.Name == sheetName)
                                ?? workbook.Worksheets.Add(sheetName);

                int row = worksheet.LastRowUsed()?.RowNumber() + 1 ?? 1;

                foreach (var pair in data)
                {
                    worksheet.Cell(row, 1).Value = pair.Key;
                    worksheet.Cell(row, 2).Value = pair.Value;
                    row++;
                }

                workbook.Save();
            }
        }


        public static void WriteHtmlSectionsToExcel(string filePath, string sheetName, List<HtmlSection> sections)
        {
            sheetName = SanitizeSheetName(sheetName);

            if (!File.Exists(filePath))
            {
                CreateExcelFile(filePath);
            }

            using (var workbook = new XLWorkbook(filePath))
            {
                sheetName = SanitizeSheetName(sheetName);
                var worksheet = workbook.Worksheets.FirstOrDefault(ws => ws.Name == sheetName)
                                ?? workbook.Worksheets.Add(sheetName);

                int row = worksheet.LastRowUsed()?.RowNumber() + 1 ?? 1;

                foreach (var section in sections)
                {
                    // Write H3 header
                    worksheet.Cell(row, 1).Value = section.Header;
                    int mergeColumns = section.Table.FirstOrDefault()?.Count ?? 1;
                    if (mergeColumns > 1)
                    {
                        worksheet.Range(row, 1, row, mergeColumns).Merge();
                    }
                    worksheet.Cell(row, 1).Style.Font.Bold = true;
                    row++;

                    // Write text content (if exists)
                    if (!string.IsNullOrWhiteSpace(section.TextContent))
                    {
                        worksheet.Cell(row, 1).Value = section.TextContent;
                        worksheet.Range(row, 1, row, mergeColumns).Merge();
                        worksheet.Cell(row, 1).Style.Font.Italic = true;
                        row++;
                    }

                    // Write table rows
                    foreach (var tableRow in section.Table)
                    {
                        for (int col = 0; col < tableRow.Count; col++)
                        {
                            worksheet.Cell(row, col + 1).Value = tableRow[col];
                        }
                        row++;
                    }

                    row++; // Add empty row between sections
                }

                workbook.Save();
            }


        }
    }
}
