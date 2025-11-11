using Microsoft.AspNetCore.Mvc.Filters;

namespace CarRentalClient.Filters
{
    public class AuthorizeSessionAttribute : Attribute, IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var httpContext = context.HttpContext;
            var token = httpContext.Session.GetString("AccessToken");
            if (string.IsNullOrEmpty(token))
            {
                context.HttpContext.Response.Redirect("/Account/Login");
            }
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Do nothing after the action executes.
        }
    }
}
