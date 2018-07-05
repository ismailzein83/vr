using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.MainExtensions.GenericRuleCriteriaFieldValues;

namespace Vanrise.GenericData.MainExtensions.DataRecordFields
{
    public class FieldTextType : DataRecordFieldType
    {
        public override Guid ConfigId { get { return new Guid("3f29315e-b542-43b8-9618-7de216cd9653"); } }

        public override string RuntimeEditor { get { return "vr-genericdata-fieldtype-text-runtimeeditor"; } }

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
        public override bool AreEqual(Object newValue, Object oldValue)
        {
            if (newValue == null && oldValue == null)
                return true;

            if (newValue == null || oldValue == null)
                return false;

            string newLongValue = (string)newValue;
            string oldLongValue = (string)oldValue;

            return newLongValue.Equals(oldLongValue);
        }
        
        Type _nonNullableRuntimeType = typeof(string);
        public override Type GetNonNullableRuntimeType()
        {
            return _nonNullableRuntimeType;
        }
        
        public override string ViewerEditor { get { return "vr-genericdata-fieldtype-text-viewereditor"; } }
        public override string GetDescription(Object value)
        {
            if (value == null)
                return null;

            IEnumerable<string> textValues = FieldTypeHelper.ConvertFieldValueToList<string>(value);

            if (textValues == null)
                return value.ToString();

            var descriptions = new List<string>();
            foreach (var textValue in textValues)
                descriptions.Add(textValue.ToString());
            return String.Join(",", descriptions);
        }

        public override bool IsMatched(object fieldValue, object filterValue)
        {
            if (fieldValue != null && filterValue != null)
            {
                var fieldValueObjList = fieldValue as List<object>;
                fieldValueObjList = (fieldValueObjList == null) ? new List<object>() { fieldValue } : fieldValueObjList;

                var fieldValueStringList = fieldValueObjList.MapRecords(itm => Convert.ToString(itm).ToUpper());
                var filterValueString = filterValue.ToString().ToUpper();

                foreach (var fieldValueStringItem in fieldValueStringList)
                {
                    if (fieldValueStringItem.Contains(filterValueString))
                        return true;
                }
                return false;
            }
            return true;
        }

        public override bool IsMatched(object fieldValue, RecordFilter recordFilter)
        {
            if (fieldValue == null)
                return false;
            StringRecordFilter stringRecordFilter = recordFilter as StringRecordFilter;
            if (stringRecordFilter == null)
                throw new NullReferenceException("stringRecordFilter");
            string valueAsString = fieldValue as string;
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

        public override RecordFilter ConvertToRecordFilter(string fieldName, List<Object> filterValues)
        {
            var values = filterValues.Select(x => x.ToString()).ToList();
            List<RecordFilter> recordFilters = new List<RecordFilter>();

            foreach (var value in values)
            {
                recordFilters.Add(new StringRecordFilter
                {
                    CompareOperator = StringRecordFilterOperator.Equals,
                    Value = value,
                    FieldName = fieldName
                });
            }
            return recordFilters.Count > 1 ? new RecordFilterGroup { LogicalOperator = RecordQueryLogicalOperator.Or, Filters = recordFilters } : recordFilters.First();
        }

        protected override dynamic ParseNonNullValueToFieldType(object originalValue)
        {
            return originalValue;
        }

        public override void GetValueByDescription(IGetValueByDescriptionContext context)
        {
            if (context.FieldDescription == null)
                return;
            context.FieldValue = context.FieldDescription.ToString().Trim();
        }

        public override string GetRuntimeTypeDescription()
        {
            return "Text";
        }
    }
}
