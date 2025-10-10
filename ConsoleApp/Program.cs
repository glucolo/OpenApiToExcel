using Microsoft.OpenApi;
using Microsoft.OpenApi.Reader;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenApiConverter;
using OpenApiConverter.Components;
using OpenApiConverter.Models;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

if (args.Length < 2)
{
    Console.WriteLine("Usage: SwaggerToExcel.exe <swagger.json> <output.xlsx>");
    return;
}



var swaggerPath = args[0];
var outputPath = args[1];
var swagger = JObject.Parse(File.ReadAllText(swaggerPath));

var filename = Path.GetFileNameWithoutExtension(swaggerPath);

Console.WriteLine("Loading OpenApi/Swagger definition...");
// Load swagger to OpenApiDocument
var doc = LoadToOpenApiDocument(swaggerPath);
Console.WriteLine("Writing OpenApi 2/3/3.1 definition...");
File.WriteAllText($"C:\\temp\\openapi_2.0_{filename}.json", await doc.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi2_0));
File.WriteAllText($"C:\\temp\\openapi_3.0_{filename}.json", await doc.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_0));
File.WriteAllText($"C:\\temp\\openapi_3.1_{filename}.json", await doc.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_1));

using (var wb = ExcelConverter.CreateExcelWorkbook())
{
    Console.WriteLine("Creating operations sheet...");
    ExcelConverter.GenerateEndpointsSheet(doc, wb);
    Console.WriteLine("Creating Requests detail sheet...");
    ExcelConverter.GenerateRequestsSheet(doc, wb);
    //ExcelConverter.GenerateParamsSheetFromOpenapi2(swagger, wb);
    Console.WriteLine("Creating Responses detail sheet...");
    ExcelConverter.GenerateResponsesSheets(doc, wb);
    Console.WriteLine("Saving...");
    ExcelConverter.SaveExcelWorkbook(wb, outputPath);
    Console.WriteLine($"Excel created at {outputPath}");
    return;
}


static OpenApiDocument LoadToOpenApiDocument(string filePath)
{
    if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
        throw new FileNotFoundException($"File not found: {filePath}");

    var content = File.OpenRead(filePath);
    var apiDoc = OpenApiDocument.LoadAsync(content, OpenApiConstants.Json).ConfigureAwait(false).GetAwaiter().GetResult();

    return apiDoc.Document;
}




