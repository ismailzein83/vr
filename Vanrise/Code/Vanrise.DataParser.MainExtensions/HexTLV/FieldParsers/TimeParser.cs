using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.DataParser.Business;
using Vanrise.DataParser.Entities;

namespace Vanrise.DataParser.MainExtensions.HexTLV2.FieldParsers
{
    public class TimeParser : HexTLVFieldParserSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("73A133A2-D4A7-4BFD-A87C-CA78B435ED80"); }
        }
        public string FieldName { get; set; }
        public override void Execute(IHexTLVFieldParserContext context)
        {
            TimeSpan timeSpan = new TimeSpan(ParserHelper.HexToInt32(ParserHelper.GetHexFromByte(context.FieldValue[0]))
                                                   , ParserHelper.HexToInt32(ParserHelper.GetHexFromByte(context.FieldValue[1]))
                                                   , ParserHelper.HexToInt32(ParserHelper.GetHexFromByte(context.FieldValue[2])));
            context.Record.SetFieldValue(FieldName, timeSpan);
        }
    }
}
