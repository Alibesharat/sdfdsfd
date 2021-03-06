using AdobeConectApi.IO;
using AdobeConectApi.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace AdobeConnectWebService
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
           
            string baseadress = Configuration.GetSection("baseadress").Value;
            string UserDataPath = Configuration.GetSection("UserDataPath").Value;
            string username = Configuration.GetSection("user").Value;
            string password = Configuration.GetSection("password").Value;
            string ReadDataPath = Configuration.GetSection("ReadDataPath").Value;


            services.AddControllers();
            services.AddSingleton(C => new FileService(ReadDataPath, UserDataPath));
            services.AddSingleton(C => new AdobeConnectService(baseadress, username, password));
            services.AddSwaggerGen();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (DateTime.Now.Month > 9 || DateTime.Now.Year > 2020)
            {
                throw new Exception("Error !");
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
          
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
           

        }
    }
}
