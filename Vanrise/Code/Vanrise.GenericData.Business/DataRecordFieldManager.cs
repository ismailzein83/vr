using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Caching;
using Vanrise.GenericData.Data;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;

namespace Vanrise.GenericData.Business
{
    public class DataRecordFieldManager
    {
        public IEnumerable<DataRecordFieldInfo> GetDataRecordFieldsInfo(DataRecordFieldInfoFilter filter)
        {
            DataRecordTypeManager dataRecordTypeManager = new DataRecordTypeManager();
            Dictionary<string, DataRecordField> dataRecordFields = dataRecordTypeManager.GetDataRecordTypeFields(filter.DataRecordTypeId);
            if (dataRecordFields == null || dataRecordFields.Count == 0)
                return null;
            List<DataRecordFieldInfo> result = new List<DataRecordFieldInfo>();
            foreach (DataRecordField dataRecordField in dataRecordFields.Values)
            {
                result.Add(DataRecordFieldInfoMapper(dataRecordField));
            }
            return result;
        }

        private DataRecordFieldInfo DataRecordFieldInfoMapper(DataRecordField dataRecordField)
        {
            return new DataRecordFieldInfo()
            {
                Entity = dataRecordField,
            };
        }

        public IEnumerable<DataRecordFieldFormulaConfig> GetDataRecordFieldFormulaExtensionConfigs()
        {
            ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
            return manager.GetExtensionConfigurations<DataRecordFieldFormulaConfig>(DataRecordFieldFormulaConfig.EXTENSION_TYPE);
        }

        public IEnumerable<DataRecordFieldTypeConfig> GetDataRecordFieldTypeConfigs()
        {
            ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
            return manager.GetExtensionConfigurations<DataRecordFieldTypeConfig>(DataRecordFieldTypeConfig.EXTENSION_TYPE);
        }




    }
}