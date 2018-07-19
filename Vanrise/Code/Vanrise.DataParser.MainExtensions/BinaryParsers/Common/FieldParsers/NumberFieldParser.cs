using System;
using Vanrise.DataParser.Business;
using Vanrise.DataParser.Entities;

namespace Vanrise.DataParser.MainExtensions.BinaryParsers.Common.FieldParsers
{
    public class NumberFieldParser : BinaryFieldParserSettings
    {
        public override Guid ConfigId { get { return new Guid("11FCE310-6BFF-43BD-ACD8-F229C8F4ED8A"); } }
        public string FieldName { get; set; }
        public NumberType NumberType { get; set; }
        public bool Reverse { get; set; }
        public int? FieldIndex { get; set; }
        public int? FieldBytesLength { get; set; }
        public bool ConvertOutputToString { get; set; }

        public override void Execute(IBinaryFieldParserContext context)
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
                    int resultAsInt = ParserHelper.HexToInt32(ParserHelper.ByteArrayToString(input, this.Reverse));
                    if (!ConvertOutputToString)
                        context.Record.SetFieldValue(this.FieldName, resultAsInt);
                    else
                        context.Record.SetFieldValue(this.FieldName, resultAsInt.ToString());
                    break;
                case NumberType.BigInt:
                    long resultAsLong = ParserHelper.HexToInt64(ParserHelper.ByteArrayToString(input, this.Reverse));
                    if (!ConvertOutputToString)
                        context.Record.SetFieldValue(this.FieldName, resultAsLong);
                    else
                        context.Record.SetFieldValue(this.FieldName, resultAsLong.ToString());
                    break;
            }
        }
    }
}
