using System;
using System.Text;
using Vanrise.DataParser.Business;
using Vanrise.DataParser.Entities;

namespace Vanrise.DataParser.MainExtensions.BinaryParsers.Common.FieldParsers
{
    public class TBCDNumberParser : BinaryFieldParserSettings
    {
        public override Guid ConfigId { get { return new Guid("55FD305B-707F-4B98-A5DA-2CAEC314FC85"); } }
        public string FieldName { get; set; }
        public bool AIsZero { get; set; }
        public bool RemoveHexa { get; set; }
        public override void Execute(IBinaryFieldParserContext context)
        {
            context.Record.SetFieldValue(this.FieldName, ParserHelper.GetTBCDNumber(context.FieldValue, this.RemoveHexa, this.AIsZero));
        }

    }
}
