using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.Extensions.PlatformAbstractions;
using System.IO;
using Voicecoin.Core;

namespace Voicecoin.WebStarter
{
    public partial class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment appEnv)
        {
            Configuration = configuration;
            CurrentEnvironment = appEnv;
        }

        public IConfiguration Configuration { get; }
        private IHostingEnvironment CurrentEnvironment { get; set; }
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddJwtAuth(Configuration);

            services.AddMvc();

            // Add framework services.
            services.AddMvc(options =>
            {
                options.RespectBrowserAcceptHeader = true;
            }).AddJsonOptions(options =>
            {
                options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                //options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            });

            services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new ApiKeyScheme() { In = "header", Description = "Please insert JWT with Bearer into field", Name = "Authorization", Type = "apiKey" });

                var info = Configuration.GetSection("Swagger").Get<Swashbuckle.AspNetCore.Swagger.Info>();
                c.SwaggerDoc(info.Version, info);

                var filePath = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "Voicecoin.RestApi.xml");
                c.IncludeXmlComments(filePath);

                filePath = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "ContentFoundation.RestApi.xml");
                c.IncludeXmlComments(filePath);
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.EnabledValidator();
                c.SupportedSubmitMethods(new[] { "get", "post", "put", "patch", "delete" });
                c.ShowRequestHeaders();
                c.ShowJsonEditor();
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
                c.RoutePrefix = "api";
            });

            app.UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin().AllowCredentials());

            app.UseAuthentication();
            app.UseMvc();

            app.UseEntityDbContext(Configuration, env.ContentRootPath, new String[] { "CustomEntityFoundation.Core", "Quickflow.Core", "Quickflow.ActivityRepository", "ContentFoundation.Core", "Voicecoin.Core" });
            app.UseInitLoader();

            new AccountReceivableStatusJob().Execute(null);
        }

    }
}
