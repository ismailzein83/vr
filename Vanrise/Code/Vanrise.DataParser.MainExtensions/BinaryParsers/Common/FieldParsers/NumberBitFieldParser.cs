using System;
using System.Collections;
using Vanrise.DataParser.Business;
using Vanrise.DataParser.Entities;

namespace Vanrise.DataParser.MainExtensions.BinaryParsers.Common.FieldParsers
{
    public class NumberBitFieldParser : BitFieldParserSettings
    {
        public override Guid ConfigId { get { return new Guid("F7213F70-3148-4BC6-96F0-69A77821EFD3"); } }
        public string FieldName { get; set; }

        public override void Execute(IBitFieldParserContext context)
        {
            BitArray reversedBitArray = ParserHelper.GetReversedBitArray(context.FieldValue);
            int value = ParserHelper.BitArrayToNumber(reversedBitArray);
            context.Record.SetFieldValue(this.FieldName, value);
        }
    }
}