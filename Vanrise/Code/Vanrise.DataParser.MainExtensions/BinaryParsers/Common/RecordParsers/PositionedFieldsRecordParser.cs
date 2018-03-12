using System;
using System.Collections.Generic;
using Vanrise.DataParser.Business;
using Vanrise.DataParser.Entities;

namespace Vanrise.DataParser.MainExtensions.BinaryParsers.Common.RecordParsers
{
    public enum ZeroBytesBlockAction { Normal = 0, Skip = 1 };

    public class PositionedFieldsRecordParser : BinaryRecordParserSettings
    {
        public override Guid ConfigId { get { return new Guid("301B945E-765F-4D90-952E-D86DA4AE4040"); } }

        public string RecordType { get; set; }

        public List<PositionedFieldParser> FieldParsers { get; set; }

        public List<ParsedRecordFieldConstantValue> FieldConstantValues { get; set; }

        public List<CompositeFieldsParser> CompositeFieldsParsers { get; set; }

        public HashSet<string> TempFieldsNames { get; set; }

        public ZeroBytesBlockAction ZeroBytesBlockAction { get; set; }


        public override void Execute(IBinaryRecordParserContext context)
        {
            bool shouldContinue = true;

            if (this.ZeroBytesBlockAction == RecordParsers.ZeroBytesBlockAction.Skip)
            {
                bool hasOneValueItem = false;
                byte[] streamBytes = new byte[context.RecordStream.Length];
                for (var index = 0; index < context.RecordStream.Length; index++)
                {
                    int currentByteValue = context.RecordStream.ReadByte();
                    if (currentByteValue != 0)
                    {
                        hasOneValueItem = true;
                        break;
                    }
                }
                if (!hasOneValueItem)
                    shouldContinue = false;
            }

            if (!shouldContinue)
                return;

            HashSet<string> tempFieldNames = TempFieldsNames != null ? new HashSet<string>(TempFieldsNames) : null;

            Dictionary<string, dynamic> globalVariables = context.GetGlobalVariables();

            if (globalVariables != null)
            {
                if (tempFieldNames == null)
                    tempFieldNames = new HashSet<string>();

                foreach (var globalVariableKvp in globalVariables)
                    tempFieldNames.Add(globalVariableKvp.Key);
            }

            ParsedRecord parsedRecord = context.CreateRecord(this.RecordType, tempFieldNames);

            if (globalVariables != null)
            {
                foreach (var globalVariableKvp in globalVariables)
                    parsedRecord.SetFieldValue(globalVariableKvp.Key, globalVariableKvp.Value);
            }

            if (this.FieldParsers != null)
                BinaryParserHelper.ExecutePositionedFieldParsers(this.FieldParsers, parsedRecord, context.RecordStream);

            if (this.CompositeFieldsParsers != null)
            {
                foreach (var compositeFieldsParser in this.CompositeFieldsParsers)
                {
                    compositeFieldsParser.Execute(new CompositeFieldsParserContext
                    {
                        Record = parsedRecord,
                        FileName = context.FileName,
                        DataSourceId = context.DataSourceId
                    });
                }
            }

            if (this.FieldConstantValues != null)
            {
                foreach (var fldConstantValue in this.FieldConstantValues)
                    parsedRecord.SetFieldValue(fldConstantValue.FieldName, fldConstantValue.Value);
            }

            context.OnRecordParsed(parsedRecord);
        }
    }
}
