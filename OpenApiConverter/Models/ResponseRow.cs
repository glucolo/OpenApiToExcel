using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenApiConverter.Models
{
    internal class ResponseRow
    {
        public string Service { get; set; }
        public string Subsection { get; set; }
        public string Output { get; set; }
        public string Description { get; set; }
        public string Format { get; set; }
        public string OutputType { get; set; }
        public string WSOutputField { get; set; }
        public string Notes { get; set; }
        public string Mapping { get; set; }
    }
}
