using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace OdinCore.Models.OdinMiddleware
{
    public class OdinRequestParamsMiddleware
    {
        private readonly RequestDelegate _next;

        public OdinRequestParamsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext context)
        {

            foreach (var item in context.Request.RouteValues)
            {

            }
            // var data = context.Request.Body.ReadAsync().Result;
            // foreach (var item in data.Keys)
            // {
            //     System.Console.WriteLine($"===RouteValues===Key:{item}====Value:{data[item]}====type:{data[item].GetType().ToString()}====");
            // }
            // Call the next delegate/middleware in the pipeline
            return this._next(context);
        }
    }

    public static class OdinRequestParamsMiddlewareExtensions
    {
        public static IApplicationBuilder UseOdinRequestParams(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<OdinRequestParamsMiddleware>();
        }
    }
}