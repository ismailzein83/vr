using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.GenericData.Data;
using Vanrise.Caching;

namespace Vanrise.GenericData.Business
{
    public class GenericUIRuntimeManager
    {
        public GenericEditor GetEditor(int editorId)
        {
            return null;
        }

        public ExtensibleBEItemRuntime GetExtensibleBEItemRuntime(int businessEntityId, int dataRecordTypeId)
        {
            ExtensibleBEItemManager manager = new ExtensibleBEItemManager();
            var extensibleBEItem = manager.GetExtensibleBEItem(businessEntityId, dataRecordTypeId);
            if (extensibleBEItem == null)
                return null;

            ExtensibleBEItemRuntime extensibleBEItemRuntime = new ExtensibleBEItemRuntime();
            BuildExtensibleBEItemRuntime(extensibleBEItem, extensibleBEItemRuntime, dataRecordTypeId);
            return extensibleBEItemRuntime;
        }

        public GenericEditorRuntime GetGenericEditorRuntime(int businessEntityDefinitionId)
        {
            BusinessEntityDefinitionManager businessEntityDefinitionManager = new BusinessEntityDefinitionManager();
            var businessEntityDefinition = businessEntityDefinitionManager.GetBusinessEntityDefinition(businessEntityDefinitionId);
            if (businessEntityDefinition == null)
                return null;
            var businessEntityDefinitionSettings = businessEntityDefinition.Settings as GenericBEDefinitionSettings;
            GenericEditorRuntime editorRuntime = new GenericEditorRuntime();
            BuildEditorRuntime(businessEntityDefinitionSettings.EditorDesign, editorRuntime, businessEntityDefinitionSettings.DataRecordTypeId);
            return editorRuntime;
        }

        public IEnumerable<DataRecordTypeInfo> GetDataRecordTypesInfo(int businessEntityId)
        {
            ExtensibleBEItemManager manager = new ExtensibleBEItemManager();
            var allExtensibleBEItems = manager.GetAllExtensibleBEItems();
            var extensibleBEItems = allExtensibleBEItems.FindAllRecords(x => x.BusinessEntityDefinitionId == businessEntityId);
             var dataRecordTypeManager = new DataRecordTypeManager();
             List<int> recordTypeIds = new List<int>();
             foreach (ExtensibleBEItem extensibleBEItem in extensibleBEItems)
             {
                 recordTypeIds.Add(extensibleBEItem.DataRecordTypeId);
             }
             return dataRecordTypeManager.GetDataRecordTypeInfo(new DataRecordTypeInfoFilter { RecordTypeIds = recordTypeIds });
        }

        public GenericManagementRuntime GetManagementRuntime(int businessEntityDefinitionId)
        {
            BusinessEntityDefinitionManager businessEntityDefinitionManager = new BusinessEntityDefinitionManager();
            var businessEntityDefinition = businessEntityDefinitionManager.GetBusinessEntityDefinition(businessEntityDefinitionId);
            if (businessEntityDefinition == null)
                return null;

            var businessEntityDefinitionSettings = businessEntityDefinition.Settings as GenericBEDefinitionSettings;
            GenericManagementRuntime genericManagementRuntime = new GenericManagementRuntime();
            BuildManagementRuntime(businessEntityDefinitionSettings.ManagementDesign, genericManagementRuntime, businessEntityDefinitionSettings.DataRecordTypeId);
            return genericManagementRuntime;
        }

        #region Private Methods

        #region Management Runtime
        private void BuildManagementRuntime(GenericManagement genericManagement, GenericManagementRuntime genericManagementRuntime, int dataRecordTypeId)
        {
            var dataRecordTypeManager = new DataRecordTypeManager();
            var fields = dataRecordTypeManager.GetDataRecordTypeFields(dataRecordTypeId);
            genericManagementRuntime.Grid = new GenericGridRuntime();
            genericManagementRuntime.Filter = new GenericFilterRuntime();
            BuildGridRuntime(genericManagement.GridDesign, genericManagementRuntime.Grid,fields, dataRecordTypeId);
            BuildFilterRuntime(genericManagement.FilterDesign, genericManagementRuntime.Filter,fields, dataRecordTypeId);
        }
    
