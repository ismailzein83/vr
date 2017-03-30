using System;
using System.Collections.Generic;
using Vanrise.GenericData.Entities;
using Vanrise.Common.Business;
using Vanrise.Common;

namespace Vanrise.GenericData.Business
{
    public class DataRecordFieldManager
    {
        #region Public Methods

        public IEnumerable<DataRecordFieldInfo> GetDataRecordFieldsInfo(Guid dataRecordTypeId, DataRecordFieldInfoFilter filter)
        {
            Dictionary<string, DataRecordField> dataRecordFields = new DataRecordTypeManager().GetDataRecordTypeFields(dataRecordTypeId);
            if (dataRecordFields == null || dataRecordFields.Count == 0)
                return null;

            Func<DataRecordField, bool> filterExpression = filterExpression = (dataRecordField) =>
            {
                if (filter == null)
                    return true;

                if (filter.IncludedFieldNames != null && !filter.IncludedFieldNames.Contains(dataRecordField.Name))
                    return false;

                return true;
            };

            return dataRecordFields.Values.MapRecords(DataRecordFieldInfoMapper, filterExpression);
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

        #endregion

        #region Mappers

        private DataRecordFieldInfo DataRecordFieldInfoMapper(DataRecordField dataRecordField)
        {
            return new DataRecordFieldInfo()
            {
                Entity = dataRecordField,
            };
        }

#endregion
    }
}