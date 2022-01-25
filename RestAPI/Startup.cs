using AmiMesApi.DataBase;
using AmiMesApi.Middleware;
using AmiMesApi.Model;
using AmiMesApi.Services;
using API_AmiOEE.Services;
using API_AmiOEE.Storages;
using API_AmiOrder.DataBase;
using API_AmiOrder.Services;
using API_AmiTrace.DataBase;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AmiMesApi
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
            services.AddScoped<IEncryptionService>(s => new EncryptionService(Configuration.GetValue<string>("AppSettings:Key")));
            services.AddControllers().AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve);
            services.AddControllersWithViews().AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
            EncryptionService encrypter= new(Configuration.GetValue<string>("AppSettings:Key"));
            //string oeeConnString = encrypter.Decrypt(Configuration.GetConnectionString("AmiOeeDb"));
            //string amiOrderConnString = encrypter.Decrypt(Configuration.GetConnectionString("AmiOrderDb"));
            string amiMesConnString = encrypter.Decrypt(Configuration.GetConnectionString("AmiMesSystemDb"));

            services.AddDbContext<MigrationsDbContext>(options => { options.UseSqlServer(amiMesConnString); });
            services.AddDbContext<AmiMesSystemDbContext>(options => { options.UseSqlServer(amiMesConnString); });
            services.AddDbContext<OeeDbContext>(options => { options.UseSqlServer(amiMesConnString); });
            services.AddDbContext<AmiOrderDbContext>(options => { options.UseSqlServer(amiMesConnString); });
            services.AddDbContext<AmiTraceDbContext>(options => { options.UseSqlServer(amiMesConnString); });

            services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve);
            services.AddMvc();
            services.AddSingleton<IAppInfo>(x => new AppInfo(Configuration.GetValue<AppSettings>("AppSettings")));
            services.AddSingleton<IWorkTimeStatusStorage, WorkTimeStatusStorage>();

            services.AddScoped<IAuthorizationService, AuthorizationService>();
            services.AddScoped<IAppSeettingsReader, AppSeettingsReader>();
            services.AddScoped<IProductionShiftService, ProductionShiftService>();
            services.AddScoped<IEventChangeService, EventChangeService>();
            services.AddScoped<IOrderProcessesService, OrderProcessesService>();
            services.AddScoped<ICookieService, CookieService>(s => new CookieService());
            services.AddScoped<IPlannedDowntimesService, PlannedDowntimesService>();
            services.AddControllersWithViews();

            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "oeewizu/dist";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseTokenVerify();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // use middleware and launch server for Vue
            app.UseSpaStaticFiles();
            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "oeewizu";

                if (env.IsDevelopment())
                {
                    spa.UseProxyToSpaDevelopmentServer("http://localhost:8080");
                }
            });
        }
    }
}
