using OpenApiConverter.Visitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenApiConverter.Models
{
    public class SimpleParam : ParamNode
    {
        public string Name, Description, Type, Format, Format2;
        public bool Required;
        public SimpleParam(string name, string description, bool required, string type, string format, string format2)
        {
            Name = name; Description = description; Required = required; Type = type; Format = format; Format2 = format2;
        }
        public override void Accept(IParamVisitor visitor, ParamContext context, List<ParamRow> output)
        {
            visitor.Visit(this, context, output);
        }
    }
}
