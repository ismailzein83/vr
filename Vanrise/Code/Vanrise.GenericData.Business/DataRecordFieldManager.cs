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
        public GenericFieldDifferencesResolver TryResolveDifferences(string loggableEntityUniqueName, List<GenericFieldChangeInfo> fieldValues)
        {
            GenericFieldDifferencesResolver genericFieldDifferencesResolver = null;
            if (fieldValues != null)
            {
                VRObjectTrackingManager vrObjectTrackingManager = new VRObjectTrackingManager();
                var genericFieldsActionAuditChangeInfoDefinition = vrObjectTrackingManager.GetVRLoggableEntityChangeInfoDefinition<GenericFieldsActionAuditChangeInfoDefinition>(loggableEntityUniqueName);

                if (genericFieldsActionAuditChangeInfoDefinition != null && genericFieldsActionAuditChangeInfoDefinition.FieldTypes != null)
                {
                    if (fieldValues != null && genericFieldsActionAuditChangeInfoDefinition.FieldTypes != null)
                    {
                        genericFieldDifferencesResolver = new GenericFieldDifferencesResolver();
                        foreach (var fieldValue in fieldValues)
                        {
                            var recordField = genericFieldsActionAuditChangeInfoDefinition.FieldTypes.GetRecord(fieldValue.FieldName);
                            if (recordField != null)
                            {
                                var dataRecordFieldTypeTryResolveDifferencesContext = new DataRecordFieldTypeTryResolveDifferencesContext
                                {
                                    NewValue = fieldValue.NewValue,
                                    OldValue = fieldValue.OldValue
                                };
                                if (recordField.Type.TryResolveDifferences(dataRecordFieldTypeTryResolveDifferencesContext))
                                {
                                    if (genericFieldDifferencesResolver.Differences == null)
                                        genericFieldDifferencesResolver.Differences = new List<GenericFieldChangeDifferences>();
                                    genericFieldDifferencesResolver.Differences.Add(new GenericFieldChangeDifferences
                                    {
                                        Changes = dataRecordFieldTypeTryResolveDifferencesContext.Changes,
                                        FieldName = fieldValue.FieldName
                                    });
                                }
                                else
                                {
                                    if (genericFieldDifferencesResolver.SimpleChanges == null)
                                        genericFieldDifferencesResolver.SimpleChanges = new List<GenericFieldChangeSimpleChange>();

                                    genericFieldDifferencesResolver.SimpleChanges.Add(new GenericFieldChangeSimpleChange
                                    {
                                        NewValue = fieldValue.NewValue,
                                        FieldName = fieldValue.FieldName,
                                        OldValue = fieldValue.OldValue,
                                        NewValueDescription = recordField.Type.GetDescription(fieldValue.NewValue),
                                        OldValueDescription = recordField.Type.GetDescription(fieldValue.OldValue),
                                    });
                                }
                            }
                        }
                    }
                }
            }
            return genericFieldDifferencesResolver;
        }
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

        #region Config
        public IEnumerable<FieldCustomObjectTypeSettingsConfig> GetFieldCustomObjectTypeSettingsConfig()
        {
            var extensionConfiguration = new ExtensionConfigurationManager();
            return extensionConfiguration.GetExtensionConfigurations<FieldCustomObjectTypeSettingsConfig>(FieldCustomObjectTypeSettingsConfig.EXTENSION_TYPE);
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

    public class GenericFieldChangeDifferences
    {
        public string FieldName { get; set; }
        public Object Changes { get; set; }
    }
    public class GenericFieldChangeSimpleChange
    {
        public string FieldName { get; set; }
        public Object OldValue { get; set; }
        public string OldValueDescription { get; set; }
        public Object NewValue { get; set; }
        public string NewValueDescription { get; set; }


    }
    public class GenericFieldDifferencesResolver
    {
        public List<GenericFieldChangeDifferences> Differences { get; set; }
        public List<GenericFieldChangeSimpleChange> SimpleChanges { get; set; }
    }
}
