using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.OpenApi;
using Newtonsoft.Json.Linq;
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
        public static XLWorkbook CreateExcelWorkbook()
        {
            var workbook = new XLWorkbook();
            return workbook;
            //GenerateOpenApiSheet(document, workbook);
        }
        public static void SaveExcelWorkbook(XLWorkbook workbook, string filePath)
        {
            workbook.SaveAs(filePath);
        }
        public static void GenerateEndpointsSheet(OpenApiDocument document, XLWorkbook workbook)
        {
            var worksheet = workbook.Worksheets.Add("Services");
            var visitor = new EndPointListVisitor();
            document.Accept(visitor);
            PopulateEndpoints(worksheet, visitor.GetEndpoints());
            worksheet.Columns().AdjustToContents();
        }

        public static void GenerateRequestsSheet(OpenApiDocument document, XLWorkbook workbook)
        {
            var worksheet = workbook.Worksheets.Add("Requests Openapi");
            var visitor = new RequestsVisitor();
            document.Accept(visitor);
            var rows = visitor.GetRequests();
            PopulateRequests(worksheet, rows);
            worksheet.Columns().AdjustToContents();
        }

        public static void GenerateResponsesSheets(OpenApiDocument doc, XLWorkbook wb)
        {
            var worksheet = wb.Worksheets.Add("Responses Gipsy");
            var visitor = new ResponsesVisitor();
            doc.Accept(visitor);
            var rows = visitor.GetResponses();
            PopulateResponses(worksheet, rows);
            worksheet.Columns().AdjustToContents();
        }



        private static void PopulateEndpoints(IXLWorksheet worksheet, IList<EndpointRow> endpointRows)
        {
            // Header
            var cols = new[] { "Service", "Description", "Resource", "http method", "path" };
            GenerateHeader(worksheet, cols);

            int row = 2;
            foreach (var pathItem in endpointRows)
            {
                worksheet.Cell(row, 1).Value = pathItem.Service;
                worksheet.Cell(row, 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#B4C6E7");
                worksheet.Cell(row, 2).Value = pathItem.Description;
                worksheet.Cell(row, 3).Value = pathItem.Resource;
                worksheet.Cell(row, 4).Value = pathItem.HttpMethod;
                worksheet.Cell(row, 5).Value = pathItem.Path;

                row++;
            }
        }

        private static void PopulateRequests(IXLWorksheet worksheet, IList<ParamRow> paramRows)
        {
            var cols = new[] {
                "Service","Subsection","Input","Description","Mandatory","Format",
                "InputType","WSInputField","Format2","Notes","Mapping"
            };
            GenerateHeader(worksheet, cols, new[] { "Input", "Description", "Mandatory", "Mapping" });
            int row = 2, colCount = cols.Length, serviceStart = row;
            string lastService = null;
            foreach (var r in paramRows)
            {
                if (string.IsNullOrEmpty(r.Service) && string.IsNullOrEmpty(r.Input) && string.IsNullOrEmpty(r.WSInputField))
                {
                    // Sezione Service: merge, colori, bordi ticky
                    MergeServiceSection(worksheet, ref row, colCount, ref serviceStart, ref lastService);
                    continue;
                }
                //Allineamento delle celle
                for (int c = 0; c < cols.Length; c++)
                {
                    var cv = cols[c];
                    var val = typeof(ParamRow).GetProperty(cv)?.GetValue(r, null)?.ToString() ?? "";
                    worksheet.Cell(row, c + 1).Value = val;
                    if (cv == "Input" || cv == "Description" || cv == "InputType" || cv == "WSInputField")
                        worksheet.Cell(row, c + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    else if (cv == "Mandatory" || cv == "Format" || cv == "Format2")
                        worksheet.Cell(row, c + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    else
                        worksheet.Cell(row, c + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
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
        }

        private static void PopulateResponses(IXLWorksheet worksheet, IList<ResponseRow> responseRows)
        {
            var cols = new[] {
                "Service","Subsection","Output","Description","Format",
                "OutputType","WSOutputField","Format2","Notes","Mapping"
            };
            GenerateHeader(worksheet, cols, new[] { "Output", "Description", "Mapping" });
            int row = 2, colCount = cols.Length, serviceStart = row;
            string lastService = null;
            foreach (var r in responseRows)
            {
                if (string.IsNullOrEmpty(r.Service) && string.IsNullOrEmpty(r.Output) && string.IsNullOrEmpty(r.WSOutputField))
                {
                    // Sezione Service: merge, colori, bordi ticky
                    MergeServiceSection(worksheet, ref row, colCount, ref serviceStart, ref lastService);
                    continue;
                }
                //Allineamento delle celle
                for (int c = 0; c < cols.Length; c++)
                {
                    var cv = cols[c];
                    var val = typeof(ResponseRow).GetProperty(cv)?.GetValue(r, null)?.ToString() ?? "";
                    worksheet.Cell(row, c + 1).Value = val;
                    if (cv == "Subsection")
                        worksheet.Cell(row, c + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    else
                        worksheet.Cell(row, c + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
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
        }

        private static void MergeServiceSection(IXLWorksheet worksheet, ref int row, int colCount, ref int serviceStart, ref string lastService)
        {
            if (lastService != null && serviceStart <= row - 1)
            {
                worksheet.Range(serviceStart, 1, row - 1, 1).Merge();
                worksheet.Cell(serviceStart, 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                worksheet.Cell(serviceStart, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                worksheet.Cell(serviceStart, 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#B4C6E7");
                for (int rr = serviceStart; rr < row; rr++)
                    worksheet.Cell(rr, 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#B4C6E7");
                var rng = worksheet.Range(serviceStart, 1, row - 1, colCount);
                rng.Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
                rng.Style.Border.InsideBorder = XLBorderStyleValues.Hair;
            }
            row++;
            lastService = null;
            serviceStart = row;
        }

        private static void GenerateHeader(IXLWorksheet ws, string[] cols, string[]? strong = null)
        {
            strong ??= Array.Empty<string>();

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

                if (strong.Contains(cols[c]))
                {
                    ws.Cell(1, c + 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#2F5597");
                    ws.Cell(1, c + 1).Style.Font.FontColor = XLColor.White;
                }
            }
            ws.SheetView.FreezeRows(1);
            ws.Range(1, 1, 1, cols.Length).SetAutoFilter();
        }

        [Obsolete("Use ProcessApiDoc instead")]
        static IList<ParamRow> ProcessJson(JObject? swagger)
        {
            var rows = new List<ParamRow>();
            var definitions = swagger["definitions"] as JObject ?? new JObject();
            var visitor = new ParamRowVisitor();

            foreach (var path in swagger["paths"])
            {
                foreach (var method in (path.First as JObject).Properties())
                {
                    var info = method.Value;
                    string service = "";
                    if (info["operationId"] != null && info["operationId"].ToString().Contains("_"))
                        service = info["operationId"].ToString().Split('_', 2)[1];

                    var parameters = info["parameters"] as JArray ?? new JArray();
                    var sectionRows = new List<ParamRow>();

                    foreach (var param in parameters)
                    {
                        ParamNode node = null;
                        if (param["schema"] != null)
                        {
                            node = ParamNodeFactory.Build(
                                param["name"]?.ToString(),
                                param["schema"],
                                definitions,
                                new HashSet<string>(),
                                param["required"]?.ToObject<bool>() ?? false,
                                param["description"]?.ToString()
                            );
                        }
                        else if (param["type"]?.ToString() == "array" && param["items"] != null)
                        {
                            node = ParamNodeFactory.Build(
                                param["name"]?.ToString(),
                                param,
                                definitions,
                                new HashSet<string>(),
                                param["required"]?.ToObject<bool>() ?? false,
                                param["description"]?.ToString()
                            );
                        }
                        else
                        {
                            node = new SimpleParam(
                                param["name"]?.ToString(),
                                param["description"]?.ToString() ?? "",
                                param["required"]?.ToObject<bool>() ?? false,
                                param["type"]?.ToString() ?? "",
                                param["format"]?.ToString() ?? "",
                                param["format"]?.ToString() ?? ""
                            );
                        }
                        if (node != null)
                        {
                            var ctx = new ParamContext()
                            {
                                Service = service,
                                InputType = param["in"]?.ToString() ?? "",
                                ParentWSField = "",//param["name"]?.ToString() ?? "",
                                ParentRequired = param["required"]?.ToObject<bool>() ?? false,
                                ParentFormat = param["type"]?.ToString() ?? "",
                                ParentFormat2 = param["format"]?.ToString() ?? "",
                                ParentDescription = param["description"]?.ToString() ?? "",
                                ParentName = param["name"]?.ToString() ?? "body"
                            };
                            node.Accept(visitor, ctx, sectionRows);
                        }
                    }
                    rows.AddRange(sectionRows);
                    rows.Add(new ParamRow()); // Riga vuota tra servizi
                }
            }

            return rows;
        }
        
        [Obsolete("Use GenerateRequestsSheet instead")]
        public static void GenerateParamsSheetFromOpenapi2(JObject? swagger, XLWorkbook workbook)
        {
            var worksheet = workbook.Worksheets.Add("Requests_swagger");
            var rows = ProcessJson(swagger);
            PopulateRequests(worksheet, rows);
            worksheet.Columns().AdjustToContents();
        }



        //public static void Export(string outputPath, IEnumerable<ParamRow> rows, string sheetName = "Parameters")
        //{
        //    //using (var package = new OfficeOpenXml.ExcelPackage())
        //    //{
        //    //    var worksheet = package.Workbook.Worksheets.Add(sheetName);
        //    //    worksheet.Cells["A1"].LoadFromCollection(rows, true);
        //    //    package.SaveAs(new System.IO.FileInfo(filePath));
        //    //}
        //    var cols = new[] {
        //        "Service","Subsection","Input","Description","Mandatory","Format",
        //        "InputType","WSInputField","Format2","Notes","Mapping"
        //    };

        //    using var wb = new XLWorkbook();
        //    var ws = wb.AddWorksheet("API Params");

        //    GenerateHeader(ws, cols);

        //    // Stronger header for Input, Description, Mandatory, Mapping
        //    var strongCols = new[] { "Input", "Description", "Mandatory", "Mapping" };
        //    ws.SheetView.FreezeRows(1);
        //    ws.Range(1, 1, 1, cols.Length).SetAutoFilter();

        //    int row = 2, colCount = cols.Length, serviceStart = row;
        //    string lastService = null;
        //    foreach (var r in rows)
        //    {
        //        if (string.IsNullOrEmpty(r.Service) && string.IsNullOrEmpty(r.Input) && string.IsNullOrEmpty(r.WSInputField))
        //        {
        //            // Sezione Service: merge, colori, bordi ticky
        //            if (lastService != null && serviceStart < row - 1)
        //            {
        //                ws.Range(serviceStart, 1, row - 1, 1).Merge();
        //                ws.Cell(serviceStart, 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
        //                ws.Cell(serviceStart, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
        //                ws.Cell(serviceStart, 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#B4C6E7");
        //                for (int rr = serviceStart; rr < row; rr++)
        //                    ws.Cell(rr, 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#B4C6E7");
        //                var rng = ws.Range(serviceStart, 1, row - 1, colCount);
        //                rng.Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
        //                rng.Style.Border.InsideBorder = XLBorderStyleValues.Hair;
        //            }
        //            row++;
        //            lastService = null;
        //            serviceStart = row;
        //            continue;
        //        }
        //        for (int c = 0; c < cols.Length; c++)
        //        {
        //            var cv = cols[c];
        //            var val = typeof(ParamRow).GetProperty(cv)?.GetValue(r, null)?.ToString() ?? "";
        //            ws.Cell(row, c + 1).Value = val;
        //            if (cv == "Input" || cv == "Description" || cv == "InputType" || cv == "WSInputField")
        //                ws.Cell(row, c + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
        //            else if (cv == "Mandatory" || cv == "Format" || cv == "Format2")
        //                ws.Cell(row, c + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        //            else
        //                ws.Cell(row, c + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
        //        }
        //        if (!string.IsNullOrEmpty(r.Service))
        //        {
        //            if (lastService == null)
        //            {
        //                lastService = r.Service;
        //                serviceStart = row;
        //            }
        //        }
        //        row++;
        //    }
        //    ws.Columns().AdjustToContents();
        //    wb.SaveAs(outputPath);
        //    Console.WriteLine($"Excel file generated: {outputPath}");
        //}

    }
}
