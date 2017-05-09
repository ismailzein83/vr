using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.DataParser.Business
{
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

    }
}
