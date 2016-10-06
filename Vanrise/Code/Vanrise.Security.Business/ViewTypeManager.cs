using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Data;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Business
{
    public class ViewTypeManager
    {
        public IEnumerable <ViewType> GetViewTypes()
        {
            return GetCachedViewTypes().Values;
        }
        public Guid GetViewTypeIdByName(string name)
        {
            if (name == null)
                throw new NullReferenceException("ViewTypeName");
            var viewTypes = GetCachedViewTypes().Values;
            var viewType = viewTypes.FirstOrDefault(x=>x.Name == name);
            if(viewType == null)
                 throw new NullReferenceException("ViewType");
            return viewType.ViewTypeId;
        }
        
        #region Private Members

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IViewTypeDataManager _dataManager = SecurityDataManagerFactory.GetDataManager<IViewTypeDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreViewTypesUpdated(ref _updateHandle);
            }
        }
        private Dictionary<Guid, ViewType> GetCachedViewTypes()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedViewTypes",
               () =>
               {
                   IViewTypeDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IViewTypeDataManager>();
                   IEnumerable<ViewType> viewTypes = dataManager.GetViewTypes();
                   return viewTypes.ToDictionary(kvp => kvp.ViewTypeId, kvp => kvp);
               });
        }
     
        #endregion
    }
}
