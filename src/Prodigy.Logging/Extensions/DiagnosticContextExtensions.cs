using Microsoft.AspNetCore.Http;
using Serilog;

namespace Prodigy.Logging
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
