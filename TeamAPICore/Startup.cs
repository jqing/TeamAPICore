using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNet.Security.OAuth.Introspection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TeamAPICore.Context;
using TeamAPICore.Settings;

namespace TeamAPICore
{
    public class Startup
    {
        public Startup(IHostingEnvironment env, IConfiguration config)
        {
            HostingEnvironment = env;
            Configuration = config;
        }

        public IHostingEnvironment HostingEnvironment { get; }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = OAuthIntrospectionDefaults.AuthenticationScheme;
            })

            .AddOAuthIntrospection(options =>
            {
                var oAuthResource = Configuration["Tridel:oAuthResource"];
                var resource = Environment.GetEnvironmentVariable(oAuthResource);
                if (string.IsNullOrWhiteSpace(resource))
                    resource = oAuthResource;

                var oAuthResourceSec = Configuration["Tridel:oAuthResourceSec"];
                var resourceSec = Environment.GetEnvironmentVariable(oAuthResourceSec);
                if (string.IsNullOrWhiteSpace(resourceSec))
                    resourceSec = oAuthResourceSec;

                var Authority = Configuration["Tridel:Authority"];
                var AuthorityURL = Environment.GetEnvironmentVariable(Authority);
                if (string.IsNullOrWhiteSpace(AuthorityURL))
                    AuthorityURL = Configuration[$"Tridel:{Authority}"];

                options.Authority = new Uri(AuthorityURL);
                options.Audiences.Add(resource);
                options.ClientId = resource;
                options.ClientSecret = resourceSec;

                // During development, you can disable the HTTPS requirement.
                //TODO: set RequireHttpsMetadata to true on production if required
                options.RequireHttpsMetadata = false;

                // Note: you can override the default name and role claims:
                // options.NameClaimType = "custom_name_claim";
                // options.RoleClaimType = "custom_role_claim";
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("NeedCRMId", policy => policy.RequireClaim("CRMContactId"));
            });

            services.Configure<ApplicationOption>(options =>
            {
                options.ConnectionString = Configuration.GetConnectionString("Tridel");
                options.DocServiceURL = Configuration["Tridel:docServiceURL"];
            });

            services.AddScoped(typeof(ContractContext));
            //services.AddScoped(typeof(SelectedOptionContext));
            //services.AddScoped(typeof(ProductContext));

            services.Configure<IISOptions>(options =>
            {
                options.ForwardClientCertificate = false;
            });

            services.AddCors();
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment() || env.EnvironmentName == "Debug")
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseAuthentication();

            app.UseStaticFiles();
            app.UseCors(c => c.AllowAnyOrigin()
                              .AllowAnyHeader()
                              .AllowAnyMethod()
                              .AllowCredentials());

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "Default",
                    template: "{controller}/{action}/{id?}"
                    );
            });
        }
    }
}
