using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demo.Module.Data;
using Demo.Module.Entities;
using Vanrise.Entities;
using Vanrise.Common;
using Vanrise.Integration.Business;
using Vanrise.Integration.Entities;
using System.ComponentModel;
namespace Demo.Module.Business
{
    public class ServiceTypeManager
    {

        public IEnumerable<ServiceType> GetServiceTypesInfo()
        {
            IEnumerable<ServiceType> allServices = GetCachedServiceTypes().Values;
            return allServices;
        }
        public ServiceType GetServiceType(int serviceTypeId)
        {
            return GetCachedServiceTypes().GetRecord(serviceTypeId);
        }

        public string GetServiceTypeName(int serviceTypeId)
        {
            ServiceType serviceType = GetServiceType(serviceTypeId);

            if (serviceType != null)
                return serviceType.Description ;

            return null;
        }
        
        private Dictionary<int, ServiceType> GetCachedServiceTypes()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetServiceTypes",
               () =>
               {
                   IServiceTypeDataManager dataManager = DemoModuleDataManagerFactory.GetDataManager<IServiceTypeDataManager>();
                   IEnumerable<ServiceType> serviceTypes = dataManager.GetAllServiceTypes();
                   return serviceTypes.ToDictionary(st => st.ServiceTypeId, st => st);
               });
        }
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IServiceTypeDataManager _dataManager = DemoModuleDataManagerFactory.GetDataManager<IServiceTypeDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreServiceTypeUpdated(ref _updateHandle);
            }
        }
        
    }
}
