using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.Common;

namespace Vanrise.GenericData.Business
{
    public class GenericEditorManager
    {
        public GenericEditor GetEditor(int editorId)
        {
            return null;
        }

        public GenericEditorRuntime GetEditorRuntime(int editorId)
        {
            var genericEditor = GetEditor(editorId);
            if (genericEditor == null)
                return null;

            GenericEditorRuntime editorRuntime = new GenericEditorRuntime();
            BuildEditorRuntime(genericEditor, editorRuntime);
            return editorRuntime;
        }

        #region Private Methods

        private void BuildEditorRuntime(GenericEditor genericEditor, GenericEditorRuntime editorRuntime)
        {
            BuildEditorRuntimeSections(genericEditor, editorRuntime);
        }

        private void BuildEditorRuntimeSections(GenericEditor genericEditor, GenericEditorRuntime editorRuntime)
        {
            if (genericEditor.Sections != null)
            {
                editorRuntime.Sections = new List<GenericEditorRuntimeSection>();
                foreach (var section in genericEditor.Sections)
                {
                    var runtimeSection = new GenericEditorRuntimeSection();
                    editorRuntime.Sections.Add(runtimeSection);
                    BuildEditorRuntimeRows(genericEditor, section, runtimeSection);
                }
            }
        }

        private void BuildEditorRuntimeRows(GenericEditor genericEditor, GenericEditorSection section, GenericEditorRuntimeSection runtimeSection)
        {
            if (section.Rows != null)
            {
                var dataRecordTypeManager = new DataRecordTypeManager();
                var fields = dataRecordTypeManager.GetDataRecordTypeFields(genericEditor.DataRecordTypeId);
                if (fields == null)
                    throw new NullReferenceException(String.Format("fields of DataRecordType '{0}'", genericEditor.DataRecordTypeId));
                runtimeSection.Rows = new List<GenericEditorRuntimeRow>();
                foreach (var row in section.Rows)
                {
                    var runtimeRow = new GenericEditorRuntimeRow();
                    runtimeSection.Rows.Add(runtimeRow);
                    BuildEditorRuntimeFields(genericEditor, row, runtimeRow, fields);
                }
            }
        }

        private void BuildEditorRuntimeFields(GenericEditor genericEditor, GenericEditorRow row, GenericEditorRuntimeRow runtimeRow, List<DataRecordField> dataRecordTypeFields)
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
                        throw new NullReferenceException(String.Format("DataRecordType '{0}' dataRecordTypeField '{1}'", genericEditor.DataRecordTypeId, runtimeField.FieldPath));
                    runtimeField.FieldType = dataRecordTypeField.Type;
                }
            }
        }

        #endregion
    }
}