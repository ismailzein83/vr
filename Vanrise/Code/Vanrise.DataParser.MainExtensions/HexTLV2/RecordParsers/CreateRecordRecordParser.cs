using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.DataParser.Business.HexTLV2;
using Vanrise.DataParser.Entities;
using Vanrise.DataParser.Entities.HexTLV2;

namespace Vanrise.DataParser.MainExtensions.HexTLV2.RecordParsers
{
    public class CreateRecordRecordParser : HexTLVRecordParserSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("59269FF4-9773-44CF-A792-2858FEA2031E"); }
        }

        public string RecordType { get; set; }

        public HexTLVFieldParserCollection FieldParsers { get; set; }

        public List<ParsedRecordFieldConstantValue> FieldConstantValues { get; set; }

        public override void Execute(IHexTLVRecordParserContext context)
        {
            ParsedRecord parsedRecord = context.CreateRecord(this.RecordType);

            if (this.FieldParsers != null)
                HexTLVHelper.ExecuteFieldParsers(this.FieldParsers, parsedRecord, context.RecordStream);

            if (this.FieldConstantValues != null)
            {
                foreach (var fldConstantValue in this.FieldConstantValues)
                {
                    parsedRecord.SetFieldValue(fldConstantValue.FieldName, fldConstantValue.Value);
                }
            }
        }
    }
}
