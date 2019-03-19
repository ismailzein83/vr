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
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.Net.Http;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System.IO;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Primitives;
using System.Threading;
using Microsoft.AspNetCore.Http;
using StandardWebLib;
using Newtonsoft.Json;

namespace CoreWebApplicationAPI
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
                .AddMvc(o =>
                {
                    o.Conventions.Add(new GenericControllerRouteConvention());
                    o.Conventions.Add(new FromBodyApplicationModelConvention());
                })
                .AddJsonOptions(json =>
                {
                    json.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Unspecified;
                    json.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;                    
                })
                .ConfigureApplicationPartManager((partManager) =>
                {
                    partManager.FeatureProviders.Add(new StandardLibControllerFeatureProvider());
                })
                //     .AddApplicationPart(Assembly.Load(new AssemblyName("CoreWebApplication1")))
                //.AddApplicationPart(Assembly.Load(new AssemblyName("CoreLib")))
                //.AddControllersAsServices()
                ;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
            app.UseStaticFiles();
            app.Use(async (context, next) =>
            {
                PathString otherSegment;
                if (context.Request.Path.StartsWithSegments(new PathString("/api"), out otherSegment))
                {
                    
                    context.Response.ContentType = "application/json; charset=utf-8";
                    await context.Response.WriteAsync(Newtonsoft.Json.JsonConvert.SerializeObject(new Controllers.Response { Text = context.Request.Path }));
                }
                else
                {
                    await context.Response.WriteAsync($"Here is the request URL: {context.Request.Path.Value}");

                    if (next != null)
                        next.Invoke();
                    await context.Response.WriteAsync($" \n  First Middleware End");
                }
            });

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync($" \n 2nd Middleware ");

            });
        }

        string ParseRequestModuleName(System.Net.Http.HttpRequestMessage request, out string pathWithoutAPI)
        {
            string absolutePath = request.RequestUri.AbsolutePath;
            pathWithoutAPI = absolutePath.Substring(absolutePath.IndexOf("api/") + 4);
            return pathWithoutAPI.Substring(0, pathWithoutAPI.IndexOf('/'));
        }

        string ParseControllerName(string pathWithoutAPI, string moduleName)
        {
            string pathWithoutModuleName = pathWithoutAPI.Substring(moduleName.Length + 1);
            return pathWithoutModuleName.Substring(0, pathWithoutModuleName.IndexOf('/'));
        }
    }

    public class MyActionDescriptorChangeProvider : IActionDescriptorChangeProvider
    {
        public static MyActionDescriptorChangeProvider Instance { get; } = new MyActionDescriptorChangeProvider();

        public CancellationTokenSource TokenSource { get; private set; }

        public bool HasChanged { get; set; }

        public IChangeToken GetChangeToken()
        {
            TokenSource = new CancellationTokenSource();
            return new CancellationChangeToken(TokenSource.Token);
        }
    }

    public class Storage<T> where T : class
    {
        private Dictionary<Guid, T> storage = new Dictionary<Guid, T>();

        public IEnumerable<T> GetAll() => storage.Values;

        public T GetById(Guid id)
        {
            return storage.FirstOrDefault(x => x.Key == id).Value;
        }

        public void AddOrUpdate(Guid id, T item)
        {
            storage[id] = item;
        }
    }

    [Route("api/[controller]")]
    public class BaseController<T> : Controller where T : class
    {
        private Storage<T> _storage;

        public BaseController(Storage<T> storage)
        {
            _storage = storage;
        }

        [HttpGet]
        public IEnumerable<T> Get()
        {
            return _storage.GetAll();
        }

        [HttpGet("{id}")]
        public T Get(Guid id)
        {
            return _storage.GetById(id);
        }

        [Microsoft.AspNetCore.Mvc.HttpPost("{id}")]
        public void Post(Guid id, [FromBody]T value)
        {
            _storage.AddOrUpdate(id, value);
        }
    }

   

    public class StandardLibControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {

            var assembly = Assembly.Load("StandardWebLib");
            var candidates = assembly.GetExportedTypes();

            foreach (var candidate in candidates)
            {
                if (candidate.GetCustomAttribute<System.Web.Http.RoutePrefixAttribute>() != null)
                    feature.Controllers.Add(candidate.GetTypeInfo());
            }

            //feature.Controllers.Add(typeof(SameAppController).GetTypeInfo());
        }
    }

    public class GenericControllerRouteConvention : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            if (controller.ControllerType.GetCustomAttribute<RouteAttribute>() == null)
            {
                var customNameAttribute = controller.ControllerType.GetCustomAttribute<System.Web.Http.RoutePrefixAttribute>();


                if (customNameAttribute?.Prefix != null)
                {
                    controller.Selectors.Clear();
                    controller.Selectors.Add(new SelectorModel
                    {
                        AttributeRouteModel = new AttributeRouteModel(new RouteAttribute($"{customNameAttribute.Prefix}/[action]")),
                    });
                }
            }
        }
    }

    public class FromBodyApplicationModelConvention : IApplicationModelConvention
    {
        public void Apply(ApplicationModel application)
        {
            foreach (var controller in application.Controllers)
            {
                foreach (var action in controller.Actions)
                {
                    if (action.ActionMethod.GetCustomAttribute<System.Web.Http.HttpPostAttribute>() != null && action.Parameters.Count == 1)
                    {
                        action.Parameters[0].BindingInfo = new Microsoft.AspNetCore.Mvc.ModelBinding.BindingInfo { BindingSource = new Microsoft.AspNetCore.Mvc.ModelBinding.BindingSource("Body", "Body", true, true) };
                        //foreach (var parameter in action.Parameters)
                        //{
                        //    if (parameter.BinderMetadata is IBinderMetadata || ValueProviderResult.CanConvertFromString(parameter.ParameterInfo.ParameterType))
                        //    {
                        //        // behavior configured or simple type so do nothing
                        //    }
                        //    else
                        //    {
                        //        // Complex types are by-default from the body.
                        //        parameter.BinderMetadata = new FromBodyAttribute();
                        //    }
                        //}
                    }
                }
            }
        }
    }

    [GeneratedController("api/mod1/SameApp/[action]")]
    //[Route("api/SameApp/[action]")]
    public class SameAppController //: Controller
    {
       // [AcceptVerbs("Get")]
        public string TestMethod(string input)
        {
            return $"your input is: {input}";
        }

        [VRHttpPost]
        public VRPostOutput TestPost(VRPostInput input)
        {
            return new VRPostOutput { Prop = $"TestPost received input: {input.Prop}" };
        }
        
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2", "value3" };
        //}
    }

    public class VRPostInput
    {
        public string Prop { get; set; }
    }

    public class VRPostOutput
    {
        public string Prop { get; set; }
    }

    public class RemoteControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            var remoteCode = new HttpClient().GetStringAsync("https://gist.githubusercontent.com/filipw/9311ce866edafde74cf539cbd01235c9/raw/6a500659a1c5d23d9cfce95d6b09da28e06c62da/types.txt").GetAwaiter().GetResult();
            if (remoteCode != null)
            {
                var compilation = CSharpCompilation.Create("DynamicAssembly",
                    new[] { CSharpSyntaxTree.ParseText(remoteCode) },
                    new[] {
                    MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(RemoteControllerFeatureProvider).Assembly.Location)
                    },
                    new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

                using (var ms = new MemoryStream())
                {
                    var emitResult = compilation.Emit(ms);

                    if (!emitResult.Success)
                    {
                        // handle, log errors etc
                        Debug.WriteLine("Compilation failed!");
                        return;
                    }

                    ms.Seek(0, SeekOrigin.Begin);
                    var assembly = Assembly.Load(ms.ToArray());
                    var candidates = assembly.GetExportedTypes();

                    foreach (var candidate in candidates)
                    {
                        feature.Controllers.Add(
                            typeof(BaseController<>).MakeGenericType(candidate).GetTypeInfo()
                        );
                    }
                }
            }
        }
    }

}
