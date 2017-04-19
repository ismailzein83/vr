using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.DataParser.Entities.HexTLV;

namespace Vanrise.DataParser.MainExtensions.HexTLV.TagValueParsers
{
    public class StringParser : TagValueParser
    {
        public override Guid ConfigId
        {
            get { return new Guid("F291C16D-74E2-4C25-B2A7-43301CD5C04F"); }
        }
        public string FieldName { get; set; }
        public override void Execute(ITagValueParserExecuteContext context)
        {
            string value = System.Text.Encoding.UTF8.GetString(context.TagValue);
            context.Record.SetFieldValue(this.FieldName, value);
        }
    }
}
