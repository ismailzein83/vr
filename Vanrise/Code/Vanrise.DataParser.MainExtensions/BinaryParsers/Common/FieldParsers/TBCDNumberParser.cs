using System;
using System.Text;
using Vanrise.DataParser.Business;
using Vanrise.DataParser.Entities;

namespace Vanrise.DataParser.MainExtensions.BinaryParsers.Common.FieldParsers
{
    public class TBCDNumberParser : BinaryFieldParserSettings
    {
        public override Guid ConfigId { get { return new Guid("55FD305B-707F-4B98-A5DA-2CAEC314FC85"); } }
        public string FieldName { get; set; }
        public bool AIsZero { get; set; }
        public bool RemoveHexa { get; set; }
        public int? ByteOffset { get; set; }

        public override void Execute(IBinaryFieldParserContext context)
        {
            byte[] newByteArray;
            if (ByteOffset.HasValue)
            {
                int newByteArrayLength = context.FieldValue.Length - ByteOffset.Value;
                newByteArray = new byte[newByteArrayLength];
                Array.Copy(context.FieldValue, ByteOffset.Value, newByteArray, 0, newByteArrayLength);
            }
            else
            {
                newByteArray = context.FieldValue;
            }

            context.Record.SetFieldValue(this.FieldName, ParserHelper.GetTBCDNumber(newByteArray, this.RemoveHexa, this.AIsZero));
        }
    }
}