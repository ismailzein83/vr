using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace Vanrise.GenericData.Business
{
    public class GenericBusinessEntityDefinitionManager
    {
        static VRLocalizationManager vrLocalizationManager = new VRLocalizationManager();

        DataRecordTypeManager _dataRecordTypeManager;

        #region Public Methods 
        public GenericBusinessEntityDefinitionManager()
        {
            _dataRecordTypeManager = new DataRecordTypeManager();
        }
        public BusinessEntityDefinition GetGenericBEDefinition(Guid businessEntityDefinitionId)
        {
            BusinessEntityDefinitionManager manager = new BusinessEntityDefinitionManager();
            var genericBEDefinition = manager.GetBusinessEntityDefinition(businessEntityDefinitionId);
            genericBEDefinition.ThrowIfNull("genericBEDefinition", businessEntityDefinitionId);
            return genericBEDefinition;
        }
        public string GetGenericBEDefinitionName(Guid businessEntityDefinitionId)
        {
            return GetGenericBEDefinition(businessEntityDefinitionId).Name;
        }
        public string GetGenericBEDefinitionTitleFieldName(Guid businessEntityDefinitionId)
        {
            return GetGenericBEDefinitionSettings(businessEntityDefinitionId, false).TitleFieldName;
        }
        public string GetGenericBEDefinitionTitle(Guid businessEntityDefinitionId)
        {
            return GetGenericBEDefinition(businessEntityDefinitionId).Title;
        }

        public GenericBESelectorRuntimeInfo GetGenericBESelectorRuntimeInfo(Guid businessEntityDefinitionId)
        {
            var genericBEDefinitionSettings = GetGenericBEDefinitionSettings(businessEntityDefinitionId, true);
            var idFieldType = GetIdFieldTypeForGenericBE(businessEntityDefinitionId);

            return new GenericBESelectorRuntimeInfo()
            {
                SelectorSingularTitle = genericBEDefinitionSettings.SelectorSingularTitle,
                SelectorPluralTitle = genericBEDefinitionSettings.SelectorPluralTitle,
                IdFieldName = idFieldType != null ? idFieldType.Name : null,
                TitleFieldName = GetGenericBEDefinitionTitleFieldName(businessEntityDefinitionId),
                IsRemote = genericBEDefinitionSettings.IsRemoteSelector
            };

        }

        public GenericBEDefinitionSettings GetGenericBEDefinitionSettings(Guid businessEntityDefinitionId, bool getTranslated = false)
        {
            var genericBEDefinition = GetGenericBEDefinition(businessEntityDefinitionId);
            genericBEDefinition.Settings.ThrowIfNull("genericBEDefinition.Settings");
            var genericBESettings = genericBEDefinition.Settings.CastWithValidate<GenericBEDefinitionSettings>("genericBEDefinition.Settings");
            if (getTranslated)
                return TranslateGenericBEDefinitionSettings(genericBESettings);
            return genericBESettings;
        }
        public Guid GetGenericBEDataRecordStorageId(Guid businessEntityDefinitionId)
        {
            var genericBEDefinitionSettings = GetGenericBEDefinitionSettings(businessEntityDefinitionId, false);
            if (genericBEDefinitionSettings.DataRecordStorageId == null)
            {
                throw new NullReferenceException("genericBEDefinitionSettings.DataRecordStorageId");
            }
            return genericBEDefinitionSettings.DataRecordStorageId.Value;

        }
        public DataRecordType GetGenericBEDataRecordType(Guid businessEntityDefinitionId)
        {
            var dataRecordTypeId = GetGenericBEDataRecordTypeId(businessEntityDefinitionId);
            var dataRecordType = _dataRecordTypeManager.GetDataRecordType(dataRecordTypeId);
            dataRecordType.ThrowIfNull("dataRecordType", dataRecordTypeId);
            return dataRecordType;
        }
        public Guid GetGenericBEDataRecordTypeId(Guid businessEntityDefinitionId)
        {
            var genericBEDefinitionSettings = GetGenericBEDefinitionSettings(businessEntityDefinitionId, false);
            return genericBEDefinitionSettings.DataRecordTypeId;
        }
        public GenericBEGridDefinition GetGenericBEDefinitionGridDefinition(Guid businessEntityDefinitionId)
        {
            VRLocalizationManager vrLocalizationManager = new VRLocalizationManager();
            return GetGenericBEDefinitionSettings(businessEntityDefinitionId, true).GridDefinition;
        }
        public List<GenericBEDefinitionGridColumnAttribute> GetGenericBEDefinitionGridColumnAttributes(Guid businessEntityDefinitionId)
        {
            List<GenericBEDefinitionGridColumnAttribute> gridColumns = new List<GenericBEDefinitionGridColumnAttribute>();
            var genericBEDefinitionSettings = GetGenericBEDefinitionSettings(businessEntityDefinitionId, true);
            genericBEDefinitionSettings.GridDefinition.ThrowIfNull("genericBEDefinitionSettings.GridDefinition");
            genericBEDefinitionSettings.GridDefinition.ColumnDefinitions.ThrowIfNull("genericBEDefinitionSettings.ColumnDefinitions");


            DataRecordTypeManager dataRecordTypeManager = new DataRecordTypeManager();
            var dataRecordTypeFields = dataRecordTypeManager.GetDataRecordTypeFields(genericBEDefinitionSettings.DataRecordTypeId);
            dataRecordTypeFields.ThrowIfNull("dataRecordTypeFields");

            foreach (var vrCaseGridColumn in genericBEDefinitionSettings.GridDefinition.ColumnDefinitions)
            {

                DataRecordField dataRecordTypeField = dataRecordTypeFields.FindRecord(x => x.Name == vrCaseGridColumn.FieldName);
                if (dataRecordTypeField == null)
                    continue;
                FieldTypeGetGridColumnAttributeContext context = new FieldTypeGetGridColumnAttributeContext();
                context.ValueFieldPath = "FieldValues." + vrCaseGridColumn.FieldName + ".Value";
                context.DescriptionFieldPath = "FieldValues." + vrCaseGridColumn.FieldName + ".Description";

                var gridColumnsAttribute = dataRecordTypeField.Type.GetGridColumnAttribute(context);
                if (gridColumnsAttribute != null)
                {
                    int? widthFactor = null;
                    int? fixedWidth = null;
                    if (vrCaseGridColumn.GridColumnSettings != null)
                    {
                        widthFactor = GridColumnWidthFactorConstants.GetColumnWidthFactor(vrCaseGridColumn.GridColumnSettings);
                        if (!widthFactor.HasValue)
                            fixedWidth = vrCaseGridColumn.GridColumnSettings.FixedWidth;
                    }
                    gridColumnsAttribute.FixedWidth = fixedWidth;
                    gridColumnsAttribute.WidthFactor = widthFactor;
                }
                var vrCaseGridColumnAttribute = new GenericBEDefinitionGridColumnAttribute
                {
                    Attribute = gridColumnsAttribute,
                    Name = vrCaseGridColumn.FieldName,
                };
                vrCaseGridColumnAttribute.Attribute.HeaderText = vrCaseGridColumn.FieldTitle;
                gridColumns.Add(vrCaseGridColumnAttribute);
            }
            return gridColumns;
        }
        public IEnumerable<BusinessEntityDefinitionInfo> GetRemoteGenericBEDefinitionInfo(RemoteGenericBEDefinitionInfoInput input)
        {
            VRConnectionManager connectionManager = new VRConnectionManager();
            var vrConnection = connectionManager.GetVRConnection<VRInterAppRestConnection>(input.VRConnectionId);
            VRInterAppRestConnection connectionSettings = vrConnection.Settings as VRInterAppRestConnection;
            return connectionSettings.Get<IEnumerable<BusinessEntityDefinitionInfo>>(string.Format("/api/VR_GenericData/BusinessEntityDefinition/GetBusinessEntityDefinitionsInfo?filter={0}", input.Filter));
        }
        public DataRecordField GetIdFieldTypeForGenericBE(Guid businessEntityDefinitionId)
        {
            var dataRecordType = GetGenericBEDataRecordType(businessEntityDefinitionId);
            var dataRecordTypeFields = GetDataRecordTypeFields(dataRecordType.DataRecordTypeId);
            dataRecordType.Settings.IdField.ThrowIfNull("dataRecordType.Settings.IdField");
            var idDataRecordField = dataRecordTypeFields.FindRecord(x => x.Name == dataRecordType.Settings.IdField);
            idDataRecordField.ThrowIfNull("idDataRecordField");
            return idDataRecordField;
        }
        public Dictionary<string, DataRecordField> GetDataRecordTypeFieldsByBEDefinitionId(Guid businessEntityDefinitionId)
        {
            var genericBEDefinitionSettings = GetGenericBEDefinitionSettings(businessEntityDefinitionId, false);
            return GetDataRecordTypeFields(genericBEDefinitionSettings.DataRecordTypeId);
        }
        public DataRecordField GetDataRecordTypeFieldByBEDefinitionId(Guid businessEntityDefinitionId, string fieldName)
        {
            var dataRecordTypeFields = GetDataRecordTypeFieldsByBEDefinitionId(businessEntityDefinitionId);
            dataRecordTypeFields.ThrowIfNull("dataRecordTypeFields");
            return dataRecordTypeFields.GetRecord(fieldName);
        }
        public Dictionary<string, DataRecordField> GetDataRecordTypeFields(Guid dataRecordTypeId)
        {
            var dataRecordTypeFields = _dataRecordTypeManager.GetDataRecordTypeFields(dataRecordTypeId);
            dataRecordTypeFields.ThrowIfNull("dataRecordTypeFields", dataRecordTypeId);
            return dataRecordTypeFields;
        }


        public IEnumerable<UpdateGenericBEFieldRuntime> GetUpdateBulkActionGenericBEFieldsRuntime(GetUpdateBulkActionsRuntimeInput input)
        {
            List<UpdateGenericBEFieldRuntime> updateBulkActionRuntime = null;
            if (input.Fields != null && input.Fields.Count > 0)
            {
                var dataRecordTypeFields = _dataRecordTypeManager.GetDataRecordTypeFields(input.DataRecordTypeId);
                dataRecordTypeFields.ThrowIfNull("dataRecordTypeFields", input.DataRecordTypeId);
                DataRecordFieldManager dataRecordFieldManager = new DataRecordFieldManager();
                IEnumerable<DataRecordFieldTypeConfig> dataRecordTypeConfigs = dataRecordFieldManager.GetDataRecordFieldTypeConfigs();
                updateBulkActionRuntime = new List<UpdateGenericBEFieldRuntime>();
                foreach (var field in input.Fields)
                {
                    DataRecordFieldTypeConfig dataRecordTypeConfig = new DataRecordFieldTypeConfig();
                    dataRecordTypeConfig = dataRecordTypeConfigs.FindRecord(x => x.ExtensionConfigurationId == dataRecordTypeFields.GetRecord(field.FieldName).Type.ConfigId);
                    dataRecordTypeConfig.ThrowIfNull("dataRecordTypeConfig");
                    var updateGenericBEFieldRuntime = new UpdateGenericBEFieldRuntime
                    {
                        Name = field.FieldName,
                        RuntimeEditor = dataRecordTypeConfig.RuntimeEditor,
                        IsRequired = field.IsRequired,
                        DefaultValue = field.DefaultValue,
                        FieldState = field.FieldState
                    };
                    var recordTypeFields = dataRecordTypeFields.GetRecord(field.FieldName);
                    recordTypeFields.ThrowIfNull("dataRecordTypeFields of field name {0}", field.FieldName);
                    updateGenericBEFieldRuntime.Title = recordTypeFields.Title;
                    updateGenericBEFieldRuntime.Type = recordTypeFields.Type;

                    updateBulkActionRuntime.Add(updateGenericBEFieldRuntime);
                }
            }
            return updateBulkActionRuntime;
        }

        public Dictionary<Guid, GenericBEAction> GetCachedGenericBEActionsByActionId(Guid businessEntityDefinitionId)
        {
            return new BusinessEntityDefinitionManager().GetCachedOrCreate(string.Format("GenericBusinessEntityDefinitionManager_GetGenericBEActionsById_{0}", businessEntityDefinitionId), () =>
            {
                var businessEntityDefinitionSettings = GetGenericBEDefinitionSettings(businessEntityDefinitionId, false);
                businessEntityDefinitionSettings.ThrowIfNull("Business Entity Definition Settings ", businessEntityDefinitionId);
                businessEntityDefinitionSettings.GenericBEActions.ThrowIfNull("Generic BE Actions", businessEntityDefinitionId);
                return businessEntityDefinitionSettings.GenericBEActions.ToDictionary(itm => itm.GenericBEActionId, itm => itm);
            });
        }

        public Object GetExtendedSettingsInfoByType(GenericBEDefinitionSettings definitionSettings, string infoType, GenericBusinessEntity genericBusinessEntity = null)
        {
            definitionSettings.ThrowIfNull("context.DefinitionSettings");
            definitionSettings.ExtendedSettings.ThrowIfNull("context.DefinitionSettings.ExtendedSettings");
            return definitionSettings.ExtendedSettings.GetInfoByType(new GenericBEExtendedSettingsContext
            {
                GenericBusinessEntity = genericBusinessEntity,
                InfoType = infoType,
                DefinitionSettings = definitionSettings
            });
        }

        public List<string> GetStatusFieldNames(Guid businessEntityDefinitionId)
        {
            return new BusinessEntityDefinitionManager().GetCachedOrCreate(string.Format("GenericBusinessEntityDefinitionManager_GetStatusFieldNames_{0}", businessEntityDefinitionId), () =>
            {
                List<string> statusFieldNames = null;
                var dataRecordFields = GetDataRecordTypeFieldsByBEDefinitionId(businessEntityDefinitionId);
                var businessEntityDefinitionManager = new BusinessEntityDefinitionManager();
                if (dataRecordFields != null)
                {
                    foreach (var dataRecordField in dataRecordFields)
                    {
                        var fieldBusinessEntityType = dataRecordField.Value.Type as IFieldBusinessEntityType;
                        if (fieldBusinessEntityType != null)
                        {
                            var businessEntityDefinition = businessEntityDefinitionManager.GetBusinessEntityDefinition(fieldBusinessEntityType.BusinessEntityDefinitionId);
                            businessEntityDefinition.ThrowIfNull("businessEntityDefinition", fieldBusinessEntityType.BusinessEntityDefinitionId);
                            if (businessEntityDefinition.Settings is StatusDefinitionBESettings)
                            {
                                if (statusFieldNames == null)
                                    statusFieldNames = new List<string>();
                                statusFieldNames.Add(dataRecordField.Key);
                            }
                        }
                    }
                }
                return statusFieldNames;
            });

        }
        #endregion

        #region Config region

        public IEnumerable<GenericBEViewDefinitionSettingsConfig> GetGenericBEViewDefinitionSettingsConfigs()
        {
            var extensionConfiguration = new ExtensionConfigurationManager();
            return extensionConfiguration.GetExtensionConfigurations<GenericBEViewDefinitionSettingsConfig>(GenericBEViewDefinitionSettingsConfig.EXTENSION_TYPE);
        }

        public IEnumerable<GenericBEEditorDefinitionSettingsConfig> GetGenericBEEditorDefinitionSettingsConfigs(ContainerType containerType)
        {
            var extensionConfiguration = new ExtensionConfigurationManager();
            var genericBEEditorDefinitionSettingsConfigs = extensionConfiguration.GetExtensionConfigurations<GenericBEEditorDefinitionSettingsConfig>(GenericBEEditorDefinitionSettingsConfig.EXTENSION_TYPE);

            if (genericBEEditorDefinitionSettingsConfigs == null)
                return null;

            return genericBEEditorDefinitionSettingsConfigs.Where(item => item.ValidContainers == null || item.ValidContainers.Contains(containerType));
        }

        public IEnumerable<GenericBEFilterDefinitionSettingsConfig> GetGenericBEFilterDefinitionSettingsConfigs()
        {
            var extensionConfiguration = new ExtensionConfigurationManager();
            return extensionConfiguration.GetExtensionConfigurations<GenericBEFilterDefinitionSettingsConfig>(GenericBEFilterDefinitionSettingsConfig.EXTENSION_TYPE);
        }

        public IEnumerable<GenericBEActionDefinitionSettingsConfig> GetGenericBEActionDefinitionSettingsConfigs()
        {
            var extensionConfiguration = new ExtensionConfigurationManager();
            return extensionConfiguration.GetExtensionConfigurations<GenericBEActionDefinitionSettingsConfig>(GenericBEActionDefinitionSettingsConfig.EXTENSION_TYPE);
        }
        public IEnumerable<GenericBEExtendedSettingsConfig> GetGenericBEExtendedSettingsConfigs()
        {
            var extensionConfiguration = new ExtensionConfigurationManager();
            return extensionConfiguration.GetExtensionConfigurations<GenericBEExtendedSettingsConfig>(GenericBEExtendedSettingsConfig.EXTENSION_TYPE);
        }
        public IEnumerable<GenericBEActionFilterConditionConfig> GetGenericBEActionFilterConditionConfigs()
        {
            var extensionConfiguration = new ExtensionConfigurationManager();
            return extensionConfiguration.GetExtensionConfigurations<GenericBEActionFilterConditionConfig>(GenericBEActionFilterConditionConfig.EXTENSION_TYPE);
        }
        public IEnumerable<GenericBEOnAfterSaveHandlerSettingsConfig> GetGenericBEOnAfterSaveHandlerSettingsConfigs()
        {
            var extensionConfiguration = new ExtensionConfigurationManager();
            return extensionConfiguration.GetExtensionConfigurations<GenericBEOnAfterSaveHandlerSettingsConfig>(GenericBEOnAfterSaveHandlerSettingsConfig.EXTENSION_TYPE);
        }

        public IEnumerable<GenericBEOnBeforeInsertHandlerSettingsConfig> GetGenericBEOnBeforeInsertHandlerSettingsConfigs()
        {
            var extensionConfiguration = new ExtensionConfigurationManager();
            return extensionConfiguration.GetExtensionConfigurations<GenericBEOnBeforeInsertHandlerSettingsConfig>(GenericBEOnBeforeInsertHandlerSettingsConfig.EXTENSION_TYPE);
        }
        public IEnumerable<GenericBEOnBeforeGetFilteredHandlerSettingsConfig> GetGenericBEOnBeforeGetFilteredHandlerSettingsConfigs()
        {
            var extensionConfiguration = new ExtensionConfigurationManager();
            return extensionConfiguration.GetExtensionConfigurations<GenericBEOnBeforeGetFilteredHandlerSettingsConfig>(GenericBEOnBeforeGetFilteredHandlerSettingsConfig.EXTENSION_TYPE);
        }

        public IEnumerable<GenericBESaveConditionSettingsConfig> GetGenericBESaveConditionSettingsConfigs()
        {
            var extensionConfiguration = new ExtensionConfigurationManager();
            return extensionConfiguration.GetExtensionConfigurations<GenericBESaveConditionSettingsConfig>(GenericBESaveConditionSettingsConfig.EXTENSION_TYPE);
        }
        public IEnumerable<GenericBEConditionSettingsConfig> GetGenericBEConditionSettingsConfigs()
        {
            var extensionConfiguration = new ExtensionConfigurationManager();
            return extensionConfiguration.GetExtensionConfigurations<GenericBEConditionSettingsConfig>(GenericBEConditionSettingsConfig.EXTENSION_TYPE);
        }

        public IEnumerable<GenericBESerialNumberPartSettingsConfig> GetGenericBESerialNumberPartSettingsConfigs()
        {
            var extensionConfiguration = new ExtensionConfigurationManager();
            return extensionConfiguration.GetExtensionConfigurations<GenericBESerialNumberPartSettingsConfig>(GenericBESerialNumberPartSettingsConfig.EXTENSION_TYPE);
        }

        public IEnumerable<GenericBEBulkActionSettingsConfig> GetGenericBEBulkActionSettingsConfigs()
        {
            var extensionConfigurationManager = new ExtensionConfigurationManager();
            return extensionConfigurationManager.GetExtensionConfigurations<GenericBEBulkActionSettingsConfig>(GenericBEBulkActionSettingsConfig.EXTENSION_TYPE);
        }

        public IEnumerable<GenericBEGridConditionConfig> GetGenericBEGridConditionConfigs()
        {
            var extensionConfiguration = new ExtensionConfigurationManager();
            return extensionConfiguration.GetExtensionConfigurations<GenericBEGridConditionConfig>(GenericBEGridConditionConfig.EXTENSION_TYPE);
        }

        public IEnumerable<GenericBEAdditionalSettingsConfig> GetGenericBEAdditionalSettingsConfigs()
        {
            var extensionConfiguration = new ExtensionConfigurationManager();
            return extensionConfiguration.GetExtensionConfigurations<GenericBEAdditionalSettingsConfig>(GenericBEAdditionalSettingsConfig.EXTENSION_TYPE);
        }

        #endregion

        #region Private Methods

        private GenericBEDefinitionSettings TranslateGenericBEDefinitionSettings(GenericBEDefinitionSettings genericBEDefinitionSettings)
        {
            var genericBEDefinitionSettingsCopy = genericBEDefinitionSettings.VRDeepCopy();

            if (vrLocalizationManager.IsLocalizationEnabled())
            {
                if (genericBEDefinitionSettingsCopy != null)
                {

                    if (genericBEDefinitionSettingsCopy.GridDefinition != null)
                    {
                        if (genericBEDefinitionSettingsCopy.GridDefinition.ColumnDefinitions != null)
                        {
                            foreach (var columnDefinition in genericBEDefinitionSettingsCopy.GridDefinition.ColumnDefinitions)
                            {
                                if (!String.IsNullOrEmpty(columnDefinition.TextResourceKey))
                                {
                                    columnDefinition.FieldTitle = vrLocalizationManager.GetTranslatedTextResourceValue(columnDefinition.TextResourceKey, columnDefinition.FieldTitle);
                                }
                            }

                            if (genericBEDefinitionSettingsCopy.GridDefinition.GenericBEGridActions != null)
                            {
                                foreach (var gridAction in genericBEDefinitionSettingsCopy.GridDefinition.GenericBEGridActions)
                                {
                                    if (!String.IsNullOrEmpty(gridAction.TextResourceKey))
                                    {
                                        gridAction.Title = vrLocalizationManager.GetTranslatedTextResourceValue(gridAction.TextResourceKey, gridAction.Title);
                                    }

                                }
                            }

                            if (genericBEDefinitionSettingsCopy.GridDefinition.GenericBEGridActionGroups != null)
                            {
                                foreach (var gridActionGroup in genericBEDefinitionSettingsCopy.GridDefinition.GenericBEGridActionGroups)
                                {
                                    if (!String.IsNullOrEmpty(gridActionGroup.TextResourceKey))
                                    {
                                        gridActionGroup.Title = vrLocalizationManager.GetTranslatedTextResourceValue(gridActionGroup.TextResourceKey, gridActionGroup.Title);
                                    }

                                }
                            }
                            if (genericBEDefinitionSettingsCopy.GridDefinition.GenericBEGridViews != null)
                            {
                                foreach (var gridView in genericBEDefinitionSettingsCopy.GridDefinition.GenericBEGridViews)
                                {
                                    if (!String.IsNullOrEmpty(gridView.TextResourceKey))
                                    {
                                        gridView.Name = vrLocalizationManager.GetTranslatedTextResourceValue(gridView.TextResourceKey, gridView.Name);
                                    }

                                }
                            }
                        }
                    }

                    if (genericBEDefinitionSettingsCopy.EditorDefinition != null && genericBEDefinitionSettingsCopy.EditorDefinition.Settings != null)
                    {
                        genericBEDefinitionSettingsCopy.EditorDefinition.Settings.TryTranslate();
                    }

                    if (genericBEDefinitionSettingsCopy.FilterDefinition != null && genericBEDefinitionSettingsCopy.FilterDefinition.Settings != null)
                    {
                        genericBEDefinitionSettingsCopy.FilterDefinition.Settings.TryTranslate();
                    }

                    if (genericBEDefinitionSettingsCopy.GenericBEBulkActions != null)
                    {
                        foreach (var bulkAction in genericBEDefinitionSettingsCopy.GenericBEBulkActions)
                        {
                            if (!String.IsNullOrEmpty(bulkAction.TextResourceKey))
                            {
                                bulkAction.Title = vrLocalizationManager.GetTranslatedTextResourceValue(bulkAction.TextResourceKey, bulkAction.Title);
                            }
                        }
                    }
                }
            }
            return genericBEDefinitionSettingsCopy;
        }

        #endregion
    }
}