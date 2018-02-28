using System;
using Vanrise.Entities;

namespace Vanrise.DataParser.MainExtensions.StringFieldParsers
{
    public class TimeFromTextParser : BaseStringParser
    {
        public override Guid ConfigId { get { return new Guid("3CA92CC0-EAA7-4AA4-A5FC-77EBE5D940B7"); } }
        public string FieldName { get; set; }
        public string TimeFormat { get; set; }

        public override void Execute(IBaseStringParserContext context)
        {
            if (string.IsNullOrEmpty(context.FieldValue))
                return;

            TimeSpan timeSpan;
            if (TimeSpan.TryParseExact(context.FieldValue, this.TimeFormat, System.Globalization.CultureInfo.InvariantCulture, out timeSpan))
                context.Record.SetFieldValue(FieldName, new Time(timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds));
        }
    }
}
