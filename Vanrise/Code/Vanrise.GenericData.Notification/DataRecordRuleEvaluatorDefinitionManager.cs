using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Common;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Notification
{
    public class DataRecordRuleEvaluatorDefinitionManager
    {

        #region Ctor/Properties

        Vanrise.Common.Business.VRComponentTypeManager _vrComponentTypeManager = new Common.Business.VRComponentTypeManager();

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

            Func<DataRecordRuleEvaluatorDefinition, bool> filterExpression = (DataRecordRuleEvaluatorDefinition) =>
            {
                if (filter == null)
                    return true;

                return true;
            };

            return cachedDataRecordRuleEvaluatorDefinitions.MapRecords(DataRecordRuleEvaluatorDefinitionInfoMapper, filterExpression).OrderBy(x => x.Name);
        }

        public DataRecordRuleEvaluatorDefinitionSettings GetDataRecordRuleEvaluatorDefinitionSettings(Guid dataRecordRuleEvaluatorDefinitionId)
        {
            Dictionary<Guid, DataRecordRuleEvaluatorDefinition> cachedDataRecordRuleEvaluatorDefinitions = GetCachedDataRecordRuleEvaluatorDefinitions();
            DataRecordRuleEvaluatorDefinition dataRecordRuleEvaluatorDefinition = cachedDataRecordRuleEvaluatorDefinitions.GetRecord<Guid, DataRecordRuleEvaluatorDefinition>(dataRecordRuleEvaluatorDefinitionId);
            return dataRecordRuleEvaluatorDefinition.Settings;
        
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
                Name = DataRecordRuleEvaluatorDefinition.Name
            };
        }

        #endregion
    }
}
