using ICSharpCode.SharpZipLib.Zip;
using System;
using System.IO;

namespace Vanrise.Common
{
    public class ZipUtility
    {
        #region public Methods
        public byte[] UnZip(byte[] p)
        {
            byte[] unzippedBytes = null;
            using (ZipInputStream s = new ZipInputStream(new MemoryStream(p)))
            {
                if ((s.GetNextEntry()) != null)
                {
                    using (MemoryStream outputStream = new MemoryStream())
                    {
                        int size;
                        byte[] data = new byte[2048];
                        while (true)
                        {
                            size = s.Read(data, 0, data.Length);
                            if (size > 0)
                                outputStream.Write(data, 0, size);
                            else
                                break;
                        }
                        unzippedBytes = outputStream.ToArray();
                    }
                }
            }
            return unzippedBytes;
        }
        public MemoryStream Zip(byte[] bufferBytes, string fileName)
        {
            MemoryStream memStream = new MemoryStream();
            using (ZipOutputStream s = new ZipOutputStream(memStream))
            {
                s.SetLevel(9); // highest level of compression
                ZipEntry entry = new ZipEntry(fileName)
                {
                    DateTime = DateTime.Today
                };

                s.PutNextEntry(entry);
                s.Write(bufferBytes, 0, bufferBytes.Length);
            }
            return memStream;
        }

        #endregion
    }
}
