using System;
using Vanrise.DataParser.Business;
using Vanrise.DataParser.Entities;
using Vanrise.Entities;

namespace Vanrise.DataParser.MainExtensions.BinaryParsers.Common.FieldParsers
{
    public class TimeFieldParser : BinaryFieldParserSettings
    {
        public override Guid ConfigId { get { return new Guid("BAE38D63-10CD-487C-9D74-4C2125F18B30"); } }

        public string FieldName { get; set; }

        public override void Execute(IBinaryFieldParserContext context)
        {
            byte[] hoursbyte = new byte[1] { context.FieldValue[0] };
            byte[] minutesbyte = new byte[1] { context.FieldValue[1] };
            byte[] secondsbyte = new byte[1] { context.FieldValue[2] };

            int hours = Convert.ToInt32(ParserHelper.ByteArrayToString(hoursbyte, false));
            int minutes = Convert.ToInt32(ParserHelper.ByteArrayToString(minutesbyte, false));
            int seconds = Convert.ToInt32(ParserHelper.ByteArrayToString(secondsbyte, false));

            Time time = new Time(hours, minutes, seconds, 0);
            context.Record.SetFieldValue(FieldName, time);
        }
    }
}