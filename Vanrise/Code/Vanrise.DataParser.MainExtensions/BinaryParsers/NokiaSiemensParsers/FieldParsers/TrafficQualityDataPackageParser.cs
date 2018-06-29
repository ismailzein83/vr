using System;
using Vanrise.DataParser.Business;
using Vanrise.DataParser.Entities;

namespace Vanrise.DataParser.MainExtensions.BinaryParsers.NokiaSiemensParsers.FieldParsers
{
    public class TrafficQualityDataPackageParser : BinaryFieldParserSettings
    {
        public override Guid ConfigId { get { return new Guid("5B97401A-8211-4E85-95AF-5C84D0CAA4E3"); } }
        public string CauseValueFieldName { get; set; }
        public string CodingStandardFieldName { get; set; }
        public string LocationFieldName { get; set; }

        public override void Execute(IBinaryFieldParserContext context)
        {
            byte[] causeValueAsBytes = new byte[2];
            Array.Copy(context.FieldValue, 0, causeValueAsBytes, 0, 2);
            int causeValue = ParserHelper.HexToInt32(ParserHelper.ByteArrayToString(causeValueAsBytes, true));

            char[] codingStandardLocationByteAsChars = Convert.ToString(context.FieldValue[2], 2).PadLeft(8, '0').ToCharArray();
            int codingStandardValue = Convert.ToInt32(string.Concat(codingStandardLocationByteAsChars[1], codingStandardLocationByteAsChars[2]), 2);
            int locationValue = Convert.ToInt32(string.Concat(codingStandardLocationByteAsChars[4], codingStandardLocationByteAsChars[5], codingStandardLocationByteAsChars[6], codingStandardLocationByteAsChars[7]), 2);

            context.Record.SetFieldValue(this.CauseValueFieldName, causeValue);
            context.Record.SetFieldValue(this.CodingStandardFieldName, codingStandardValue);
            context.Record.SetFieldValue(this.LocationFieldName, locationValue);
        }
    }
}