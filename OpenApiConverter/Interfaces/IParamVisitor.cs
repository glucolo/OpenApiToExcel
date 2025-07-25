using OpenApiConverter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenApiConverter.Visitor
{
    public interface IParamVisitor
    {
        void Visit(SimpleParam param, ParamContext context, List<ParamRow> output);
        void Visit(ObjectParam param, ParamContext context, List<ParamRow> output);
        void Visit(ArrayParam param, ParamContext context, List<ParamRow> output);
    }
}
