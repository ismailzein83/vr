using System;
using System.Collections.Generic;
using Vanrise.DataParser.Business;
using Vanrise.DataParser.Entities;

namespace Vanrise.DataParser.MainExtensions.BinaryParsers.HexTLV.RecordParsers
{
    public class CreateRecordRecordParser : BinaryRecordParserSettings
    {
        public override Guid ConfigId { get { return new Guid("59269FF4-9773-44CF-A792-2858FEA2031E"); } }
        public string RecordType { get; set; }
        public BinaryFieldParserCollection FieldParsers { get; set; }
        public List<ParsedRecordFieldConstantValue> FieldConstantValues { get; set; }
        public List<CompositeFieldsParser> CompositeFieldsParsers { get; set; }
        public HashSet<string> TempFieldsNames { get; set; }

        public override void Execute(IBinaryRecordParserContext context)
        {
            ParsedRecord parsedRecord = context.CreateRecord(this.RecordType, this.TempFieldsNames);

            if (this.FieldParsers != null)
                HexTLVParserHelper.ExecuteFieldParsers(this.FieldParsers, parsedRecord, context.RecordStream);

            if (this.CompositeFieldsParsers != null)
            {
                foreach (var compositeFieldsParser in this.CompositeFieldsParsers)
                {
                    compositeFieldsParser.Execute(new CompositeFieldsParserContext
                    {
                        Record = parsedRecord,
                        FileName = context.FileName
                    });
                }
            }
            if (this.FieldConstantValues != null)
            {
                foreach (var fldConstantValue in this.FieldConstantValues)
                {
                    parsedRecord.SetFieldValue(fldConstantValue.FieldName, fldConstantValue.Value);
                }
            }
            context.OnRecordParsed(parsedRecord);
        }
    }
}