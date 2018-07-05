using System;
using System.Collections.Generic;
using System.Globalization;
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
        Date = 2,
        [FieldDateTimeDataTypeInfo(RuntimeType = typeof(DateTime))]
        YearMonth = 3,
        [FieldDateTimeDataTypeInfo(RuntimeType = typeof(DateTime))]
        YearWeek = 4,
        [FieldDateTimeDataTypeInfo(RuntimeType = typeof(Vanrise.Entities.Time))]
        Hour = 5
    }

    public class FieldDateTimeType : DataRecordFieldType
    {
        public override Guid ConfigId { get { return new Guid("b8712417-83ab-4d4b-9ee1-109d20ceb909"); } }

        public override string RuntimeEditor { get { return "vr-genericdata-fieldtype-datetime-runtimeeditor"; } }

        public override string ViewerEditor { get { return "vr-genericdata-fieldtype-datetime-viewereditor"; } }

        public FieldDateTimeDataType DataType { get; set; }

        public bool IsNullable { get; set; }


        #region Public Methods

        public override bool AreEqual(Object newValue, Object oldValue)
        {
            if (newValue == null && oldValue == null)
                return true;
            if (newValue == null || oldValue == null)
                return false;
            return DateTime.Parse(newValue.ToString()) == DateTime.Parse(oldValue.ToString());
        }

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
                        var attributeInfo = Utilities.GetEnumAttribute<FieldDateTimeDataType, FieldDateTimeDataTypeInfoAttribute>(this.DataType);
                        if (attributeInfo == null)
                            throw new NullReferenceException("FieldDateTimeDataTypeInfoAttribute");
                        _nonNullableRuntimeType = attributeInfo.RuntimeType;
                    }
                }

            }
            return _nonNullableRuntimeType;
        }

        public override string GetDescription(object value)
        {
            if (value == null)
                return null;

            switch (this.DataType)
            {
                case FieldDateTimeDataType.DateTime:
                case FieldDateTimeDataType.Date:
                case FieldDateTimeDataType.YearMonth:
                    return GetDateTimeDescription(value);

                case FieldDateTimeDataType.Time:
                    return GetTimeDescription(value);

                case FieldDateTimeDataType.Hour:
                    return GetHourDescription(value);

                case FieldDateTimeDataType.YearWeek:
                    return GetYearWeekDescription(value);

                default: throw new NotSupportedException(string.Format("fieldDateTimeDataType '{0}'", this.DataType));
            }
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

            DateTime valueAsDateTime;

            DateTime filterValue;
            DateTime? filterValue2 = null;

            bool hasSecondValue = Vanrise.Common.Utilities.GetEnumAttribute<DateTimeRecordFilterOperator, DateTimeRecordFilterOperatorAttribute>(dateTimeRecordFilter.CompareOperator).HasSecondValue;
            int valueHour;
            int valueMinute;
            int valueSecond;
            int valueMillisecond;
                    
            switch (dateTimeRecordFilter.ComparisonPart)
            {
                #region DateTime

                case DateTimeRecordFilterComparisonPart.DateTime:
                    valueAsDateTime = (DateTime)fieldValue;
                    filterValue = Convert.ToDateTime(dateTimeRecordFilter.Value);
                    if (hasSecondValue)
                        filterValue2 = Convert.ToDateTime(dateTimeRecordFilter.Value2);
                    break;

                #endregion

                #region DateOnly

                case DateTimeRecordFilterComparisonPart.DateOnly:
                    valueAsDateTime = ((DateTime)fieldValue).Date;
                    filterValue = Convert.ToDateTime(dateTimeRecordFilter.Value).Date;
                    if (hasSecondValue)
                        filterValue2 = Convert.ToDateTime(dateTimeRecordFilter.Value2).Date;
                    break;

                #endregion

                #region TimeOnly

                case DateTimeRecordFilterComparisonPart.TimeOnly:
                    DateTime timeOnlyNowDateTime = DateTime.Now;

                    ParseTimeValue(fieldValue, out valueHour, out valueMinute, out valueSecond, out valueMillisecond);

                    valueAsDateTime = new DateTime(timeOnlyNowDateTime.Year, timeOnlyNowDateTime.Month, timeOnlyNowDateTime.Day, valueHour,
                        valueMinute, valueSecond, valueMillisecond);

                    Time timeOnlyFilterValueAsTime = dateTimeRecordFilter.Value as Time;
                    filterValue = Vanrise.Common.Utilities.AppendTimeToDateTime(timeOnlyFilterValueAsTime, timeOnlyNowDateTime);
                    if (hasSecondValue)
                    {
                        Time timeOnlyFilterValue2AsTime = dateTimeRecordFilter.Value2 as Time;
                        filterValue2 = Vanrise.Common.Utilities.AppendTimeToDateTime(timeOnlyFilterValue2AsTime, timeOnlyNowDateTime);
                    }
                    break;

                #endregion

                #region YearMonth

                case DateTimeRecordFilterComparisonPart.YearMonth:
                    DateTime fieldValueAsDate = (DateTime)fieldValue;
                    valueAsDateTime = new DateTime(fieldValueAsDate.Year, fieldValueAsDate.Month, 1);

                    DateTime filterValueAsDate = Convert.ToDateTime(dateTimeRecordFilter.Value);
                    filterValue = new DateTime(filterValueAsDate.Year, filterValueAsDate.Month, 1);
                    if (hasSecondValue)
                    {
                        DateTime filterValue2AsDate = Convert.ToDateTime(dateTimeRecordFilter.Value2);
                        filterValue2 = new DateTime(filterValue2AsDate.Year, filterValue2AsDate.Month, 1);
                    }
                    break;

                #endregion

                #region YearWeek

                case DateTimeRecordFilterComparisonPart.YearWeek:
                    valueAsDateTime = Vanrise.Common.Utilities.GetMonday((DateTime)fieldValue);

                    filterValue = Vanrise.Common.Utilities.GetMonday((DateTime)dateTimeRecordFilter.Value);
                    if (hasSecondValue)
                        filterValue2 = Vanrise.Common.Utilities.GetMonday((DateTime)dateTimeRecordFilter.Value2);
                    break;

                #endregion

                #region Hour

                case DateTimeRecordFilterComparisonPart.Hour:
                    DateTime hourNowDateTime = DateTime.Now;


                    
                    ParseTimeValue(fieldValue, out valueHour, out valueMinute, out valueSecond, out valueMillisecond);
                                        
                    valueAsDateTime = new DateTime(hourNowDateTime.Year, hourNowDateTime.Month, hourNowDateTime.Day, valueHour, 0, 0, 0);

                    Time hourFilterValueAsTime = dateTimeRecordFilter.Value as Time;
                    filterValue = Vanrise.Common.Utilities.AppendTimeToDateTime(hourFilterValueAsTime, hourNowDateTime);
                    if (hasSecondValue)
                    {
                        Time hourFilterValue2AsTime = dateTimeRecordFilter.Value2 as Time;
                        filterValue2 = Vanrise.Common.Utilities.AppendTimeToDateTime(hourFilterValue2AsTime, hourNowDateTime);
                    }
                    break;

                #endregion

                default: throw new NotSupportedException(string.Format("fieldDateTimeDataType '{0}'", this.DataType));
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
                    if (!filterValue2.HasValue)
                        throw new NullReferenceException("filterValue2");

                    if (dateTimeRecordFilter.ExcludeValue2)
                        return valueAsDateTime >= filterValue && valueAsDateTime < filterValue2.Value;
                    else
                        return valueAsDateTime >= filterValue && valueAsDateTime <= filterValue2.Value;

                case DateTimeRecordFilterOperator.NotBetween:
                    if (!filterValue2.HasValue)
                        throw new NullReferenceException("filterValue2");

                    if (dateTimeRecordFilter.ExcludeValue2)
                        return valueAsDateTime < filterValue || valueAsDateTime >= filterValue2.Value;
                    else
                        return valueAsDateTime < filterValue || valueAsDateTime > filterValue2.Value;
            }

            return false;
        }

        private void ParseTimeValue(Object value, out int hour, out int minute, out int second, out int millisecond)
        {
            value.ThrowIfNull("value");
            Vanrise.Entities.Time time = value as Vanrise.Entities.Time;
            if (time != null)
            {
                ParseTimeValue(time, out hour, out minute, out second, out millisecond);
                return;
            }
            else if (value is TimeSpan)
            {
                TimeSpan timeSpan = (TimeSpan)value;
                hour = timeSpan.Hours;
                minute = timeSpan.Minutes;
                second = timeSpan.Seconds;
                millisecond = timeSpan.Milliseconds;
                return;
            }
            else if (value is DateTime)
            {
                DateTime dateTime = (DateTime)value;
                hour = dateTime.Hour;
                minute = dateTime.Minute;
                second = dateTime.Second;
                millisecond = dateTime.Millisecond;
                return;
            }
            else
            {
                string timeString = value as string;
                if (timeString != null)
                {
                    ParseTimeValue(new Time(timeString), out hour, out minute, out second, out millisecond);
                    return;
                }
            }
            throw new Exception(String.Format("value is not of valid Time/TimeSpan type. value type is '{0}'", value.GetType()));
        }

        private static void ParseTimeValue(Vanrise.Entities.Time time, out int hour, out int minute, out int second, out int millisecond)
        {
            hour = time.Hour;
            minute = time.Minute;
            second = time.Second;
            millisecond = time.MilliSecond;
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
                case FieldDateTimeDataType.Time: type = "Text"; break;
                case FieldDateTimeDataType.YearMonth: type = "Yearmonth"; break;
                case FieldDateTimeDataType.YearWeek: type = "Text"; break;
                case FieldDateTimeDataType.Hour: type = "Text"; break;
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

                case FieldDateTimeDataType.YearMonth:
                    var yearMonthDateTimeValues = filterValues.Select(value => Convert.ToDateTime(value)).ToList();
                    recordFilters = GetDateTimeRecordFilters(fieldName, yearMonthDateTimeValues, DateTimeRecordFilterComparisonPart.YearMonth);
                    break;

                case FieldDateTimeDataType.YearWeek:
                    var yearWeekDateTimeValues = filterValues.Select(value => Convert.ToDateTime(value)).ToList();
                    recordFilters = GetDateTimeRecordFilters(fieldName, yearWeekDateTimeValues, DateTimeRecordFilterComparisonPart.YearWeek);
                    break;

                case FieldDateTimeDataType.Hour:
                    var hourValues = filterValues.Select(value => new Time(value.ToString())).ToList();
                    recordFilters = GetDateTimeRecordFilters(fieldName, hourValues, DateTimeRecordFilterComparisonPart.Hour);
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

        public override string GetRuntimeTypeDescription()
        {
            switch(this.DataType)
            {
                case FieldDateTimeDataType.Date: return "Date";
                case FieldDateTimeDataType.DateTime: return "DateTime";
                case FieldDateTimeDataType.Hour: return "Hour";
                case FieldDateTimeDataType.Time: return "Time";
                case FieldDateTimeDataType.YearMonth: return "YearMonth";
                case FieldDateTimeDataType.YearWeek: return "YearWeek";
                default: throw new NotSupportedException(string.Format("DataType {0}", this.DataType));
            }
        }

        #endregion

        #region Private Methods

        string GetYearWeekDescription(object fieldValue)
        {
            if (fieldValue is DateTime)
                return Vanrise.Common.Utilities.GetWeekOfYearDescription((DateTime)fieldValue);
            else
                throw new DataIntegrityValidationException("fieldDateTimeType.Value should be of type DateTime");
        }

        string GetHourDescription(object fieldValue)
        {
            int valueHour;
            int valueMinute;
            int valueSecond;
            int valueMillisecond;
            ParseTimeValue(fieldValue, out valueHour, out valueMinute, out valueSecond, out valueMillisecond);
            return valueHour.ToString();
        }

        string GetTimeDescription(object fieldValue)
        {
            IEnumerable<Time> timeValues = FieldTypeHelper.ConvertFieldValueToList<Time>(fieldValue);
            if (timeValues == null)
            {

                Time time = fieldValue as Time;
                if(time == null)
                {
                    int valueHour;
                    int valueMinute;
                    int valueSecond;
                    int valueMillisecond;
                    ParseTimeValue(fieldValue, out valueHour, out valueMinute, out valueSecond, out valueMillisecond);
                    time = new Time(valueHour, valueMinute, valueSecond, valueMillisecond);
                }
                return time.ToShortTimeString();
            }

            var descriptions = new List<string>();

            foreach (Time timeValue in timeValues)
                descriptions.Add(timeValue.ToShortTimeString());

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
                case FieldDateTimeDataType.DateTime: return Convert.ToDateTime(value).ToString("yyyy-MM-dd HH:mm:ss");
                case FieldDateTimeDataType.Date: return Convert.ToDateTime(value).ToString("yyyy-MM-dd");
                case FieldDateTimeDataType.Time: return ((Vanrise.Entities.Time)value).ToShortTimeString();
                case FieldDateTimeDataType.YearMonth: return Convert.ToDateTime(value).ToString("yyyy-MM");
                default: throw new NotSupportedException(string.Format("fieldDateTimeDataType '{0}'", this.DataType));
            }
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
        protected override dynamic ParseNonNullValueToFieldType(Object originalValue)
        {
            switch(this.DataType)
            {
                case FieldDateTimeDataType.Date:
                case FieldDateTimeDataType.DateTime:
                case FieldDateTimeDataType.YearMonth:
                case FieldDateTimeDataType.YearWeek:
                    if (originalValue is DateTime)
                        return (DateTime)originalValue;
                    else
                        return DateTime.Parse(originalValue.ToString());
                case FieldDateTimeDataType.Time:
                case FieldDateTimeDataType.Hour:
                    Time valueAsTime = originalValue as Time;
                    if (valueAsTime != null)
                        return valueAsTime;
                    else
                        return new Time(originalValue.ToString());                        
                default: throw new NotSupportedException(String.Format("DataType '{0}'", this.DataType.ToString()));
            }
            
        }

        public override void GetValueByDescription(IGetValueByDescriptionContext context)
        {

            if (context.FieldDescription == null)
                return;
            switch (this.DataType)
            {
                case FieldDateTimeDataType.DateTime:
                    if (context.FieldDescription is DateTime)
                    {
                        context.FieldValue = (DateTime)context.FieldDescription;
                    }
                    else
                    {
                        DateTime result;
                        if (DateTime.TryParse(context.FieldDescription.ToString(), out result))
                            context.FieldValue = result;
                    }
                    break;
                case FieldDateTimeDataType.Time:
                    if (context.FieldDescription is Time)
                    {
                        context.FieldValue = (Time)context.FieldDescription;
                    }
                    else if (context.FieldDescription is TimeSpan)
                    {
                        context.FieldValue = (TimeSpan)context.FieldDescription;

                    }
                    else
                    {
                        Time result = new Time(context.FieldDescription.ToString());
                        context.FieldValue = result;
                    }
                    break;
                case FieldDateTimeDataType.Date:
                    if (context.FieldDescription is DateTime)
                    {
                        context.FieldValue = (DateTime)context.FieldDescription;
                    }
                    else
                    {
                        DateTime result;
                        if (DateTime.TryParse(context.FieldDescription.ToString(), out result))
                            context.FieldValue = result;
                    }
                    break;
                case FieldDateTimeDataType.YearMonth:
                    if (context.FieldDescription is DateTime)
                    {
                        context.FieldValue = (DateTime)context.FieldDescription;
                    }
                    else
                    {
                        DateTime result;
                        if (DateTime.TryParse(context.FieldDescription.ToString(), out result))
                            context.FieldValue = result;
                    }
                    break;
                case FieldDateTimeDataType.YearWeek:
                    if (context.FieldDescription is DateTime)
                    {
                        context.FieldValue = (DateTime)context.FieldDescription;
                    }
                    else
                    {
                        DateTime result;
                        if (DateTime.TryParse(context.FieldDescription.ToString(), out result))
                            context.FieldValue = result;
                    }
                    break;
                case FieldDateTimeDataType.Hour:
                    if (context.FieldDescription is Time)
                    {
                        context.FieldValue = (Time)context.FieldDescription;
                    }
                    else if (context.FieldDescription is TimeSpan)
                    {
                        context.FieldValue = (TimeSpan)context.FieldDescription;
                    }
                    else
                    {
                        Time result = new Time(context.FieldDescription.ToString());
                        context.FieldValue = result;
                    }
                    break;
                default:
                    context.ErrorMessage = "Error while converting field of Datetime value:" + context.FieldDescription.ToString();
                    break;
            }
        }

        #endregion
    }

    public class FieldDateTimeDataTypeInfoAttribute : Attribute
    {
        public Type RuntimeType { get; set; }
    }
}
