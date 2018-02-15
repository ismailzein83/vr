using System;
using System.Collections.Generic;
using Vanrise.DataParser.Business;
using Vanrise.DataParser.Entities;

namespace Vanrise.DataParser.MainExtensions.HexTLV.RecordParsers
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
        public List<CompositeFieldsParser> CompositeFieldsParsers { get; set; }
        public HashSet<string> TempFieldsNames { get; set; }
        public override void Execute(IHexTLVRecordParserContext context)
        {
            ParsedRecord parsedRecord = context.CreateRecord(this.RecordType, this.TempFieldsNames);

            if (this.FieldParsers != null)
                HexTLVHelper.ExecuteFieldParsers(this.FieldParsers, parsedRecord, context.RecordStream);

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

    public class CompositeFieldsParserContext : ICompositeFieldsParserContext
    {
        public ParsedRecord Record
        {
            get;
            set;
        }


        public string FileName
        {
            get;
            set;
        }
    }
}
