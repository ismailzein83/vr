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
      
        #region Private Methods
       
        public GenericEditor GetEditor(int editorId)
        {
            return null;
        }

        public ExtensibleBEItemRuntime GetExtensibleBEItemRuntime(Guid businessEntityId, Guid dataRecordTypeId)
        {
            ExtensibleBEItemManager manager = new ExtensibleBEItemManager();
            var extensibleBEItem = manager.GetExtensibleBEItem(businessEntityId, dataRecordTypeId);
            if (extensibleBEItem == null)
                return null;

            ExtensibleBEItemRuntime extensibleBEItemRuntime = new ExtensibleBEItemRuntime();
            BuildExtensibleBEItemRuntime(extensibleBEItem, extensibleBEItemRuntime, dataRecordTypeId);
            return extensibleBEItemRuntime;
        }

        //public GenericEditorRuntime GetGenericEditorRuntime(Guid businessEntityDefinitionId)
        //{
        //    BusinessEntityDefinitionManager businessEntityDefinitionManager = new BusinessEntityDefinitionManager();
        //    var businessEntityDefinition = businessEntityDefinitionManager.GetBusinessEntityDefinition(businessEntityDefinitionId);
        //    if (businessEntityDefinition == null)
        //        return null;
        //    var businessEntityDefinitionSettings = businessEntityDefinition.Settings as GenericBEDefinitionSettings;
        //    GenericEditorRuntime editorRuntime = new GenericEditorRuntime();
        //    BuildEditorRuntime(businessEntityDefinitionSettings.EditorDesign, editorRuntime, businessEntityDefinitionSettings.DataRecordTypeId);
        //    return editorRuntime;
        //}

        public List<GenericEditorRuntimeSection> GetGenericEditorRuntimeSections(List<GenericEditorSection> sections, Guid dataRecordTypeId)
        {
            var runtimeSections = new List<GenericEditorRuntimeSection>();
            foreach (var section in sections)
            {
                var runtimeSection = new GenericEditorRuntimeSection();
                runtimeSection.SectionTitle = section.SectionTitle;
                runtimeSections.Add(runtimeSection);
                BuildEditorRuntimeRows(section, runtimeSection, dataRecordTypeId);
            }
            return runtimeSections;
        }

        public List<GenericEditorRuntimeRow> GetGenericEditorRuntimeRows(List<GenericEditorRow> rows, Guid dataRecordTypeId)
        {
            return BuildEditorRuntimeRows(rows, dataRecordTypeId);
        }
        public IEnumerable<DataRecordTypeInfo> GetDataRecordTypesInfo(Guid businessEntityId)
        {
            ExtensibleBEItemManager manager = new ExtensibleBEItemManager();
            var allExtensibleBEItems = manager.GetAllExtensibleBEItems();
            var extensibleBEItems = allExtensibleBEItems.FindAllRecords(x => x.BusinessEntityDefinitionId == businessEntityId);
             var dataRecordTypeManager = new DataRecordTypeManager();
             List<Guid> recordTypeIds = new List<Guid>();
             foreach (ExtensibleBEItem extensibleBEItem in extensibleBEItems)
             {
                 recordTypeIds.Add(extensibleBEItem.DataRecordTypeId);
             }
             return dataRecordTypeManager.GetDataRecordTypeInfo(new DataRecordTypeInfoFilter { RecordTypeIds = recordTypeIds }).OrderBy(x => x.Name);
        }

        //public GenericManagementRuntime GetManagementRuntime(Guid businessEntityDefinitionId)
        //{
        //    BusinessEntityDefinitionManager businessEntityDefinitionManager = new BusinessEntityDefinitionManager();
        //    var businessEntityDefinition = businessEntityDefinitionManager.GetBusinessEntityDefinition(businessEntityDefinitionId);
        //    if (businessEntityDefinition == null)
        //        return null;

        //    var businessEntityDefinitionSettings = businessEntityDefinition.Settings as GenericBEDefinitionSettings;
        //    GenericManagementRuntime genericManagementRuntime = new GenericManagementRuntime();
        //    BuildManagementRuntime(businessEntityDefinitionSettings.ManagementDesign, genericManagementRuntime, businessEntityDefinitionSettings.DataRecordTypeId);
        //    return genericManagementRuntime;
        //}

        public T BuildRuntimeField<T>(GenericUIField field, Dictionary<string, DataRecordField> dataRecordTypeFieldsByName, Guid dataRecordTypeId) where T : GenericUIRuntimeField
        {
            var runtimeField = Activator.CreateInstance<T>();
            runtimeField.FieldTitle = field.FieldTitle;
            runtimeField.FieldPath = field.FieldPath;
            runtimeField.FieldType = GetFieldType(field.FieldPath, dataRecordTypeFieldsByName, dataRecordTypeId);
            return runtimeField;
        }

        public DataRecordFieldType GetFieldType(string fieldPath, Dictionary<string, DataRecordField> dataRecordTypeFieldsByName, Guid dataRecordTypeId)
        {
            DataRecordField dataRecordTypeField;
            if (!dataRecordTypeFieldsByName.TryGetValue(fieldPath, out dataRecordTypeField))
                throw new NullReferenceException(String.Format("DataRecordType '{0}' dataRecordTypeField '{1}'", dataRecordTypeId, fieldPath));
            return dataRecordTypeField.Type;
        }
        #endregion

        #region Private Methods

        #region Management Runtime
        private void BuildManagementRuntime(GenericManagement genericManagement, GenericManagementRuntime genericManagementRuntime, Guid dataRecordTypeId)
        {
            var dataRecordTypeManager = new DataRecordTypeManager();
            var fields = dataRecordTypeManager.GetDataRecordTypeFields(dataRecordTypeId);
            genericManagementRuntime.Grid = new GenericGridRuntime();
            genericManagementRuntime.Filter = new GenericFilterRuntime();
            BuildGridRuntime(genericManagement.GridDesign, genericManagementRuntime.Grid,fields, dataRecordTypeId);
            BuildFilterRuntime(genericManagement.FilterDesign, genericManagementRuntime.Filter,fields, dataRecordTypeId);
        }
    
        #region Grid Runtime
        private void BuildGridRuntime(GenericGrid gridDesign, GenericGridRuntime genericGridRuntime, Dictionary<string, DataRecordField> dataRecordTypeFieldsByName, Guid dataRecordTypeId)
        {
            if (gridDesign.Columns != null)
            {
                genericGridRuntime.Columns = new List<GenericGridRuntimeField>();
                foreach (var column in gridDesign.Columns)
                {
                    var runtimeColumn = BuildRuntimeField<GenericGridRuntimeField>(column, dataRecordTypeFieldsByName, dataRecordTypeId);
                    runtimeColumn.Attribute = runtimeColumn.FieldType.GetGridColumnAttribute(null);
                    genericGridRuntime.Columns.Add(runtimeColumn);
                }
            }
        }
        #endregion
        
        #region Filter Runtime
        private void BuildFilterRuntime(GenericFilter genericFilter, GenericFilterRuntime genericFilterRuntime, Dictionary<string, DataRecordField> dataRecordTypeFieldsByName, Guid dataRecordTypeId)
        {
            if (genericFilter.Fields != null)
            {
                genericFilterRuntime.Fields = new List<GenericFilterRuntimeField>();
                foreach (var field in genericFilter.Fields)
                {
                    var runtimeField = BuildRuntimeField<GenericFilterRuntimeField>(field, dataRecordTypeFieldsByName, dataRecordTypeId);
                    runtimeField.IsRequired = field.IsRequired;
                    genericFilterRuntime.Fields.Add(runtimeField);
                }
            }
        }
        #endregion
     
        #endregion

        #region ExtensibleBEItem Runtime
        private void BuildExtensibleBEItemRuntime(ExtensibleBEItem extensibleBEItem, ExtensibleBEItemRuntime extensibleBEItemRuntime, Guid dataRecordTypeId)
        {
            BuildExtensibleBEItemRuntimeSections(extensibleBEItem.Sections, extensibleBEItemRuntime, dataRecordTypeId);
        }
        private void BuildExtensibleBEItemRuntimeSections(List<GenericEditorSection> sections, ExtensibleBEItemRuntime extensibleBEItemRuntime, Guid dataRecordTypeId)
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
        private void ExtensibleBEItemRuntimeRows(GenericEditorSection section, GenericEditorRuntimeSection runtimeSection, Guid dataRecordTypeId)
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
        private void BuildExtensibleBEItemRuntimeFields(GenericEditorRow row, GenericEditorRuntimeRow runtimeRow, Dictionary<string, DataRecordField> dataRecordTypeFieldsByName, Guid dataRecordTypeId)
        {
            if (row.Fields != null)
            {
                runtimeRow.Fields = new List<GenericEditorRuntimeField>();
                foreach (var field in row.Fields)
                {
                    var runtimeField = BuildRuntimeField<GenericEditorRuntimeField>(field, dataRecordTypeFieldsByName, dataRecordTypeId);
                    runtimeField.IsRequired = field.IsRequired;
                    runtimeRow.Fields.Add(runtimeField);
                }
            }
        }
        #endregion

        #region GeenricEditor Runtime
        private void BuildEditorRuntime(GenericEditor genericEditor, GenericEditorRuntime editorRuntime, Guid dataRecordTypeId)
        {
            BuildEditorRuntimeSections(genericEditor.Sections, editorRuntime, dataRecordTypeId);
        }

        private void BuildEditorRuntimeSections(List<GenericEditorSection> sections, GenericEditorRuntime editorRuntime, Guid dataRecordTypeId)
        {
            if (sections != null)
            {
                editorRuntime.Sections = GetGenericEditorRuntimeSections(sections, dataRecordTypeId);
                
            }
        }

        private List<GenericEditorRuntimeRow> BuildEditorRuntimeRows(List<GenericEditorRow> rows, Guid dataRecordTypeId)
        {
            var runtimeRows = new List<GenericEditorRuntimeRow>();
            if (rows != null)
            {
                var dataRecordTypeManager = new DataRecordTypeManager();
                var fields = dataRecordTypeManager.GetDataRecordTypeFields(dataRecordTypeId);
                if (fields == null)
                    throw new NullReferenceException(String.Format("fields of DataRecordType '{0}'", dataRecordTypeId));
                foreach (var row in rows)
                {
                    var runtimeRow = new GenericEditorRuntimeRow();
                    runtimeRows.Add(runtimeRow);
                    BuildEditorRuntimeFields(row, runtimeRow, fields, dataRecordTypeId);
                }
            }
            return runtimeRows;
        }
        private void BuildEditorRuntimeRows(GenericEditorSection section, GenericEditorRuntimeSection runtimeSection, Guid dataRecordTypeId)
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
        private void BuildEditorRuntimeFields(GenericEditorRow row, GenericEditorRuntimeRow runtimeRow, Dictionary<string, DataRecordField> dataRecordTypeFieldsByName, Guid dataRecordTypeId)
        {
            if (row.Fields != null)
            {
                runtimeRow.Fields = new List<GenericEditorRuntimeField>();
                foreach (var field in row.Fields)
                {
                    var runtimeField = BuildRuntimeField<GenericEditorRuntimeField>(field, dataRecordTypeFieldsByName, dataRecordTypeId);
                    runtimeField.IsRequired = field.IsRequired;
                    runtimeField.IsDisabled = field.IsDisabled;
                    runtimeRow.Fields.Add(runtimeField);
                }
            }
        }

        #endregion


        #endregion

    }
}