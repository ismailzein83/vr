using System;
using Vanrise.DataParser.Business;
using Vanrise.DataParser.Entities;

namespace Vanrise.DataParser.MainExtensions.BinaryParsers.Common.FieldParsers
{
    public class DateTimeParser : BinaryFieldParserSettings
    {
        public override Guid ConfigId { get { return new Guid("F95D8834-4A38-4197-A6C7-D3D4BCD1B0FD"); } }
        public string FieldName { get; set; }
        public DateTimeParsingType DateTimeParsingType { get; set; }
        public bool WithOffset { get; set; }
        public int DayIndex { get; set; }
        public int MonthIndex { get; set; }
        public int YearIndex { get; set; }
        public int SecondsIndex { get; set; }
        public int MinutesIndex { get; set; }
        public int HoursIndex { get; set; }
        public int HoursTimeShiftIndex { get; set; }
        public int MinutesTimeShiftIndex { get; set; }
        public int TimeShiftIndicatorIndex { get; set; }
        public bool IsBCD { get; set; }

        public override void Execute(IBinaryFieldParserContext context)
        {
            Object recordValue = context.Record.GetFieldValue(FieldName);
            DateTime value = recordValue == null ? default(DateTime) : (DateTime)recordValue;

            string yearValue;
            string monthValue;
            string dayValue;

            if (IsBCD)
            {
                yearValue = ParserHelper.GetHexFromByte(context.FieldValue[YearIndex]);
                monthValue = ParserHelper.GetHexFromByte(context.FieldValue[MonthIndex]);
                dayValue = ParserHelper.GetHexFromByte(context.FieldValue[DayIndex]);
            }
            else
            {
                yearValue = ParserHelper.GetIntWithLeftPadding(context.FieldValue, YearIndex, 1, 2);
                monthValue = ParserHelper.GetIntWithLeftPadding(context.FieldValue, MonthIndex, 1, 2);
                dayValue = ParserHelper.GetIntWithLeftPadding(context.FieldValue, DayIndex, 1, 2);
            }

            bool dateParsed = false;
            switch (DateTimeParsingType)
            {
                case DateTimeParsingType.Date:
                    dateParsed = DateTime.TryParseExact(string.Format("{0}/{1}/{2}", dayValue, monthValue, yearValue),
                                                        "dd/MM/yy",
                                                        System.Globalization.CultureInfo.InvariantCulture,
                                                        System.Globalization.DateTimeStyles.None,
                                                        out value);
                    break;

                case DateTimeParsingType.Time:
                    int hoursValue;
                    int minutesValue;
                    int secondsValue;

                    if (IsBCD)
                    {
                        hoursValue = int.Parse(ParserHelper.GetHexFromByte(context.FieldValue[HoursIndex]));
                        minutesValue = int.Parse(ParserHelper.GetHexFromByte(context.FieldValue[MinutesIndex]));
                        secondsValue = int.Parse(ParserHelper.GetHexFromByte(context.FieldValue[SecondsIndex]));
                    }
                    else
                    {
                        hoursValue = ParserHelper.GetInt(context.FieldValue, HoursIndex, 1);
                        minutesValue = ParserHelper.GetInt(context.FieldValue, MinutesIndex, 1);
                        secondsValue = ParserHelper.GetInt(context.FieldValue, SecondsIndex, 1);
                    }

                    TimeSpan timeSpan = new TimeSpan(hoursValue, minutesValue, secondsValue);
                    value = new DateTime(value.Year, value.Month, value.Day, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);
                    break;

                case DateTimeParsingType.DateTime:
                    string hoursValueAsString;
                    string minutesValueAsString;
                    string secondsValueAsString;

                    if (IsBCD)
                    {
                        hoursValueAsString = ParserHelper.GetHexFromByte(context.FieldValue[HoursIndex]);
                        minutesValueAsString = ParserHelper.GetHexFromByte(context.FieldValue[MinutesIndex]);
                        secondsValueAsString = ParserHelper.GetHexFromByte(context.FieldValue[SecondsIndex]);
                    }
                    else
                    {
                        hoursValueAsString = ParserHelper.GetIntWithLeftPadding(context.FieldValue, HoursIndex, 1, 2);
                        minutesValueAsString = ParserHelper.GetIntWithLeftPadding(context.FieldValue, MinutesIndex, 1, 2);
                        secondsValueAsString = ParserHelper.GetIntWithLeftPadding(context.FieldValue, SecondsIndex, 1, 2);
                    }

                    string datetimeAsString = string.Format("{0}/{1}/{2} {3}:{4}:{5}", dayValue, monthValue, yearValue, hoursValueAsString, minutesValueAsString, secondsValueAsString);

                    dateParsed = DateTime.TryParseExact(datetimeAsString, "dd/MM/yy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture,
                        System.Globalization.DateTimeStyles.None, out value);

                    //Removed As discussed with Sari, date should be saved without time shift from mediation
                    //if (dateParsed && WithOffset)
                    //{
                    //    int offset = (int)context.FieldValue[TimeShiftIndicatorIndex] == 43 ? 1 : -1;
                    //    DateTimeOffset offsetDateTime = new DateTimeOffset(value, new TimeSpan(context.FieldValue[HoursTimeShiftIndex] * offset, context.FieldValue[MinutesTimeShiftIndex] * offset, 0));
                    //    value = offsetDateTime.ToLocalTime().DateTime;
                    //}

                    break;
            }

            context.Record.SetFieldValue(this.FieldName, value);
        }
    }
}