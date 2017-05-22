using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.DataParser.Entities;

namespace Vanrise.DataParser.Business
{
    public static class HexTLVHelper
    {
        public static void ReadTagsFromStream(Stream stream, Action<HexTLVTagValue> onTagValueRead)
        {
            byte[] rawData = new byte[stream.Length];
            stream.Read(rawData, 0, (int)stream.Length);
            List<HexTLVTagValue> tags = new List<HexTLVTagValue>();

            for (int i = 0, start = 0; i < rawData.Length; start = i)
            {
                // parse Tag
                bool constructedTlv = (rawData[i] & 0x20) != 0;
                bool moreBytes = (rawData[i] & 0x1F) == 0x1F;
                while (moreBytes && (rawData[++i] & 0x80) != 0) ;
                i++;

                int tag = ParserHelper.GetInt(rawData, start, i - start);

                // parse Length
                bool multiByteLength = (rawData[i] & 0x80) != 0;

                int length = multiByteLength ? ParserHelper.GetInt(rawData, i + 1, rawData[i] & 0x1F) : rawData[i];
                i = multiByteLength ? i + (rawData[i] & 0x1F) + 1 : i + 1;

                // fill data
                byte[] result = new byte[length];
                Array.Copy(rawData, i, result, 0, length);

                HexTLVTagValue tagValue = new HexTLVTagValue
                {
                    Value = result,
                    Length = length,
                    Tag = tag.ToString("X2")
                };
                tags.Add(tagValue);
                i += length;
                onTagValueRead(tagValue);
            }
        }

        public static void ExecuteRecordParser(HexTLVRecordParser subRecordsParser, Stream recordStream, IHexTLVRecordParserContext parentRecordContext)
        {
            subRecordsParser.Settings.ThrowIfNull("subRecordParser.Settings");
            var subRecordContext = new SubRecordHexTLVRecordParserContext(recordStream, parentRecordContext);
            subRecordsParser.Settings.Execute(subRecordContext);
        }

        public static void ExecuteFieldParsers(HexTLVFieldParserCollection fieldParsers, ParsedRecord parsedRecord, Stream recordStream)
        {
            fieldParsers.ThrowIfNull("fieldParsers");
            fieldParsers.FieldParsersByTag.ThrowIfNull("fieldParsers.FieldParsersByTag");
            ReadTagsFromStream(recordStream,
                (tagValue) =>
                {
                    HexTLVFieldParser fldParser;
                    if (fieldParsers.FieldParsersByTag.TryGetValue(tagValue.Tag, out fldParser))
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
