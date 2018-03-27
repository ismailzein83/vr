using System;
using System.Linq;
using System.Collections.Generic;
using Vanrise.GenericData.Entities;
using Vanrise.Common;

namespace Vanrise.GenericData.MainExtensions.DataRecordFields
{
    public class FieldDictionaryType : DataRecordFieldType
    {
        public override Guid ConfigId { get { return new Guid("414C5C8D-48AD-4343-ABDA-4CD34D570C53"); } }

        public override string RuntimeEditor { get { return "vr-genericdata-fieldtype-dictionary-runtimeeditor"; } }

        public override DataRecordFieldOrderType OrderType
        {
            get
            {
                return DataRecordFieldOrderType.ByFieldDescription;
            }
        }
        public override Type GetRuntimeType()
        {
            return GetNonNullableRuntimeType();
        }

        Type _nonNullableRuntimeType = typeof(Dictionary<string, string>);
        public override Type GetNonNullableRuntimeType()
        {
            return _nonNullableRuntimeType;
        }

        public override string GetDescription(Object value)
        {
            if (value == null)
                return null;

            Dictionary<string, string> valueAsDict = value as Dictionary<string, string>;

            if (valueAsDict == null)
                return value.ToString();

            var descriptions = new List<string>();
            foreach (var item in valueAsDict)
                descriptions.Add(string.Format("{0}:{1}", item.Key, item.Value));
            return String.Join(",", descriptions);
        }

        public override bool IsMatched(object fieldValue, object filterValue)
        {
            if (fieldValue != null && filterValue != null)
            {
                string valueAsString = GetDescription(fieldValue);
                if (valueAsString != null && valueAsString.IndexOf(filterValue.ToString(), StringComparison.OrdinalIgnoreCase) >= 0)
                    return true;
                else
                    return false;
            }
            return true;
        }

        public override bool IsMatched(object fieldValue, RecordFilter recordFilter)
        {
            if (fieldValue == null)
                return false;

            string valueAsString = GetDescription(fieldValue);
            StringRecordFilter stringRecordFilter = recordFilter as StringRecordFilter;
            if (stringRecordFilter == null)
                throw new NullReferenceException("stringRecordFilter");

            string filterValue = stringRecordFilter.Value;
            if (filterValue == null)
                throw new NullReferenceException("filterValue");

            return valueAsString != null && valueAsString.IndexOf(filterValue, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public override Vanrise.Entities.GridColumnAttribute GetGridColumnAttribute(FieldTypeGetGridColumnAttributeContext context)
        {
            return new Vanrise.Entities.GridColumnAttribute() { Type = "Text", Field = context != null ? context.DescriptionFieldPath : null };
        }

        public override RecordFilter ConvertToRecordFilter(string fieldName, List<Object> filterValues)
        {
            var values = filterValues.Select(x => x.ToString()).ToList();
            List<RecordFilter> recordFilters = new List<RecordFilter>();

            foreach (var value in values)
            {
                recordFilters.Add(new StringRecordFilter
                {
                    CompareOperator = StringRecordFilterOperator.Contains,
                    Value = value,
                    FieldName = fieldName
                });
            }
            return recordFilters.Count > 1 ? new RecordFilterGroup { LogicalOperator = RecordQueryLogicalOperator.Or, Filters = recordFilters } : recordFilters.First();
        }

        public override bool StoreValueSerialized { get { return true; } }

        public override string SerializeValue(ISerializeDataRecordFieldValueContext context)
        {
            context.ThrowIfNull("context");
            if (context.Object == null)
                return string.Empty;

            return Vanrise.Common.Serializer.Serialize(context.Object, true);
        }

        public override object DeserializeValue(IDeserializeDataRecordFieldValueContext context)
        {
            context.ThrowIfNull("context");
            if (string.IsNullOrEmpty(context.Value))
                return null;

            return Vanrise.Common.Serializer.Deserialize<Dictionary<string, string>>(context.Value);
        }

        public override string DetailViewerEditor { get { return "vr-genericdata-datarecordfield-dictionarydetailviewer"; } }

        protected override dynamic ParseNonNullValueToFieldType(Object originalValue)
        {
            return originalValue;
        }
    }
}