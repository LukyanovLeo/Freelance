using FluentValidation.AspNetCore;
using Freelance.WebApi.Contracts.Settings;
using Freelance.WebApi.Common.Filters;
using Freelance.QueryHandlers;
using Freelance.QueryHandlers.Interfaces;
using Freelance.Repositories;
using Freelance.Repositories.Interfaces;
using Freelance.Services;
using Freelance.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using Freelance.Hubs;
using Freelance.Providers;
using Microsoft.AspNetCore.SignalR;

namespace Freelance
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                options.Filters.Add(typeof(ValidateModelStateFilter));
            });

            services.AddSignalR();
            services.AddFluentValidation(options =>
            {
                options.RegisterValidatorsFromAssemblyContaining<Startup>();
            });

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            var authOption = Configuration.GetSection("AuthOptions").Get<AuthOptions>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false; //TODO: add enviroment
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = authOption.Issuer,

                        ValidateAudience = true,
                        ValidAudience = authOption.Audience,

                        ValidateLifetime = true,

                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = authOption.GetSymmetricSecurityKey()
                    };
                });

            AddSettings(services);
            AddServices(services);
            AddRepositories(services);
            AddQueryHandlers(services);
            AddValidators(services);
            AddProviders(services);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseExceptionHandler("/error");

            app.UseRouting();

            app.UseCookiePolicy(new CookiePolicyOptions
            {
                MinimumSameSitePolicy = SameSiteMode.Strict,
                HttpOnly = HttpOnlyPolicy.Always,
                Secure = env.IsDevelopment() ? CookieSecurePolicy.None : CookieSecurePolicy.Always
            });

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseCors(x => x
                .WithOrigins(Configuration.GetValue<string>("AllowedCORS").Split(";").ToArray())
                .AllowCredentials()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                context.Response.Headers.Add("X-Xss-Protection", "1");
                context.Response.Headers.Add("X-Frame-Options", "DENY");

                await next();
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<MessageHub>("/message");
            });
        }

        private void AddSettings(IServiceCollection services)
        {
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            var authOptionsSection = Configuration.GetSection("AuthOptions");
            services.Configure<AuthOptions>(authOptionsSection);
        }

        private void AddServices(IServiceCollection services)
        {
            services.AddSingleton<ITokenService, TokenService>();
            services.AddSingleton<ISessionSaltService, SessionSaltService>();
            services.AddSingleton<IHashService, HashService>();
            services.AddSingleton<ICertificateService, CertificateService>();
            services.AddSingleton<IStoreService, StoreService>();
            services.AddSingleton<IImageService, ImageService>();
        }

        private void AddRepositories(IServiceCollection services)
        {
            services.AddSingleton<IDapperService, DapperService>();
            services.AddSingleton<IAuthSaltRepository, AuthSaltRepository>();
            services.AddSingleton<IUserRepository, UserRepository>();
            services.AddSingleton<ITokenRepository, TokenRepository>();
            services.AddSingleton<IStoreRepository, StoreRepository>();
            services.AddSingleton<IChatRepository, ChatRepository>();
        }

        private void AddQueryHandlers(IServiceCollection services)
        {
            services.AddSingleton<IAuthQueryHandler, AuthQueryHandler>();
            services.AddSingleton<IAttachmentQueryHandler, AttachmentQueryHandler>();
            services.AddSingleton<IChatQueryHandler, ChatQueryHandler>();
            services.AddSingleton<IImageQueryHandler, ImageQueryHandler>();
        }

        private void AddProviders(IServiceCollection services)
        {
            services.AddSingleton<IUserIdProvider, UserIdProvider>();
        }

        private void AddValidators(IServiceCollection services)
        {
            services.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Startup>());
        }
    }
}
