using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Common.Data;
using Vanrise.Entities;
using Vanrise.Security.Entities;

namespace Vanrise.Common.Business
{
    public class SettingManager
    {
        ISecurityContext _securityContext;

        public SettingManager()
        {
            _securityContext = ContextFactory.GetContext();
        }
        public T GetSetting<T>(string type) where T : SettingData
        {
            var setting = GetCachedSettingsByType().GetRecord(type);
            return setting != null ? setting.Data as T : null;
        }

        public IDataRetrievalResult<SettingDetail> GetFilteredSettings(DataRetrievalInput<SettingQuery> input)
        {
            var allSettings = GetCachedSettings();

            var hasTechnicalSettingsView = HasViewTechnicalSettings();

            Func<Setting, bool> filterExpression = (itemObject) =>
            {
                 if (!hasTechnicalSettingsView && itemObject.IsTechnical == true) 
                    return false;

                if (input.Query == null)
                    return true;

                if (!string.IsNullOrEmpty(input.Query.Name) && !itemObject.Name.ToLower().Contains(input.Query.Name.ToLower()))
                    return false;

                if (!string.IsNullOrEmpty(input.Query.Category) && string.Compare(itemObject.Category, input.Query.Category, true) != 0)
                    return false;

               

                return true;
            };
            VRActionLogger.Current.LogGetFilteredAction(SettingLoggableEntity.Instance, input);
            return DataRetrievalManager.Instance.ProcessResult(input, allSettings.ToBigResult(input, filterExpression, SettingDetailMapper));
        }
        public string GetSettingName(Setting setting)
        {

            if (setting != null)
                return setting.Name;
            else
                return null;
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
                VRActionLogger.Current.TrackAndLogObjectUpdated(SettingLoggableEntity.Instance, setting);
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

        public Setting GetSetting(Guid settingId, bool isViewedFromUI)
        {
            var allSettings = GetCachedSettings();
            var setting  =  allSettings.GetRecord(settingId);          
            if (setting != null && isViewedFromUI)
                VRActionLogger.Current.LogObjectViewed(SettingLoggableEntity.Instance, setting);
            return setting;
        }

        public Setting GetSetting(Guid settingId)
        {
           
            return GetSetting(settingId,false);
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

        public T GetCachedOrCreate<T>(string cacheName, Func<T> createObject)
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject(cacheName, createObject);
        }

        private Dictionary<Guid, Setting> GetCachedSettings()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetSettings",
               () =>
               {
                   ISettingDataManager dataManager = CommonDataManagerFactory.GetDataManager<ISettingDataManager>();
                   IEnumerable<Setting> settings = dataManager.GetSettings();
                   return settings.ToDictionary(item => item.SettingId, item => item);
               });
        }

        private Dictionary<string, Setting> GetCachedSettingsByType()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedSettingsByType",
               () =>
               {
                   var cachedSettings = GetCachedSettings();
                   if (cachedSettings == null)
                       return null;
                   else
                       return cachedSettings.Values.ToDictionary(itm => itm.Type, itm => itm);
               });
        }

        private SettingDetail SettingDetailMapper(Setting setting)
        {
            return new SettingDetail()
            {
                Entity = setting
            };
        }
    


        public class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ISettingDataManager _dataManager = CommonDataManagerFactory.GetDataManager<ISettingDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreSettingsUpdated(ref _updateHandle);
            }
        }

        private class SettingLoggableEntity : VRLoggableEntityBase
        {
            public static SettingLoggableEntity Instance = new SettingLoggableEntity();

            private SettingLoggableEntity()
            {

            }

            static SettingManager s_settingManager = new SettingManager();

            public override string EntityUniqueName
            {
                get { return "VR_Common_Setting"; }
            }

            public override string ModuleName
            {
                get { return "Common"; }
            }

            public override string EntityDisplayName
            {
                get { return "Setting"; }
            }

            public override string ViewHistoryItemClientActionName
            {
                get { return "VR_Common_Setting_ViewHistoryItem"; }
            }

            public override object GetObjectId(IVRLoggableEntityGetObjectIdContext context)
            {
                Setting setting = context.Object.CastWithValidate<Setting>("context.Object");
                return setting.SettingId;
            }

            public override string GetObjectName(IVRLoggableEntityGetObjectNameContext context)
            {
                Setting setting = context.Object.CastWithValidate<Setting>("context.Object");
                return s_settingManager.GetSettingName(setting);
            }
            
        }

        #region Security
        public bool DoesUserHaveUpdatePermission(Guid settingId)
        {
            var setting = GetSetting(settingId);
            return setting.IsTechnical && !HasUpdateTechnicalSettings();
        }

        public bool DoesUserHaveGetSettings(Guid settingId)
        {
            var setting = GetSetting(settingId);
            return setting.IsTechnical && !HasGetTechnicalSettings();
        }

        private bool HasViewTechnicalSettings()
        {
            return _securityContext.HasPermissionToActions("VRCommon/Settings/GetFilteredTechnicalSettings");
        }
        private bool HasUpdateTechnicalSettings()
        {
            return _securityContext.HasPermissionToActions("VRCommon/Settings/UpdateTechnicalSetting");
        }
        private bool HasGetTechnicalSettings()
        {
            return _securityContext.HasPermissionToActions("VRCommon/Settings/GetTechnicalSetting");
        }
        #endregion
    }
}
