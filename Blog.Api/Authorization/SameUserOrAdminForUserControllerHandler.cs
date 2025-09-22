using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace Blog.Api.Authorization
{
    public class SameUserOrAdminForUserControllerHandler : AuthorizationHandler<SameUserOrAdminRequirementForUserController>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SameUserOrAdminRequirementForUserController requirement)
        {
            var isAdmin = context.User.IsInRole("Admin");
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(context.Resource is HttpContext httpContext)
            {
                var requestId = httpContext.Request.RouteValues["id"]?.ToString();
                if(requestId == userId || isAdmin)
                    context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
