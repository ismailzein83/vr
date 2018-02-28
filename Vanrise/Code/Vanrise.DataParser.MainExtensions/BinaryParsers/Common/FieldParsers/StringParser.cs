using System;
using Vanrise.DataParser.Entities;
using Vanrise.DataParser.MainExtensions.StringFieldParsers;

namespace Vanrise.DataParser.MainExtensions.BinaryParsers.Common.FieldParsers
{
    public class StringParser : BinaryFieldParserSettings
    {
        public override Guid ConfigId { get { return new Guid("F291C16D-74E2-4C25-B2A7-43301CD5C04F"); } }
        public string FieldName { get; set; }
        public BaseStringParser Parser { get; set; }

        public override void Execute(IBinaryFieldParserContext context)
        {
            string value = System.Text.Encoding.UTF8.GetString(context.FieldValue);
            if (string.IsNullOrEmpty(value))
                return;

            value = value.Trim();
            if (string.IsNullOrEmpty(value))
                return;

            if (this.Parser != null)
                Parser.Execute(new BaseStringParserContext() { FieldValue = value, Record = context.Record });
            else
                context.Record.SetFieldValue(this.FieldName, value);
        }

        private class BaseStringParserContext : IBaseStringParserContext
        {
            public string FieldValue { get; set; }
            public ParsedRecord Record { get; set; }
        }
    }
}