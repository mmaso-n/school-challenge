using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SchoolChallenge.Client.ClientApp;
using SchoolChallenge.Client.Dependencies;

namespace SchoolChallenge.Client
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();

            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var endpointConfig = new Config();
            config.GetSection("Endpoints").Bind(endpointConfig);

            var tenantConfig = new TenantConfiguration();
            config.GetSection("TenantConfiguration").Bind(tenantConfig);

            //services.Configure<TenantConfiguration>(Configuration.GetSection("TenantConfiguration"));
            services.AddSingleton<ITenantConfiguration>(new TenantConfiguration { Tenant = tenantConfig.Tenant });
            services.AddSingleton<IHttpClient>(new ServicesHttpClient(endpointConfig.SchoolServiceEndpoint));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            #region webpack-middleware-registration
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebpackDevMiddleware();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            // Call UseWebpackDevMiddleware before UseStaticFiles
            app.UseStaticFiles();
            #endregion

            #region mvc-routing-table
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Home", action = "Index" });
            });
            #endregion
        }
    }
}