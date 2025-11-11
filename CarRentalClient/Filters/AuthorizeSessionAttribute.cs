using Microsoft.AspNetCore.Mvc.Filters;

namespace CarRentalClient.Filters
{
    public class AuthorizeSessionAttribute : Attribute, IActionFilter
    {
        public string[] Roles;

        public AuthorizeSessionAttribute(params string[] roles)
        {
            Roles = roles;
        }
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var httpContext = context.HttpContext;
            var token = httpContext.Session.GetString("AccessToken");

            // Check if user is authenticated
            if (string.IsNullOrEmpty(token))
            {
                context.HttpContext.Response.Redirect("/Account/Login");
            }

            // If roles are specified, check if user has one of the required roles
            if (Roles != null && Roles.Length > 0)
            {
                var rolesJson = httpContext.Session.GetString("UserRoles");
                if (string.IsNullOrEmpty(rolesJson))
                {
                    context.HttpContext.Response.Redirect("/Account/Login");
                    return;
                }
                var userRoles = System.Text.Json.JsonSerializer.Deserialize<List<string>>(rolesJson);

                //Check if user has any of the required roles
                bool hasRequiredRole = userRoles.Any(ur => Roles.Contains(ur));

                if(!hasRequiredRole)
                {
                    context.HttpContext.Response.Redirect("/Account/AccessDenied");
                }
            }
            {
                
            }
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Do nothing after the action executes.
        }
    }
}
