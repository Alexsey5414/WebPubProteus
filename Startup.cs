using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Prometheus;
using System;
using WebTestProteus.Classes;

namespace WebTestProteus
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        [Obsolete]
        public void ConfigureServices(IServiceCollection services)
        {
           // services.AddControllers();
              //{
              //    // Use the default property (Pascal) casing
              //    options.SerializerSettings.ContractResolver = new DefaultContractResolver();

              //    // Configure a custom converter
              //    options.SerializerOptions.Converters.Add(new MyCustomJsonConverter());
              //});
            // добавление кэширования
           // services.AddMemoryCache();
            //  provider.
            //  services.AddControllers();
            //UseInMemoryDatabase("test")
            // var serv = services.gBuildServiceProvider().GetService<IMemoryCache>();
            // Microsoft.Extensions.Caching.Memory.MemoryCache mem = new Microsoft.Extensions.Caching.Memory.MemoryCache(new MemoryCacheOptions());

        //    services.AddDbContext<ApiContext>(ServiceLifetime.Singleton);
             services.AddDbContext<ApiContext>(opt=>opt.UseInMemoryDatabase("test"));
            // services.AddDbContext<ApiContext>(opt => opt.UseMemoryCache(new Microsoft.Extensions.Caching.Memory.MemoryCache(new MemoryCacheOptions())));
            services.AddMvc()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.WriteIndented = true;
             //   options.JsonSerializerOptions.;

            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }


            //using (var scope = app.ApplicationServices.CreateScope())
            //{

            //    var context = scope.ServiceProvider.GetRequiredService<ApiContext>();

            //  //  var context = app.ApplicationServices.GetService<ApiContext>();
            //    context.AddTestData();

            //   // do your stuff....
            //}

            //  var context = app.ApplicationServices.GetService<ApiContext>();


            app.UseRouting();
            app.UseHttpMetrics();

            app.UseAuthorization();
            // app.UseMvc();
          //  app.UseStartFillData();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapMetrics();
            });

  
        }
    }
}
