using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Data;
using Vanrise.Analytic.Entities;
using Vanrise.Caching;
using Vanrise.Common;
namespace Vanrise.Analytic.Business
{
    public class WidgetDefinitionManager
    {
        #region Public Methods
        public IEnumerable<WidgetDefinition> GetWidgetDefinitions()
        {
            var widgetDefinitions = GetCachedWidgetDefinitions();
            return widgetDefinitions.Values;
        }
        public WidgetDefinition GetWidgetDefinitionById(int widgetDefinitionId)
        {
            var widgetDefinitions = GetCachedWidgetDefinitions();
            return widgetDefinitions.GetRecord(widgetDefinitionId);
        }
        #endregion

        #region Private Methods

        Dictionary<int, WidgetDefinition> GetCachedWidgetDefinitions()
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedWidgetDefinitions",
                () =>
                {
                    IWidgetDefinitionDataManager dataManager = AnalyticDataManagerFactory.GetDataManager<IWidgetDefinitionDataManager>();
                    IEnumerable<WidgetDefinition> widgetDefinitions = dataManager.GetWidgetDefinitions();
                    return widgetDefinitions.ToDictionary(x => x.WidgetDefinitionId, widgetDefinition => widgetDefinition);
                });
        }
        #endregion

        #region Private Classes

        public class CacheManager : Vanrise.Caching.BaseCacheManager
        {

            IWidgetDefinitionDataManager _dataManager = AnalyticDataManagerFactory.GetDataManager<IWidgetDefinitionDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreWidgetDefinitionUpdated(ref _updateHandle);
            } 
        }

        #endregion

        #region Mappers
        #endregion
    }
}
