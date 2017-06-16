using System;
using System.IO;
using Vanrise.DataParser.Business;
using Vanrise.DataParser.Entities;

namespace Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers
{
    public class SequenceFieldParser : HexTLVFieldParserSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("300353A5-1105-4BAC-9207-1602BB0A3B1A"); }
        }

        public HexTLVFieldParserCollection FieldParsers { get; set; }

        public override void Execute(IHexTLVFieldParserContext context)
        {
            HexTLVHelper.ExecuteFieldParsers(this.FieldParsers, context.Record, new MemoryStream(context.FieldValue));
        }
    }
}
