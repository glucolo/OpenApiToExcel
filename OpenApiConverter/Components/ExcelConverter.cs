using ClosedXML.Excel;
using OpenApiConverter.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenApiConverter.Components
{
    public class ExcelConverter
    {
        public static void Export(string outputPath, IEnumerable<ParamRow> rows, string sheetName = "Parameters")
        {
            //using (var package = new OfficeOpenXml.ExcelPackage())
            //{
            //    var worksheet = package.Workbook.Worksheets.Add(sheetName);
            //    worksheet.Cells["A1"].LoadFromCollection(rows, true);
            //    package.SaveAs(new System.IO.FileInfo(filePath));
            //}
            var cols = new[] {
                "Service","Subsection","Input","Description","Mandatory","Format",
                "InputType","WSInputField","Format2","Notes","Mapping"
            };

            using var wb = new XLWorkbook();
            var ws = wb.AddWorksheet("API Params");
            // Header
            for (int c = 0; c < cols.Length; c++)
            {
                ws.Cell(1, c + 1).Value = cols[c] switch
                {
                    "InputType" => "Input Type",
                    "WSInputField" => "WS input field",
                    "Format2" => "Format",
                    _ => cols[c]
                };
                ws.Cell(1, c + 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#D9E1F2");
                ws.Cell(1, c + 1).Style.Font.Bold = true;
                ws.Cell(1, c + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell(1, c + 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            }
            // Stronger header for Input, Description, Mandatory, Mapping
            var strongCols = new[] { "Input", "Description", "Mandatory", "Mapping" };
            for (int c = 0; c < cols.Length; c++)
            {
                if (strongCols.Contains(cols[c]))
                {
                    ws.Cell(1, c + 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#2F5597");
                    ws.Cell(1, c + 1).Style.Font.FontColor = XLColor.White;
                }
            }
            ws.SheetView.FreezeRows(1);
            ws.Range(1, 1, 1, cols.Length).SetAutoFilter();

            int row = 2, colCount = cols.Length, serviceStart = row;
            string lastService = null;
            foreach (var r in rows)
            {
                if (string.IsNullOrEmpty(r.Service) && string.IsNullOrEmpty(r.Input) && string.IsNullOrEmpty(r.WSInputField))
                {
                    // Sezione Service: merge, colori, bordi ticky
                    if (lastService != null && serviceStart < row - 1)
                    {
                        ws.Range(serviceStart, 1, row - 1, 1).Merge();
                        ws.Cell(serviceStart, 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                        ws.Cell(serviceStart, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        ws.Cell(serviceStart, 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#B4C6E7");
                        for (int rr = serviceStart; rr < row; rr++)
                            ws.Cell(rr, 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#B4C6E7");
                        var rng = ws.Range(serviceStart, 1, row - 1, colCount);
                        rng.Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
                        rng.Style.Border.InsideBorder = XLBorderStyleValues.Hair;
                    }
                    row++;
                    lastService = null;
                    serviceStart = row;
                    continue;
                }
                for (int c = 0; c < cols.Length; c++)
                {
                    var cv = cols[c];
                    var val = typeof(ParamRow).GetProperty(cv)?.GetValue(r, null)?.ToString() ?? "";
                    ws.Cell(row, c + 1).Value = val;
                    if (cv == "Input" || cv == "Description" || cv == "InputType" || cv == "WSInputField")
                        ws.Cell(row, c + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    else if (cv == "Mandatory" || cv == "Format" || cv == "Format2")
                        ws.Cell(row, c + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    else
                        ws.Cell(row, c + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                }
                if (!string.IsNullOrEmpty(r.Service))
                {
                    if (lastService == null)
                    {
                        lastService = r.Service;
                        serviceStart = row;
                    }
                }
                row++;
            }
            ws.Columns().AdjustToContents();
            wb.SaveAs(outputPath);
            Console.WriteLine($"Excel file generated: {outputPath}");
        }
    }
}
