using System;
using Vanrise.DataParser.Business;
using Vanrise.DataParser.Entities;
using Vanrise.Entities;

namespace Vanrise.DataParser.MainExtensions.BinaryParsers.AlcatelParsers.FieldParsers
{
    public class NumberFieldParser : BinaryFieldParserSettings
    {
        public override Guid ConfigId { get { return new Guid("D7CAEB35-5C3C-469A-904E-AF94F19952E0"); } }

        public string FieldName { get; set; }

        public int ReservedBytesLength { get; set; }

        public override void Execute(IBinaryFieldParserContext context)
        {
            Byte[] numberBytes = new Byte[4];
            Array.Copy(context.FieldValue, 0, numberBytes, 0, 4);
            string number = ParserHelper.ByteArrayToString(numberBytes, false);

            Byte[] reservedBytes = new Byte[ReservedBytesLength];
            Array.Copy(context.FieldValue, 4, reservedBytes, 0, ReservedBytesLength);
            string reserved = ParserHelper.ByteArrayToString(reservedBytes, false);

            string result;

            int indexOfFirstFInReserved = reserved.ToLower().IndexOf('f');
            if (indexOfFirstFInReserved == -1)
                result = string.Concat(number, reserved);
            else
                result = string.Concat(number, reserved.Substring(0, indexOfFirstFInReserved));

            if (result[0] == '0' && result[1] != '0')
                result = result.Substring(1);

            context.Record.SetFieldValue(this.FieldName, result);
        }
    }
}