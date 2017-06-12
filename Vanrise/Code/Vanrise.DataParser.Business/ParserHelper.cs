using System;
using System.Collections.Generic;
using System.IO;
using Vanrise.Common;
using Vanrise.DataParser.Entities;

namespace Vanrise.DataParser.Business
{
    public class ParserHelper
    {
        public static void ExecuteParser(Stream stream, Guid parserTypeId, Action<ParsedBatch> onBatchParsed)
        {
            ParserType parserType = new ParserTypeManager().GetParserType(parserTypeId);
            Action<ParsedRecord> onRecordParsed = (parsedRecord) =>
            {

            };
            ParserTypeExecuteContext context = new ParserTypeExecuteContext(onRecordParsed)
            {
                Input = new StreamDataParserInput
                {
                    Stream = stream
                }
            };
            parserType.Settings.ExtendedSettings.Execute(context);
            foreach (var parsedRecords in context.ParsedRecords)
            {
                onBatchParsed(new ParsedBatch
                {
                    RecordType = parsedRecords.Key,
                    Records = parsedRecords.Value
                });
            }
        }
        public static int ByteToNumber(byte[] bytes)
        {
            int pos = 0;
            int result = 0;
            foreach (byte by in bytes)
            {
                result |= (int)(by << pos);
                pos += 8;
            }
            return result;
        }
        public static byte[] SplitByteToNibble(byte data)
        {
            byte[] nibbles = new byte[2];
            nibbles[0] = (byte)((data & 0xF0) >> 4);
            nibbles[1] = (byte)(data & 0x0F);
            return nibbles;
        }
        public static Int32 HexToInt32(string hexValue)
        {
            Int32 value = 0;
            Int32.TryParse(hexValue.Trim(), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out value);
            return value;
        }
        public static decimal HexToDecimal(string hexValue)
        {
            decimal value = 0;
            Decimal.TryParse(hexValue.Trim(), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out value);
            return value;
        }
        public static Int64 HexToInt64(string hexValue)
        {
            Int64 value = 0;
            Int64.TryParse(hexValue.Trim(), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out value);
            return value;
        }
        public static int GetInt(byte[] data, int offset, int length)
        {
            var result = 0;
            for (var i = 0; i < length; i++)
            {
                result = (result << 8) | data[offset + i];
            }

            return result;
        }
        public static string GetHexFromInt(int value)
        {
            return value.ToString("X2");
        }
        public static string GetHexFromByte(byte value)
        {
            return value.ToString("X2");
        }
        public static string ByteArrayToString(byte[] ba)
        {
            string hex = BitConverter.ToString(ba);
            return hex.Replace("-", "");
        }

    }
    public class ParserTypeExecuteContext : IParserTypeExecuteContext
    {
        Dictionary<string, List<FldDictParsedRecord>> _parsedRecords;

        Action<ParsedRecord> _OnRecordParsed;
        public Dictionary<string, List<FldDictParsedRecord>> ParsedRecords { get { return _parsedRecords; } }
        public ParserTypeExecuteContext(Action<ParsedRecord> onRecordParsed)
        {
            _OnRecordParsed = onRecordParsed;
            _parsedRecords = new Dictionary<string, List<FldDictParsedRecord>>();
        }
        public IDataParserInput Input
        {
            get;
            set;
        }
        public void OnRecordParsed(ParsedRecord parsedRecord)
        {
            FldDictParsedRecord fldParsedRecord = parsedRecord as FldDictParsedRecord;
            fldParsedRecord.ThrowIfNull("fldParsedRecord", "");
            _parsedRecords.GetOrCreateItem(fldParsedRecord.RecordName, () => new List<FldDictParsedRecord>()).Add(fldParsedRecord);

            _OnRecordParsed(parsedRecord);
        }


        public ParsedRecord CreateRecord(string recordType, HashSet<string> tempFieldNames)
        {
            FldDictParsedRecord parsedRecord = new FldDictParsedRecord
            {
                RecordName = recordType,
                TempFieldNames = tempFieldNames
            };
            return parsedRecord;
        }
    }
}
