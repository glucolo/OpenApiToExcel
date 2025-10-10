using ClosedXML.Excel;
using Microsoft.OpenApi;
using OpenApiConverter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenApiConverter.Components
{
    internal class EndPointListVisitor : OpenApiVisitorBase
    {
        private readonly IList<EndpointRow> _rows = new List<EndpointRow>();

        public override void Visit(OpenApiDocument doc)
        {
            //Ricerca tutti gli endpoint (path) e genera una riga di excel per ognuno
            doc.Paths.Accept(this);
        }

        public override void Visit(OpenApiPaths paths)
        {
            foreach (var path in paths)
            {
                foreach (var method in path.Value?.Operations)
                {
                    var pathRow = new EndpointRow();
                    pathRow.Path = path.Key;
                    pathRow.Service = method.Value.OperationId.Contains("_") ? method.Value.OperationId.Split('_', 2)[1] : method.Value.OperationId;
                    pathRow.Description = method.Value.Description ?? method.Value.Summary ?? "";
                    pathRow.Resource = string.Join(',', method.Value.Tags?.Select(t => t.Name));
                    pathRow.HttpMethod = method.Key.Method;
                    _rows.Add(pathRow);
                }
            }
        }

        public IList<EndpointRow> GetEndpoints() => _rows;

    }
}
