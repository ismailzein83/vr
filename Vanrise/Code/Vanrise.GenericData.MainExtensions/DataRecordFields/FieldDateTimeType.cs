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
    public enum FieldDateTimeDataType
    {
        [FieldDateTimeDataTypeInfo(RuntimeType = typeof(DateTime))]
        DateTime = 0,
        [FieldDateTimeDataTypeInfo(RuntimeType = typeof(Vanrise.Entities.Time))]
        Time = 1,
        [FieldDateTimeDataTypeInfo(RuntimeType = typeof(DateTime))]
        Date = 2
    }

    public class FieldDateTimeType : DataRecordFieldType
    {
        public override Guid ConfigId { get { return new Guid("b8712417-83ab-4d4b-9ee1-109d20ceb909"); } }

        public FieldDateTimeDataType DataType { get; set; }

        public bool IsNullable { get; set; }


        #region Public Methods

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

            DateTime filterValue = default(DateTime);
            DateTime filterValue2 = default(DateTime);

            bool hasSecondValue = Vanrise.Common.Utilities.GetEnumAttribute<DateTimeRecordFilterOperator, DateTimeRecordFilterOperatorAttribute>(dateTimeRecordFilter.CompareOperator).HasSecondValue;

            switch (dateTimeRecordFilter.ComparisonPart)
            {
                case DateTimeRecordFilterComparisonPart.DateTime:
                    filterValue = Convert.ToDateTime(dateTimeRecordFilter.Value);
                    if (hasSecondValue)
                        filterValue2 = Convert.ToDateTime(dateTimeRecordFilter.Value2);
                    break;

                case DateTimeRecordFilterComparisonPart.DateOnly:
                    valueAsDateTime = valueAsDateTime.Date;
                    filterValue = Convert.ToDateTime(dateTimeRecordFilter.Value).Date;
                    if (hasSecondValue)
                        filterValue2 = Convert.ToDateTime(dateTimeRecordFilter.Value2).Date;
                    break;

                case DateTimeRecordFilterComparisonPart.TimeOnly:
                    DateTime tempDateTime = DateTime.Now;

                    TimeSpan valueAsTimeSpan = valueAsDateTime.TimeOfDay;
                    valueAsDateTime = new DateTime(tempDateTime.Year, tempDateTime.Month, tempDateTime.Day, valueAsTimeSpan.Hours, valueAsTimeSpan.Minutes, valueAsTimeSpan.Seconds, valueAsTimeSpan.Milliseconds);

                    Time filterValueAsTime = dateTimeRecordFilter.Value as Time;
                    filterValue = Vanrise.Common.Utilities.AppendTimeToDateTime(filterValueAsTime, tempDateTime);
                    if (hasSecondValue)
                    {
                        Time filterValue2AsTime = dateTimeRecordFilter.Value2 as Time;
                        filterValue2 = Vanrise.Common.Utilities.AppendTimeToDateTime(filterValue2AsTime, tempDateTime);
                    }
                    break;
            }

            switch (dateTimeRecordFilter.CompareOperator)
            {
                case DateTimeRecordFilterOperator.Equals: return valueAsDateTime == filterValue;
                case DateTimeRecordFilterOperator.NotEquals: return valueAsDateTime != filterValue;
                case DateTimeRecordFilterOperator.Greater: return valueAsDateTime > filterValue;
                case DateTimeRecordFilterOperator.GreaterOrEquals: return valueAsDateTime >= filterValue;
                case DateTimeRecordFilterOperator.Less: return valueAsDateTime < filterValue;
                case DateTimeRecordFilterOperator.LessOrEquals: return valueAsDateTime <= filterValue;
                case DateTimeRecordFilterOperator.Between:
                    if (dateTimeRecordFilter.ExcludeValue2)
                        return valueAsDateTime >= filterValue && valueAsDateTime < filterValue2;
                    else
                        return valueAsDateTime >= filterValue && valueAsDateTime <= filterValue2;
            }

            return false;
        }

        public override void SetExcelCellType(IDataRecordFieldTypeSetExcelCellTypeContext context)
        {
            context.HeaderCell.ThrowIfNull("context.HeaderCell");
            var headerCell = context.HeaderCell;
            headerCell.CellType = ExcelCellType.DateTime;
            switch (this.DataType)
            {
                case FieldDateTimeDataType.DateTime: headerCell.DateTimeType = DateTimeType.LongDateTime; break;
                case FieldDateTimeDataType.Date: headerCell.DateTimeType = DateTimeType.Date; break;
                default: headerCell.DateTimeType = DateTimeType.LongDateTime; break;
            }
        }

        public override GridColumnAttribute GetGridColumnAttribute(FieldTypeGetGridColumnAttributeContext context)
        {
            string type;
            switch (DataType)
            {
                case FieldDateTimeDataType.DateTime: type = "LongDatetime"; break;
                case FieldDateTimeDataType.Date: type = "Date"; break;
                default: type = "Datetime"; break;
            }
            return new Vanrise.Entities.GridColumnAttribute() { Type = type, NumberPrecision = "NoDecimal", Field = context != null ? context.ValueFieldPath : null };
        }

        public override RecordFilter ConvertToRecordFilter(string fieldName, List<Object> filterValues)
        {
            if (filterValues == null || filterValues.Count == 0)
                return null;

            List<RecordFilter> recordFilters = null;

            switch (this.DataType)
            {
                case FieldDateTimeDataType.DateTime:
                    var dateTimeValues = filterValues.Select(value => Convert.ToDateTime(value)).ToList();
                    recordFilters = GetDateTimeRecordFilters(fieldName, dateTimeValues, DateTimeRecordFilterComparisonPart.DateTime);
                    break;

                case FieldDateTimeDataType.Date:
                    var dateValues = filterValues.Select(value => Convert.ToDateTime(value)).ToList();
                    recordFilters = GetDateTimeRecordFilters(fieldName, dateValues, DateTimeRecordFilterComparisonPart.DateOnly);
                    break;

                case FieldDateTimeDataType.Time:
                    var timeValues = filterValues.Select(value => new Time(value.ToString())).ToList();
                    recordFilters = GetDateTimeRecordFilters(fieldName, timeValues, DateTimeRecordFilterComparisonPart.TimeOnly);
                    break;

                default: throw new NotSupportedException(string.Format("fieldDateTimeDataType '{0}'", this.DataType));
            }

            return recordFilters.Count > 1 ? new RecordFilterGroup { LogicalOperator = RecordQueryLogicalOperator.Or, Filters = recordFilters } : recordFilters.First();
        }

        private List<RecordFilter> GetDateTimeRecordFilters<T>(string fieldName, List<T> filterValues, DateTimeRecordFilterComparisonPart comparisonPart)
        {
            List<RecordFilter> recordFilters = new List<RecordFilter>();

            foreach (var value in filterValues)
            {
                recordFilters.Add(new DateTimeRecordFilter
                {
                    CompareOperator = DateTimeRecordFilterOperator.Equals,
                    ComparisonPart = comparisonPart,
                    FieldName = fieldName,
                    Value = value
                });
            };

            return recordFilters;
        }

        #endregion

        #region Private Methods

        string GetTimeDescription(object fieldValue)
        {
            IEnumerable<Time> timeValues = FieldTypeHelper.ConvertFieldValueToList<Time>(fieldValue);

            if (timeValues == null)
            {
                Time time;
                if (fieldValue is TimeSpan)
                {
                    TimeSpan ts = TimeSpan.Parse(fieldValue.ToString());
                    time = new Time() { Hour = ts.Hours, MilliSecond = ts.Milliseconds, Minute = ts.Minutes, Second = ts.Seconds };
                    return time.ToLongTimeString();
                }

                time = fieldValue as Time;
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
            switch (this.DataType)
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
            if (timeValues != null)
            {
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
    }

    public class FieldDateTimeDataTypeInfoAttribute : Attribute
    {
        public Type RuntimeType { get; set; }
    }
}
