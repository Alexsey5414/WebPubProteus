using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace WebTestProteus.Filters
{
    public class JsonToStringAttribute : ActionFilterAttribute
    {
      
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            // You can simply use filterContext.ActionArguments to get whatever param that you have set in the action
            // For instance you can get the "json" param like this: filterContext.ActionArguments["json"]
            // Or better yet just loop through the arguments and find the type
            foreach (var elem in filterContext.ActionArguments)
            {
                if (elem.Value is JsonElement)
                {
                    // Convert json obj to string
                    var json = ((JsonElement)elem.Value).GetRawText();
                    break;
                }
            }

        }
    }
}
