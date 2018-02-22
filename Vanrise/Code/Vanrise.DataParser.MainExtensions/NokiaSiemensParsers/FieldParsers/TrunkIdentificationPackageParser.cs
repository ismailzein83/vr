using System;
using Vanrise.DataParser.Business;
using Vanrise.DataParser.Entities;

namespace Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers
{
    public class TrunkIdentificationPackageParser : HexTLVFieldParserSettings
    {
        public override Guid ConfigId { get { return new Guid("213C598D-8800-41B4-9BB9-5FFEF706433A"); } }
        public string TrunkGroupNumberFieldName { get; set; }
        public string TrunkNumberFieldName { get; set; }

        public override void Execute(IHexTLVFieldParserContext context)
        {
            byte[] trunkGroupNumberBytes = ParserHelper.ExtractFromByteArray(context.FieldValue, 0, 6, false);
            string trunkGroupNumber = System.Text.Encoding.UTF8.GetString(trunkGroupNumberBytes);

            byte[] trunkNumberBytes = ParserHelper.ExtractFromByteArray(context.FieldValue, 6, 2, false);
            int trunkNumber = ParserHelper.ByteToNumber(trunkNumberBytes);

            context.Record.SetFieldValue(this.TrunkGroupNumberFieldName, trunkGroupNumber);
            context.Record.SetFieldValue(this.TrunkNumberFieldName, trunkNumber);
        }
    }
}