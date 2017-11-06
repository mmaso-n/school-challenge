using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SchoolChallenge.Repository;

namespace SchoolChallenge.Services
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
            services.AddMvc();

            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            // TODO: Store & Pull the repo credentials from a KeyVault secret instead of using a settings file
            var repoConfig = new Config();
            config.GetSection("RepositorySettings").Bind(repoConfig);

            services.Configure<Config>(Configuration.GetSection("RepositorySettings"));
            services.AddSingleton<IDataRepository>(provider => new DataRepository(repoConfig.StorageConnectionString, repoConfig.StudentTable, repoConfig.TeacherTable));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
