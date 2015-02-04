using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace Vanrise.Common
{
    public static class Compressor
    {
        public static byte[] Compress(byte[] data)
        {
            byte[] compressedData = null;
            using (MemoryStream memory = new MemoryStream())
            {
                using (GZipStream gzip = new GZipStream(memory,  CompressionLevel.Optimal))
                {
                    gzip.Write(data, 0, data.Length);
                }
                compressedData = memory.ToArray();
            }
            return compressedData;
        }

        public static byte[] Decompress(byte[] compressedData)
        {
            byte[] data = null;
            const int size = 4096;
            using (GZipStream stream = new GZipStream(new MemoryStream(compressedData), CompressionMode.Decompress))
            {

                byte[] buffer = new byte[size];
                using (MemoryStream memory = new MemoryStream())
                {
                    int count = 0;
                    do
                    {
                        count = stream.Read(buffer, 0, size);
                        if (count > 0)
                        {
                            memory.Write(buffer, 0, count);
                        }
                    }
                    while (count > 0);
                    data = memory.ToArray();
                }
            }
            return data;
        }
    }
}
