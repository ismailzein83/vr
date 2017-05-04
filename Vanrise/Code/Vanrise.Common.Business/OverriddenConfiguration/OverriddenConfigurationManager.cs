using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Data;
using Vanrise.Entities;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace Vanrise.Common.Business
{
   public class OverriddenConfigurationManager
    {
        #region Public Methods

       public IDataRetrievalResult<OverriddenConfigurationDetail> GetFilteredOverriddenConfigurations(Vanrise.Entities.DataRetrievalInput<OverriddenConfigurationQuery> input)
        {
            var allOverriddenConfigurations = GetCachedOverriddenConfigurations();

            Func<OverriddenConfiguration, bool> filterExpression = (prod) =>
                 (input.Query.Name == null || prod.Name.ToLower().Contains(input.Query.Name.ToLower()))
                 &&
                 (input.Query.OverriddenConfigGroupIds == null || input.Query.OverriddenConfigGroupIds.Contains(prod.GroupId));
            VRActionLogger.Current.LogGetFilteredAction(OverriddenConfigurationLoggableEntity.Instance, input);
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allOverriddenConfigurations.ToBigResult(input, filterExpression,OverriddenConfigurationDetailMapper));
        }

       public OverriddenConfiguration GetOverriddenConfigurationHistoryDetailbyHistoryId(int overriddenConfigurationhistoryId)
        {
            VRObjectTrackingManager s_vrObjectTrackingManager = new VRObjectTrackingManager();
            var overriddenConfig = s_vrObjectTrackingManager.GetObjectDetailById(overriddenConfigurationhistoryId);
            return overriddenConfig.CastWithValidate<OverriddenConfiguration>("OverriddenConfiguration : overriddenConfigurationhistoryId ", overriddenConfigurationhistoryId);
        }
        public OverriddenConfiguration GetOverriddenConfiguration(Guid overriddenConfigurationId, bool isViewedFromUI)
        {
            var allOverriddenConfigurations = GetCachedOverriddenConfigurations();
            var overriddenConfig = allOverriddenConfigurations.GetRecord(overriddenConfigurationId);
            if (overriddenConfig != null && isViewedFromUI)
                VRActionLogger.Current.LogObjectViewed(OverriddenConfigurationLoggableEntity.Instance, overriddenConfig);
            return overriddenConfig;
        }
        public OverriddenConfiguration GetOverriddenConfiguration(Guid overriddenConfigurationId)
        {

            return GetOverriddenConfiguration(overriddenConfigurationId, false);
        }
       
        public Vanrise.Entities.InsertOperationOutput<OverriddenConfigurationDetail> AddOverriddenConfiguration(OverriddenConfiguration overriddenConfiguration)
        {
            Vanrise.Entities.InsertOperationOutput<OverriddenConfigurationDetail> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<OverriddenConfigurationDetail>();
            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            IOverriddenConfigurationDataManager dataManager = CommonDataManagerFactory.GetDataManager<IOverriddenConfigurationDataManager>();
            overriddenConfiguration.OverriddenConfigurationId = Guid.NewGuid();

            if (dataManager.Insert(overriddenConfiguration))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                VRActionLogger.Current.TrackAndLogObjectAdded(OverriddenConfigurationLoggableEntity.Instance, overriddenConfiguration);
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = OverriddenConfigurationDetailMapper(overriddenConfiguration);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }
        public Vanrise.Entities.UpdateOperationOutput<OverriddenConfigurationDetail> UpdateOverriddenConfiguration(OverriddenConfiguration overriddenConfiguration)
        {
            IOverriddenConfigurationDataManager dataManager = CommonDataManagerFactory.GetDataManager<IOverriddenConfigurationDataManager>();
            Vanrise.Entities.UpdateOperationOutput<OverriddenConfigurationDetail> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<OverriddenConfigurationDetail>();
            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (dataManager.Update(overriddenConfiguration))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                VRActionLogger.Current.TrackAndLogObjectUpdated(OverriddenConfigurationLoggableEntity.Instance, overriddenConfiguration);
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = OverriddenConfigurationDetailMapper(overriddenConfiguration);
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }
            return updateOperationOutput;
        }
        public string GetOverriddenConfigurationName(Guid overriddenConfigurationId)
        {
            var overriddenConfiguration = this.GetOverriddenConfiguration(overriddenConfigurationId);
            if (overriddenConfiguration != null)
                return overriddenConfiguration.Name;
            else
                return null;
        }
        public IEnumerable<OverriddenConfigurationConfig> GetOverriddenConfigSettingConfigs()
        {
            var extensionConfigurationManager = new ExtensionConfigurationManager();
            return extensionConfigurationManager.GetExtensionConfigurations<OverriddenConfigurationConfig>(OverriddenConfigurationConfig.EXTENSION_TYPE);
        }

        #endregion

        #region Private Classes
       
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IOverriddenConfigurationDataManager _dataManager = CommonDataManagerFactory.GetDataManager<IOverriddenConfigurationDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreOverriddenConfigurationsUpdated(ref _updateHandle);
            }
        }

        private class OverriddenConfigurationLoggableEntity : VRLoggableEntityBase
        {
            public static OverriddenConfigurationLoggableEntity Instance = new OverriddenConfigurationLoggableEntity();

            private OverriddenConfigurationLoggableEntity()
            {

            }

            static OverriddenConfigurationManager s_overriddenConfigManager = new OverriddenConfigurationManager();

            public override string EntityUniqueName
            {
                get { return "VR_Common_OverriddenConfig"; }
            }

            public override string ModuleName
            {
                get { return "Common"; }
            }

            public override string EntityDisplayName
            {
                get { return "Overridden Configuration"; }
            }

            public override string ViewHistoryItemClientActionName
            {
                get { return "VR_Common_OverriddenConfig_ViewHistoryItem"; }
            }

            public override object GetObjectId(IVRLoggableEntityGetObjectIdContext context)
            {
                OverriddenConfiguration overriddenConfiguration = context.Object.CastWithValidate<OverriddenConfiguration>("context.Object");
                return overriddenConfiguration.OverriddenConfigurationId;
            }

            public override string GetObjectName(IVRLoggableEntityGetObjectNameContext context)
            {
                OverriddenConfiguration overriddenConfiguration = context.Object.CastWithValidate<OverriddenConfiguration>("context.Object");
                return s_overriddenConfigManager.GetOverriddenConfigurationName(overriddenConfiguration.OverriddenConfigurationId);
            }
        }
        #endregion

        #region Private Methods

        private Dictionary<Guid, OverriddenConfiguration> GetCachedOverriddenConfigurations()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetOverriddenConfigurations",
              () =>
              {
                  IOverriddenConfigurationDataManager dataManager = CommonDataManagerFactory.GetDataManager<IOverriddenConfigurationDataManager>();
                  IEnumerable<OverriddenConfiguration> overriddenConfigurations = dataManager.GetOverriddenConfigurations();
                  return overriddenConfigurations.ToDictionary(o => o.OverriddenConfigurationId, o => o);
              });
        }

        #endregion

        #region Mappers

        public OverriddenConfigurationDetail OverriddenConfigurationDetailMapper(OverriddenConfiguration overriddenConfiguration)
        {
            OverriddenConfigurationDetail overriddenConfigurationDetail = new OverriddenConfigurationDetail();

            OverriddenConfigurationGroupManager overriddenConfigurationGroupManager = new OverriddenConfigurationGroupManager();
            OverriddenConfigurationGroup overriddenConfigurationGroup = overriddenConfigurationGroupManager.GetOverriddenConfigurationGroup(overriddenConfiguration.GroupId);

            overriddenConfigurationDetail.OverriddenConfigurationId = overriddenConfiguration.OverriddenConfigurationId;
            overriddenConfigurationDetail.Name = overriddenConfiguration.Name;
            overriddenConfigurationDetail.OverriddenConfigurationGroupName = (overriddenConfigurationGroup != null ? overriddenConfigurationGroup.Name : string.Empty);
            return overriddenConfigurationDetail;
        }

        #endregion

    }
}
