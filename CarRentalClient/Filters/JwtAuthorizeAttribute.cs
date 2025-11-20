using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CarRentalClient.Filters
{
    public class JwtAuthorizeAttribute: ActionFilterAttribute
    {
        private readonly string[] roles;

        public JwtAuthorizeAttribute(params string[] roles)
        {
            this.roles = roles ?? Array.Empty<string>();
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var token = context.HttpContext.Session.GetString("AccessToken");

            if (string.IsNullOrEmpty(token))
            {
                context.Result = new RedirectToActionResult("Login", "Auth", null);
                return;
            }

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                // Hämta roller från JWT (kan vara claim typ "role" eller "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")
                var tokenRoles = jwtToken.Claims
                    .Where(c => c.Type == "role" || c.Type.EndsWith("/role"))
                    .Select(c => c.Value)
                    .ToList();

                // Om roller är angivna i attributet, kolla att användaren har minst en av dem
                if (roles.Length > 0 && !roles.Any(r => tokenRoles.Contains(r)))
                {
                    context.Result = new RedirectResult("/Auth/AccessDenied");
                    return;
                }
            }
            catch
            {
                context.Result = new RedirectToActionResult("Login", "Auth", null);
                return;
            }

            base.OnActionExecuting(context);


        }
    }
}
