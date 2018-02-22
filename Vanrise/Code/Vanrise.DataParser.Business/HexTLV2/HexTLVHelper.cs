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
        public static void ReadRecordFromStream(Stream stream, int lengthNbOfBytes, bool reverseLengthBytes, Action<RecordValue> onRecordValueRead)
        {

            int position = 0;
            byte[] bytes = null;
            int recordLength = 0;
            byte[] recordData = null;
            while (position < stream.Length)
            {
                try
                {

                    bytes = new byte[lengthNbOfBytes];
                    stream.Read(bytes, 0, lengthNbOfBytes);
                    bytes = reverseLengthBytes ? bytes.Reverse().ToArray() : bytes;
                    position += lengthNbOfBytes;
                    recordLength = ParserHelper.GetInt(bytes, 0, lengthNbOfBytes);

                    position += recordLength;
                    recordData = new byte[recordLength - lengthNbOfBytes];
                    stream.Read(recordData, 0, recordLength - lengthNbOfBytes);

                    RecordValue recordValue = new RecordValue
                    {
                        Length = recordLength,
                        Value = Combine(bytes, recordData)
                    };
                    onRecordValueRead(recordValue);
                }
                catch (Exception ex)
                {

                    // throw ex;
                }

            }

        }
        public static void ReadRecordFieldFromStream(byte[] data, int position, int length, Action<RecordValue> onRecordFieldValue)
        {
            byte[] bytes = new byte[length];

            Array.Copy(data, position, bytes, 0, length);
            RecordValue recordType = new RecordValue
            {
                Length = length,
                Value = bytes
            };
            onRecordFieldValue(recordType);

        }
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
                if (tag == 0 || tag == 255)
                    continue;
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

        public static void ExecuteFieldParser(HexTLVFieldParserSettings fieldParser, ParsedRecord parsedRecord, byte[] fieldValue)
        {
            fieldParser.ThrowIfNull("fieldParsers");

            var fieldParserContext = new HexTLVFieldParserContext { Record = parsedRecord, FieldValue = fieldValue };
            fieldParser.Execute(fieldParserContext);
        }

        public static void ExecutePositionedFieldParsers(List<PositionedFieldParser> positionedFieldParsers, ParsedRecord parsedRecord, Stream recordStream)
        {
            positionedFieldParsers.ThrowIfNull("positionedFieldParsers");
            byte[] data = ((MemoryStream)recordStream).ToArray();
            foreach (var positionedFieldParser in positionedFieldParsers)
            {
                ReadRecordFieldFromStream(data, positionedFieldParser.Position, positionedFieldParser.Length,
                      (recordType) =>
                      {
                          var fieldParserContext = new HexTLVFieldParserContext { Record = parsedRecord, FieldValue = recordType.Value };
                          positionedFieldParser.FieldParser.Settings.Execute(fieldParserContext);
                      });
            }
        }

        public static void ReadBlockFromStream(Stream stream, int blockSize, Action<RecordValue> onBlockRead, bool readOnce = false)
        {
            while (stream.Length - stream.Position >= blockSize)
            {
                byte[] bytes = null;
                bytes = new byte[blockSize];
                stream.Read(bytes, 0, blockSize);

                RecordValue recordValue = new RecordValue
                {
                    Length = blockSize,
                    Value = bytes
                };
                onBlockRead(recordValue);

                if (readOnce)
                    break;
            }

        }


        #region Private Classes
        public static byte[] Combine(byte[] first, byte[] second)
        {
            byte[] ret = new byte[first.Length + second.Length];
            Buffer.BlockCopy(first, 0, ret, 0, first.Length);
            Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);
            return ret;
        }
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
                FileName = parentContext.FileName;
            }

            public Stream RecordStream
            {
                get { return _recordStream; }
            }

            public void OnRecordParsed(ParsedRecord parsedRecord)
            {
                _parentContext.OnRecordParsed(parsedRecord);
            }


            public HexTLVRecordParser GetParserTemplate(Guid templateId)
            {
                return _parentContext.GetParserTemplate(templateId);
            }


            public ParsedRecord CreateRecord(string recordType, HashSet<string> tempFieldNames)
            {
                return _parentContext.CreateRecord(recordType, tempFieldNames);
            }

            public string FileName
            {
                get;
                set;
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

    public class RecordValue
    {
        public int Length { get; set; }
        public byte[] Value { get; set; }
    }

}
