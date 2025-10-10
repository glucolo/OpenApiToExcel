using Microsoft.OpenApi;
using OpenApiConverter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenApiConverter.Components
{
    internal class ResponsesVisitor : OpenApiVisitorBase
    {
        private readonly IList<ResponseRow> _rows = new List<ResponseRow>();
        private string _service;

        public override void Visit(OpenApiDocument doc)
        {
            var ops = doc.Paths.Select(p => p.Value.Operations);
            foreach (var op in ops)
            {
                op.Accept(this);
            }
        }

        public override void Visit(IDictionary<HttpMethod, OpenApiOperation> operations)
        {
            foreach (var kvp in operations)
            {
                OpenApiOperation operation = kvp.Value;
                _service = operation.OperationId?.Split('_').Last() ?? "";

                if (operation.Responses != null)
                {
                    var resp200 = operation.Responses.FirstOrDefault(r => r.Key.Equals("200")).Value;
                    resp200?.Accept(this);
                    _rows.Add(new ResponseRow()); // Separatore tra operazioni
                }
            }
        }

        public override void Visit(IOpenApiResponse response)
        {
            // Prendi il primo content-type, tipicamente "application/json"
            var content = response.Content?.FirstOrDefault().Value;
            var schema = content?.Schema;
            if (schema != null)
            {
                VisitSchemaProperties(schema, _service, "response", "");
            }

        }

        // Ricorsivo per oggetti/array
        private void VisitSchemaProperties(IOpenApiSchema schema, string service, string parentWSField, string outputType)
        {
            if (schema.Type == JsonSchemaType.Object && schema.Properties != null)
            {
                foreach (var prop in schema.Properties)
                {
                    var wsField = string.IsNullOrEmpty(parentWSField) ? prop.Key : $"{parentWSField}.{prop.Key}";
                    if (prop.Value.Type == JsonSchemaType.Object)
                    {
                        VisitSchemaProperties(prop.Value, service, wsField, outputType);
                    }
                    else if (prop.Value.Type == JsonSchemaType.Array)
                    {
                        var arrayField = wsField + "[]";
                        VisitSchemaProperties(prop.Value.Items, service, arrayField, outputType);
                    }
                    else
                    {
                        CreateRow(prop, service, parentWSField, outputType);
                    }
                }
            }
            else if (schema.Type == JsonSchemaType.Array && schema.Items != null)
            {
                var arrayField = (parentWSField ?? "response") + "[]";
                VisitSchemaProperties(schema.Items, service, arrayField, outputType);
            }
            else
            {
                // Se non c'è schema, crea una riga con la descrizione della risposta
                var newRow = new ResponseRow
                {
                    Service = service,
                    Description = "Empty OK response",
                };
                _rows.Add(newRow);
            }
        }

        // Crea riga per proprietà semplice
        private void CreateRow(KeyValuePair<string, IOpenApiSchema> schema, string service, string parentWSField, string outputType)
        {
            var newRow = new ResponseRow();
            newRow.Service = service;
            newRow.Output = schema.Key;
            newRow.Description = schema.Value.Description ?? "";
            newRow.Format = schema.Value?.Type?.ToString() ?? "";
            newRow.OutputType = outputType;
            newRow.WSOutputField = string.IsNullOrEmpty(parentWSField) ? schema.Key : $"{parentWSField}.{schema.Key}";
            _rows.Add(newRow);
        }

        public IList<ResponseRow> GetResponses() => _rows;
    }
}
