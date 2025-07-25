using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenApiConverter.Models
{
    public class ParamRow
    {
        public string Service { get; set; }
        public string Subsection { get; set; } = "";
        public string Input { get; set; }
        public string Description { get; set; }
        public string Mandatory { get; set; }
        public string Format { get; set; }
        public string InputType { get; set; }
        public string WSInputField { get; set; }
        public string Format2 { get; set; }
        public string Notes { get; set; } = "";
        public string Mapping { get; set; } = "";
    }
}
