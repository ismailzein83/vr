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
    public class GenericEditorManager
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

        public GenericEditorRuntime GetGenericEditorRuntime(int businessEntityId, int dataRecordTypeId)
        {
            ExtensibleBEItemManager manager = new ExtensibleBEItemManager();
            var extensibleBEItem = manager.GetExtensibleBEItem(businessEntityId, dataRecordTypeId);
            if (extensibleBEItem == null)
                return null;

            GenericEditorRuntime editorRuntime = new GenericEditorRuntime();
            BuildEditorRuntimeSections(extensibleBEItem.Sections, editorRuntime, dataRecordTypeId);
            return editorRuntime;
        }
        public GenericEditorRuntime GetEditorRuntime(int editorId)
        {
            //var genericEditor = GetEditor(editorId);
            //if (genericEditor == null)
               return null;

            //GenericEditorRuntime editorRuntime = new GenericEditorRuntime();
            //BuildEditorRuntime(genericEditor, editorRuntime);
            //return editorRuntime;
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



        #region Private Methods
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
                    BuildEditorRuntimeRows(section, runtimeSection, dataRecordTypeId);
                }
            }
        }

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
                    BuildEditorRuntimeFields( row, runtimeRow, fields, dataRecordTypeId);
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
                    var runtimeField = new GenericEditorRuntimeField();
                    runtimeRow.Fields.Add(runtimeField);
                    runtimeField.FieldTitle = field.FieldTitle;
                    runtimeField.FieldPath = field.FieldPath;
                    var dataRecordTypeField = dataRecordTypeFields.FindRecord(itm => itm.Name == runtimeField.FieldPath);
                    if (dataRecordTypeField == null)
                        throw new NullReferenceException(String.Format("DataRecordType '{0}' dataRecordTypeField '{1}'", dataRecordTypeId, runtimeField.FieldPath));
                    runtimeField.FieldType = dataRecordTypeField.Type;
                }
            }
        }

        #endregion



       
    }
}