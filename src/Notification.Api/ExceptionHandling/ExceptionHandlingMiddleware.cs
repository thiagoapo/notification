using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http.Extensions;
using System.Text;
using System.Linq;
using Notification.Application.Utils.Exception;

namespace Notification.Api.ExceptionHandling
{
    /// <summary>
    /// exception handler
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="next"></param>
        /// <param name="logger"></param>
        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            this.next = next;
            _logger = logger;
        }

        /// <summary>
        /// invoke
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context /* other dependencies */)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An unhandled exception has occurred while executing the request. Url: {context.Request.GetDisplayUrl()}. Request Data: " + GetRequestData(context));

                await HandleExceptionAsync(context, ex);
            }
        }

        private static string GetRequestData(HttpContext context)
        {
            var sb = new StringBuilder();

            if (context.Request.HasFormContentType && context.Request.Form.Any())
            {
                sb.Append("Form variables:");
                foreach (var x in context.Request.Form)
                {
                    sb.AppendFormat("Key={0}, Value={1}<br/>", x.Key, x.Value);
                }
            }

            sb.AppendLine("Method: " + context.Request.Method);

            return sb.ToString();
        }

        /// <summary>
        /// ResultResponse
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="context"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public Task ResultResponse(Exception ex, HttpContext context, int code)
        {

            _logger.LogError($"error {ex.Message}");
            var result = JsonConvert.SerializeObject(new { error = ex.Message });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = code;
            return context.Response.WriteAsync(result);
        }

        private Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            if (ex is NotificationException)
                return ResultResponse(ex, context, StatusCodes.Status400BadRequest);
            
            return ResultResponse(ex, context, StatusCodes.Status500InternalServerError);
        }
    }
}
