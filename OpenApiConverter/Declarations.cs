using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenApiConverter
{
    public enum ParamLocation
    {
        Query,
        Header,
        Path,
        FormData,
        Body
    }

    public enum SheetType
    {
        Endpoint,
        Requests,
        Responses,
        Errors
    }
}
