using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using photography_gallery.Models;
using photography_gallery.Services;
using StackExchange.Redis;

namespace photography_gallery
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddTransient<ImageListService>();
            services.AddHealthChecks();

            ConfigurationOptions redisOptions = new ConfigurationOptions
            {
                AbortOnConnectFail = false,
                ConnectTimeout = 30000,
                ConnectRetry = 3,
                SyncTimeout = 30000,
                EndPoints = { Configuration.GetSection("Config").GetSection("RedisLocation").Value }
            };
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(redisOptions);
            RedisDatabaseClass.RedisDatabase = redis.GetDatabase(Convert.ToInt32(Configuration.GetSection("Config").GetSection("RedisDatabaseRef").Value));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
                endpoints.MapHealthChecks("/health");
            });

            // Convert images
            ImageConversionService imageConversionService = new ImageConversionService(
                Configuration.GetSection("Config").GetSection("ImageInputDirectory").Value,
                Configuration.GetSection("Config").GetSection("ImageOutputDirectory").Value,
                RedisDatabaseClass.RedisDatabase);

            imageConversionService.ProcessImages();
        }
    }
}
