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
    public static class ParamNodeFactory
    {
        private static readonly List<IParamBuilder> _builders = new()
        {
            new ArrayParamBuilder(),
            new ObjectParamBuilder(),
            new SimpleParamBuilder()
        };

        public static ParamNode Build(string name, JToken schema, JObject definitions, HashSet<string> visitedRefs, bool required, string description = null, string format = null, string format2 = null)
        {
            foreach (var builder in _builders)
            {
                if (builder.CanHandle(schema))
                {
                    return builder.Build(name, schema, definitions, visitedRefs, required, description, format, format2);
                }
            }
            return null;
        }
    }
}
