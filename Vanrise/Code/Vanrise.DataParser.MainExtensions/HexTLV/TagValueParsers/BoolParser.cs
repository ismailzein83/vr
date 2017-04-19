using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.DataParser.Entities.HexTLV;

namespace Vanrise.DataParser.MainExtensions.HexTLV.TagValueParsers
{
    public class BoolParser : TagValueParser
    {
        public override Guid ConfigId
        {
            get { return new Guid("3BD9B2B7-9993-4664-9E08-3AF2C2819489"); }
        }
        public string FieldName { get; set; }

        public override void Execute(ITagValueParserExecuteContext context)
        {
        }
    }
}
