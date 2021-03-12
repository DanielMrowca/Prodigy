using Microsoft.AspNetCore.Http;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace HoneyComb.Logging
{
    public static class DiagnosticContextExtensions
    {
        public static void SetHeaderValue(this IDiagnosticContext diagnosticContext, HttpContext httpContext, string headerName)
        {
            var value = httpContext.GetHeaderValue(headerName);
            if (string.IsNullOrWhiteSpace(value))
                return;

            diagnosticContext.Set(headerName, value);
        }
    }
}
