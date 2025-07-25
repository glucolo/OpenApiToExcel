using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenApiConverter.Models
{
    public class ParamContext
    {
        public string Service { get; set; }
        public string Subsection { get; set; } = "";
        public string InputType { get; set; }
        public string ParentInput { get; set; }
        public string ParentWSField { get; set; }
        public bool ParentRequired { get; set; } = false;
        public string ParentFormat { get; set; }
        public string ParentFormat2 { get; set; }
        public string ParentDescription { get; set; }
        public string ParentName { get; set; }
        public ParamContext Clone() => (ParamContext)this.MemberwiseClone();
    }
}
