using System;
using Vanrise.DataParser.Business;
using Vanrise.DataParser.Entities;

namespace Vanrise.DataParser.MainExtensions.BinaryParsers.Common.FieldParsers
{
    public class BoolBitFieldParser : BitFieldParserSettings
    {
        public override Guid ConfigId { get { return new Guid("E391D018-31CD-434A-906E-DE3F8443B758"); } }

        public string FieldName { get; set; }

        public override void Execute(IBitFieldParserContext context)
        {
            bool value = ParserHelper.BitToBoolean(context.FieldValue, 0);
            context.Record.SetFieldValue(this.FieldName, value);
        }
    }
}
