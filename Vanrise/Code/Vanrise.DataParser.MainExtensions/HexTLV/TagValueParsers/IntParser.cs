using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.DataParser.Business;
using Vanrise.DataParser.Entities.HexTLV;
using Vanrise.DataParser.MainExtensions.HexTLV.RecordReaders;

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
            int value = ParserHelper.ByteToNumber(context.TagValue);
            context.Record.SetFieldValue(this.FieldName, value);
        }
    }
}
