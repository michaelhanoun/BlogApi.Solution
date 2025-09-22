using System.Security.Claims;
using System.Text;
using Blog.Api.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace Blog.Api.Extensions
{
    public static class AuthServiceExtension
    {
        public static IServiceCollection AddAuthService( this IServiceCollection services,IConfiguration configuration)
        {
            services.AddAuthorization(options =>
             {
                options.AddPolicy("SameUserOrAdmin", policy =>
                    policy.AddRequirements(new SameUserOrAdminRequirement()));
                 options.AddPolicy("SameUserOrAdminForUserController", policy => policy.AddRequirements(new SameUserOrAdminRequirementForUserController()));
             });
            services.AddScoped<IAuthorizationHandler, SameUserOrAdminHandler>();
            services.AddTransient<IAuthorizationHandler, SameUserOrAdminForUserControllerHandler>();
            services.AddScoped(typeof(Core.Services.Contracts.Authentication_Service.IAuthenticationService), typeof(Application.Auth_Service.AuthenticationService));
            services.AddAuthentication(options => { options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options => { options.TokenValidationParameters = new()
                    {
                    ValidateIssuer = true,
                    ValidIssuer = configuration["Jwt:Issuer"] ,
                    ValidateAudience=true,
                    ValidAudience= configuration["Jwt:Audience"],
                    ValidateIssuerSigningKey=true,
                    IssuerSigningKey=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]??string.Empty)),
                    ValidateLifetime = true,ClockSkew=TimeSpan.Zero
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context => {

                            if(context.Exception.GetType()==typeof(SecurityTokenExpiredException))
                            {
                                context.Response.Headers.Add("Token-Expired", "true");
                            }
                                return Task.CompletedTask;
                        }
                    };
                }).AddGoogle(options => {
                    options.ClientId = configuration["Authentication:Google:ClientId"];
                    options.ClientSecret = configuration["Authentication:Google:CliectSecret"];
                    options.Scope.Add("profile");
                    options.Scope.Add("email");
                    options.Scope.Add("https://www.googleapis.com/auth/user.phonenumbers.read");
                    options.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
                    options.ClaimActions.MapJsonKey(ClaimTypes.MobilePhone, "phoneNumber");
                }
                );
            services.AddAuthorization();
            return services;
        }
    }
}
