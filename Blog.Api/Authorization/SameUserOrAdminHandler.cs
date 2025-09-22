using System.Security.Claims;
using Blog.Core;
using Blog.Core.Entities.Post_Aggregate;
using Blog.Core.Specifications.Post_Specs;
using Microsoft.AspNetCore.Authorization;

namespace Blog.Api.Authorization
{
    public class SameUserOrAdminHandler : AuthorizationHandler<SameUserOrAdminRequirement>
    {
        private readonly IUnitOfWork _unitOfWork;

        public SameUserOrAdminHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, SameUserOrAdminRequirement requirement)
        {
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = context.User.IsInRole("Admin");
            if (context.Resource is HttpContext httpContext)
            {
                var routeParamName = "id";
                if (int.TryParse(httpContext.Request.RouteValues[routeParamName]?.ToString(), out int requestId))
                {
                    var post = await _unitOfWork.Repository<Post>().GetWithSpecAsync(new PostDraftSpecification(requestId));
                    if (post != null)
                    {
                        if (post.ApplicationUser.Id == userId || isAdmin)
                            context.Succeed(requirement);
                    }
                }
            }
        }
    }
}
