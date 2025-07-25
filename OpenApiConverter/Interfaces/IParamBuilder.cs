using Newtonsoft.Json.Linq;
using OpenApiConverter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenApiConverter.Visitor
{
    public interface IParamBuilder
    {
        bool CanHandle(JToken schema);
        ParamNode Build(string name, JToken schema, JObject definitions, HashSet<string> visitedRefs, bool required, string description = null, string format = null, string format2 = null);
    }
}
