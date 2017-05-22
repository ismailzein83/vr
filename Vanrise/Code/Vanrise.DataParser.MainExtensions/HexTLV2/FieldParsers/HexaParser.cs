using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.DataParser.Entities;

namespace Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers
{
    public class HexaParser : HexTLVFieldParserSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("21B7367D-B5B8-41B2-95A5-2B8AF9071BEE"); }
        }

        public string FieldName { get; set; }
        public bool TrimZeros { get; set; }
        public override void Execute(IHexTLVFieldParserContext context)
        {
            string value = BitConverter.ToString(context.FieldValue).Replace("-", string.Empty);

            if (TrimZeros)
                value = value.TrimStart(new char[] { '0' });

            context.Record.SetFieldValue(FieldName, value);
        }
    }
}
