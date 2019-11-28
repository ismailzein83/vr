using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;

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

                if (filter.ExcludeFormula && dataRecordField.Formula != null)
                    return false;
                if (filter.Filters != null && filter.Filters.Count() > 0)
                {
                    foreach (var fieldFilter in filter.Filters)
                    {
                        if (fieldFilter.IsExcluded(new DataRecordFieldFilterContext { DataRecordField = dataRecordField }))
                            return false;
                    }
                }
                return true;
            };

            return dataRecordFields.Values.MapRecords(DataRecordFieldInfoMapper, filterExpression);
        }

        public List<GenericBEDefinitionGridColumnAttribute> GetListDataRecordTypeGridViewColumnAtts(DataRecordTypeGridViewColumnsInput input)
        {
            List<GenericBEDefinitionGridColumnAttribute> columnsAttributes = null;
            if (input != null && input.ColumnsInfo != null && input.ColumnsInfo.Count > 0)
            {
                columnsAttributes = new List<GenericBEDefinitionGridColumnAttribute>();

                foreach (var columnInfo in input.ColumnsInfo)
                {
                    var columnAttribute = new GenericBEDefinitionGridColumnAttribute
                    {
                        Name = columnInfo.Name,
                        Attribute = new GridColumnAttribute
                        {
                            HeaderText = columnInfo.Title
                        }
                    };
                    int? widthFactor = null;
                    int? fixedWidth = null;

                    if (columnInfo.GridColumnSettings != null)
                    {
                        widthFactor = GridColumnWidthFactorConstants.GetColumnWidthFactor(columnInfo.GridColumnSettings);
                        if (!widthFactor.HasValue)
                            fixedWidth = columnInfo.GridColumnSettings.FixedWidth;
                    }

                    columnAttribute.Attribute.FixedWidth = fixedWidth;
                    columnAttribute.Attribute.WidthFactor = widthFactor;

                    columnsAttributes.Add(columnAttribute);
                }
            }
            return columnsAttributes;
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

        public IEnumerable<ListRecordRuntimeViewTypeConfig> GetListRecordRuntimeViewTypeConfigs()
        {
            ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
            return manager.GetExtensionConfigurations<ListRecordRuntimeViewTypeConfig>(ListRecordRuntimeViewTypeConfig.EXTENSION_TYPE);
        }

        public DataRecordFieldTypeInfo GetFieldTypeDescription(FieldTypeDescriptionInput input)
        {
            if (input.FieldType == null)
                return null;

            var dataRecordFieldStyleDefinitionContext = new DataRecordFieldStyleDefinitionContext { FieldValue = input.FieldValue };
            input.FieldType.TryGetStyleDefinitionId(dataRecordFieldStyleDefinitionContext);

            var dataRecordFieldTypeParseValueToFieldTypeContext = new DataRecordFieldTypeParseValueToFieldTypeContext(input.FieldValue);
            dynamic parsedFieldValue = input.FieldType.ParseValueToFieldType(dataRecordFieldTypeParseValueToFieldTypeContext);

            var fieldTypeInfo = new DataRecordFieldTypeInfo()
            {
                FieldDescription = input.FieldType.GetDescription(parsedFieldValue),
                StyleDefinitionId = dataRecordFieldStyleDefinitionContext.StyleDefinitionId
            };

            return fieldTypeInfo;
        }

        public List<Dictionary<string, string>> GetFieldTypeListDescription(ListFieldTypeDescriptionInput input)
        {
            List<Dictionary<string, string>> fieldsDescription = null;

            if (input.FieldTypes != null && input.FieldsValues != null && input.FieldsValues.Count > 0)
            {
                fieldsDescription = new List<Dictionary<string, string>>();

                foreach (var rowFieldsValues in input.FieldsValues)
                {
                    if (rowFieldsValues != null && rowFieldsValues.Count > 0)
                    {
                        var rowFieldsDescription = new Dictionary<string, string>();

                        foreach (var fieldValue in rowFieldsValues)
                        {
                            if (fieldValue.Key != null)
                            {
                                var fieldType = input.FieldTypes.GetRecord(fieldValue.Key);

                                if (fieldType != null)
                                    rowFieldsDescription.Add(fieldValue.Key, fieldType.GetDescription(fieldValue.Value));
                            }
                        }
                        fieldsDescription.Add(rowFieldsDescription);
                    }
                }
            }
            return fieldsDescription;
        }

        public List<Dictionary<string, string>> GetFieldsDescription(FieldsDescriptionInput input)
        {
            DataRecordTypeManager dataRecordTypeManager = new DataRecordTypeManager();
            var fields = dataRecordTypeManager.GetDataRecordTypeFields(input.DataRecordTypeId);
            List<Dictionary<string, string>> fieldsDescription = null;

            if (fields != null && fields.Count > 0 && input.FieldsValues != null && input.FieldsValues.Count > 0)
            {
                fieldsDescription = new List<Dictionary<string, string>>();
                foreach (var rowValues in input.FieldsValues)
                {
                    Dictionary<string, string> fieldsDescriptionRow;
                    if (rowValues != null && rowValues.Count > 0)
                    {
                        fieldsDescriptionRow = new Dictionary<string, string>();
                        foreach (var fieldValue in rowValues)
                        {
                            DataRecordField field;
                            if (fields.TryGetValue(fieldValue.Key, out field))
                            {
                                fieldsDescriptionRow.Add(fieldValue.Key, field.Type.GetDescription(fieldValue.Value));
                            }
                        }
                        fieldsDescription.Add(fieldsDescriptionRow);
                    }
                }
            }
            return fieldsDescription;
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

    public class FieldTypeDescriptionInput
    {
        public DataRecordFieldType FieldType { get; set; }
        public object FieldValue { get; set; }
    }

    public class ListFieldTypeDescriptionInput
    {
        public Dictionary<string, DataRecordFieldType> FieldTypes { get; set; }
        public List<Dictionary<string, object>> FieldsValues { get; set; }
    }

    public class FieldsDescriptionInput
    {
        public Guid DataRecordTypeId { get; set; }
        public List<Dictionary<string, object>> FieldsValues { get; set; }
    }
}