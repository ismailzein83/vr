using System;
using Vanrise.DataParser.Business;
using Vanrise.DataParser.Entities;

namespace Vanrise.DataParser.MainExtensions.BinaryParsers.HuaweiParser.FieldParsers
{
    public class DiagnosticsParser : BinaryFieldParserSettings
    {
        public override Guid ConfigId { get { return new Guid("759CFA83-240C-4184-BD13-3F72090D3111"); } }
        public string FieldName { get; set; }

        public override void Execute(IBinaryFieldParserContext context)
        {
            //As Per Discussion with Ali Taher Diagnostic length is always 2 bytes
            //First byte is ignored and second one is used taking the less significant seven bits and then converting it to hexadecimal.

            int byteAsInt = context.FieldValue[1] & 0x7F;
            string byteArrayAsString = ParserHelper.GetHexFromInt(byteAsInt);

            context.Record.SetFieldValue(this.FieldName, byteArrayAsString);
        }
    }
}