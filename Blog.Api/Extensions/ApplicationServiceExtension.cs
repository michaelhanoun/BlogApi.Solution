using Blog.Api.Errors;
using Blog.Api.Helper;
using Blog.Api.Middlewares;
using Blog.Application.Cache_Service;
using Blog.Application.EmailService;
using Blog.Application.Product_Service;
using Blog.Core;
using Blog.Core.Services.Contracts;
using Blog.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Api.Extensions
{
    public static class ApplicationServiceExtension
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection services)
        {
            services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
            services.AddSignalR();
            services.AddSingleton<IResponseCacheService, ResponseCacheService>();
            services.AddAutoMapper(cfg => cfg.AddProfile(new MappingProfile()));
            services.AddScoped(typeof(IPostService), typeof(PostService));
            services.AddScoped(typeof(IEmailSenderService), typeof(EmailSenderService));
            return services;
        }
    }
}
