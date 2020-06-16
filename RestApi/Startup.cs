using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using LiteDbSsRepositoryService;
using logic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ServiceInterfaces;

namespace RestApi
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
            services
                .AddControllers()
                .AddJsonOptions(opts => opts.JsonSerializerOptions.Converters.Add(
                    new JsonStringEnumConverter()
                ));

            if (!services.Any(svc => svc.ServiceType == typeof(ISsRepository)))
                services.AddSingleton<ISsRepository>(_ => new LiteDbSsRepository("stipistopi.litedb"));

            services.AddSingleton<StipiStopi>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseMiddleware(typeof(ErrorHandlingMiddleware));

            app.UseFileServer();
            app.UseStaticFiles(ExtendedOptionsForHandlebarsTemplates());

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }

        private static StaticFileOptions ExtendedOptionsForHandlebarsTemplates()
        {
            var fileExtensionContentTypeProvider = new FileExtensionContentTypeProvider();
            fileExtensionContentTypeProvider.Mappings[".hbs"] = "text/plain";
            return new StaticFileOptions
            {
                ContentTypeProvider = fileExtensionContentTypeProvider,
            };
        }
    }
}
