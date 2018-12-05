using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Vanrise.Common;

namespace Vanrise.Web.Base
{
    public static class VRWebHost
    {
        static VRWebHost()
        {
            VRWebContext.SetWebContextReader(new ASPNETCoreVRWebRequestContextReader());
        }

        static VRWebHostOptions s_webHostOptions;
                
        internal static VRWebHostOptions GetWebHostOptionsWithValidate()
        {
            s_webHostOptions.ThrowIfNull("s_webHostOptions");
            return s_webHostOptions;
        }

        static string s_rootPhysicalPath;

        internal static void SetRootPhysicalPath(string rootPhysicalPath)
        {
            s_rootPhysicalPath = rootPhysicalPath;
        }

        internal static string GetRootPhysicalPathWithValidate()
        {
            s_rootPhysicalPath.ThrowIfNull("s_rootPhysicalPath");
            return s_rootPhysicalPath;
        }

        public static void Run<T>(VRWebHostOptions options, string[] args) where T : VRWebStartup
        {
            s_webHostOptions = options;
            BuildWebHost<T>(args).Run();
        }

        private static IWebHost BuildWebHost<T>(string[] args) where T : VRWebStartup =>
           WebHost.CreateDefaultBuilder(args)
               .UseWebRoot("")
               .UseContentRoot(@"C:\TFS\Vanrise\Code\Vanrise.Web")
               .UseStartup<T>()
               //.UseKestrel(opt => opt.ApplicationSchedulingMode = Microsoft.AspNetCore.Server.Kestrel.Transport.Abstractions.Internal.SchedulingMode.ThreadPool)
               .Build();

        private class ASPNETCoreVRWebRequestContextReader : IVRWebContextReader
        {
            public string GetCurrentRequestBaseURL()
            {
                var currentRequest = GetCurrentRequestWithValidate();
                return $"{currentRequest.Scheme}://{currentRequest.Host}{currentRequest.PathBase}";
            }

            public VRWebCookieCollection GetCurrentRequestCookies()
            {
                var cookies = new VRWebCookieCollection();
                foreach(var cookie in GetCurrentRequestWithValidate().Cookies)
                {
                    cookies.Add(cookie.Key, new VRWebCookie { Name = cookie.Key, Value = cookie.Value });
                }
                return cookies;
            }

            public string GetCurrentRequestHeader(string headerKey)
            {
                return GetCurrentRequestWithValidate().Headers[headerKey];
            }

            public string GetCurrentRequestQueryString(string parameterName)
            {
                return GetCurrentRequestWithValidate().Query[parameterName];
            }

            public VRURLParts GetCurrentRequestURLParts()
            {
                var currentRequest = GetCurrentRequestWithValidate();
                if (!currentRequest.Host.HasValue)
                    throw new NullReferenceException("currentRequest.Host");
                if (!currentRequest.Host.Port.HasValue)
                    throw new NullReferenceException("currentRequest.Host.Port");
                return new VRURLParts
                {
                    Scheme = currentRequest.Scheme,
                    Host = currentRequest.Host.Host,
                    Port = currentRequest.Host.Port.Value
                };
            }

            public string MapVirtualToPhysicalPath(string virtualPath)
            {
                var rootPhysicalPath = VRWebHost.GetRootPhysicalPathWithValidate();
                virtualPath = virtualPath.TrimStart('~');
                string physicalPath = Path.Combine(rootPhysicalPath, virtualPath.Replace("/", @"\").TrimStart('\\'));
                if (Utilities.PhysicalPathExists(physicalPath))
                {
                    return physicalPath;
                }

                foreach (var virtualDirectory in VRWebHost.GetWebHostOptionsWithValidate().VirtualDirectoryOptions.GetSortedVirtualDirectories())
                {
                    if (virtualPath.StartsWith(virtualDirectory.VirtualPath))
                    {
                        physicalPath = Path.Combine(virtualDirectory.PhysicalPath, virtualPath.Replace(virtualDirectory.VirtualPath, "").Replace("/", @"\").TrimStart('\\'));
                        if (Utilities.PhysicalPathExists(physicalPath))
                        {
                            return physicalPath;
                        }
                    }
                }
                throw new Exception($"Could not Find Physical Path of Virtual Path '{virtualPath}");
            }

            public HttpRequest GetCurrentRequestWithValidate()
            {
                var request = GetCurrentHttpContextWithValidate().Request;
                request.ThrowIfNull("request");
                return request;
            }

            private HttpContext GetCurrentHttpContextWithValidate()
            {
                var currentRequestContext = VRWebStartup.GetCurrentRequestContext();
                currentRequestContext.ThrowIfNull("currentRequestContext");
                return currentRequestContext;
            }

            public bool AreDllsInBinFolder()
            {
                return false;
            }

            public string DecodeCookieValue(string cookieValue)
            {
                return VRWebUtilities.HtmlDecode(cookieValue);
            }
        }
    }
}
