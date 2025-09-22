using Blog.Api.Errors;
using Blog.Api.Middlewares;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Api.Extensions
{
    public static class ErrorsHandlingServiceExtension
    {
        public static IServiceCollection AddErrorsHandlingService(this IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(Options =>

                Options.InvalidModelStateResponseFactory = (options) => {

                    var errors = options.ModelState.Where(M => M.Value.Errors.Any()).SelectMany(M => M.Value.Errors).Select(M => M.ErrorMessage).ToList();
                    var response = new ApiValidationErrorResponse() { Errors = errors };
                    return new BadRequestObjectResult(response);
                }
            );
            services.AddTransient<ExceptionMiddleware>();
            return services;
        }
    }
}
