using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.DataParser.Business;
using Vanrise.DataParser.Entities;

namespace Vanrise.DataParser.MainExtensions.HexTLV.RecordParsers
{
    public class PositionedFieldsRecordParser : HexTLVRecordParserSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("301B945E-765F-4D90-952E-D86DA4AE4040"); }
        }

        public string RecordType { get; set; }

        public List<PositionedFieldParser> FieldParsers { get; set; }

        public List<ParsedRecordFieldConstantValue> FieldConstantValues { get; set; }

        public List<CompositeFieldsParser> CompositeFieldsParsers { get; set; }

        public HashSet<string> TempFieldsNames { get; set; }

        public override void Execute(IHexTLVRecordParserContext context)
        {
            ParsedRecord parsedRecord = context.CreateRecord(this.RecordType, this.TempFieldsNames);

            if (this.FieldParsers != null)
                HexTLVHelper.ExecutePositionedFieldParsers(this.FieldParsers, parsedRecord, context.RecordStream);

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
