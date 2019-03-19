using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Http;
using CoreWebStandardLib;

namespace CoreWebApplicationMVCandAPI
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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            CoreWebStandardLib.VRWebRazor.WebRootPath = env.WebRootPath.Replace(@"\wwwroot", "");
            CoreWebStandardLib.VRBundle.WebRootPath = env.WebRootPath.Replace(@"\wwwroot", "");
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            //foreach (VRVirtualDirectory virtualDirectory in VRVirtualDirectoriesConfig.GetConfig().VirtualDirectories)
            //{
            //    app.UseFileServer(new FileServerOptions
            //    {
            //        FileProvider = new PhysicalFileProvider(virtualDirectory.PhysicalPath),
            //        RequestPath = new PathString(virtualDirectory.VirtualPath),
            //        EnableDirectoryBrowsing = false
            //    });
            //}

            //CoreWebStandardLib.VRWebRazor.Add(new CoreWebStandardLib.VRScriptBundle("bundle1").Include("/Client/javascript1.js", "/Client/javascript2.js")
            //    .IncludeDirectory("/Client", "*.*", true)
            //    .IncludeDirectory("/wwwRoot", "*.js", true)
            //    .IncludeDirectory("/Client/Common", "*.js", true)
            //    );
        }
    }
}
