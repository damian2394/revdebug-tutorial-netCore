using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Invoices.DataAccess;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace InvoicesCore
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
#if! DEBUG
            var connstring = Configuration.GetConnectionString("DefaultConnection");
            var server = Environment.GetEnvironmentVariable("LS_PSQL_NODE_PORT_5432_TCP_ADDR");
            var port = Environment.GetEnvironmentVariable("LS_PSQL_NODE_PORT_5432_TCP_PORT");
            var user = Environment.GetEnvironmentVariable("LS_PSQL_NODE_ENV_POSTGRES_USER");
            var password = Environment.GetEnvironmentVariable("LS_PSQL_NODE_ENV_POSTGRES_PASSWORD");

            connstring = connstring.Replace("$Server", server).Replace("$Port", port).Replace("$User", user).Replace("$Password", password);
#else
            var connstring = Configuration.GetConnectionString("DebugConnection");
#endif

            services.AddMvc().AddJsonOptions(options => { options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore; });

            services.AddMvc();

            services.AddDbContext<InvoicesContext>(options => options.UseNpgsql(connstring));
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, InvoicesContext context)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            new InvoicesInitializer().Seed(context);
            
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
