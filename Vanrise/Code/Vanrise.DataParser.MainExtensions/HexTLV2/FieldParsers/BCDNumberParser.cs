using System;
using System.Text;
using Vanrise.DataParser.Business;
using Vanrise.DataParser.Entities;

namespace Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers
{
    public class TBCDNumberParser : HexTLVFieldParserSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("55FD305B-707F-4B98-A5DA-2CAEC314FC85"); }
        }
        public string FieldName { get; set; }
        public bool AIsZero { get; set; }
        public bool RemoveHexa { get; set; }
        public override void Execute(IHexTLVFieldParserContext context)
        {
            StringBuilder number = new StringBuilder();
            foreach (var byteItem in context.FieldValue)
            {
                byte[] nibbles = ParserHelper.SplitByteToNibble(byteItem);
                for (int i = nibbles.Length - 1; i >= 0; i--)
                {
                    int val = (int)nibbles[i];
                    number.Append(GetNumberValue(val));
                }
            }
            context.Record.SetFieldValue(this.FieldName, number.ToString());
        }

        string GetNumberValue(int val)
        {
            if (AIsZero && val == 10)
                return "0";
            else if (AIsZero && (val > 10 || val == 0) || (RemoveHexa && val > 9))
                return "";
            return val.ToString();
        }
    }
}
