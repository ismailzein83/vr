using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            foreach (var tagType in this.TagTypes)
            {
                tagType.Value.ValueParser.Execute(context);
            }
        }
    }
}
