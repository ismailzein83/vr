using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.DataParser.Business;
using Vanrise.DataParser.Entities.HexTLV2;

namespace Vanrise.DataParser.MainExtensions.HexTLV2.FieldParsers
{
    public class BoolFieldParser : HexTLVFieldParserSettings
    {
        public override Guid ConfigId
        {
            get { throw new NotImplementedException(); }
        }

        public string FieldName { get; set; }

        public override void Execute(IHexTLVFieldParserContext context)
        {
            bool value = Convert.ToBoolean(ParserHelper.ByteToNumber(context.FieldValue));
            context.Record.SetFieldValue(this.FieldName, value);
        }
    }
}
