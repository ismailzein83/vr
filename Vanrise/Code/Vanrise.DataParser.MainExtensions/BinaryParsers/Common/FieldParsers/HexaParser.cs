using System;
using System.Linq;
using Vanrise.DataParser.Entities;

namespace Vanrise.DataParser.MainExtensions.BinaryParsers.Common.FieldParsers
{
    public class HexaParser : BinaryFieldParserSettings
    {
        public override Guid ConfigId { get { return new Guid("21B7367D-B5B8-41B2-95A5-2B8AF9071BEE"); } }
        public string FieldName { get; set; }
        public bool TrimZeros { get; set; }
        public bool Reverse { get; set; }

        public override void Execute(IBinaryFieldParserContext context)
        {
            byte[] data = Reverse ? context.FieldValue.Reverse().ToArray() : context.FieldValue;
            string value = BitConverter.ToString(data).Replace("-", string.Empty);
            if (TrimZeros)
                value = value.TrimStart(new char[] { '0' });

            context.Record.SetFieldValue(FieldName, value);
        }
    }
}
