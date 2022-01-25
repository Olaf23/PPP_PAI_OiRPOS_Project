using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.Extensions.Configuration;
using AmiMesApi.Services;
using AmiMesApi.Model;
using Microsoft.AspNetCore.Authorization;
using IAuthorizationService = AmiMesApi.Services.IAuthorizationService;

namespace AmiMesApi.Middleware
{
    public class TokenVerifyMiddleware
    {
        private readonly RequestDelegate next;
        private readonly List<string> tokenlessPaths;

        public TokenVerifyMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            this.next = next;
            tokenlessPaths = new();
            var tokenlessPathsTemp = configuration.GetSection("TokenlessPaths").Get<List<string>>();
            tokenlessPathsTemp.ForEach(x => tokenlessPaths.Add(x.ToLower()));
        }

        public async Task InvokeAsync(HttpContext context, IAuthorizationService authorizationService)
        {
            //await next(context);
            if (IsPathAllowedToTokenlessPass(context.Request.Path))
            {
                await next(context);
            }
            else
            {
                int userId = authorizationService.GetUserIdFromRequest(context.Request);
                if (userId != -1)
                {
                    await next(context);
                }
                else
                {
                    UnauthorizeRequest(context);
                }
            }
        }

        private bool IsPathAllowedToTokenlessPass(string path)
        {
            return tokenlessPaths.Contains(path.ToLower());
        }

        private void UnauthorizeRequest(HttpContext context)
        {
            context.Response.Clear();
            context.Response.ContentType = "fetch";
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        }
    }
}
