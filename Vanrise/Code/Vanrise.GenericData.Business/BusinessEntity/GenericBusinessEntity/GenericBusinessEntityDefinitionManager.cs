﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
namespace Vanrise.GenericData.Business
{
    public class GenericBusinessEntityDefinitionManager
    {
        DataRecordTypeManager _dataRecordTypeManager;
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
            return GetGenericBEDefinitionSettings(businessEntityDefinitionId).TitleFieldName;
        }
        public string GetGenericBEDefinitionTitle(Guid businessEntityDefinitionId)
        {
            return GetGenericBEDefinition(businessEntityDefinitionId).Title;
        }

        public GenericBESelectorRuntimeInfo GetGenericBESelectorRuntimeInfo(Guid businessEntityDefinitionId)
        {
            var genericBEDefinitionSettings = GetGenericBEDefinitionSettings(businessEntityDefinitionId);
            var idFieldType = GetIdFieldTypeForGenericBE(businessEntityDefinitionId);

            return new GenericBESelectorRuntimeInfo()
            {
                SelectorSingularTitle = genericBEDefinitionSettings.SelectorSingularTitle,
                SelectorPluralTitle = genericBEDefinitionSettings.SelectorPluralTitle,
                IdFieldName = idFieldType != null ? idFieldType.Name : null,
                TitleFieldName = GetGenericBEDefinitionTitleFieldName(businessEntityDefinitionId)
            };
            
        }

        public GenericBEDefinitionSettings GetGenericBEDefinitionSettings(Guid businessEntityDefinitionId)
        { 
            var genericBEDefinition = GetGenericBEDefinition(businessEntityDefinitionId);
            genericBEDefinition.Settings.ThrowIfNull("genericBEDefinition.Settings");
            return genericBEDefinition.Settings.CastWithValidate<GenericBEDefinitionSettings>("genericBEDefinition.Settings");
        }
        public Guid GetGenericBEDataRecordStorageId(Guid businessEntityDefinitionId)
        {
            var genericBEDefinitionSettings =  GetGenericBEDefinitionSettings(businessEntityDefinitionId);
            if (genericBEDefinitionSettings.DataRecordStorageId==null)
            {
                throw new NullReferenceException("genericBEDefinitionSettings.DataRecordStorageId");
            }
            return genericBEDefinitionSettings.DataRecordStorageId.Value;

        }
        public DataRecordType GetGenericBEDataRecordType(Guid businessEntityDefinitionId)
        {
            var genericBEDefinitionSettings = GetGenericBEDefinitionSettings(businessEntityDefinitionId);
            var dataRecordType = _dataRecordTypeManager.GetDataRecordType(genericBEDefinitionSettings.DataRecordTypeId);
            dataRecordType.ThrowIfNull("dataRecordType", genericBEDefinitionSettings.DataRecordTypeId);
            return dataRecordType;
        }
        public GenericBEGridDefinition GetGenericBEDefinitionGridDefinition(Guid businessEntityDefinitionId)
        {
            return GetGenericBEDefinitionSettings(businessEntityDefinitionId).GridDefinition;
        }
        public List<GenericBEDefinitionGridColumnAttribute> GetGenericBEDefinitionGridColumnAttributes(Guid businessEntityDefinitionId)
        {
            List<GenericBEDefinitionGridColumnAttribute> gridColumns = new List<GenericBEDefinitionGridColumnAttribute>();
            var genericBEDefinitionSettings = GetGenericBEDefinitionSettings(businessEntityDefinitionId);
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
                var vrCaseGridColumnAttribute = new GenericBEDefinitionGridColumnAttribute
                {
                    Attribute = dataRecordTypeField.Type.GetGridColumnAttribute(context),
                    Name = vrCaseGridColumn.FieldName
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
            var genericBEDefinitionSettings = GetGenericBEDefinitionSettings(businessEntityDefinitionId);
            return GetDataRecordTypeFields(genericBEDefinitionSettings.DataRecordTypeId);
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
                var businessEntityDefinitionSettings = GetGenericBEDefinitionSettings(businessEntityDefinitionId);
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
           }) ;
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

        #region Config region

        public IEnumerable<GenericBEViewDefinitionSettingsConfig> GetGenericBEViewDefinitionSettingsConfigs()
        {
            var extensionConfiguration = new ExtensionConfigurationManager();
            return extensionConfiguration.GetExtensionConfigurations<GenericBEViewDefinitionSettingsConfig>(GenericBEViewDefinitionSettingsConfig.EXTENSION_TYPE);
        }

        public IEnumerable<GenericBEEditorDefinitionSettingsConfig> GetGenericBEEditorDefinitionSettingsConfigs()
        {
            var extensionConfiguration = new ExtensionConfigurationManager();
            return extensionConfiguration.GetExtensionConfigurations<GenericBEEditorDefinitionSettingsConfig>(GenericBEEditorDefinitionSettingsConfig.EXTENSION_TYPE);
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

        #endregion
    }
}
