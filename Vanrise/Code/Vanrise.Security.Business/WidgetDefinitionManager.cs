using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Data;
using Vanrise.Security.Entities;
using Vanrise.Common;
namespace Vanrise.Security.Business
{
    public class WidgetDefinitionManager
    {

        public List<WidgetDefinition> GetWidgetsDefinition()
        {
            var widgetsDefinition = GetCachedWidgetsDefinition();
            return widgetsDefinition.Values.ToList();
        }
        public WidgetDefinition GetWidgetDefinitionById(int widgetDefinitionId)
        {
            var widgetsDefinition = GetCachedWidgetsDefinition();
            return widgetsDefinition.GetRecord(widgetDefinitionId);
        }

        #region Private Members

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IWidgetDefinitionDataManager _dataManager = SecurityDataManagerFactory.GetDataManager<IWidgetDefinitionDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreWidgetDefinitionsUpdated(ref _updateHandle);
            }
        }
        private Dictionary<int, WidgetDefinition> GetCachedWidgetsDefinition()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetWidgetsDefinition",
               () =>
               {
                   IWidgetDefinitionDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IWidgetDefinitionDataManager>();
                   IEnumerable<WidgetDefinition> widgetsDefinition = dataManager.GetWidgetsDefinition();
                   return widgetsDefinition.ToDictionary(kvp => kvp.ID, kvp => kvp);
               });
        }


        #endregion
    }
}
