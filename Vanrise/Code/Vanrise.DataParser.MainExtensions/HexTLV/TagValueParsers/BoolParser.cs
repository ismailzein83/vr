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
    public class BoolParser : TagValueParser
    {
        public override Guid ConfigId
        {
            get { return new Guid("3BD9B2B7-9993-4664-9E08-3AF2C2819489"); }
        }
        public string FieldName { get; set; }

        public override void Execute(ITagValueParserExecuteContext context)
        {
            bool value = Convert.ToBoolean(ParserHelper.ByteToNumber(context.TagValue));
            context.Record.SetFieldValue(this.FieldName, value);
        }
    }
}
