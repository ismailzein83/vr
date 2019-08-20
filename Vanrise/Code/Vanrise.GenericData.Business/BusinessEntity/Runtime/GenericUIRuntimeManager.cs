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

        public List<GenericEditorRuntimeRow> GetGenericEditorRuntimeRows(List<GenericEditorRow> rows, Guid? dataRecordTypeId, List<GenericEditorRecordField> recordFields)
        {
            if (dataRecordTypeId.HasValue)
            {
                return BuildEditorRuntimeRows(rows, dataRecordTypeId.Value);
            }

            if (recordFields != null && recordFields.Count > 0)
            {
                var recordFieldsDic = recordFields.ToDictionary(key => key.Name, val => new DataRecordField { Name = val.Name, Type = val.Type });
                return BuildEditorRuntimeRows(rows, recordFieldsDic, dataRecordTypeId);
            }

            return null;
        }

        public GenericFieldsRuntimeRowOutput GetGenericFieldsRuntimeRow(List<GenericEditorField> fields, Guid? dataRecordTypeId, List<GenericEditorRecordField> recordFields)
        {
            if (dataRecordTypeId.HasValue)
            {
                var runtimeFields = BuildEditorRuntimeFields(fields, dataRecordTypeId.Value);
                return runtimeFields != null && runtimeFields.Count > 0 ? new GenericFieldsRuntimeRowOutput() { Fields = runtimeFields } : null;
            }

            if (recordFields != null && recordFields.Count > 0)
            {
                var recordFieldsDic = recordFields.ToDictionary(key => key.Name, val => new DataRecordField { Name = val.Name, Type = val.Type });
                var runtimeFields  = BuildEditorRuntimeFields(fields, recordFieldsDic, dataRecordTypeId);
                return runtimeFields != null && runtimeFields.Count > 0 ? new GenericFieldsRuntimeRowOutput() { Fields = runtimeFields } : null;
            }

            return null;
        }
        //public GenericEditorRuntimeField GetGenericEditorRuntimeField(GenericEditorField field, Guid dataRecordTypeId)
        //{
        //     return BuildEditorRuntimeField(field, dataRecordTypeId);
        //}
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

        public T BuildRuntimeField<T>(GenericUIField field, Dictionary<string, DataRecordField> dataRecordTypeFieldsByName, Guid? dataRecordTypeId) where T : GenericUIRuntimeField
        {
            var runtimeField = Activator.CreateInstance<T>();
            runtimeField.FieldTitle = field.FieldTitle; //!string.IsNullOrEmpty(field.FieldTitle) ? field.FieldTitle : GetFieldTitle(field.FieldPath, dataRecordTypeFieldsByName, dataRecordTypeId); 
            runtimeField.FieldPath = field.FieldPath;
            runtimeField.FieldViewSettings = field.FieldViewSettings;
            runtimeField.FieldType = GetFieldType(field.FieldPath, dataRecordTypeFieldsByName, dataRecordTypeId);
            runtimeField.DefaultFieldValue = field.DefaultFieldValue;
            return runtimeField;
        }

        public DataRecordFieldType GetFieldType(string fieldPath, Dictionary<string, DataRecordField> dataRecordTypeFieldsByName, Guid? dataRecordTypeId)
        {
            DataRecordField dataRecordTypeField;
            if (!dataRecordTypeFieldsByName.TryGetValue(fieldPath, out dataRecordTypeField))
                throw new NullReferenceException(String.Format("DataRecordType '{0}' dataRecordTypeField '{1}'", dataRecordTypeId, fieldPath));
            return dataRecordTypeField.Type;
        }

        //public string GetFieldTitle(string fieldPath, Dictionary<string, DataRecordField> dataRecordTypeFieldsByName, Guid dataRecordTypeId)
        //{
        //    DataRecordField dataRecordTypeField;
        //    if (!dataRecordTypeFieldsByName.TryGetValue(fieldPath, out dataRecordTypeField))
        //        throw new NullReferenceException(String.Format("DataRecordType '{0}' dataRecordTypeField '{1}'", dataRecordTypeId, fieldPath));
        //    return dataRecordTypeField.Title;
        //}

        //public GenericEditorRuntimeField GetRequiredParentBEDefData(Guid requiredParentBEDefinitionId)
        //{
        //    RequiredParentBEDefData requiredParentBEDefData = null;
        //    var businessEntityDefinition = new GenericBusinessEntityDefinitionManager().GetGenericBEDefinition(requiredParentBEDefinitionId);
        //    var genericBEDefinitionSettings = businessEntityDefinition.Settings.CastWithValidate<GenericBEDefinitionSettings>("businessEntityDefinition.Settings", requiredParentBEDefinitionId);

        //    DataRecordType dataRecordType = new DataRecordTypeManager().GetDataRecordType(genericBEDefinitionSettings.DataRecordTypeId);
        //    dataRecordType.ThrowIfNull("dataRecordType", genericBEDefinitionSettings.DataRecordTypeId);

        //    requiredParentBEDefData = new RequiredParentBEDefData()
        //    {
        //        Name = dataRecordType.Name, //or businessEntityDefinition.Title
        //        Title = businessEntityDefinition.Title,
        //        ViewerEditor = genericBEDefinitionSettings.ViewerEditor,

        //    };
        //    return new GenericEditorRuntimeField
        //    {
        //        FieldPath = dataRecordType.Name,
        //        FieldTitle = businessEntityDefinition.Title,
        //        IsDisabled = false,
        //        IsRequired = true,
        //    };
        //}

        //public class RequiredParentBEDefData
        //{
        //    public string Name { get; set; }

        //    public string Title { get; set; }

        //    public string ViewerEditor { get; set; }

        //    public DataRecordField DataRecordField { get; set; }

        //    public GenericEditorRuntimeField GenericEditorRuntimeField { get; set; }
        //}

        #endregion

        #region Private Methods

        #region Management Runtime
        private void BuildManagementRuntime(GenericManagement genericManagement, GenericManagementRuntime genericManagementRuntime, Guid dataRecordTypeId)
        {
            var dataRecordTypeManager = new DataRecordTypeManager();
            var fields = dataRecordTypeManager.GetDataRecordTypeFields(dataRecordTypeId);
            genericManagementRuntime.Grid = new GenericGridRuntime();
            genericManagementRuntime.Filter = new GenericFilterRuntime();
            BuildGridRuntime(genericManagement.GridDesign, genericManagementRuntime.Grid, fields, dataRecordTypeId);
            BuildFilterRuntime(genericManagement.FilterDesign, genericManagementRuntime.Filter, fields, dataRecordTypeId);
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
            var dataRecordTypeManager = new DataRecordTypeManager();
            var fields = dataRecordTypeManager.GetDataRecordTypeFields(dataRecordTypeId);
            if (fields == null)
                throw new NullReferenceException(String.Format("fields of DataRecordType '{0}'", dataRecordTypeId));
            return BuildEditorRuntimeRows(rows, fields, dataRecordTypeId);
        }

        private List<GenericEditorRuntimeRow> BuildEditorRuntimeRows(List<GenericEditorRow> rows, Dictionary<string, DataRecordField> recordFields, Guid? dataRecordTypeId)
        {
            var runtimeRows = new List<GenericEditorRuntimeRow>();
            if (rows != null)
            {
                foreach (var row in rows)
                {
                    var runtimeRow = new GenericEditorRuntimeRow();
                    runtimeRows.Add(runtimeRow);
                    BuildEditorRuntimeFields(row, runtimeRow, recordFields, dataRecordTypeId);
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
        private void BuildEditorRuntimeFields(GenericEditorRow row, GenericEditorRuntimeRow runtimeRow, Dictionary<string, DataRecordField> dataRecordTypeFieldsByName, Guid? dataRecordTypeId)
        {
            if (row.Fields != null)
            {
                runtimeRow.Fields = new List<GenericEditorRuntimeField>();
                foreach (var field in row.Fields)
                {
                    var runtimeField = BuildRuntimeField<GenericEditorRuntimeField>(field, dataRecordTypeFieldsByName, dataRecordTypeId);
                    runtimeField.IsRequired = field.IsRequired;
                    runtimeField.IsDisabled = field.IsDisabled;
                    runtimeField.ShowAsLabel = field.ShowAsLabel;
                    runtimeField.FieldWidth = field.FieldWidth;
                    runtimeField.HideLabel = field.HideLabel;
                    runtimeField.ReadOnly = field.ReadOnly;
                    runtimeRow.Fields.Add(runtimeField);
                }
            }
        }

        private List<GenericEditorRuntimeField> BuildEditorRuntimeFields(List<GenericEditorField> fields, Guid dataRecordTypeId)
        {
            if (fields == null || fields.Count == 0)
                return null;

            var dataRecordTypeManager = new DataRecordTypeManager();
            var dataRecordTypeFields = dataRecordTypeManager.GetDataRecordTypeFields(dataRecordTypeId);
            if (dataRecordTypeFields == null)
                throw new NullReferenceException($"fields of DataRecordType '{dataRecordTypeId}'");

            return BuildEditorRuntimeFields(fields, dataRecordTypeFields, dataRecordTypeId);
        }

        private List<GenericEditorRuntimeField> BuildEditorRuntimeFields(List<GenericEditorField> fields, Dictionary<string, DataRecordField> dataRecordTypeFields, Guid? dataRecordTypeId)
        {
            if (fields == null || fields.Count == 0)
                return null;

            List<GenericEditorRuntimeField> genericEditorRuntimeFields = new List<GenericEditorRuntimeField>();
            foreach (var field in fields)
            {
                var runtimeField = BuildRuntimeField<GenericEditorRuntimeField>(field, dataRecordTypeFields, dataRecordTypeId);
                runtimeField.IsRequired = field.IsRequired;
                runtimeField.IsDisabled = field.IsDisabled;
                runtimeField.ShowAsLabel = field.ShowAsLabel;
                runtimeField.FieldWidth = field.FieldWidth;
                runtimeField.HideLabel = field.HideLabel;
                genericEditorRuntimeFields.Add(runtimeField);
            }

            return genericEditorRuntimeFields;
        }

        //private GenericEditorRuntimeField BuildEditorRuntimeField(GenericEditorField field, Guid dataRecordTypeId)
        //{
        //    var dataRecordTypeManager = new DataRecordTypeManager();
        //    var dataRecordTypeFields = dataRecordTypeManager.GetDataRecordTypeFields(dataRecordTypeId);
        //    if (dataRecordTypeFields == null)
        //        throw new NullReferenceException($"fields of DataRecordType '{dataRecordTypeId}'");

        //    List<GenericEditorRuntimeField> genericEditorRuntimeFields = new List<GenericEditorRuntimeField>();
        //    var runtimeField = BuildRuntimeField<GenericEditorRuntimeField>(field, dataRecordTypeFields, dataRecordTypeId);
        //    runtimeField.IsRequired = field.IsRequired;
        //    runtimeField.IsDisabled = field.IsDisabled;
        //    //runtimeField.IsDisabledOnEdit = field.IsDisabledOnEdit;

        //    return runtimeField;
        //}

        #endregion


        #endregion

    }


}