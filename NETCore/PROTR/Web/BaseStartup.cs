using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PROTR.Core;
using PROTR.Core.Interfaces;
using PROTR.Core.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace PROTR.Web
{
    public class BaseStartup
    {
        protected BusinessBaseProvider provider;

        public BaseStartup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Latest);
            services.AddScoped(typeof(IAppLogger<>), typeof(LoggerAdapter<>));

            services.AddSession(o => {
                IConfigurationSection section = Configuration.GetSection("Session");
                int minutes = section["Expiration"].NoNullInt();
                string cookieName = section["CookieName"] ?? ".PROTR";

                if (minutes == 0)
                {
                    minutes = 20;
                }

                o.IdleTimeout = TimeSpan.FromMinutes(minutes);
                o.Cookie.Name = cookieName;

                PROTR.Web.Helpers.CookiesHelper.CookieName = cookieName;
            });

            provider.Configure(Configuration, ConnectionMultiplexer.Connect("localhost"));
            provider.RegisterBusinessCreators();

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddAutoMapper(provider.GetType().GetTypeInfo().Assembly, typeof(BusinessBase).GetTypeInfo().Assembly);
        }

        public virtual void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Use(async (context, next) =>
            {
                await next();
                if (context.Response.StatusCode == 404)
                {
                    if (!context.Request.Path.Value.StartsWith("/api/"))
                    {
                        context.Request.Path = "/";
                        context.Response.StatusCode = 200;
                        await next();
                    }
                }
            });

            app.UseStaticFiles();

            app.UseSession();

            if (env.IsDevelopment())
            {
                app.UseCors(builder =>
                    builder
                        .WithOrigins("http://localhost:4200")
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            }

            app.UseMvc();
        }
    }
}
