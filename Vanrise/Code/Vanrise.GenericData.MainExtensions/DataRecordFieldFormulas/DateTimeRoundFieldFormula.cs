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
                    return new Time() { Hour = ts.Hours, Minute = ts.Minutes };

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
                        dateTimeRecordFilter.FieldName = this.DateTimeFieldName;
                        return dateTimeRecordFilter;

                    case DateTimeRecordFilterComparisonPart.TimeOnly:
                        dateTimeRecordFilter.FieldName = this.DateTimeFieldName;
                        dateTimeRecordFilter.CompareOperator = DateTimeRecordFilterOperator.Between;
                        dateTimeRecordFilter.ExcludeValue2 = true;

                        var valueAsTime = (Vanrise.Entities.Time)dateTimeRecordFilter.Value;
                        int timeRoundingIntervalInMinutes = Vanrise.Common.Utilities.GetEnumAttribute<TimeRoundingInterval, TimeRoundingIntervalAttribute>(this.TimeRoundingInterval.Value).Value;
                        TimeSpan value2AsTimeSpan = new TimeSpan(valueAsTime.Hour, valueAsTime.Minute + timeRoundingIntervalInMinutes, valueAsTime.Second);

                        dateTimeRecordFilter.Value2 = new Time() { Hour = value2AsTimeSpan.Hours, Minute = value2AsTimeSpan.Minutes, Second = value2AsTimeSpan.Seconds, MilliSecond = value2AsTimeSpan.Milliseconds };
                        return dateTimeRecordFilter;

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
