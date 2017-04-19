using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Vanrise.Common;
using System.Threading.Tasks;
using Vanrise.DataParser.Entities.HexTLV;

namespace Vanrise.DataParser.MainExtensions.HexTLV.RecordReaders
{
    public class TagRecordReader : RecordReader
    {
        public override Guid ConfigId
        {
            get { return new Guid("1F558330-45F4-4C00-8E5C-4B51ED8F6349"); }
        }
        public Dictionary<string, TagRecordType> RecordTypesByTag { get; set; }
        public int NumberOfBytesToSkip { get; set; }
        public override void Execute(IRecordReaderExecuteContext context)
        {
            MemoryStream stream = new MemoryStream(context.Data);
            int position = 0;
            while (position < stream.Length)
            {
                string tag = ParserHelper.GetStringValue(stream, 1);
                position++;
                stream.SkipBytes(NumberOfBytesToSkip);
                position += NumberOfBytesToSkip;
                int recordLength = ParserHelper.GetIntValue(stream, 1);
                position += recordLength;
                TagRecordType tagRecordType;
                byte[] recordData = new byte[recordLength];
                stream.Read(recordData, 0, recordLength);

                if (RecordTypesByTag.TryGetValue(tag, out tagRecordType))
                {
                    context.OnRecordRead(tagRecordType.RecordType, recordData, tagRecordType.TagTypes);
                }
            }
        }
    }

    public class TagRecordType
    {
        public string RecordType { get; set; }

        public Dictionary<string, HexTLVTagType> TagTypes { get; set; }
    }

    public class ParserHelper
    {
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

        static int ByteToNumber(byte[] bytes)
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

    }
}
