using System;
using Vanrise.DataParser.Business;
using Vanrise.DataParser.Entities;

namespace Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers
{
    public enum NumberType { Decimal, Int, BigInt }
    public class NumberFieldParser : HexTLVFieldParserSettings
    {
        public override Guid ConfigId { get { return new Guid("11FCE310-6BFF-43BD-ACD8-F229C8F4ED8A"); } }
        public string FieldName { get; set; }
        public NumberType NumberType { get; set; }
        public bool Reverse { get; set; }
        public int? FieldIndex { get; set; }
        public int? FieldBytesLength { get; set; }

        public override void Execute(IHexTLVFieldParserContext context)
        {
            byte[] input;
            if (!this.FieldIndex.HasValue || !this.FieldBytesLength.HasValue)
            {
                input = context.FieldValue;
            }
            else
            {
                input = new byte[this.FieldBytesLength.Value];
                Array.Copy(context.FieldValue, FieldIndex.Value, input, 0, FieldBytesLength.Value);
            }

            switch (this.NumberType)
            {
                case NumberType.Decimal:
                    //context.Record.SetFieldValue(this.FieldName, ParserHelper.HexToDecimal(ParserHelper.ByteArrayToString(context.FieldValue)));
                    break;
                case NumberType.Int:
                    context.Record.SetFieldValue(this.FieldName, ParserHelper.HexToInt32(ParserHelper.ByteArrayToString(input, this.Reverse)));
                    break;
                case NumberType.BigInt:
                    context.Record.SetFieldValue(this.FieldName, ParserHelper.HexToInt64(ParserHelper.ByteArrayToString(input, this.Reverse)));
                    break;
            }
        }
    }
}
