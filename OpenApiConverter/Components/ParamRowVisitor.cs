using OpenApiConverter.Models;
using OpenApiConverter.Visitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenApiConverter.Components
{
    public class ParamRowVisitor : IParamVisitor
    {
        public void Visit(SimpleParam param, ParamContext context, List<ParamRow> output)
        {
            output.Add(new ParamRow
            {
                Service = context.Service,
                Subsection = context.Subsection,
                Input = param.Name,
                Description = param.Description ?? context.ParentDescription ?? "",
                Mandatory = param.Required || context.ParentRequired ? "S" : "N",
                Format = param.Type ?? context.ParentFormat ?? "",
                InputType = context.InputType,
                WSInputField = string.IsNullOrEmpty(context.ParentWSField) ? param.Name : $"{context.ParentWSField}.{param.Name}",
                Format2 = param.Format2 ?? context.ParentFormat2 ?? "",
                Notes = "",
                Mapping = ""
            });
        }

        public void Visit(ObjectParam param, ParamContext context, List<ParamRow> output)
        {
            var nextCtx = context.Clone();
            nextCtx.ParentName = param.Name;
            nextCtx.ParentInput = param.Name;
            nextCtx.ParentWSField = string.IsNullOrEmpty(context.ParentWSField) ? param.Name : $"{context.ParentWSField}.{param.Name}";
            nextCtx.ParentFormat = param.Format ?? context.ParentFormat;
            nextCtx.ParentFormat2 = param.Format2 ?? context.ParentFormat2;
            nextCtx.ParentDescription = param.Description ?? context.ParentDescription;
            nextCtx.ParentRequired = param.Required || context.ParentRequired;
            foreach (var child in param.Properties)
                child.Accept(this, nextCtx, output);
        }

        public void Visit(ArrayParam param, ParamContext context, List<ParamRow> output)
        {
            var nextCtx = context.Clone();
            nextCtx.ParentName = param.Name;
            nextCtx.ParentInput = param.Name;
            nextCtx.ParentWSField = string.IsNullOrEmpty(context.ParentWSField) ? param.Name + "[]" : $"{context.ParentWSField}.{param.Name}[]";
            nextCtx.ParentFormat = param.Format ?? context.ParentFormat;
            nextCtx.ParentFormat2 = param.Format2 ?? context.ParentFormat2;
            nextCtx.ParentDescription = param.Description ?? context.ParentDescription;
            nextCtx.ParentRequired = param.Required || context.ParentRequired;
            param.Items?.Accept(this, nextCtx, output);
        }
    }

}
