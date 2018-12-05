using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Vanrise.Common;

namespace Vanrise.Web.Base
{
    public class VRWebStartup
    {
        public VRWebStartup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddMvc(o =>
                {
                    o.Conventions.Add(new VRControllerRouteConvention());
                    o.Conventions.Add(new FromBodyApplicationModelConvention());
                })
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                    options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Unspecified;
                    options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                    options.SerializerSettings.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Objects;
                })
                .ConfigureApplicationPartManager((partManager) =>
                {
                    partManager.FeatureProviders.Add(new VRControllerFeatureProvider());
                })
                .AddApplicationPart(Assembly.Load(new AssemblyName("Vanrise.Web.Base")))
                .AddControllersAsServices();
        }

        [ThreadStatic]
        static HttpContext s_currentRequestContext;

        internal static HttpContext GetCurrentRequestContext()
        {
            return s_currentRequestContext;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            VRWebHost.SetRootPhysicalPath(env.WebRootPath);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            foreach (var virtualDirectory in VRWebHost.GetWebHostOptionsWithValidate().VirtualDirectoryOptions.GetSortedVirtualDirectories())
            {
                var fileServiceOptions = new FileServerOptions
                {
                    RequestPath = new PathString(virtualDirectory.VirtualPath),
                    FileProvider = new PhysicalFileProvider(virtualDirectory.PhysicalPath),
                    EnableDirectoryBrowsing = false
                };
                fileServiceOptions.StaticFileOptions.ServeUnknownFileTypes = true;
                fileServiceOptions.StaticFileOptions.DefaultContentType = "text/plain";
                app.UseFileServer(fileServiceOptions);
            }

            app.UseStaticFiles();

            VRWebContext.RegisterApplicationWebBundles();

            app.Use(async (context, next) =>
            {
                if (s_currentRequestContext != null)
                    throw new Exception("s_currentRequestContext is already set");
                s_currentRequestContext = context;
                
                if (next != null)
                    await next.Invoke();
                s_currentRequestContext = null;
            });
            
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            //app.Use(async (context, next) =>
            //{
            //    PathString otherSegment;
            //    if (context.Request.Path.StartsWithSegments(new PathString("/api"), out otherSegment))
            //    {

            //        context.Response.ContentType = "application/json; charset=utf-8";
            //        await context.Response.WriteAsync(Newtonsoft.Json.JsonConvert.SerializeObject(new Controllers.Response { Text = context.Request.Path }));
            //    }
            //    else
            //    {
            //        await context.Response.WriteAsync($"Here is the request URL: {context.Request.Path.Value}");

            //        if (next != null)
            //            await next.Invoke();
            //        await context.Response.WriteAsync($" \n  First Middleware End");
            //    }
            //});

            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync($" \n 2nd Middleware ");

            //});
        }

        //string ParseRequestModuleName(System.Net.Http.HttpRequestMessage request, out string pathWithoutAPI)
        //{
        //    string absolutePath = request.RequestUri.AbsolutePath;
        //    pathWithoutAPI = absolutePath.Substring(absolutePath.IndexOf("api/") + 4);
        //    return pathWithoutAPI.Substring(0, pathWithoutAPI.IndexOf('/'));
        //}

        //string ParseControllerName(string pathWithoutAPI, string moduleName)
        //{
        //    string pathWithoutModuleName = pathWithoutAPI.Substring(moduleName.Length + 1);
        //    return pathWithoutModuleName.Substring(0, pathWithoutModuleName.IndexOf('/'));
        //}
    }
}