        #region Grid Runtime
        private void BuildGridRuntime(GenericGrid gridDesign, GenericGridRuntime genericGridRuntime, List<DataRecordField> dataRecordTypeFields, int dataRecordTypeId)
        {
            if (gridDesign.Columns != null)
            {
                genericGridRuntime.Columns = new List<GenericGridRuntimeField>();
                foreach (var column in gridDesign.Columns)
                {
                    var runtimeColumn = new GenericGridRuntimeField();
                    genericGridRuntime.Columns.Add(runtimeColumn);
                    runtimeColumn.FieldTitle = column.FieldTitle;
                    runtimeColumn.FieldPath = column.FieldPath;
                    var dataRecordTypeField = dataRecordTypeFields.FindRecord(itm => itm.Name == runtimeColumn.FieldPath);
                    if (dataRecordTypeField == null)
                        throw new NullReferenceException(String.Format("DataRecordType '{0}' dataRecordTypeField '{1}'", dataRecordTypeId, runtimeColumn.FieldPath));
                    runtimeColumn.FieldType = dataRecordTypeField.Type;
                }
            }
        }
        #endregion
        
        #region Filter Runtime
        private void BuildFilterRuntime(GenericFilter genericFilter, GenericFilterRuntime genericFilterRuntime, List<DataRecordField> dataRecordTypeFields, int dataRecordTypeId)
        {
            if (genericFilter.Fields != null)
            {
                genericFilterRuntime.Fields = new List<GenericFilterRuntimeField>();
                foreach (var field in genericFilter.Fields)
                {
                    var runtimeField = new GenericFilterRuntimeField();
                    genericFilterRuntime.Fields.Add(runtimeField);

                    runtimeField.FieldTitle = field.FieldTitle;
                    runtimeField.FieldPath = field.FieldPath;
                    runtimeField.IsRequired = field.IsRequired;
                    var dataRecordTypeField = dataRecordTypeFields.FindRecord(itm => itm.Name == runtimeField.FieldPath);
                    if (dataRecordTypeField == null)
                        throw new NullReferenceException(String.Format("DataRecordType '{0}' dataRecordTypeField '{1}'", dataRecordTypeId, runtimeField.FieldPath));
                    runtimeField.FieldType = dataRecordTypeField.Type;
                }
            }
        }
        #endregion
     
        #endregion

        #region ExtensibleBEItem Runtime
        private void BuildExtensibleBEItemRuntime(ExtensibleBEItem extensibleBEItem, ExtensibleBEItemRuntime extensibleBEItemRuntime, int dataRecordTypeId)
        {
            BuildExtensibleBEItemRuntimeSections(extensibleBEItem.Sections, extensibleBEItemRuntime, dataRecordTypeId);
        }
        private void BuildExtensibleBEItemRuntimeSections(List<GenericEditorSection> sections, ExtensibleBEItemRuntime extensibleBEItemRuntime, int dataRecordTypeId)
        {
            if (sections != null)
            {
                extensibleBEItemRuntime.Sections = new List<GenericEditorRuntimeSection>();
                foreach (var section in sections)
                {
                    var runtimeSection = new GenericEditorRuntimeSection();
                    runtimeSection.SectionTitle = section.SectionTitle;
                    extensibleBEItemRuntime.Sections.Add(runtimeSection);
                    ExtensibleBEItemRuntimeRows(section, runtimeSection, dataRecordTypeId);
                }
            }
        }
        private void ExtensibleBEItemRuntimeRows(GenericEditorSection section, GenericEditorRuntimeSection runtimeSection, int dataRecordTypeId)
        {
            if (section.Rows != null)
            {
                var dataRecordTypeManager = new DataRecordTypeManager();
                var fields = dataRecordTypeManager.GetDataRecordTypeFields(dataRecordTypeId);
                if (fields == null)
                    throw new NullReferenceException(String.Format("fields of DataRecordType '{0}'", dataRecordTypeId));
                runtimeSection.Rows = new List<GenericEditorRuntimeRow>();
                foreach (var row in section.Rows)
                {
                    var runtimeRow = new GenericEditorRuntimeRow();
                    runtimeSection.Rows.Add(runtimeRow);
                    BuildExtensibleBEItemRuntimeFields(row, runtimeRow, fields, dataRecordTypeId);
                }
            }
        }
        private void BuildExtensibleBEItemRuntimeFields(GenericEditorRow row, GenericEditorRuntimeRow runtimeRow, List<DataRecordField> dataRecordTypeFields, int dataRecordTypeId)
        {
            if (row.Fields != null)
            {
                runtimeRow.Fields = new List<GenericEditorRuntimeField>();
                foreach (var field in row.Fields)
                {
                    var runtimeField = BuildRuntimeField(field, dataRecordTypeFields, dataRecordTypeId);
                    runtimeRow.Fields.Add(runtimeField);
                }
            }
        }
        #endregion

