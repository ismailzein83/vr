using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Common;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions.DataRecordFields
{
    public class FieldCustomObjectType : DataRecordFieldType
    {
        public override Guid ConfigId { get { return new Guid("28411D23-EA66-47AC-A323-106BE0B9DA7E"); } }

        public override string ViewerEditor { get { return "vr-genericdata-fieldtype-customobject-viewereditor"; } }

        public bool IsNullable { get; set; }

        public FieldCustomObjectTypeSettings Settings { get; set; }

        public override string RuntimeEditor { get { return null; } }

        public override bool StoreValueSerialized { get { return true; } }

        Type _runtimeType;
        public override Type GetRuntimeType()
        {
            if (_runtimeType == null)
            {
                var type = GetNonNullableRuntimeType();
                lock (this)
                {
                    if (_runtimeType == null)
                        _runtimeType = (IsNullable) ? GetNullableType(type) : type;
                }
            }
            return _runtimeType;
        }

        Type _nonNullableRuntimeType;
        public override Type GetNonNullableRuntimeType()
        {
            if (_nonNullableRuntimeType == null)
            {
                lock (this)
                {
                    if (_nonNullableRuntimeType == null)
                    {
                        this.Settings.ThrowIfNull("Settings");
                        _nonNullableRuntimeType = this.Settings.GetNonNullableRuntimeType();
                    }
                }

            }
            return _nonNullableRuntimeType;
        }

        public override bool AreEqual(Object newValue, Object oldValue)
        {
            if (this.Settings != null)
                return Settings.AreEqual(newValue, oldValue);
            return base.AreEqual(newValue, oldValue);
        }

        public override string GetDescription(object value)
        {
            if (this.Settings != null)
                return this.Settings.GetDescription(new FieldCustomObjectTypeSettingsContext
                {
                    FieldValue = value
                });
            return null;
        }

        public override bool IsMatched(object fieldValue, object filterValue)
        {
            throw new NotImplementedException();
        }

        public override bool IsMatched(object fieldValue, RecordFilter recordFilter)
        {
			if (fieldValue == null)
				return false;
			StringRecordFilter stringRecordFilter = recordFilter as StringRecordFilter;
			if (stringRecordFilter == null)
				throw new NullReferenceException("stringRecordFilter");
			string valueAsString = GetDescription(fieldValue) as string;
			if (valueAsString == null)
				throw new NullReferenceException("valueAsString");
			string filterValue = stringRecordFilter.Value;
			if (filterValue == null)
				throw new NullReferenceException("filterValue");
			switch (stringRecordFilter.CompareOperator)
			{
				case StringRecordFilterOperator.Equals: return String.Compare(valueAsString, filterValue, true) == 0;
				case StringRecordFilterOperator.NotEquals: return String.Compare(valueAsString, filterValue, true) != 0;
				case StringRecordFilterOperator.Contains: return valueAsString.IndexOf(filterValue, StringComparison.OrdinalIgnoreCase) >= 0;
				case StringRecordFilterOperator.NotContains: return valueAsString.IndexOf(filterValue, StringComparison.OrdinalIgnoreCase) < 0;
				case StringRecordFilterOperator.StartsWith: return valueAsString.StartsWith(filterValue, StringComparison.InvariantCultureIgnoreCase);
				case StringRecordFilterOperator.NotStartsWith: return !valueAsString.StartsWith(filterValue, StringComparison.InvariantCultureIgnoreCase);
				case StringRecordFilterOperator.EndsWith: return valueAsString.EndsWith(filterValue, StringComparison.InvariantCultureIgnoreCase);
				case StringRecordFilterOperator.NotEndsWith: return !valueAsString.EndsWith(filterValue, StringComparison.InvariantCultureIgnoreCase);
			}
			return false;
		}

        public override Vanrise.Entities.GridColumnAttribute GetGridColumnAttribute(FieldTypeGetGridColumnAttributeContext context)
        {
            return new Vanrise.Entities.GridColumnAttribute() { Type = "Text", NumberPrecision = "NoDecimal", Field = context != null ? context.DescriptionFieldPath : null };
        }

        public override RecordFilter ConvertToRecordFilter(IDataRecordFieldTypeConvertToRecordFilterContext context)
        {
			if (context.FilterValues == null || context.FilterValues.Count == 0)
				return null;

			var values = context.FilterValues.Select(x => x.ToString()).ToList();
			List<RecordFilter> recordFilters = new List<RecordFilter>();

			foreach (var value in values)
			{
				recordFilters.Add(new StringRecordFilter
				{
					CompareOperator = context.StrictEqual ? StringRecordFilterOperator.Equals : StringRecordFilterOperator.Contains,
					Value = value,
					FieldName = context.FieldName
				});
			}
			return recordFilters.Count > 1 ? new RecordFilterGroup { LogicalOperator = RecordQueryLogicalOperator.Or, Filters = recordFilters } : recordFilters.First();
		}

        protected override dynamic ParseNonNullValueToFieldType(Object originalValue)
        {
            return this.Settings.ParseNonNullValueToFieldType(originalValue);
        }

        public override string GetRuntimeTypeDescription()
        {
            return Settings.GetRuntimeTypeDescription();
        }
    }
}