using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using Vanrise.Entities;
using Vanrise.Common;

namespace Vanrise.Web
{
    public class VRHttpControllerSelector : DefaultHttpControllerSelector
    {
        HttpConfiguration _configuration;
        Dictionary<string, APIDiscoveryState> _apiDiscoveryStatesByModuleName;
        public VRHttpControllerSelector(HttpConfiguration configuration)
            : base(configuration)
        {
            _configuration = configuration;
            LoadAPIDiscoveries();

        }

        private void LoadAPIDiscoveries()
        {
            _apiDiscoveryStatesByModuleName = new System.Collections.Generic.Dictionary<string, APIDiscoveryState>();
            var apiDiscoveryTypes = Common.Utilities.GetAllImplementations<VRAPIDiscovery>();
            if (apiDiscoveryTypes != null)
            {
                foreach (var apiDiscoveryType in apiDiscoveryTypes)
                {
                    var apiDiscovery = Activator.CreateInstance(apiDiscoveryType).CastWithValidate<VRAPIDiscovery>("apiDiscovery");
                    List<string> moduleNames = apiDiscovery.GetModuleNames(null);
                    var apiDiscoveryState = new APIDiscoveryState(_configuration, apiDiscovery);
                    if (moduleNames != null)
                    {
                        foreach (var moduleName in moduleNames)
                        {
                            if (_apiDiscoveryStatesByModuleName.ContainsKey(moduleName))
                                throw new Exception(String.Format("Duplicate Module Name '{0}'", moduleName));
                            _apiDiscoveryStatesByModuleName.Add(moduleName, apiDiscoveryState);
                        }
                    }
                }
            }
        }

        public override HttpControllerDescriptor SelectController(System.Net.Http.HttpRequestMessage request)
        {
            string pathWithoutAPI;
            string moduleName = ParseRequestModuleName(request, out pathWithoutAPI);
            APIDiscoveryState moduleAPIDiscoveryState;
            if (moduleName != null && _apiDiscoveryStatesByModuleName.TryGetValue(moduleName, out moduleAPIDiscoveryState))
            {
                string controllerName = ParseControllerName(pathWithoutAPI, moduleName);
                VRHttpControllerDescriptor controllerDescriptor = moduleAPIDiscoveryState.GetControllerDescriptor(controllerName);
                return controllerDescriptor;
            }
            else
            {
                var controllerDescriptor = base.SelectController(request);
                return controllerDescriptor;
            }
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


        #region Private Classes

        private class APIDiscoveryState
        {
            HttpConfiguration _configuration;
            VRAPIDiscovery _apiDiscovery;

            CacheManager _cacheManager;
            public APIDiscoveryState(HttpConfiguration configuration, VRAPIDiscovery apiDiscovery)
            {
                _configuration = configuration;
                _apiDiscovery = apiDiscovery;
                _cacheManager = Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>();
                _cacheManager._apiDiscovery = apiDiscovery;
            }

            public VRHttpControllerDescriptor GetControllerDescriptor(string controllerName)
            {
                VRHttpControllerDescriptor controllerDescriptor;
                if (!GetCachedControllerDescriptors().TryGetValue(controllerName, out controllerDescriptor))
                    throw new Exception(String.Format("Controller '{0}' not found", controllerName));
                return controllerDescriptor;
            }

            Dictionary<string, VRHttpControllerDescriptor> GetCachedControllerDescriptors()
            {
                return _cacheManager.GetOrCreateObject("GetCachedControllerDescriptors", () =>
                {
                    Dictionary<string, VRHttpControllerDescriptor> controllerDescriptors = new System.Collections.Generic.Dictionary<string, VRHttpControllerDescriptor>();
                    List<Type> controllerTypes = _apiDiscovery.GetControllerTypes(null);
                    if (controllerTypes != null)
                    {
                        foreach (var controllerType in controllerTypes)
                        {
                            string controllerName = controllerType.Name.Replace("Controller", "");
                            if (controllerDescriptors.ContainsKey(controllerName))
                                throw new Exception(String.Format("Duplicate Controller Name '{0}' in APIDiscovery '{1}'", controllerName, _apiDiscovery.GetType().FullName));
                            controllerDescriptors.Add(controllerName, new VRHttpControllerDescriptor(_configuration, controllerName, controllerType));
                        }
                    }
                    return controllerDescriptors;
                });
            }

            private class CacheManager : Vanrise.Caching.BaseCacheManager
            {
                DateTime _lastCheckTime;

                internal VRAPIDiscovery _apiDiscovery;

                protected override bool ShouldSetCacheExpired()
                {
                    return _apiDiscovery.IsCacheExpired(ref _lastCheckTime);
                }
            }
        }

        #endregion

    }
}