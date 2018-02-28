using System;
using Vanrise.DataParser.Business;
using Vanrise.DataParser.Entities;

namespace Vanrise.DataParser.MainExtensions.BinaryParsers.Common.FieldParsers
{
    public class BoolFieldParser : BinaryFieldParserSettings
    {
        public override Guid ConfigId { get { return new Guid("3BD9B2B7-9993-4664-9E08-3AF2C2819489"); } }

        public string FieldName { get; set; }

        public override void Execute(IBinaryFieldParserContext context)
        {
            bool value = Convert.ToBoolean(ParserHelper.ByteToNumber(context.FieldValue));
            context.Record.SetFieldValue(this.FieldName, value);
        }
    }
}
