using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions.DataRecordFieldFormulas
{
    public class DateTimeRoundFieldFormula : DataRecordFieldFormula
    {
        public override Guid ConfigId { get { return new Guid("9A9E268C-0DC3-4488-8F11-CBEFF8D70E1D"); } }

        public string DateTimeFieldName { get; set; }

        public DateTimeRecordFilterComparisonPart ComparisonPart { get; set; }

        public TimeRoundingInterval? TimeRoundingInterval { get; set; }

        public override List<string> GetDependentFields(IDataRecordFieldFormulaGetDependentFieldsContext context)
        {
            return new List<string>() { this.DateTimeFieldName };
        }

        public override dynamic CalculateValue(IDataRecordFieldFormulaCalculateValueContext context)
        {
            DateTime? dateTime = (DateTime?)context.GetFieldValue(this.DateTimeFieldName);
            if (!dateTime.HasValue)
                return null;

            switch (this.ComparisonPart)
            {
                case DateTimeRecordFilterComparisonPart.DateTime:
                    return dateTime.Value;

                case DateTimeRecordFilterComparisonPart.DateOnly:
                    return dateTime.Value.Date;

                case DateTimeRecordFilterComparisonPart.TimeOnly:
                    if (!TimeRoundingInterval.HasValue)
                        throw new NullReferenceException("TimeRoundingInterval");

                    int timeRoundingIntervalInMinutes = Vanrise.Common.Utilities.GetEnumAttribute<TimeRoundingInterval, TimeRoundingIntervalAttribute>(this.TimeRoundingInterval.Value).Value;
                    int minuteNumber = dateTime.Value.Hour * 60 + dateTime.Value.Minute;
                    int roundedMinuteNumber = (((int)minuteNumber / timeRoundingIntervalInMinutes) * timeRoundingIntervalInMinutes);
                    TimeSpan ts = TimeSpan.FromMinutes(roundedMinuteNumber);
                    return new Time(ts.Hours, ts.Minutes, 0, 0);

                case DateTimeRecordFilterComparisonPart.YearMonth:
                    return new DateTime(dateTime.Value.Year, dateTime.Value.Month, 1);

                case DateTimeRecordFilterComparisonPart.YearWeek:
                    return Vanrise.Common.Utilities.GetMonday(dateTime.Value);

                case DateTimeRecordFilterComparisonPart.Hour:
                    return new Time(dateTime.Value.Hour, 0, 0, 0);

                default: throw new NotSupportedException(string.Format("ComparisonPart '{0}'", this.ComparisonPart));
            }
        }

        public override RecordFilter ConvertFilter(IDataRecordFieldFormulaConvertFilterContext context)
        {
            if (context.InitialFilter == null)
                throw new ArgumentNullException("context.InitialFilter");

            DateTimeRecordFilter dateTimeRecordFilter = context.InitialFilter as DateTimeRecordFilter;
            if (dateTimeRecordFilter != null)
            {
                switch (this.ComparisonPart)
                {
                    case DateTimeRecordFilterComparisonPart.DateTime:
                    case DateTimeRecordFilterComparisonPart.DateOnly:
                    case DateTimeRecordFilterComparisonPart.YearMonth:
                    case DateTimeRecordFilterComparisonPart.YearWeek:
                    case DateTimeRecordFilterComparisonPart.Hour:
                        dateTimeRecordFilter.FieldName = this.DateTimeFieldName;
                        return dateTimeRecordFilter;

                    case DateTimeRecordFilterComparisonPart.TimeOnly:
                        dateTimeRecordFilter.FieldName = this.DateTimeFieldName;
                        return BuildTimeOnlyRecordFilter(dateTimeRecordFilter);

                    default: throw new NotSupportedException(string.Format("ComparisonPart '{0}'", this.ComparisonPart));
                }
            }

            EmptyRecordFilter emptyFilter = context.InitialFilter as EmptyRecordFilter;
            if (emptyFilter != null)
                return new EmptyRecordFilter() { FieldName = this.DateTimeFieldName };

            NonEmptyRecordFilter nonEmptyFilter = context.InitialFilter as NonEmptyRecordFilter;
            if (nonEmptyFilter != null)
                return new NonEmptyRecordFilter { FieldName = this.DateTimeFieldName };

            throw new Exception(String.Format("Invalid Record Filter '{0}'", context.InitialFilter.GetType()));
        }

        private RecordFilter BuildTimeOnlyRecordFilter(DateTimeRecordFilter dateTimeRecordFilter)
        {
            int timeRoundingIntervalInMinutes = Vanrise.Common.Utilities.GetEnumAttribute<TimeRoundingInterval, TimeRoundingIntervalAttribute>(this.TimeRoundingInterval.Value).Value;

            var valueAsTime = (Time)dateTimeRecordFilter.Value;
            if (!IsTimeRoundingValid(valueAsTime, timeRoundingIntervalInMinutes))
                return new AlwaysFalseRecordFilter();

            Time value2AsTime = null;
            bool hasSecondValue = Vanrise.Common.Utilities.GetEnumAttribute<DateTimeRecordFilterOperator, DateTimeRecordFilterOperatorAttribute>(dateTimeRecordFilter.CompareOperator).HasSecondValue;
            if (hasSecondValue)
            {
                value2AsTime = (Time)dateTimeRecordFilter.Value2;
                if (!IsTimeRoundingValid(value2AsTime, timeRoundingIntervalInMinutes))
                    return new AlwaysFalseRecordFilter();
            }

            bool isAlwaysFalseRecordFilter = false;
            switch (dateTimeRecordFilter.CompareOperator)
            { 
                case DateTimeRecordFilterOperator.Equals: AdjustDateTimeRecordFilter(dateTimeRecordFilter, valueAsTime, DateTimeRecordFilterOperator.Between, timeRoundingIntervalInMinutes); break;
                case DateTimeRecordFilterOperator.NotEquals: AdjustDateTimeRecordFilter(dateTimeRecordFilter, valueAsTime, DateTimeRecordFilterOperator.NotBetween, timeRoundingIntervalInMinutes); break;
                case DateTimeRecordFilterOperator.Between: AdjustDateTimeRecordFilter(dateTimeRecordFilter, value2AsTime, DateTimeRecordFilterOperator.Between, timeRoundingIntervalInMinutes); break;
                case DateTimeRecordFilterOperator.NotBetween: AdjustDateTimeRecordFilter(dateTimeRecordFilter, value2AsTime, DateTimeRecordFilterOperator.NotBetween, timeRoundingIntervalInMinutes); break;

                case DateTimeRecordFilterOperator.Greater:
                    TimeSpan greaterThanValueTimeSpan = new TimeSpan(0, valueAsTime.Hour, valueAsTime.Minute + timeRoundingIntervalInMinutes, valueAsTime.Second, valueAsTime.MilliSecond);
                    if (greaterThanValueTimeSpan.Days > 0)
                        isAlwaysFalseRecordFilter = true;
                    else
                        dateTimeRecordFilter.Value = new Time(greaterThanValueTimeSpan.Hours, greaterThanValueTimeSpan.Minutes, greaterThanValueTimeSpan.Seconds, greaterThanValueTimeSpan.Milliseconds);
                    break;

                case DateTimeRecordFilterOperator.GreaterOrEquals: break;

                case DateTimeRecordFilterOperator.Less:
                    TimeSpan greaterOrEqualValueTimeSpan = new TimeSpan(0, valueAsTime.Hour, valueAsTime.Minute, valueAsTime.Second, valueAsTime.MilliSecond);
                    if (greaterOrEqualValueTimeSpan.TotalMilliseconds == 0)
                        isAlwaysFalseRecordFilter = true;
                    break;

                case DateTimeRecordFilterOperator.LessOrEquals:
                    TimeSpan lessOrEqualValueTimeSpan = new TimeSpan(0, valueAsTime.Hour, valueAsTime.Minute + timeRoundingIntervalInMinutes, valueAsTime.Second, valueAsTime.MilliSecond);
                    if (lessOrEqualValueTimeSpan.Days > 0)
                        lessOrEqualValueTimeSpan = new TimeSpan(0, 23, 59, 59, 998);
                    dateTimeRecordFilter.Value = new Time(lessOrEqualValueTimeSpan.Hours, lessOrEqualValueTimeSpan.Minutes, lessOrEqualValueTimeSpan.Seconds, lessOrEqualValueTimeSpan.Milliseconds);
                    break;

                default: throw new NotSupportedException(string.Format("CompareOperator '{0}'", dateTimeRecordFilter.CompareOperator));
            }

            if (!isAlwaysFalseRecordFilter)
                return dateTimeRecordFilter;
            else
                return new AlwaysFalseRecordFilter();
        } 

        private bool IsTimeRoundingValid(Time time, int timeRoundingIntervalInMinutes)
        {
            TimeSpan timeSpan = new TimeSpan(0, time.Hour, time.Minute, time.Second, time.MilliSecond);
            decimal valueInMinutes = Convert.ToDecimal(timeSpan.TotalMinutes);

            if ((valueInMinutes % timeRoundingIntervalInMinutes) != 0)
                return false;

            return true;
        }

        private void AdjustDateTimeRecordFilter(DateTimeRecordFilter dateTimeRecordFilter, Time valueAsTime, DateTimeRecordFilterOperator compareOperator, int timeRoundingIntervalInMinutes)
        {
            dateTimeRecordFilter.CompareOperator = compareOperator;
            dateTimeRecordFilter.ExcludeValue2 = true;

            TimeSpan value2AsTimeSpan = new TimeSpan(0, valueAsTime.Hour, valueAsTime.Minute + timeRoundingIntervalInMinutes, valueAsTime.Second, valueAsTime.MilliSecond);
            if (value2AsTimeSpan.Days > 0)
            {
                value2AsTimeSpan = new TimeSpan(0, 23, 59, 59, 998);
                dateTimeRecordFilter.ExcludeValue2 = false;
            }

            dateTimeRecordFilter.Value2 = new Time(value2AsTimeSpan.Hours, value2AsTimeSpan.Minutes, value2AsTimeSpan.Seconds, value2AsTimeSpan.Milliseconds);
        }
    }

    public enum TimeRoundingInterval
    {
        [TimeRoundingIntervalAttribute(5)]
        FiveMinutes = 0,
        [TimeRoundingIntervalAttribute(10)]
        TenMinutes = 1,
        [TimeRoundingIntervalAttribute(15)]
        FifteenMinutes = 2,
        [TimeRoundingIntervalAttribute(30)]
        ThirtyMinutes = 3,
        [TimeRoundingIntervalAttribute(60)]
        OneHour = 4,
        [TimeRoundingIntervalAttribute(120)]
        TwoHours = 5,
        [TimeRoundingIntervalAttribute(360)]
        SixHours = 6,
        [TimeRoundingIntervalAttribute(720)]
        TwelveHours = 7
    }

    public class TimeRoundingIntervalAttribute : Attribute
    {
        public int Value { get; set; }

        public TimeRoundingIntervalAttribute(int value)
        {
            this.Value = value;
        }
    }
}
