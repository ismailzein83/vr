using System;
using Vanrise.DataParser.Business;
using Vanrise.DataParser.Entities;

namespace Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers
{
    public class PartyNumberPackageParser : HexTLVFieldParserSettings
    {
        public override Guid ConfigId { get { return new Guid("D15F9394-E33A-4E13-BEC2-77670B650B81"); } }
        public string PartyNumberFieldName { get; set; }

        public override void Execute(IHexTLVFieldParserContext context)
        {
            string partyNumber;

            int numberOfDigits = ParserHelper.HexToInt32(ParserHelper.GetHexFromByte(context.FieldValue[2]));
            int digitBytesLength = context.FieldValue.Length - 3;

            byte[] digitsBytes = new byte[digitBytesLength];
            Array.Copy(context.FieldValue, 3, digitsBytes, 0, digitBytesLength);

            string digits = ParserHelper.ByteArrayToString(digitsBytes, false);
            partyNumber = numberOfDigits % 2 != 0 ? digits.Remove(digits.Length - 1, 1) : digits;

            context.Record.SetFieldValue(this.PartyNumberFieldName, partyNumber);
        }
    }
}