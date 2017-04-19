using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.DataParser.Entities.HexTLV;

namespace Vanrise.DataParser.MainExtensions.HexTLV.TagValueParsers
{
    public class IntParser : TagValueParser
    {
        public override Guid ConfigId
        {
            get { return new Guid("11FCE310-6BFF-43BD-ACD8-F229C8F4ED8A"); }
        }
        public string FieldName { get; set; }

        public override void Execute(ITagValueParserExecuteContext context)
        {
            throw new NotImplementedException();
        }
    }
}
