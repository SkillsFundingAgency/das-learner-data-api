﻿using Microsoft.Extensions.Primitives;
using SFA.DAS.LearnerData.Extensions;

namespace SFA.DAS.LearnerData.Api.Middleware;

public class SecurityHeadersMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        context.Response.Headers.AddIfNotPresent("x-frame-options", new StringValues("DENY"));
        context.Response.Headers.AddIfNotPresent("x-content-type-options", new StringValues("nosniff"));
        context.Response.Headers.AddIfNotPresent("X-Permitted-Cross-Domain-Policies", new StringValues("none"));
        context.Response.Headers.AddIfNotPresent("Content-Security-Policy", new StringValues("default-src *; script-src *; connect-src *; img-src *; style-src *; object-src *;"));
        
        await next(context);
    }
}