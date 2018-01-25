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
    public class FieldGuidType : DataRecordFieldType
    {
        public override Guid ConfigId { get { return new Guid("EBD22F77-6275-4194-8710-7BF3063DCB68"); } }

        public override string RuntimeEditor { get { return "vr-genericdata-fieldtype-guid-runtimeeditor"; } }

        public override DataRecordFieldOrderType OrderType
        {
            get
            {
                return DataRecordFieldOrderType.ByFieldDescription;
            }
        }
        public bool IsNullable { get; set; }
        public override bool TryGenerateUniqueIdentifier(out Guid? uniqueIdentifier)
        {
            uniqueIdentifier = Guid.NewGuid();
            return true;
        }
        public override Type GetRuntimeType()
        {
            var type = GetNonNullableRuntimeType();
            return (IsNullable) ? GetNullableType(type) : type;
        }

        public override Type GetNonNullableRuntimeType()
        {
            return typeof(Guid);
        }

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

            //IEnumerable<Guid> guidValues = FieldTypeHelper.ConvertFieldValueToList<Guid>(fieldValue);
            //var parsedFieldValue = Guid.Parse(fieldValue.ToString());
            //var parsedFilterValue = Guid.Parse(filterValue.ToString());
            //return (guidValues != null) ? guidValues.Contains(parsedFilterValue) : parsedFieldValue.CompareTo(parsedFilterValue) == 0;
        }

        public override Vanrise.Entities.GridColumnAttribute GetGridColumnAttribute(FieldTypeGetGridColumnAttributeContext context)
        {
            return new Vanrise.Entities.GridColumnAttribute() { Type = "Text", NumberPrecision = "NoDecimal", Field = context != null ? context.DescriptionFieldPath : null };
        }

        public override bool IsMatched(object fieldValue, RecordFilter recordFilter)
        {
            if (fieldValue == null)
                return false;
            StringRecordFilter stringRecordFilter = recordFilter as StringRecordFilter;
            if (stringRecordFilter == null)
                throw new NullReferenceException("stringRecordFilter");
            string valueAsString = fieldValue != null?fieldValue.ToString():null;
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

            //if (fieldValue == null)
            //    return false;
            //StringRecordFilter stringRecordFilter = recordFilter as StringRecordFilter;
            //stringRecordFilter.ThrowIfNull("guidRecordFilter");
            //return stringRecordFilter.Value.ToUpper() == fieldValue.ToString().ToUpper();
        }

        public override RecordFilter ConvertToRecordFilter(string fieldName, List<Object> filterValues)
        {
            if (filterValues == null || filterValues.Count == 0)
                return null;

            var values = filterValues.Select(value => value.ToString()).ToList();
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
    }
}
