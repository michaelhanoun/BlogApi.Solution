using Blog.Core.Entities;
using Blog.Infrastructure._Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Blog.Api.Extensions
{
    public static class AppMigrationAndDataSeedExtension
    {
        public static async Task AppMigrationAndDataSeed(this WebApplication app) {
            var scope = app.Services.CreateScope();
            var serviceProvider = scope.ServiceProvider;
            var userManger = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManger = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var blogContext = serviceProvider.GetRequiredService<BlogContext>();
            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            try
            {
                await blogContext.Database.MigrateAsync();
                await BlogContextSeed.SeedAsync(roleManger, userManger, blogContext);
            }
            catch (Exception e)
            {
                var logger = loggerFactory.CreateLogger<Program>();
                logger.LogError(e.Message, "An error has been occured during apply the migration");
            }
        }
    }
}
