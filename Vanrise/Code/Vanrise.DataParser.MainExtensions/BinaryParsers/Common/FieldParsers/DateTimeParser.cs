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

            string yearValue = IsBCD ? ParserHelper.GetHexFromByte(context.FieldValue[YearIndex]) : ParserHelper.HexToInt32(ParserHelper.GetHexFromByte(context.FieldValue[YearIndex])).ToString();
            string monthValue = IsBCD ? ParserHelper.GetHexFromByte(context.FieldValue[MonthIndex]) : ParserHelper.HexToInt32(ParserHelper.GetHexFromByte(context.FieldValue[MonthIndex])).ToString();
            string dayValue = IsBCD ? ParserHelper.GetHexFromByte(context.FieldValue[DayIndex]) : ParserHelper.HexToInt32(ParserHelper.GetHexFromByte(context.FieldValue[DayIndex])).ToString();
            monthValue = monthValue.Length == 1 ? "0" + monthValue : monthValue;
            dayValue = dayValue.Length == 1 ? "0" + dayValue : dayValue;

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
                    TimeSpan timeSpan = new TimeSpan(
                                            ParserHelper.HexToInt32(ParserHelper.GetHexFromByte(context.FieldValue[HoursIndex]))
                                          , ParserHelper.HexToInt32(ParserHelper.GetHexFromByte(context.FieldValue[MinutesIndex]))
                                          , ParserHelper.HexToInt32(ParserHelper.GetHexFromByte(context.FieldValue[SecondsIndex])));
                    value = new DateTime(value.Year, value.Month, value.Day, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);
                    break;
                case DateTimeParsingType.DateTime:
                    dateParsed = DateTime.TryParseExact(string.Format("{0}/{1}/{2} {3}:{4}:{5}",
                                                          dayValue,
                                                          monthValue,
                                                          yearValue,
                                                          IsBCD ? ParserHelper.GetHexFromByte(context.FieldValue[HoursIndex]) : ParserHelper.HexToInt32(ParserHelper.GetHexFromByte(context.FieldValue[HoursIndex])).ToString(),
                                                          IsBCD ? ParserHelper.GetHexFromByte(context.FieldValue[MinutesIndex]) : ParserHelper.HexToInt32(ParserHelper.GetHexFromByte(context.FieldValue[MinutesIndex])).ToString(),
                                                          IsBCD ? ParserHelper.GetHexFromByte(context.FieldValue[SecondsIndex]) : ParserHelper.HexToInt32(ParserHelper.GetHexFromByte(context.FieldValue[SecondsIndex])).ToString()),
                                                          "dd/MM/yy HH:mm:ss",
                                                          System.Globalization.CultureInfo.InvariantCulture,
                                                          System.Globalization.DateTimeStyles.None,
                                                          out value);

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
