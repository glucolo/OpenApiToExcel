using Newtonsoft.Json.Linq;
using OpenApiConverter.Models;
using OpenApiConverter.Visitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenApiConverter.Components
{
    internal class SimpleParamBuilder : IParamBuilder
    {
        public bool CanHandle(JToken schema) =>
            (schema["type"]?.ToString() != "object" && schema["type"]?.ToString() != "array" && schema["$ref"] == null);

        public ParamNode Build(string name, JToken schema, JObject definitions, HashSet<string> visitedRefs, bool required, string description = null, string format = null, string format2 = null)
        {
            return new SimpleParam(
                name,
                description ?? schema["description"]?.ToString() ?? "",
                required,
                schema["type"]?.ToString() ?? format ?? "",
                schema["format"]?.ToString() ?? "",
                format2 ?? schema["format"]?.ToString() ?? ""
            );
        }
    }
}
