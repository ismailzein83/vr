using System;
using Vanrise.DataParser.Business;
using Vanrise.DataParser.Entities;

namespace Vanrise.DataParser.MainExtensions.BinaryParsers.NokiaSiemensParsers.FieldParsers
{ 
    public class PartyNumberPackageParser : BinaryFieldParserSettings
    {
        public override Guid ConfigId { get { return new Guid("D15F9394-E33A-4E13-BEC2-77670B650B81"); } }
        public string PartyNumberFieldName { get; set; }
        public string NADIFieldName { get; set; }
        public string NPIFieldName { get; set; }

        public override void Execute(IBinaryFieldParserContext context)
        {
            char[] nadiByteAsChars = Convert.ToString(context.FieldValue[0], 2).PadLeft(8, '0').ToCharArray();
            int nadiValue = Convert.ToInt32(string.Concat(nadiByteAsChars[1], nadiByteAsChars[2], nadiByteAsChars[3], nadiByteAsChars[4], nadiByteAsChars[5], nadiByteAsChars[6], nadiByteAsChars[7]), 2);

            char[] npiAsChars = Convert.ToString(context.FieldValue[1], 2).PadLeft(8, '0').ToCharArray();
            int npiValue = Convert.ToInt32(string.Concat(nadiByteAsChars[1], nadiByteAsChars[2], nadiByteAsChars[3]), 2);

            int numberOfDigits = ParserHelper.HexToInt32(ParserHelper.GetHexFromByte(context.FieldValue[2]));
            int digitBytesLength = context.FieldValue.Length - 3;

            byte[] digitsBytes = new byte[digitBytesLength];
            Array.Copy(context.FieldValue, 3, digitsBytes, 0, digitBytesLength);

            string digits = ParserHelper.GetBCDNumber(digitsBytes, true, false);
            string partyNumber = numberOfDigits % 2 != 0 ? digits.Remove(digits.Length - 1, 1) : digits;

            context.Record.SetFieldValue(this.PartyNumberFieldName, partyNumber);
            context.Record.SetFieldValue(this.NADIFieldName, nadiValue);
            context.Record.SetFieldValue(this.NPIFieldName, npiValue);
        }
    }
}