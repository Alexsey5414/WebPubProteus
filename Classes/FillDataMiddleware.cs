using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace WebTestProteus.Classes
{
    public static class AppFillDataExtensions
    {
        public static IApplicationBuilder UseStartFillData(this IApplicationBuilder app)
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));
            return app.UseMiddleware<FillDataMiddleware>();
        }
    }
    public class FillDataMiddleware
    {
       // private ApiContext _apicontext;
        public FillDataMiddleware(ApiContext apicontext)
        {
           // _apicontext = apicontext;
      

        }

        private  Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError; // 500 if unexpected
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return  context.Response.WriteAsync("An error occured.");
        }
    

         public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next.Invoke(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }
    }
}
