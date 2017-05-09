using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.DataParser.Entities.HexTLV2;
using Vanrise.Common;
using Vanrise.DataParser.Entities;

namespace Vanrise.DataParser.Business.HexTLV2
{
    public static class HexTLVHelper
    {
        public static void ReadTagsFromStream(Stream stream, Action<HexTLVTagValue> onTagValueRead)
        {            
            int position = 0;
            while (position < stream.Length)
            {
                HexTLVTagValue tagValue = new HexTLVTagValue();
                tagValue.Tag = ParserHelper.GetStringValue(stream, 1);
                position++;
                int valueLength = ParserHelper.GetIntValue(stream, 1);
                tagValue.Length = valueLength;
                position += valueLength;
                tagValue.Value = new byte[valueLength];
                stream.Read(tagValue.Value, 0, valueLength);
                onTagValueRead(tagValue);
            }
        }

        public static void ExecuteRecordParsers(List<HexTLVRecordParser> subRecordsParsers, Stream recordStream, IHexTLVRecordParserContext parentRecordContext)
        {
            subRecordsParsers.ThrowIfNull("subRecordsParsers");
            foreach (var subRecordParser in subRecordsParsers)
            {
                subRecordParser.Settings.ThrowIfNull("subRecordParser.Settings");
                var subRecordContext = new SubRecordHexTLVRecordParserContext(recordStream, parentRecordContext);
                subRecordParser.Settings.Execute(subRecordContext);
            }
        }

        public static void ExecuteFieldParsers(HexTLVFieldParserCollection fieldParsers, ParsedRecord parsedRecord, Stream recordStream)
        {
            fieldParsers.ThrowIfNull("fieldParsers");
            fieldParsers.FieldParsersByTag.ThrowIfNull("fieldParsers.FieldParsersByTag");
            ReadTagsFromStream(recordStream,
                (tagValue) =>
                {
                    HexTLVFieldParser fldParser;
                    if(fieldParsers.FieldParsersByTag.TryGetValue(tagValue.Tag, out fldParser))
                    {
                        var fieldParserContext = new HexTLVFieldParserContext { Record = parsedRecord, FieldValue = tagValue.Value };
                        fldParser.Settings.Execute(fieldParserContext);
                    }
                });
        }

        #region Private Classes

        private class SubRecordHexTLVRecordParserContext : IHexTLVRecordParserContext
        {
            Stream _recordStream;
            IHexTLVRecordParserContext _parentContext;

            public SubRecordHexTLVRecordParserContext(Stream recordStream, IHexTLVRecordParserContext parentContext)
            {
                recordStream.ThrowIfNull("recordData");
                parentContext.ThrowIfNull("parentContext");
                _recordStream = recordStream;
                _parentContext = parentContext;
            }

            public Stream RecordStream
            {
                get { return _recordStream; }
            }

            public ParsedRecord CreateRecord(string recordType)
            {
                return _parentContext.CreateRecord(recordType);
            }

            public void OnRecordParsed(ParsedRecord parsedRecord)
            {
                _parentContext.OnRecordParsed(parsedRecord);
            }


            public HexTLVRecordParser GetParserTemplate(Guid templateId)
            {
                return _parentContext.GetParserTemplate(templateId);
            }
        }

        private class HexTLVFieldParserContext : IHexTLVFieldParserContext
        {
            public byte[] FieldValue { get; set; }

            public ParsedRecord Record { get; set; }
        }

        #endregion
    }

    public class HexTLVTagValue
    {
        public string Tag { get; set; }

        public int Length { get; set; }

        public byte[] Value { get; set; }
    }
}
