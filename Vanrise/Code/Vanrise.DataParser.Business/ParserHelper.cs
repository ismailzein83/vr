using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Vanrise.Common;
using Vanrise.DataParser.Entities;
using System.Linq;
using Vanrise.GenericData.Entities;

namespace Vanrise.DataParser.Business
{
    public class ParserHelper
    {
        public static void ExecuteParser(Stream stream, string fileName, Guid parserTypeId, Action<ParsedBatch> onParsedBatch)
        {
            ParserType parserType = new ParserTypeManager().GetParserType(parserTypeId);
            Action<ParsedRecord> onRecordParsed = (parsedRecord) =>
            {

            };
            ParserTypeExecuteContext context = new ParserTypeExecuteContext(onRecordParsed)
            {
                Input = new StreamDataParserInput
                {
                    Stream = stream,
                    FileName = fileName
                }
            };
            parserType.Settings.ExtendedSettings.Execute(context);

            foreach (var parsedRecords in context.ParsedRecords)
            {
                Vanrise.GenericData.Business.DataRecordTypeManager dataRecordTypeManager = new Vanrise.GenericData.Business.DataRecordTypeManager();
                Type dataRecordRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType(parsedRecords.Key);
                List<dynamic> records = new List<dynamic>();

                foreach (Vanrise.DataParser.Entities.FldDictParsedRecord item in parsedRecords.Value)
                {
                    dynamic record = Activator.CreateInstance(dataRecordRuntimeType) as dynamic;
                    record.FillDataRecordTypeFromDictionary(item.FieldValues);
                    records.Add(record);
                }
                onParsedBatch(new ParsedBatch
                {
                    Records = records,
                    RecordType = parsedRecords.Key
                });
            }
        }
        public static void ExecuteParser(Stream stream, string fileName, Guid parserTypeId, ExecuteParserOptions options, Action<ParsedBatch> onParsedBatch)
        {
            ParserType parserType = new ParserTypeManager().GetParserType(parserTypeId);
            Action<ParsedRecord> onRecordParsed = (parsedRecord) =>
            {

            };
            ParserTypeExecuteContext context = new ParserTypeExecuteContext(onRecordParsed)
            {
                Input = new StreamDataParserInput
                {
                    Stream = stream,
                    FileName = fileName
                }
            };

            parserType.Settings.ExtendedSettings.Execute(context);

            foreach (var parsedRecords in context.ParsedRecords)
            {
                Vanrise.GenericData.Business.DataRecordTypeManager dataRecordTypeManager = new Vanrise.GenericData.Business.DataRecordTypeManager();
                Type dataRecordRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType(parsedRecords.Key);
                DataRecordType recordType = dataRecordTypeManager.GetDataRecordType(parsedRecords.Key);

                long startingId;
                var dataRecordVanriseType = new Vanrise.GenericData.Entities.DataRecordVanriseType(parsedRecords.Key);

                Vanrise.Common.Business.IDManager.Instance.ReserveIDRange(dataRecordVanriseType, parsedRecords.Value.Count, out startingId);
                List<dynamic> records = new List<dynamic>();
                long currentId = startingId;
                foreach (Vanrise.DataParser.Entities.FldDictParsedRecord item in parsedRecords.Value)
                {
                    dynamic record = Activator.CreateInstance(dataRecordRuntimeType) as dynamic;
                    if (options.GenerateIds && !string.IsNullOrEmpty(recordType.Settings.IdField))
                        item.SetFieldValue(recordType.Settings.IdField, currentId);
                    record.FillDataRecordTypeFromDictionary(item.FieldValues);
                    currentId++;
                    records.Add(record);
                }
                onParsedBatch(new ParsedBatch
                {
                    Records = records,
                    RecordType = parsedRecords.Key
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
        public static string ByteArrayToString(byte[] ba, bool reverse)
        {
            string hex = BitConverter.ToString(reverse ? ba.Reverse().ToArray() : ba);
            return hex.Replace("-", "");
        }

        public static byte[] ExtractFromByteArray(byte[] ba, int startIndex, int length, bool reverse)
        {
            byte[] result = new byte[length];
            Array.Copy(ba, startIndex, result, 0, length);
            return !reverse ? result : result.Reverse().ToArray();
        }

        public static string GetBCDNumber(byte[] data, bool removeHexa, bool aIsZero)
        {
            StringBuilder number = new StringBuilder();
            foreach (var byteItem in data)
            {
                byte[] nibbles = ParserHelper.SplitByteToNibble(byteItem);
                for (int i = 0; i < nibbles.Length; i++)
                {
                    int val = (int)nibbles[i];
                    number.Append(GetNumberValue(val, removeHexa, aIsZero));
                }
            }
            return number.ToString();
        }
        public static string GetTBCDNumber(byte[] data, bool removeHexa, bool aIsZero)
        {
            StringBuilder number = new StringBuilder();
            foreach (var byteItem in data)
            {
                byte[] nibbles = ParserHelper.SplitByteToNibble(byteItem);
                for (int i = nibbles.Length - 1; i >= 0; i--)
                {
                    int val = (int)nibbles[i];
                    number.Append(GetNumberValue(val, removeHexa, aIsZero));
                }
            }
            return number.ToString();
        }
        static string GetNumberValue(int val, bool removeHexa, bool aIsZero)
        {
            if (aIsZero && val == 10)
                return "0";
            else if (aIsZero && (val > 10 || val == 0) || (removeHexa && val > 9))
                return "";
            return val.ToString();
        }

        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .Reverse().ToArray();
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
    public class ExecuteParserOptions
    {
        public bool GenerateIds { get; set; }
    }
}
