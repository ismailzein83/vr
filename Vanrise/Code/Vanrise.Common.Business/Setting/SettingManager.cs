using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Common.Data;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public class SettingManager
    {
        public T GetSetting<T>(string type) where T : SettingData
        {
            var allSettings = GetCachedSettings();
            if (allSettings == null || allSettings.Count == 0)
                return null;

            return allSettings.FindRecord(itm => itm.Type == type).Data as T;
        }

        public IDataRetrievalResult<SettingDetail> GetFilteredSettings(DataRetrievalInput<SettingQuery> input)
        {
            var allSettings = GetCachedSettings();

            Func<Setting, bool> filterExpression = (itemObject) =>
                 (string.IsNullOrEmpty(input.Query.Name) || itemObject.Name.ToLower().Contains(input.Query.Name.ToLower()))
                 && (string.IsNullOrEmpty(input.Query.Category) || string.Compare(itemObject.Category, input.Query.Category, true) == 0);

            return DataRetrievalManager.Instance.ProcessResult(input, allSettings.ToBigResult(input, filterExpression, SettingDetailMapper));
        }

        public UpdateOperationOutput<SettingDetail> UpdateSetting(Setting setting)
        {
            ISettingDataManager dataManager = CommonDataManagerFactory.GetDataManager<ISettingDataManager>();

            bool updateActionSucc = dataManager.UpdateSetting(setting);
            UpdateOperationOutput<SettingDetail> updateOperationOutput = new UpdateOperationOutput<SettingDetail>();

            updateOperationOutput.Result = UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = SettingDetailMapper(setting);
            }
            return updateOperationOutput;

        }

        public List<string> GetDistinctSettingCategories()
        {
            var allSettings = GetCachedSettings();
            if (allSettings == null || allSettings.Count == 0)
                return null;

            return allSettings.Select(itm => itm.Value.Category).Distinct().ToList();
        }

        public Setting GetSetting(int settingId)
        {
            var allSettings = GetCachedSettings();
            return allSettings.GetRecord(settingId);
        }

        public Setting GetSettingByType(string type)
        {
            var allSettings = GetCachedSettings();
            if (allSettings == null)
                return null;

            return allSettings.FindRecord(itm => itm.Type == type);
        }

        public IEnumerable<Setting> GetAllSettings()
        {
            var allSettings = GetCachedSettings();
            if (allSettings == null)
                return null;

            return allSettings.Values;
        }

        private Dictionary<int, Setting> GetCachedSettings()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetSettings",
               () =>
               {
                   ISettingDataManager dataManager = CommonDataManagerFactory.GetDataManager<ISettingDataManager>();
                   IEnumerable<Setting> settings = dataManager.GetSettings();
                   return settings.ToDictionary(item => item.SettingId, item => item);
               });
        }

        private SettingDetail SettingDetailMapper(Setting setting)
        {
            return new SettingDetail()
            {
                Entity = setting
            };
        }
        #region Private Members

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ISettingDataManager _dataManager = CommonDataManagerFactory.GetDataManager<ISettingDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreSettingsUpdated(ref _updateHandle);
            }
        }

        #endregion
    }
}
