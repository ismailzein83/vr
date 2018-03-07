using System;
using Vanrise.DataParser.Business;
using Vanrise.DataParser.Entities;

namespace Vanrise.DataParser.MainExtensions.BinaryParsers.NokiaSiemensParsers.FieldParsers
{
    public class ZonePackageParser : BinaryFieldParserSettings
    {
        public override Guid ConfigId { get { return new Guid("F1FC606C-F862-41CB-A832-1C670F555EF9"); } }

        public string ZoneFieldName { get; set; }

        public override void Execute(IBinaryFieldParserContext context)
        {
            int zone = ParserHelper.ByteToNumber(context.FieldValue);
            context.Record.SetFieldValue(this.ZoneFieldName, zone);
        }
    }
}