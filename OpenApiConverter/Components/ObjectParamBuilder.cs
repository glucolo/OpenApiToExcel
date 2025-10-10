using Newtonsoft.Json.Linq;
using OpenApiConverter.Models;
using OpenApiConverter.Visitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace OpenApiConverter.Components
{
    /// <summary>
    /// Builder per un nodo di tipo Object - Compatibile con Swagger 2.0
    /// </summary>
    internal class ObjectParamBuilder : IParamBuilder
    {
        public bool CanHandle(JToken schema)
        {
            return schema["type"]?.ToString() == "object" || schema["$ref"] != null;
        }

        public ParamNode Build(string name, JToken schema, JObject definitions, HashSet<string> visitedRefs, bool required, string description = null, string format = null, string format2 = null)
        {
            JObject def = null;
            if (schema["$ref"] != null)
            {
                var refStr = schema["$ref"].ToString();
                if (visitedRefs.Contains(refStr)) return null; // Avoid infinite recursion
                visitedRefs.Add(refStr);
                var defName = HttpUtility.UrlDecode( refStr.Split('/').Last());
                def = definitions[defName] as JObject;
                if (def == null) return null;
            }
            else
            {
                def = schema as JObject;
            }
            var props = new List<ParamNode>();
            var requiredFields = new HashSet<string>(def?["required"]?.Select(r => r.ToString()) ?? Enumerable.Empty<string>());
            foreach (var p in (def?["properties"] as JObject)?.Properties() ?? Enumerable.Empty<JProperty>())
            {
                var node = ParamNodeFactory.Build(
                    p.Name,
                    p.Value,
                    definitions,
                    visitedRefs,
                    requiredFields.Contains(p.Name),
                    p.Value["description"]?.ToString() ?? "",
                    p.Value["type"]?.ToString() ?? "",
                    p.Value["format"]?.ToString() ?? ""
                );
                if (node != null) props.Add(node);
            }
            return new ObjectParam(
                name,
                description ?? def?["description"]?.ToString() ?? "",
                required,
                format ?? "object",
                format2 ?? "",
                props
            );
        }
    }
}
