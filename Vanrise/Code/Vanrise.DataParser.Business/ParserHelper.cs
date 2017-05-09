using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.DataParser.Business.HexTLV;
using Vanrise.DataParser.Entities;
using Vanrise.DataParser.Entities.HexTLV;

namespace Vanrise.DataParser.Business
{
    public class ParserHelper
    {
        public static void EvaluteParser(Dictionary<string, HexTLVTagType> tagTypes, string tag, byte[] fieldData, ParsedRecord record)
        {
            HexTLVTagType hexTLVTagType;
            if (tagTypes.TryGetValue(tag, out hexTLVTagType))
            {
                TagValueParserExecuteContext tagValueParserExecuteContext = new TagValueParserExecuteContext
                {
                    TagValue = fieldData,
                    Record = record
                };
                hexTLVTagType.ValueParser.Execute(tagValueParserExecuteContext);
            }
        }

        public static byte[] ParseData(MemoryStream stream, out string tag, ref int position)
        {
            tag = ParserHelper.GetStringValue(stream, 1);
            position++;
            int recordLength = ParserHelper.GetIntValue(stream, 1);
            position++;
            position += recordLength;
            byte[] fieldData = new byte[recordLength];
            stream.Read(fieldData, 0, recordLength);
            return fieldData;
        }
        private static byte[] Read(System.IO.Stream stream, int byteCount)
        {
            byte[] buffer = new byte[byteCount];
            int read = stream.Read(buffer, 0, byteCount);
            if (read < byteCount) throw new Exception("Expected Bytes: " + byteCount + ", read: " + read);
            return buffer;
        }

        public static int GetIntValue(Stream stream, int length)
        {
            return ByteToNumber(ParserHelper.Read(stream, length));
        }
        public static string GetStringValue(Stream stream, int length, int startIndex = 0)
        {
            return BitConverter.ToString(ParserHelper.Read(stream, length), startIndex);
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
            nibbles[0] = (byte)(data & 0x0F);
            nibbles[1] = (byte)((data & 0xF0) >> 4);
            return nibbles;
        }
    }
}
