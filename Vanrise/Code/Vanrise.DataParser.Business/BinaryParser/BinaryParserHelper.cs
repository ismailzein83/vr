using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Vanrise.Common;
using Vanrise.DataParser.Entities;

namespace Vanrise.DataParser.Business
{
    public static class BinaryParserHelper
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

        public static void ExecuteRecordParser(BinaryRecordParser subRecordsParser, Stream recordStream, IBinaryRecordParserContext parentRecordContext)
        {
            subRecordsParser.Settings.ThrowIfNull("subRecordParser.Settings");
            var subRecordContext = new SubRecordBinaryRecordParserContext(recordStream, parentRecordContext);
            subRecordsParser.Settings.Execute(subRecordContext);
        }

        public static void ExecuteFieldParser(BinaryFieldParserSettings fieldParser, ParsedRecord parsedRecord, byte[] fieldValue)
        {
            fieldParser.ThrowIfNull("fieldParsers");

            var fieldParserContext = new BinaryFieldParserContext { Record = parsedRecord, FieldValue = fieldValue };
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
                          var fieldParserContext = new BinaryFieldParserContext { Record = parsedRecord, FieldValue = recordType.Value };
                          positionedFieldParser.FieldParser.Settings.Execute(fieldParserContext);
                      });
            }
        }

        public static byte[] Combine(byte[] first, byte[] second)
        {
            byte[] ret = new byte[first.Length + second.Length];
            Buffer.BlockCopy(first, 0, ret, 0, first.Length);
            Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);
            return ret;
        }


        #region Private Classes

        private class SubRecordBinaryRecordParserContext : IBinaryRecordParserContext
        {
            Stream _recordStream;
            string _fileName;
            Guid _dataSourceId;
            IBinaryRecordParserContext _parentContext;

            public Stream RecordStream { get { return _recordStream; } }

            public string FileName { get { return _fileName; } }

            public Guid DataSourceId { get { return _dataSourceId; } }

            public SubRecordBinaryRecordParserContext(Stream recordStream, IBinaryRecordParserContext parentContext)
            {
                recordStream.ThrowIfNull("recordData");
                parentContext.ThrowIfNull("parentContext");
                _parentContext = parentContext;
                _recordStream = recordStream;
                _fileName = parentContext.FileName;
                _dataSourceId = parentContext.DataSourceId;
            }

            public void OnRecordParsed(ParsedRecord parsedRecord)
            {
                _parentContext.OnRecordParsed(parsedRecord);
            }

            public BinaryRecordParser GetParserTemplate(Guid templateId)
            {
                return _parentContext.GetParserTemplate(templateId);
            }

            public ParsedRecord CreateRecord(string recordType, HashSet<string> tempFieldNames)
            {
                return _parentContext.CreateRecord(recordType, tempFieldNames);
            }

            public Dictionary<string, dynamic> GetGlobalVariables()
            {
                return _parentContext.GetGlobalVariables();
            }

            public void SetGlobalVariable(string variableName, dynamic value)
            {
                _parentContext.SetGlobalVariable(variableName, value);
            }
        }

        #endregion
    }

    public class RecordValue
    {
        public int Length { get; set; }
        public byte[] Value { get; set; }
    }
}