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
            get { throw new NotImplementedException(); }
        }
        public string FieldName { get; set; }
        public override void Execute(ITagValueParserExecuteContext context)
        {
            string value = System.Text.Encoding.UTF8.GetString(context.TagValue);
            context.Record.SetFieldValue(this.FieldName, value);
        }
    }
}
