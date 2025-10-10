using DocumentFormat.OpenXml.Presentation;
using Microsoft.OpenApi;
using OpenApiConverter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenApiConverter.Components
{
    internal class RequestsVisitor : OpenApiVisitorBase
    {
        private readonly IList<ParamRow> _rows = new List<ParamRow>();

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
                HttpMethod method = kvp.Key;
                OpenApiOperation operation = kvp.Value;
                string service = operation.OperationId?.Split('_').Last() ?? "";

                // Parametri semplici (query, path, header, ecc.)
                if (operation.Parameters != null)
                    foreach (var param in operation.Parameters)
                        CreateRow(param, service);

                // Parametri nel body (object/array ricorsivo)
                var schema = operation.RequestBody?.Content?.First().Value?.Schema;
                if (schema != null)
                {
                    // Prova a recuperare il nome del parametro body
                    string bodyName = null;
                    IOpenApiExtension ext = null;
                    if (operation.RequestBody.Extensions?.TryGetValue("x-bodyName", out ext) ?? false)
                        bodyName = (ext as JsonNodeExtension)?.Node?.ToString();
                    // Se non c'è un nome, usa "body"
                    VisitSchemaProperties(schema, service, bodyName ?? "bodyObject", "body", schema.Required?.Any() == true);
                }

                _rows.Add(new ParamRow()); // Riga vuota per separare le operazioni
            }
        }

        // Ricorsivo per oggetti/array
        private void VisitSchemaProperties(IOpenApiSchema schema, string service, string parentWSField, string inputType, bool required)
        {

            if (schema.Type == JsonSchemaType.Object && schema.Properties != null)
            {
                foreach (var prop in schema.Properties)
                {
                    var wsField = string.IsNullOrEmpty(parentWSField) ? prop.Key : $"{parentWSField}.{prop.Key}";
                    if (prop.Value.Type == JsonSchemaType.Object)
                    {
                        VisitSchemaProperties(prop.Value, service, wsField, inputType, schema.Required?.Contains(prop.Key) == true);
                    }
                    else if (prop.Value.Type == JsonSchemaType.Array)
                    {
                        var arrayField = wsField + "[]";
                        VisitSchemaProperties(prop.Value.Items, service, arrayField, inputType, schema.Required?.Contains(prop.Key) == true);
                    }
                    else
                    {
                        // Semplice
                        CreateRow(prop, service, parentWSField, schema.Required?.Contains(prop.Key) == true, inputType);
                    }
                }
            }
            else if (schema.Type == JsonSchemaType.Array && schema.Items != null)
            {
                var arrayField = (parentWSField ?? "body") + "[]";
                VisitSchemaProperties(schema.Items, service, arrayField, inputType, required);
            }
        }

        // Crea riga per proprietà semplice
        private void CreateRow(KeyValuePair<string, IOpenApiSchema> schema, string service, string parentWSField, bool required, string inputType)
        {
            var newRow = new ParamRow();
            newRow.Input = schema.Key;
            newRow.Description = schema.Value.Description ?? "";
            newRow.Mandatory = required ? "Yes" : "No";
            newRow.Format = schema.Value?.Type?.ToString() ?? "";
            newRow.InputType = inputType;
            newRow.WSInputField = string.IsNullOrEmpty(parentWSField) ? schema.Key : $"{parentWSField}.{schema.Key}";
            newRow.Format2 = schema.Value?.Format ?? "";
            newRow.Service = service;
            _rows.Add(newRow);
        }

        public override void Visit(IOpenApiParameter parameter)
        {

        }
        private void CreateRow(IOpenApiParameter param, string service, string padre = null, bool required = false)
        {
            var newRow = new ParamRow();
            newRow.Input = param.Name?.Split('.')?.Last();
            newRow.Description = param.Description ?? "";
            newRow.Mandatory = param.Required ? "Yes" : "No";
            newRow.Format = param.Schema?.Type?.ToString() ?? "";
            newRow.InputType = param.In.ToString();
            newRow.WSInputField = param.Name;
            newRow.Format2 = param.Schema?.Format;
            newRow.Service = service;
            _rows.Add(newRow);
        }

        private void CreateRow(KeyValuePair<string, IOpenApiSchema> schema, string service, string padre = null, bool required = false)
        {
            var newRow = new ParamRow();
            newRow.Input = schema.Key;
            newRow.Description = schema.Value.Description ?? "";
            newRow.Mandatory = required ? "Yes" : "No";
            newRow.Format = schema.Value?.Type?.ToString() ?? "";
            newRow.InputType = "body";
            newRow.WSInputField = !string.IsNullOrEmpty(padre) ? $"{padre}.{schema.Key}" : schema.Key;
            newRow.Format2 = schema.Value?.Format ?? "";
            newRow.Service = service;
            _rows.Add(newRow);
        }

        //public override void Visit(OpenApiOperation openApiOperation)
        //{
        //    foreach (var param in openApiOperation.Parameters)
        //    {
        //        var paramRow = new ParamRow();
        //        paramRow.Input = param.Name;
        //        paramRow.Description = param.Description ?? "";
        //        paramRow.Mandatory = param.Required ? "Yes" : "No";
        //        //paramRow.Format = param.Schema?.Type ?? "";
        //        paramRow.InputType = param.In.ToString();
        //        _rows.Add(paramRow);
        //    }
        //    foreach (var requestBody in openApiOperation.RequestBody?.Content ?? Enumerable.Empty<KeyValuePair<string, OpenApiMediaType>>())
        //    {
        //        var paramRow = new ParamRow();
        //        paramRow.Input = "RequestBody";
        //        paramRow.Description = requestBody.Value?.Schema?.Description ?? "";
        //        paramRow.Mandatory = openApiOperation.RequestBody.Required ? "Yes" : "No";
        //        //paramRow.Format = requestBody.Value?.Schema?.Type ?? "";
        //        paramRow.InputType = "body";
        //        _rows.Add(paramRow);
        //    }
        //}

        //public override void Visit(IList<IOpenApiParameter> parameters)
        //{
        //    foreach (var param in parameters)
        //    {
        //        Visit(param);
        //    }
        //}

        //public override void Visit(IOpenApiParameter openApiParameter)
        //{
        //    // Questo metodo può essere lasciato vuoto se non è necessario
        //}

        public IList<ParamRow> GetRequests() => _rows;

    }
}
