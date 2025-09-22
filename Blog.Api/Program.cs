
using System.Threading.Tasks;
using Blog.Api.Errors;
using Blog.Api.Extensions;
using Blog.Api.Helper;
using Blog.Api.Hubs;
using Blog.Api.Middlewares;
using Blog.Application.Product_Service;
using Blog.Core;
using Blog.Core.Entities;
using Blog.Core.Services.Contracts;
using Blog.Infrastructure;
using Blog.Infrastructure._Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Blog.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers().AddNewtonsoftJson(O=>O.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);
            builder.Services.AddDbContext<BlogContext>(options=>options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(Options => Options.User.RequireUniqueEmail = true).AddEntityFrameworkStores<BlogContext>().AddDefaultTokenProviders();
            builder.Services.AddSingleton<IConnectionMultiplexer>(serviceProvider=>ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis")));
            builder.Services.AddApplicationService();
            builder.Services.AddErrorsHandlingService();
            builder.Services.AddAuthService(builder.Configuration);
            builder.Services.AddCors(options => options.AddPolicy("MyPolicy", policyOptions => { policyOptions.AllowAnyHeader().AllowAnyMethod().WithOrigins(builder.Configuration["FrontBaseUrl"]); }));
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddSwaggerService();

            var app = builder.Build();

            await app.AppMigrationAndDataSeed();
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseStatusCodePagesWithReExecute("/errors/{0}");
            app.UseHttpsRedirection();
            app.UseCors("MyPolicy");
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.MapHub<NotificationHub>("/notificationHub");

            app.Run();
        }
    }
}
