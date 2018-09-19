using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Common;
using Vanrise.GenericData.Entities;
using System.Security;
using Vanrise.Security.Entities;

namespace Vanrise.GenericData.Notification
{
    public class DataRecordRuleEvaluatorDefinitionManager
    {

        #region Ctor/Properties

        Vanrise.Common.Business.VRComponentTypeManager _vrComponentTypeManager = new Common.Business.VRComponentTypeManager();
        Vanrise.Security.Business.SecurityManager s_securityManager = new Vanrise.Security.Business.SecurityManager();
        #endregion

        #region Public Methods

        public IEnumerable<DataRecordRuleEvaluatorDefinitionConfig> GetDataRecordRuleEvaluatorDefinitionConfigs()
        {
            var templateConfigManager = new ExtensionConfigurationManager();
            return templateConfigManager.GetExtensionConfigurations<DataRecordRuleEvaluatorDefinitionConfig>(DataRecordRuleEvaluatorDefinitionConfig.EXTENSION_TYPE);
        }

        public IEnumerable<DataRecordRuleEvaluatorDefinitionInfo> GetDataRecordRuleEvaluatorDefinitionsInfo(DataRecordRuleEvaluatorDefinitionInfoFilter filter)
        {
            Dictionary<Guid, DataRecordRuleEvaluatorDefinition> cachedDataRecordRuleEvaluatorDefinitions = GetCachedDataRecordRuleEvaluatorDefinitions();

            Func<DataRecordRuleEvaluatorDefinition, bool> filterExpression = (dataRecordRuleEvaluatorDefinition) =>
            {
                if (filter == null)
                    return true;
                if(filter.Filters != null)
                {
                    var context = new DataRecordRuleEvaluatorDefinitionInfoFilterContext
                    {
                        DataRecordRuleEvaluatorDefinitionId = dataRecordRuleEvaluatorDefinition.VRComponentTypeId
                    };
                    foreach(var filterObj in filter.Filters)
                    {
                        if (!filterObj.IsMatched(context))
                            return false;
                    }
                }
                return true;
            };

            return cachedDataRecordRuleEvaluatorDefinitions.MapRecords(DataRecordRuleEvaluatorDefinitionInfoMapper, filterExpression).OrderBy(x => x.Name);
        }

        public DataRecordRuleEvaluatorDefinitionSettings GetDataRecordRuleEvaluatorDefinitionSettings(Guid dataRecordRuleEvaluatorDefinitionId)
        {
            Dictionary<Guid, DataRecordRuleEvaluatorDefinition> cachedDataRecordRuleEvaluatorDefinitions = GetCachedDataRecordRuleEvaluatorDefinitions();
            DataRecordRuleEvaluatorDefinition dataRecordRuleEvaluatorDefinition = cachedDataRecordRuleEvaluatorDefinitions.GetRecord(dataRecordRuleEvaluatorDefinitionId);
            return dataRecordRuleEvaluatorDefinition.Settings;
        
        }

        private bool DoesUserHaveAccess(int userId, DataRecordRuleEvaluatorDefinitionSettings dataRecordRuleEvaluatorDefinitionSettings, Func<DataRecordRuleEvaluatorDefinitionSecurity, Vanrise.Security.Entities.RequiredPermissionSettings> getRequiredPermissionSetting)
        {
            if (dataRecordRuleEvaluatorDefinitionSettings != null && dataRecordRuleEvaluatorDefinitionSettings.Security != null)
                return s_securityManager.IsAllowed(getRequiredPermissionSetting(dataRecordRuleEvaluatorDefinitionSettings.Security), userId);
            return true;

        }

        public bool DoesUserHaveStartInstanceAccess(Guid dataRecordRuleEvaluatorDefinitionId)
        {
            int userId = Vanrise.Security.Business.SecurityContext.Current.GetLoggedInUserId();
            var beRecieveDefinition = GetDataRecordRuleEvaluatorDefinitionSettings(dataRecordRuleEvaluatorDefinitionId);
            return DoesUserHaveAccess(userId, beRecieveDefinition, (sec) => sec.StartInstancePermission);
        }
        public bool DoesUserHaveViewAccess(int userId)
        {
            var alldef = GetCachedDataRecordRuleEvaluatorDefinitions();
            foreach (var def in alldef)
            {
                if (DoesUserHaveAccess(userId, def.Value.Settings, (sec) => sec.ViewPermission))
                    return true;
            }
            return false;
        }
        public bool DoesUserHaveStartNewInstanceAccess(int userId)
        {
            var alldef = GetCachedDataRecordRuleEvaluatorDefinitions();
            foreach (var def in alldef)
            {
                if (DoesUserHaveAccess(userId, def.Value.Settings, (sec) => sec.StartInstancePermission))
                    return true;
            }
            return false;
        }

        public bool DoesUserHaveStartSpecificInstanceAccess(int userId, Guid dataRecordRuleEvaluatorDefinitionId)
        {
            var def = GetDataRecordRuleEvaluatorDefinitionSettings(dataRecordRuleEvaluatorDefinitionId);
            if (!DoesUserHaveAccess(userId, def, (sec) => sec.StartInstancePermission))
                return false;
            return true;
        }
        public RequiredPermissionSettings GetViewInstanceRequiredPermissions(Guid dataRecordRuleEvaluatorDefinitionId)
        {
            List<RequiredPermissionSettings> readInstanceRequiredPermissions = new List<RequiredPermissionSettings>();
            var def = GetDataRecordRuleEvaluatorDefinitionSettings(dataRecordRuleEvaluatorDefinitionId);
            if (def.Security != null && def.Security.ViewPermission != null)
                readInstanceRequiredPermissions.Add(def.Security.ViewPermission);
            return new Vanrise.Security.Business.SecurityManager().MergeRequiredPermissions(readInstanceRequiredPermissions);
        }

        #endregion

        #region Private Methods

        private Dictionary<Guid, DataRecordRuleEvaluatorDefinition> GetCachedDataRecordRuleEvaluatorDefinitions()
        {
            return new VRComponentTypeManager().GetCachedOrCreate("GetCachedDataRecordRuleEvaluatorDefinitions", () =>
            {
                VRComponentTypeManager vrComponentTypeManager = new Vanrise.Common.Business.VRComponentTypeManager();
                return vrComponentTypeManager.GetCachedComponentTypes<DataRecordRuleEvaluatorDefinitionSettings, DataRecordRuleEvaluatorDefinition>();
            });
        }

        private DataRecordRuleEvaluatorDefinitionInfo DataRecordRuleEvaluatorDefinitionInfoMapper(DataRecordRuleEvaluatorDefinition DataRecordRuleEvaluatorDefinition)
        {
            return new DataRecordRuleEvaluatorDefinitionInfo
            {
                DataRecordRuleEvaluatorDefinitionId = DataRecordRuleEvaluatorDefinition.VRComponentTypeId,
                Name = DataRecordRuleEvaluatorDefinition.Name,
                AreDatesHardCoded = DataRecordRuleEvaluatorDefinition.Settings.AreDatesHardCoded
            };
        }

        #endregion
    }
}
