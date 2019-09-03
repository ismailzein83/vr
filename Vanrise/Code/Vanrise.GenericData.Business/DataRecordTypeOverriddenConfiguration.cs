using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
using Vanrise.GenericData.Data;

namespace Vanrise.GenericData.Business
{
    public class DataRecordTypeOverriddenConfiguration : OverriddenConfigurationExtendedSettings
    {
        public override Guid ConfigId => new Guid("2DB9DE98-6BE2-4F5C-A4F9-470794E94740");
        public Guid DataRecordTypeId { get; set; }
        public string OverriddenName { get; set; }
        public List<DataRecordField> OverriddenFields { get; set; }
        public DataRecordTypeSettings OverriddenSettings { get; set; }
        public DataRecordTypeExtraField OverriddenExtraFieldsEvaluator { get; set; }

        public override Type GetBehaviorType(IOverriddenConfigurationGetBehaviorContext context)
        {
            return typeof(DataRecordTypeOverriddenConfigurationBehavior);
        }

        #region Private Methods

        private class DataRecordTypeOverriddenConfigurationBehavior : OverriddenConfigurationBehavior
        {
            public override void GenerateScript(IOverriddenConfigurationBehaviorGenerateScriptContext context)
            {
                DataRecordTypeManager dataRecordTypeManager = new DataRecordTypeManager();
                List<DataRecordType> dataRecordTypes = new List<DataRecordType>();
                foreach (var config in context.Configs)
                {
                    DataRecordTypeOverriddenConfiguration dataRecordTypeConfig = config.Settings.ExtendedSettings.CastWithValidate<DataRecordTypeOverriddenConfiguration>("dataRecordTypeConfig", config.OverriddenConfigurationId);

                    var dataRecordType = dataRecordTypeManager.GetDataRecordType(dataRecordTypeConfig.DataRecordTypeId);
                    dataRecordType.ThrowIfNull("dataRecordType", dataRecordTypeConfig.DataRecordTypeId);
                    dataRecordType = dataRecordType.VRDeepCopy();
                    if (!String.IsNullOrEmpty(dataRecordTypeConfig.OverriddenName))
                        dataRecordType.Name = dataRecordTypeConfig.OverriddenName;

                    if (dataRecordTypeConfig.OverriddenSettings != null)
                        dataRecordType.Settings = dataRecordTypeConfig.OverriddenSettings;

                    if (dataRecordTypeConfig.OverriddenFields != null)
                        dataRecordType.Fields = dataRecordTypeConfig.OverriddenFields;

                    if (dataRecordTypeConfig.OverriddenExtraFieldsEvaluator != null)
                        dataRecordType.ExtraFieldsEvaluator = dataRecordTypeConfig.OverriddenExtraFieldsEvaluator;

                    dataRecordTypes.Add(dataRecordType);
                }
                GenerateScript(dataRecordTypes, context.AddEntityScript);
            }

            public override void GenerateDevScript(IOverriddenConfigurationBehaviorGenerateDevScriptContext context)
            {
                IEnumerable<Guid> ids = context.Configs.Select(config => config.Settings.ExtendedSettings.CastWithValidate<DataRecordTypeOverriddenConfiguration>("config.Settings.ExtendedSettings", config.OverriddenConfigurationId).DataRecordTypeId).Distinct();
                DataRecordTypeManager dataRecordTypeManager = new DataRecordTypeManager();
                List<DataRecordType> dataRecordTypes = new List<DataRecordType>();
                foreach (var id in ids)
                {
                    var dataRecordType = dataRecordTypeManager.GetDataRecordType(id);
                    dataRecordType.ThrowIfNull("dataRecordType", id);
                    dataRecordTypes.Add(dataRecordType);
                }
                GenerateScript(dataRecordTypes, context.AddEntityScript);
            }

            private void GenerateScript(List<DataRecordType> dataRecordTypes, Action<string, string> addEntityScript)
            {
                IDataRecordTypeDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IDataRecordTypeDataManager>();
                dataManager.GenerateScript(dataRecordTypes, addEntityScript);
            }
        }

        #endregion
    }



}
