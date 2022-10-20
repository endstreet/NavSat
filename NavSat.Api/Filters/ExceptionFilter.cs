using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace NavSat.Api.Filters
{
    /// <summary>
    /// HttpResponse Exception
    /// </summary>
    public class HttpResponseException : Exception
    {

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="statusCode"></param>
        /// <param name="value"></param>
        public HttpResponseException(int statusCode, object? value = null) => (StatusCode, Value) = (statusCode, value);

        /// <summary>
        /// StatusCode
        /// </summary>
        public int StatusCode { get; }

        /// <summary>
        /// Value
        /// </summary>
        public object? Value { get; }
    }
    /// <summary>
    /// ResponseException Filter
    /// </summary>
    public class HttpResponseExceptionFilter : IActionFilter, IOrderedFilter
    {
        /// <summary>
        /// Order
        /// </summary>
        public int Order => int.MaxValue - 10;
        /// <summary>
        /// Excecuting Action
        /// </summary>
        /// <param name="context"></param>
        public void OnActionExecuting(ActionExecutingContext context) { }

        /// <summary>
        /// Executed Action
        /// </summary>
        /// <param name="context"></param>
        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception is HttpResponseException httpResponseException)
            {
                context.Result = new ObjectResult(httpResponseException.Value)
                {
                    StatusCode = httpResponseException.StatusCode
                };

                context.ExceptionHandled = true;
            }
        }
    }
}
