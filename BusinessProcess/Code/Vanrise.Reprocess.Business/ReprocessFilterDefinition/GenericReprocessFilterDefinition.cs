using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Reprocess.Entities;
using Vanrise.Common;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.Business;

namespace Vanrise.Reprocess.Business
{
    public class GenericReprocessFilterDefinition : ReprocessFilterDefinition
    {
        public override Guid ConfigId { get { return new Guid("7C8426D8-B447-4574-BF5D-479FCE1A3FA1"); } }

        public List<GenericReprocessFilterFieldDefinition> Fields { get; set; }

        public override string RuntimeEditor { get { return "reprocess-reprocessfilterdefinition-runtime-genericfilter"; } }

        public override RecordFilterGroup GetFilterGroup(IReprocessFilterGetFilterGroupContext context)
        {
            Fields.ThrowIfNull("Fields");

            if (!context.TargetDataRecordTypeId.HasValue)
                throw new NullReferenceException("context.TargetDataRecordTypeId");

            Guid recordTypeId = context.TargetDataRecordTypeId.Value;

            GenericReprocessFilter genericReprocessFilter = context.ReprocessFilter.CastWithValidate<GenericReprocessFilter>("context.ReprocessFilter");
            if (genericReprocessFilter.Fields == null || genericReprocessFilter.Fields.Count == 0)
                return null;

            Dictionary<string, string> mappings = MappingFields != null ? MappingFields.GetRecord(recordTypeId) : null;

            Dictionary<string, GenericReprocessFilterFieldDefinition> fields = Fields.ToDictionary(itm => itm.FieldName, itm => itm);

            RecordFilterGroup recordFilterGroup = new RecordFilterGroup() { Filters = new List<RecordFilter>(), LogicalOperator = genericReprocessFilter.LogicalOperator };
            foreach (var fieldKvp in genericReprocessFilter.Fields)
            {
                string fieldName = fieldKvp.Key;
                List<object> fieldValues = fieldKvp.Value;
                if (fieldValues != null && fieldValues.Count > 0)
                {
                    string fieldMappingName = fieldName;
                    if (mappings != null)
                    {
                        string mappedFieldName = mappings.GetRecord(fieldName);
                        if (!string.IsNullOrEmpty(mappedFieldName))
                            fieldMappingName = mappedFieldName;
                    }

                    GenericReprocessFilterFieldDefinition filterField = fields.GetRecord(fieldName);
                    RecordFilter recordFilter = filterField.FieldType.ConvertToRecordFilter(new DataRecordFieldTypeConvertToRecordFilterContext { FieldName = fieldMappingName, FilterValues = fieldValues, StrictEqual = true });
                    recordFilterGroup.Filters.Add(recordFilter);
                }
            }

            return recordFilterGroup.Filters.Count > 0 ? new DataRecordStorageManager().ConvertFilterGroup(recordFilterGroup, recordTypeId) : null;
        }
    }
}
