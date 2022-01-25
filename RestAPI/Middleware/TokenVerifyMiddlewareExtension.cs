using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AmiMesApi.Middleware
{
    public static class TokenVerifyMiddlewareExtensions
    {
        public static IApplicationBuilder UseTokenVerify(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TokenVerifyMiddleware>();
        }
    }
}
