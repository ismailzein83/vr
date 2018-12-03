using System;
using Vanrise.DataParser.Business;
using Vanrise.DataParser.Entities;

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
            string number = ParserHelper.GetBCDNumber(numberBytes, true, false);

            Byte[] reservedBytes = new Byte[ReservedBytesLength];
            Array.Copy(context.FieldValue, 4, reservedBytes, 0, ReservedBytesLength);
            string reserved = ParserHelper.ByteArrayToString(reservedBytes, false);

            string result;

            int indexOfFirstFInReserved = reserved.ToLower().IndexOf('f');
            if (indexOfFirstFInReserved == -1 || indexOfFirstFInReserved == 0)
                result = number;
            else
                result = number.Substring(indexOfFirstFInReserved);

            context.Record.SetFieldValue(this.FieldName, result);
        }
    }
}
