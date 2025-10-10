using OpenApiConverter.Models;
using OpenApiConverter.Visitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenApiConverter.Components
{
    /// <summary>
    /// Visitor per la struttura ad oggetti dei parametri, che popola una lista di ParamRow
    /// </summary>
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
            // Se il parent termina con [], NON aggiungere il nome dell'oggetto
            if (!string.IsNullOrEmpty(context.ParentWSField) && context.ParentWSField.EndsWith("[]"))
                nextCtx.ParentWSField = context.ParentWSField;
            else
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
            // Imposta ParentWSField aggiungendo solo [] al nome dell'array
            nextCtx.ParentWSField = string.IsNullOrEmpty(context.ParentWSField) ? param.Name + "[]" : $"{context.ParentWSField}.{param.Name}[]";
            nextCtx.ParentFormat = param.Format ?? context.ParentFormat;
            nextCtx.ParentFormat2 = param.Format2 ?? context.ParentFormat2;
            nextCtx.ParentDescription = param.Description ?? context.ParentDescription;
            nextCtx.ParentRequired = param.Required || context.ParentRequired;

            // Se il figlio è un oggetto, NON aggiungere di nuovo il nome dell'array
            if (param.Items is ObjectParam objParam)
            {
                var objCtx = nextCtx.Clone();
                objCtx.ParentWSField = nextCtx.ParentWSField; // Non aggiungere il nome dell'oggetto
                objParam.Accept(this, objCtx, output);
            }
            else
            {
                param.Items?.Accept(this, nextCtx, output);
            }
        }
    }

}
