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
    public class DataRecordStorageOverriddenConfiguration : OverriddenConfigurationExtendedSettings
    {
        public override Guid ConfigId => new Guid("13193178-F531-4B94-8E1E-F02AA88ED7DB");

        public Guid DataRecordStorageId { get; set; }
        public string OverriddenName { get; set; }
        public DataRecordStorageOverriddenSettings Settings { get; set; }
        public override Type GetBehaviorType(IOverriddenConfigurationGetBehaviorContext context)
        {
            return typeof(DataRecordStorageOverriddenConfigurationBehavior);
        }

        public class DataRecordStorageOverriddenSettings
        {
            public Guid OverriddenDataRecordTypeId { get; set; }
            public Guid OverriddenDataStoreId { get; set; }
            public DataRecordStorageSettings OverriddenSettings { get; set; }
        }

        #region Private Methods

        private class DataRecordStorageOverriddenConfigurationBehavior : OverriddenConfigurationBehavior
        {
            public override void GenerateScript(IOverriddenConfigurationBehaviorGenerateScriptContext context)
            {
                DataRecordStorageManager dataRecordStorageManager = new DataRecordStorageManager();
                List<DataRecordStorage> dataRecordStorages = new List<DataRecordStorage>();
                foreach (var config in context.Configs)
                {
                    DataRecordStorageOverriddenConfiguration dataRecordStorageConfig = config.Settings.ExtendedSettings.CastWithValidate<DataRecordStorageOverriddenConfiguration>("dataRecordStorageConfig", config.OverriddenConfigurationId);

                    var dataRecordStorage = dataRecordStorageManager.GetDataRecordStorage(dataRecordStorageConfig.DataRecordStorageId);
                    dataRecordStorage.ThrowIfNull("dataRecordStorage", dataRecordStorageConfig.DataRecordStorageId);
                    dataRecordStorage = dataRecordStorage.VRDeepCopy();
                    if (!String.IsNullOrEmpty(dataRecordStorageConfig.OverriddenName))
                        dataRecordStorage.Name = dataRecordStorageConfig.OverriddenName;

                    if (dataRecordStorageConfig.Settings != null)
                    {
                        dataRecordStorage.DataRecordTypeId = dataRecordStorageConfig.Settings.OverriddenDataRecordTypeId;
                        dataRecordStorage.DataStoreId = dataRecordStorageConfig.Settings.OverriddenDataStoreId;
                        if (dataRecordStorageConfig.Settings.OverriddenSettings != null)
                            dataRecordStorage.Settings = dataRecordStorageConfig.Settings.OverriddenSettings;
                    }

                    dataRecordStorages.Add(dataRecordStorage);
                }
                GenerateScript(dataRecordStorages, context.AddEntityScript);
            }

            public override void GenerateDevScript(IOverriddenConfigurationBehaviorGenerateDevScriptContext context)
            {
                IEnumerable<Guid> ids = context.Configs.Select(config => config.Settings.ExtendedSettings.CastWithValidate<DataRecordStorageOverriddenConfiguration>("config.Settings.ExtendedSettings", config.OverriddenConfigurationId).DataRecordStorageId).Distinct();
                DataRecordStorageManager dataRecordStorageManager = new DataRecordStorageManager();
                List<DataRecordStorage> dataRecordStorages = new List<DataRecordStorage>();
                foreach (var id in ids)
                {
                    var dataRecordStorage = dataRecordStorageManager.GetDataRecordStorage(id);
                    dataRecordStorage.ThrowIfNull("dataRecordStorage", id);
                    dataRecordStorages.Add(dataRecordStorage);
                }
                GenerateScript(dataRecordStorages, context.AddEntityScript);
            }

            private void GenerateScript(List<DataRecordStorage> dataRecordStorages, Action<string, string> addEntityScript)
            {
                IDataRecordStorageDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IDataRecordStorageDataManager>();
                dataManager.GenerateScript(dataRecordStorages, addEntityScript);
            }
        }

        #endregion
    }
}
