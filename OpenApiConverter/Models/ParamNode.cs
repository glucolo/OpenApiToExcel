using OpenApiConverter.Visitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenApiConverter.Models
{
    public abstract class ParamNode
    {
        public abstract void Accept(IParamVisitor visitor, ParamContext context, List<ParamRow> output);
    }
}
