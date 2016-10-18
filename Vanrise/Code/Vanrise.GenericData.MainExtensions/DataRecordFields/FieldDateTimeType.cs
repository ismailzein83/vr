using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.MainExtensions.GenericRuleCriteriaFieldValues;

namespace Vanrise.GenericData.MainExtensions.DataRecordFields
{
    public class FieldDateTimeType : DataRecordFieldType
    {
        public override Guid ConfigId { get { return new Guid("b8712417-83ab-4d4b-9ee1-109d20ceb909"); } }

        #region Public Methods

        public FieldDateTimeDataType DataType { get; set; }

        public bool IsNullable { get; set; }

        public override Type GetRuntimeType()
        {
            var type = GetNonNullableRuntimeType();
            return (IsNullable) ? GetNullableType(type) : type;
        }

        public override Type GetNonNullableRuntimeType()
        {
            var attributeInfo = Utilities.GetEnumAttribute<FieldDateTimeDataType, FieldDateTimeDataTypeInfoAttribute>(this.DataType);
            if (attributeInfo == null)
                throw new NullReferenceException("FieldDateTimeDataTypeInfoAttribute");
            return attributeInfo.RuntimeType;
        }

        public override string GetDescription(object value)
        {
            if (value == null) { return null; }
            return (DataType == FieldDateTimeDataType.Time) ? GetTimeDescription(value) : GetDateTimeDescription(value);
        }

        public override bool IsMatched(object fieldValue, object filterValue)
        {
            return (DataType == FieldDateTimeDataType.Time) ? DoTimesMatch(fieldValue, filterValue) : DoDateTimesMatch(fieldValue, filterValue);
        }
        public override bool IsMatched(object fieldValue, RecordFilter recordFilter)
        {
            if (fieldValue == null)
                return false;
            DateTimeRecordFilter dateTimeRecordFilter = recordFilter as DateTimeRecordFilter;
            if (dateTimeRecordFilter == null)
                throw new NullReferenceException("dateTimeRecordFilter");
            DateTime valueAsDateTime = (DateTime)fieldValue;
            DateTime filterValue = dateTimeRecordFilter.Value;
            switch (dateTimeRecordFilter.CompareOperator)
            {
                case DateTimeRecordFilterOperator.Equals: return valueAsDateTime == filterValue;
                case DateTimeRecordFilterOperator.NotEquals: return valueAsDateTime == filterValue;
                case DateTimeRecordFilterOperator.Greater: return valueAsDateTime > filterValue;
                case DateTimeRecordFilterOperator.GreaterOrEquals: return valueAsDateTime >= filterValue;
                case DateTimeRecordFilterOperator.Less: return valueAsDateTime < filterValue;
                case DateTimeRecordFilterOperator.LessOrEquals: return valueAsDateTime <= filterValue;
            }
            return false;
        }
        #endregion

        #region Private Methods

        string GetTimeDescription(object fieldValue)
        {
            IEnumerable<Time> timeValues = FieldTypeHelper.ConvertFieldValueToList<Time>(fieldValue);

            if (timeValues == null)
            {
                Time time = fieldValue as Time;
                return time.ToLongTimeString();
            }

            var descriptions = new List<string>();

            foreach (Time timeValue in timeValues)
                descriptions.Add(timeValue.ToLongTimeString());

            return String.Join(",", descriptions);
        }

        string GetDateTimeDescription(object fieldValue)
        {
            IEnumerable<DateTime> dateTimeValues = FieldTypeHelper.ConvertFieldValueToList<DateTime>(fieldValue);

            if (dateTimeValues == null)
                return DateTimeValueToString(fieldValue);

            var descriptions = new List<string>();

            foreach (DateTime dateTimeValue in dateTimeValues)
                descriptions.Add(GetDateTimeDescription(dateTimeValue));

            return String.Join(",", descriptions);
        }

        string DateTimeValueToString(object value)
        {
            if (value == null)
                return null;
            switch(this.DataType)
            {
                case FieldDateTimeDataType.Date: return Convert.ToDateTime(value).ToString("yyyy-MM-dd");
                case FieldDateTimeDataType.DateTime: return Convert.ToDateTime(value).ToString("yyyy-MM-dd HH:mm:ss");
                case FieldDateTimeDataType.Time: return ((Vanrise.Entities.Time)value).ToShortTimeString();
            }
            return null;
        }
        
        bool DoDateTimesMatch(object fieldValue, object filterValue)
        {
            IEnumerable<DateTime> dateTimeValues = FieldTypeHelper.ConvertFieldValueToList<DateTime>(fieldValue);
            return (dateTimeValues != null) ? dateTimeValues.Contains(Convert.ToDateTime(filterValue)) : Convert.ToDateTime(fieldValue).CompareTo(Convert.ToDateTime(filterValue)) == 0;
        }

        bool DoTimesMatch(object fieldValue, object filterValue)
        {
            IEnumerable<Time> timeValues = FieldTypeHelper.ConvertFieldValueToList<Time>(fieldValue);
            var filterValueAsTime = filterValue as Time;
            if (timeValues != null) {
                foreach (Time timeValue in timeValues)
                {
                    if (timeValue.Equals(filterValueAsTime))
                        return true;
                }
                return false;
            }
            else
            {
                var fieldValueAsTime = fieldValue as Time;
                return fieldValueAsTime.Equals(filterValueAsTime);
            }
        }

        #endregion

        public override GridColumnAttribute GetGridColumnAttribute()
        {
            string type;
            switch (DataType)
            {
                case FieldDateTimeDataType.DateTime: type = "LongDatetime"; break;
                case FieldDateTimeDataType.Date: type = "Date"; break;
                default: type = "Datetime"; break;
            }
            return new Vanrise.Entities.GridColumnAttribute() { Type = type, NumberPrecision = "NoDecimal" };
        }

        public override RecordFilter ConvertToRecordFilter(string fieldName, List<Object> filterValues)
        {
            var values = filterValues.Select(value => Convert.ToDateTime(value)).ToList();
            RecordFilterGroup recordFilterGroup = new RecordFilterGroup
            {
                LogicalOperator = RecordQueryLogicalOperator.Or,
                Filters = new List<RecordFilter>(),
            };
            foreach (var value in values)
            {
                recordFilterGroup.Filters.Add(new DateTimeRecordFilter
                {
                    CompareOperator = DateTimeRecordFilterOperator.Equals,
                    Value = value,
                    FieldName = fieldName
                });
            }
            return recordFilterGroup;
        }

        public override string GetFilterDescription(RecordFilter filter)
        {
            DateTimeRecordFilter dateTimeRecordFilter = filter as DateTimeRecordFilter;
            return string.Format(" {0} {1} {2} ", dateTimeRecordFilter.FieldName, Utilities.GetEnumDescription(dateTimeRecordFilter.CompareOperator), GetDescription(dateTimeRecordFilter.Value));
        }
    }
    public enum FieldDateTimeDataType
    {
        [FieldDateTimeDataTypeInfo(RuntimeType = typeof(DateTime))]
        DateTime = 0,
        [FieldDateTimeDataTypeInfo(RuntimeType = typeof(Vanrise.Entities.Time))]
        Time = 1,
        [FieldDateTimeDataTypeInfo(RuntimeType = typeof(DateTime))]
        Date = 2
    }

    public class FieldDateTimeDataTypeInfoAttribute : Attribute
    {
        public Type RuntimeType { get; set; }
    }
}
