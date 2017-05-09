using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.DataParser.Business;
using Vanrise.DataParser.Business.HexTLV;
using Vanrise.DataParser.Entities;
using Vanrise.DataParser.Entities.HexTLV;

namespace Vanrise.DataParser.MainExtensions.HexTLV.TagValueParsers
{
    public class SequenceParser : TagValueParser
    {
        public override Guid ConfigId
        {
            get { return new Guid("300353A5-1105-4BAC-9207-1602BB0A3B1A"); }
        }
        public Dictionary<string, HexTLVTagType> TagTypes { get; set; }

        public override void Execute(ITagValueParserExecuteContext context)
        {
            MemoryStream stream = new MemoryStream(context.TagValue);
            int position = 0;
            while (position < stream.Length)
            {
                string tag = "";
                byte[] fieldData = ParserHelper.ParseData(stream, out tag, ref position);
                ParserHelper.EvaluteParser(this.TagTypes, tag, fieldData, context.Record);
            }
        }
    }
}
