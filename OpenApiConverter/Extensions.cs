using Microsoft.OpenApi;
using OpenApiConverter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenApiConverter
{
    internal static class Extensions
    {
        public static void Accept(this OpenApiDocument document, OpenApiVisitorBase visitor)
            => visitor.Visit(document);

        public static void Accept(this OpenApiPaths paths, OpenApiVisitorBase visitor)
            => visitor.Visit(paths);

        public static void Accept(this Dictionary<HttpMethod, OpenApiOperation>? operations, OpenApiVisitorBase visitor)
            => visitor.Visit(operations);

        public static void Accept(this IOpenApiResponse response, OpenApiVisitorBase visitor)
            => visitor.Visit(response);

        //public static void Accept(this OpenApiOperation operation, OpenApiVisitorBase visitor)
        //    => visitor.Visit(operation);

        //public static void Accept(this IOpenApiParameter openApiParameter, OpenApiVisitorBase visitor)
        //    => visitor.Visit(openApiParameter);

    }
}
