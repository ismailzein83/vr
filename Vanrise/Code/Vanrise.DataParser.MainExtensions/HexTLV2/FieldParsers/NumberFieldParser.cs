using System;
using Vanrise.DataParser.Business;
using Vanrise.DataParser.Entities;

namespace Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers
{
    public enum NumberType { Decimal, Int, BigInt }
    public class NumberFieldParser : HexTLVFieldParserSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("11FCE310-6BFF-43BD-ACD8-F229C8F4ED8A"); }
        }

        public string FieldName { get; set; }
        public NumberType NumberType { get; set; }

        public override void Execute(IHexTLVFieldParserContext context)
        {
            switch (this.NumberType)
            {
                case NumberType.Decimal:
                    //context.Record.SetFieldValue(this.FieldName, ParserHelper.HexToDecimal(ParserHelper.ByteArrayToString(context.FieldValue)));
                    break;
                case NumberType.Int:
                    context.Record.SetFieldValue(this.FieldName, ParserHelper.HexToInt32(ParserHelper.ByteArrayToString(context.FieldValue)));
                    break;
                case NumberType.BigInt:
                    context.Record.SetFieldValue(this.FieldName, ParserHelper.HexToInt64(ParserHelper.ByteArrayToString(context.FieldValue)));
                    break;
            }
        }
    }
}
