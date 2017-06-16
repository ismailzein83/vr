using System;
using Vanrise.DataParser.Business;
using Vanrise.DataParser.Entities;

namespace Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers
{
    public class BoolFieldParser : HexTLVFieldParserSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("3BD9B2B7-9993-4664-9E08-3AF2C2819489"); }
        }

        public string FieldName { get; set; }

        public override void Execute(IHexTLVFieldParserContext context)
        {
            bool value = Convert.ToBoolean(ParserHelper.ByteToNumber(context.FieldValue));
            context.Record.SetFieldValue(this.FieldName, value);
        }
    }
}
