using Newtonsoft.Json.Linq;
using OpenApiConverter.Components;
using OpenApiConverter.Models;
using System.Runtime.CompilerServices;

if (args.Length < 2)
{
    Console.WriteLine("Usage: SwaggerToExcelOOPWithInheritance <swagger.json> <output.xlsx>");
    return;
}

var swaggerPath = args[0];
var outputPath = args[1];
var swagger = JObject.Parse(File.ReadAllText(swaggerPath));
var rowObjs = ProcessJson(swagger);
ExcelConverter.Export(outputPath, rowObjs, "Gipsy API");




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
                        ParentWSField = "",
                        ParentRequired = param["required"]?.ToObject<bool>() ?? false,
                        ParentFormat = param["type"]?.ToString() ?? "",
                        ParentFormat2 = param["format"]?.ToString() ?? "",
                        ParentDescription = param["description"]?.ToString() ?? "",
                        ParentName = param["name"]?.ToString() ?? ""
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
