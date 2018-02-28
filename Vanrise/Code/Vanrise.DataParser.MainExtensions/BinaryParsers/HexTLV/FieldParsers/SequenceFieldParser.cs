using System;
using System.IO;
using Vanrise.DataParser.Business;
using Vanrise.DataParser.Entities;

namespace Vanrise.DataParser.MainExtensions.BinaryParsers.HexTLV.FieldParsers
{
    public class SequenceFieldParser : BinaryFieldParserSettings
    {
        public override Guid ConfigId { get { return new Guid("300353A5-1105-4BAC-9207-1602BB0A3B1A"); } }

        public BinaryFieldParserCollection FieldParsers { get; set; }

        public override void Execute(IBinaryFieldParserContext context)
        {
            HexTLVParserHelper.ExecuteFieldParsers(this.FieldParsers, context.Record, new MemoryStream(context.FieldValue));
        }
    }
}
