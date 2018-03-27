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
    public class FieldBooleanType : DataRecordFieldType
    {
        public override Guid ConfigId { get { return new Guid("a77fad19-d044-40d8-9d04-6362b79b177b"); } }

        public override string RuntimeEditor { get { return "vr-genericdata-fieldtype-boolean-runtimeeditor"; } }

        public bool IsNullable { get; set; }
        public override string ViewerEditor { get { return "vr-genericdata-fieldtype-boolean-viewereditor"; } }

        public override Type GetRuntimeType()
        {
            var type = GetNonNullableRuntimeType();
            return (IsNullable) ? GetNullableType(type) : type;
        }

        public override Type GetNonNullableRuntimeType()
        {
            return typeof(bool);
        }
        public override bool AreEqual(Object newValue, Object oldValue)
        {
            if (newValue == null && oldValue == null)
                return true;

            if (newValue == null || oldValue == null)
                return false;

            bool newLongValue = (bool)newValue;
            bool oldLongValue = (bool)oldValue;
            return newLongValue.Equals(oldLongValue);
        }
        public override string GetDescription(Object value)
        {
            if (value == null)
                return null;

            IEnumerable<bool> selectedBooleanValues = FieldTypeHelper.ConvertFieldValueToList<bool>(value);

            if (selectedBooleanValues == null)
                return value.ToString();

            var descriptions = new List<string>();

            foreach (bool selectedBooleanValue in selectedBooleanValues)
                descriptions.Add(selectedBooleanValue.ToString());

            return String.Join(",", descriptions);
        }

        public override bool IsMatched(object fieldValue, object filterValue)
        {
            IEnumerable<bool> boolValues = FieldTypeHelper.ConvertFieldValueToList<bool>(fieldValue);
            return (boolValues != null) ? boolValues.Contains(Convert.ToBoolean(filterValue)) : Convert.ToBoolean(fieldValue).CompareTo(Convert.ToBoolean(filterValue)) == 0;
        }

        public override Vanrise.Entities.GridColumnAttribute GetGridColumnAttribute(FieldTypeGetGridColumnAttributeContext context)
        {
            return new Vanrise.Entities.GridColumnAttribute() { Type = "Boolean", NumberPrecision = "NoDecimal", Field = context != null ? context.DescriptionFieldPath : null };
        }

        public override bool IsMatched(object fieldValue, RecordFilter recordFilter)
        {
            if (fieldValue == null)
                return false;
            BooleanRecordFilter booleanRecordFilter = recordFilter as BooleanRecordFilter;
            if (booleanRecordFilter == null)
                throw new NullReferenceException("booleanRecordFilter");
            return booleanRecordFilter.IsTrue == (bool)fieldValue;
        }

        public override RecordFilter ConvertToRecordFilter(string fieldName, List<Object> filterValues)
        {
            if (filterValues == null || filterValues.Count == 0)
                return null;

            var values = filterValues.Select(value => Convert.ToBoolean(value)).ToList();
            List<RecordFilter> recordFilters = new List<RecordFilter>();

            foreach (var value in values)
            {
                recordFilters.Add(new BooleanRecordFilter
                {
                    IsTrue = value,
                    FieldName = fieldName
                });
            }
            return recordFilters.Count > 1 ? new RecordFilterGroup { LogicalOperator = RecordQueryLogicalOperator.Or, Filters = recordFilters } : recordFilters.First();
        }
        public override void GetValueByDescription(IDataRecordFieldTypeTryGetValueByDescriptionContext context)
        {

            if (context.FieldDescription == null)
                return;
            else
            {
                if (context.FieldDescription is bool)
                    context.FieldValue = (bool)context.FieldDescription;
                else
                {
                    bool result;
                    if (bool.TryParse(context.FieldDescription.ToString(), out result))
                        context.FieldValue = result;
                    else
                        context.ErrorMessage = "Error while parsing field of boolean type.";
                }
            }
        }
    }
}
