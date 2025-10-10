using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenApiConverter.Models
{
    public class EndpointRow
    {
        public string Service { get; set; }
        public string Description { get; set; }
        public string Resource { get; set; }
        public string HttpMethod { get; set; }
        public string Path { get; set; }
        public string Intermediation { get; set; }
        public string Caller { get; set; }
        public string AuthType { get; set; }
        public string Note { get; set; }
    }
}
