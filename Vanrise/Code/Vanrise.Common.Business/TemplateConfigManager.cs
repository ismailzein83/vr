using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Data;

namespace Vanrise.Common.Business
{
    public class TemplateConfigManager
    {
        //public List<Entities.TemplateConfig> GetTemplateConfigurations(string configType)
        //{
        //    ITemplateConfigDataManager manager = CommonDataManagerFactory.GetDataManager<ITemplateConfigDataManager>();
        //    return manager.GetTemplateConfigurations(configType);
        //}

        //public List<Entities.TemplateConfig> GetAllTemplateConfigurations()
        //{
        //    ITemplateConfigDataManager manager = CommonDataManagerFactory.GetDataManager<ITemplateConfigDataManager>();
        //    return manager.GetTemplateConfigurations();
        //}
        //public Entities.TemplateConfig GetTemplateConfiguration(int templateConfigID)
        //{
        //    var allTemplateConfigs = GetCachedTemplateConfigurations();
        //    return allTemplateConfigs.GetRecord(templateConfigID);
        //}
        //public T GetBehavior<T>(int configId) where T : class
        //{
        //    var allTemplates = GetAllTemplateConfigurations();
        //    if (allTemplates != null)
        //    {
        //        Vanrise.Entities.TemplateConfig templateConfig = allTemplates.FirstOrDefault(itm => itm.TemplateConfigID == configId);
        //        if (templateConfig != null)
        //        {
        //            Type t = Type.GetType(templateConfig.BehaviorFQTN);
        //            return Activator.CreateInstance(t) as T;
        //        }
        //    }
        //    return null;
        //}

        //#region Private Methods

        //Dictionary<int, Entities.TemplateConfig> GetCachedTemplateConfigurations()
        //{
        //    return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetTemplateConfigurations",
        //       () =>
        //       {
        //           IEnumerable<Entities.TemplateConfig> templateConfigurations = GetAllTemplateConfigurations();
        //           return templateConfigurations.ToDictionary(t => t.TemplateConfigID, t => t);
        //       });
        //}

        //private class CacheManager : Vanrise.Caching.BaseCacheManager
        //{
        //    ITemplateConfigDataManager _dataManager = CommonDataManagerFactory.GetDataManager<ITemplateConfigDataManager>();
        //    object _updateHandle;

        //    protected override bool ShouldSetCacheExpired(object parameter)
        //    {
        //        return _dataManager.AreTemplateConfigurationsUpdated(ref _updateHandle);
        //    }
        //}
        //#endregion
    }
}
