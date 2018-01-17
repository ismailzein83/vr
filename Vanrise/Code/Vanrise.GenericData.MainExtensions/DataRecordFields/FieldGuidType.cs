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

            IEnumerable<Guid> selectedGuidValues = FieldTypeHelper.ConvertFieldValueToList<Guid>(value);

            if (selectedGuidValues == null)
                return value.ToString();

            var descriptions = new List<string>();

            foreach (Guid selectedGuidValue in selectedGuidValues)
                descriptions.Add(selectedGuidValue.ToString());

            return String.Join(",", descriptions);
        }

        public override bool IsMatched(object fieldValue, object filterValue)
        {
            IEnumerable<Guid> guidValues = FieldTypeHelper.ConvertFieldValueToList<Guid>(fieldValue);
            var parsedFieldValue = Guid.Parse(fieldValue.ToString());
            var parsedFilterValue = Guid.Parse(filterValue.ToString());
            return (guidValues != null) ? guidValues.Contains(parsedFilterValue) : parsedFieldValue.CompareTo(parsedFilterValue) == 0;
        }

        public override Vanrise.Entities.GridColumnAttribute GetGridColumnAttribute(FieldTypeGetGridColumnAttributeContext context)
        {
            return new Vanrise.Entities.GridColumnAttribute() { Type = "Guid", NumberPrecision = "NoDecimal", Field = context != null ? context.DescriptionFieldPath : null };
        }

        public override bool IsMatched(object fieldValue, RecordFilter recordFilter)
        {
            if (fieldValue == null)
                return false;
            GuidRecordFilter guidRecordFilter = recordFilter as GuidRecordFilter;
            guidRecordFilter.ThrowIfNull("guidRecordFilter");
            return guidRecordFilter.GuidValue == Guid.Parse(fieldValue.ToString());
        }

        public override RecordFilter ConvertToRecordFilter(string fieldName, List<Object> filterValues)
        {
            if (filterValues == null || filterValues.Count == 0)
                return null;

            var values = filterValues.Select(value => Guid.Parse(value.ToString())).ToList();
            List<RecordFilter> recordFilters = new List<RecordFilter>();

            foreach (var value in values)
            {
                recordFilters.Add(new GuidRecordFilter
                {
                    GuidValue = value,
                    FieldName = fieldName
                });
            }
            return recordFilters.Count > 1 ? new RecordFilterGroup { LogicalOperator = RecordQueryLogicalOperator.Or, Filters = recordFilters } : recordFilters.First();
        }
    }
}
