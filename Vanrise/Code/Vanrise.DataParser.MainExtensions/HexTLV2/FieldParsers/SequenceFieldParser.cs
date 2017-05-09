using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.DataParser.Business.HexTLV2;
using Vanrise.DataParser.Entities.HexTLV2;

namespace Vanrise.DataParser.MainExtensions.HexTLV2.FieldParsers
{
    public class SequenceFieldParser : HexTLVFieldParserSettings
    {
        public override Guid ConfigId
        {
            get { throw new NotImplementedException(); }
        }

        public HexTLVFieldParserCollection FieldParsers { get; set; }

        public override void Execute(IHexTLVFieldParserContext context)
        {
            HexTLVHelper.ExecuteFieldParsers(this.FieldParsers, context.Record, new MemoryStream(context.FieldValue));
        }
    }
}
