using System;
using Vanrise.DataParser.Business;
using Vanrise.DataParser.Entities;

namespace Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers
{
    public class DateTimePackageParser : HexTLVFieldParserSettings
    {
        public override Guid ConfigId { get { return new Guid("B60D9CC3-3C8C-494F-A272-4CADBF47BAFE"); } }
        public string BeginDateTimeFieldName { get; set; }
        public string EndDateTimeFieldName { get; set; }
        public string DurationFieldName { get; set; }
        public int YearIndex { get; set; }
        public int MonthIndex { get; set; }
        public int DayIndex { get; set; }
        public int HoursIndex { get; set; }
        public int MinutesIndex { get; set; }
        public int SecondsIndex { get; set; }
        public int FlagsMillisecondsIndex { get; set; }
        public int DurationIndex { get; set; }

        public override void Execute(IHexTLVFieldParserContext context)
        {
            DateTime beginDateTime;
            DateTime endDateTime;

            int yearValue = ParserHelper.HexToInt32(ParserHelper.GetHexFromByte(context.FieldValue[YearIndex])) + 2000;
            int monthValue = ParserHelper.HexToInt32(ParserHelper.GetHexFromByte(context.FieldValue[MonthIndex]));
            int dayValue = ParserHelper.HexToInt32(ParserHelper.GetHexFromByte(context.FieldValue[DayIndex]));

            int hoursValue = ParserHelper.HexToInt32(ParserHelper.GetHexFromByte(context.FieldValue[HoursIndex]));
            int minutesValue = ParserHelper.HexToInt32(ParserHelper.GetHexFromByte(context.FieldValue[MinutesIndex]));
            int secondsValue = ParserHelper.HexToInt32(ParserHelper.GetHexFromByte(context.FieldValue[SecondsIndex]));

            char[] chars = Convert.ToString(context.FieldValue[FlagsMillisecondsIndex], 2).PadLeft(8, '0').ToCharArray();
            int millisecondsValue = Convert.ToInt32(string.Concat(chars[3], chars[4], chars[5], chars[6]), 2);

            byte[] durationBytes = new byte[3];
            Array.Copy(context.FieldValue, DurationIndex, durationBytes, 0, 3);

            int durationInSeconds = ParserHelper.ByteToNumber(durationBytes);

            char f6 = chars[1];

            DateTime parsedDateTime = new DateTime(yearValue, monthValue, dayValue, hoursValue, minutesValue, secondsValue, millisecondsValue);

            if (f6 == '0')
            {
                beginDateTime = parsedDateTime;
                endDateTime = beginDateTime.AddSeconds(durationInSeconds);
            }
            else
            {
                endDateTime = parsedDateTime;
                beginDateTime = endDateTime.AddSeconds(-1 * durationInSeconds);
            }

            context.Record.SetFieldValue(this.BeginDateTimeFieldName, beginDateTime);
            context.Record.SetFieldValue(this.EndDateTimeFieldName, endDateTime);
            context.Record.SetFieldValue(this.DurationFieldName, durationInSeconds);
        }
    }
}