        #region GeenricEditor Runtime
        private void BuildEditorRuntime(GenericEditor genericEditor, GenericEditorRuntime editorRuntime, int dataRecordTypeId)
        {
            BuildEditorRuntimeSections(genericEditor.Sections, editorRuntime, dataRecordTypeId);
        }
        private void BuildEditorRuntimeSections(List<GenericEditorSection> sections, GenericEditorRuntime editorRuntime, int dataRecordTypeId)
        {
            if (sections != null)
            {
                editorRuntime.Sections = new List<GenericEditorRuntimeSection>();
                foreach (var section in sections)
                {
                    var runtimeSection = new GenericEditorRuntimeSection();
                    runtimeSection.SectionTitle = section.SectionTitle;
                    editorRuntime.Sections.Add(runtimeSection);
                    BuildEditorRuntimeRows(section, runtimeSection, dataRecordTypeId);
                }
            }
        }
        private void BuildEditorRuntimeRows(GenericEditorSection section, GenericEditorRuntimeSection runtimeSection, int dataRecordTypeId)
        {
            if (section.Rows != null)
            {
                var dataRecordTypeManager = new DataRecordTypeManager();
                var fields = dataRecordTypeManager.GetDataRecordTypeFields(dataRecordTypeId);
                if (fields == null)
                    throw new NullReferenceException(String.Format("fields of DataRecordType '{0}'", dataRecordTypeId));
                runtimeSection.Rows = new List<GenericEditorRuntimeRow>();
                foreach (var row in section.Rows)
                {
                    var runtimeRow = new GenericEditorRuntimeRow();
                    runtimeSection.Rows.Add(runtimeRow);
                    BuildEditorRuntimeFields(row, runtimeRow, fields, dataRecordTypeId);
                }
            }
        }
        private void BuildEditorRuntimeFields(GenericEditorRow row, GenericEditorRuntimeRow runtimeRow, List<DataRecordField> dataRecordTypeFields, int dataRecordTypeId)
        {
            if (row.Fields != null)
            {
                runtimeRow.Fields = new List<GenericEditorRuntimeField>();
                foreach (var field in row.Fields)
                {
                    var runtimeField = BuildRuntimeField(field, dataRecordTypeFields, dataRecordTypeId);
                    runtimeRow.Fields.Add(runtimeField);
                }
            }
        }

        #endregion

        public GenericEditorRuntimeField BuildRuntimeField(GenericUIField field, List<DataRecordField> dataRecordTypeFields, int dataRecordTypeId)
        {
            var runtimeField = new GenericEditorRuntimeField();
            runtimeField.FieldTitle = field.FieldTitle;
            runtimeField.FieldPath = field.FieldPath;
            var dataRecordTypeField = dataRecordTypeFields.FindRecord(itm => itm.Name == runtimeField.FieldPath);
            if (dataRecordTypeField == null)
                throw new NullReferenceException(String.Format("DataRecordType '{0}' dataRecordTypeField '{1}'", dataRecordTypeId, runtimeField.FieldPath));
            runtimeField.FieldType = dataRecordTypeField.Type;
            return runtimeField;
        }

        #endregion



       
    }
}