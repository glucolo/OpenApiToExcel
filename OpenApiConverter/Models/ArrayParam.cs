using OpenApiConverter.Visitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenApiConverter.Models
{
    public class ArrayParam : ParamNode
    {
        public string Name, Description, Format, Format2;
        public bool Required;
        public ParamNode Items;
        public ArrayParam(string name, string description, bool required, string format, string format2, ParamNode items)
        {
            Name = name; Description = description; Required = required; Format = format; Format2 = format2; Items = items;
        }
        public override void Accept(IParamVisitor visitor, ParamContext context, List<ParamRow> output)
        {
            visitor.Visit(this, context, output);
        }
    }
}
