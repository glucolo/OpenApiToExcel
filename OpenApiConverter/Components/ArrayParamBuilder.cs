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
    internal class ArrayParamBuilder : IParamBuilder
    {
        public bool CanHandle(JToken schema) => schema["type"]?.ToString() == "array";
        public ParamNode Build(string name, JToken schema, JObject definitions, HashSet<string> visitedRefs, bool required, string description = null, string format = null, string format2 = null)
        {
            var items = schema["items"];
            var node = ParamNodeFactory.Build(
                name,
                items,
                definitions,
                visitedRefs,
                false,
                items?["description"]?.ToString(),
                items?["type"]?.ToString(),
                items?["format"]?.ToString()
            );
            return new ArrayParam(
                name,
                description ?? schema["description"]?.ToString() ?? "",
                required,
                format ?? "array",
                format2 ?? schema["format"]?.ToString() ?? "",
                node
            );
        }
    }
}
