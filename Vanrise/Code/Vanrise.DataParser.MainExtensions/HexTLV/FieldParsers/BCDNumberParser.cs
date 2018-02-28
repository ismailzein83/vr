using System;
using System.Text;
using Vanrise.DataParser.Business;
using Vanrise.DataParser.Entities;
using System.Linq;

namespace Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers
{
    public class BCDNumberParser : HexTLVFieldParserSettings
    {
        public override Guid ConfigId { get { return new Guid("55FD305B-707F-4B98-A5DA-2CAEC314FC85"); } }
        public string FieldName { get; set; }
        public bool AIsZero { get; set; }
        public bool RemoveHexa { get; set; }
        public bool Reverse { get; set; }
        public override void Execute(IHexTLVFieldParserContext context)
        {
            context.Record.SetFieldValue(this.FieldName, ParserHelper.GetBCDNumber(Reverse ? context.FieldValue.Reverse().ToArray() : context.FieldValue, this.RemoveHexa, this.AIsZero));
        }
    }
}
