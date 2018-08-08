using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aspirin.Api.Localization;
using Aspirin.Api.Model.Core;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Serilog;
using Serilog.Events;

namespace Aspirin.Api.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private static readonly ILogger _logger = Log.ForContext<ExceptionHandlingMiddleware>();
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, ILocalizationHelper localizationHelper)
        {
            try
            {
                await _next(context);
            }
            catch (AspirinException aspirinException)
            {
                var logLevel = GetLevel(aspirinException.StatusCode);
                _logger.Write(logLevel, aspirinException, "AspirinException occured with message {AspirinMessage}", aspirinException.Message);
                await ClearResponseAndBuildErrorDto(context, aspirinException.StatusCode, new List<string> { aspirinException.Message }).ConfigureAwait(false);
            }
            catch (ValidationException validationException)
            {
                var messages = new List<string>();
                if (validationException.Errors.Any())
                {
                    messages.AddRange(validationException.Errors.Select(i => i.ErrorMessage));
                }
                else
                {
                    messages.Add(validationException.Message);
                }
                _logger.Warning("Validation failed with messages {@ValidationMessages}", messages);
                await ClearResponseAndBuildErrorDto(context, StatusCodes.Status400BadRequest, messages).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                var logger = GetErrorLogger(context);
                logger.Error(exception, "Internal Server Error!");
                await ClearResponseAndBuildErrorDto(context, StatusCodes.Status500InternalServerError, new List<string> { localizationHelper.GetLocalizedString("InternalServerError") }).ConfigureAwait(false);
            }
        }

        private static LogEventLevel GetLevel(int statusCode)
        {
            var level = LogEventLevel.Information;
            if (399 < statusCode && statusCode < 500)
            {
                level = LogEventLevel.Warning;
            }
            if (statusCode >= 500)
            {
                level = LogEventLevel.Error;
            }

            return level;
        }

        private static ILogger GetErrorLogger(HttpContext httpContext)
        {
            var request = httpContext.Request;

            var result = Log
                .ForContext("RequestCookies", request.Cookies.ToDictionary(c => c.Key, c => c.Value.ToString()), true)
                .ForContext("RequestHeaders", request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()), true)
                .ForContext("RequestHost", request.Host)
                .ForContext("RequestProtocol", request.Protocol);

            if (request.HasFormContentType)
            {
                result = result.ForContext("RequestForm", request.Form.ToDictionary(v => v.Key, v => v.Value.ToString()));
            }

            return result;
        }

        private static Task ClearResponseAndBuildErrorDto(HttpContext context, int statusCode, List<string> messages)
        {
            var error = JsonConvert.SerializeObject(new ErrorDto(statusCode, messages));
            context.Response.Clear();
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;
            context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            return context.Response.WriteAsync(error, Encoding.UTF8);
        }
    }
}
