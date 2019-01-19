using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Demo.Library.Business;
using Demo.Library.Business.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PROTR.Core;
using PROTR.Web;

namespace Library.API
{
    public class Startup : BaseStartup
    {
        public Startup(IConfiguration configuration) : base(configuration)
        {
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<LibraryContext>(c =>
            {
                c.UseSqlServer(Configuration.GetSection("Data").GetSection("Instance_0")["ConnectionString"]);
            });
            base.ConfigureServices(services);

            services.AddScoped(typeof(ContextProvider), typeof(LibraryContextProvider));
            services.AddSingleton(DbDialect.Instance(DbDialectEnum.SQLServer));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        //public override void Configure(IApplicationBuilder app, IHostingEnvironment env)
        //{
        //    base.Configure(app, env);
        //}

        protected override BusinessBaseProvider CreateProvider()
        {
            return new LibraryBusinessProvider();
        }
    }
}
