using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Data;
using Vanrise.Entities;
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

        public string GenerateOverriddenConfigurationGroupScript(Guid overriddenConfigurationGroupId)
        {
            List<OverriddenConfiguration> overriddenConfigurations = GetCachedOverriddenConfigurations().Values.Where(itm => itm.GroupId == overriddenConfigurationGroupId).ToList();
            if (overriddenConfigurations == null)
                return null;
            Action<OverriddenConfigurationBehavior, List<OverriddenConfiguration>, Action<string, string>> generateSingleConfigBehaviorScript =
                (behavior, behaviorOverriddenConfigurations, addEntityScriptAction) =>
                {
                    var context = new OverriddenConfigurationBehaviorGenerateScriptContext(behaviorOverriddenConfigurations, addEntityScriptAction);
                    behavior.GenerateScript(context);
                };
            return GenerateOverriddenConfigurationsScript(overriddenConfigurations, generateSingleConfigBehaviorScript);
        }

       public string GenerateOverriddenConfigurationDevScript ()
        {
            List<OverriddenConfiguration> overriddenConfigurations = GetCachedOverriddenConfigurations().Values.ToList();
            if (overriddenConfigurations == null)
                return null;
            StringBuilder strBuilder = new StringBuilder();
            Action<string, string> addEntityScriptAction = (entityName, entityScript) => AddEntityScript(strBuilder, entityName, entityScript);
           OverriddenConfigurationGroupManager groupManager = new OverriddenConfigurationGroupManager();
            List<OverriddenConfigurationGroup> groups = new List<OverriddenConfigurationGroup>();
           foreach(var groupId in overriddenConfigurations.Select(itm => itm.GroupId).Distinct())
           {
               OverriddenConfigurationGroup group = groupManager.GetOverriddenConfigurationGroup(groupId);
               group.ThrowIfNull("group", groupId);
               groups.Add(group);
           }
           groupManager.GenerateScript(groups, addEntityScriptAction);

           IOverriddenConfigurationDataManager dataManager = CommonDataManagerFactory.GetDataManager<IOverriddenConfigurationDataManager>();
           dataManager.GenerateScript(overriddenConfigurations, addEntityScriptAction);

           Action<OverriddenConfigurationBehavior, List<OverriddenConfiguration>, Action<string, string>> generateSingleConfigBehaviorScript =
               (behavior, behaviorOverriddenConfigurations, addEntityScriptAction_local) =>
               {
                   var context = new OverriddenConfigurationBehaviorGenerateDevScriptContext(behaviorOverriddenConfigurations, addEntityScriptAction_local);
                   behavior.GenerateDevScript(context);
               };
           string overriddenConfigsScript = GenerateOverriddenConfigurationsScript(overriddenConfigurations, generateSingleConfigBehaviorScript);
           strBuilder.AppendLine();
           strBuilder.AppendLine();
           strBuilder.Append(overriddenConfigsScript);
           return strBuilder.ToString();
        }

       public string GenerateOverriddenConfigurationsScript(List<OverriddenConfiguration> overriddenConfigurations, Action<OverriddenConfigurationBehavior, List<OverriddenConfiguration>, Action<string, string>> generateSingleConfigBehaviorScript)
       {
           Dictionary<Type, List<OverriddenConfiguration>> configsByBehaviorType = new Dictionary<Type, List<OverriddenConfiguration>>();
           foreach (var config in overriddenConfigurations)
           {
               config.Settings.ThrowIfNull("config.Settings", config.OverriddenConfigurationId);
               config.Settings.ExtendedSettings.ThrowIfNull("config.Settings.ExtendedSettings", config.OverriddenConfigurationId);
               var behaviorType = config.Settings.ExtendedSettings.GetBehaviorType(null);
               behaviorType.ThrowIfNull("behaviorType", config.OverriddenConfigurationId);
               configsByBehaviorType.GetOrCreateItem(behaviorType).Add(config);
           }
           StringBuilder builder = new StringBuilder();
           foreach (var configsByBehaviorTypeEntry in configsByBehaviorType)
           {
               OverriddenConfigurationBehavior behavior = Activator.CreateInstance(configsByBehaviorTypeEntry.Key).CastWithValidate<OverriddenConfigurationBehavior>("behavior");
               StringBuilder modScriptBuilder = new StringBuilder();
               Action<string, string> addEntityScriptAction = (entityName, entityScript) => AddEntityScript(modScriptBuilder, entityName, entityScript);
               generateSingleConfigBehaviorScript(behavior, configsByBehaviorTypeEntry.Value,  addEntityScriptAction);
               if (modScriptBuilder.Length > 0)
               {
                   string moduleName = behavior.ModuleName;
                   if (!string.IsNullOrEmpty(moduleName))
                   {
                       builder.AppendLine();
                       builder.AppendFormat("-------------- START Module Id '{0}' -------------------", moduleName);
                       builder.AppendLine();
                       builder.AppendLine("-----------------------------------------------------------------------------------------");
                   }

                   builder.AppendLine();
                   builder.AppendLine(modScriptBuilder.ToString());
                   builder.AppendLine();

                   if (!string.IsNullOrEmpty(moduleName))
                   {
                       builder.AppendLine("-----------------------------------------------------------------------------------------");
                       builder.AppendFormat("-------------- END Module Id '{0}' -------------------", moduleName);
                       modScriptBuilder.AppendLine();
                   }
               }
           }
           return builder.ToString();
       }

       void AddEntityScript(StringBuilder scriptBuilder, string entityName, string entityScript)
       {
           scriptBuilder.AppendLine();
           scriptBuilder.AppendFormat("-------------- START Entity '{0}' -------------------", entityName);
           scriptBuilder.AppendLine();
           scriptBuilder.AppendLine("-----------------------------------------------------------------------------------------");
           scriptBuilder.AppendLine("BEGIN");
           scriptBuilder.AppendLine();
           scriptBuilder.AppendLine(entityScript);
           scriptBuilder.AppendLine();
           scriptBuilder.AppendLine("END");
           scriptBuilder.AppendLine("-----------------------------------------------------------------------------------------");
           scriptBuilder.AppendFormat("-------------- END Entity '{0}' -------------------", entityName);
           scriptBuilder.AppendLine();
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

        private class OverriddenConfigurationBehaviorGenerateScriptContext : IOverriddenConfigurationBehaviorGenerateScriptContext
        {
            Action<string, string> _addEntityScriptAction;

            public OverriddenConfigurationBehaviorGenerateScriptContext(List<OverriddenConfiguration> configs, Action<string, string> addEntityScriptAction)
            {
                configs.ThrowIfNull("configs");
                addEntityScriptAction.ThrowIfNull("addEntityScriptAction");
                _configs = configs;
                _addEntityScriptAction = addEntityScriptAction;
            }

            List<OverriddenConfiguration> _configs;
            public List<OverriddenConfiguration> Configs
            {
                get
                {
                    return _configs;
                }
            }
            public void AddEntityScript(string entityName, string entityScript)
            {
                _addEntityScriptAction(entityName, entityScript);
            }
        }

        private class OverriddenConfigurationBehaviorGenerateDevScriptContext : IOverriddenConfigurationBehaviorGenerateDevScriptContext
        {
            Action<string, string> _addEntityScriptAction;

            public OverriddenConfigurationBehaviorGenerateDevScriptContext(List<OverriddenConfiguration> configs, Action<string, string> addEntityScriptAction)
            {
                configs.ThrowIfNull("configs");
                addEntityScriptAction.ThrowIfNull("addEntityScriptAction");
                _configs = configs;
                _addEntityScriptAction = addEntityScriptAction;
            }

            List<OverriddenConfiguration> _configs;
            public List<OverriddenConfiguration> Configs
            {
                get
                {
                    return _configs;
                }
            }
            public void AddEntityScript(string entityName, string entityScript)
            {
                _addEntityScriptAction(entityName, entityScript);
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
