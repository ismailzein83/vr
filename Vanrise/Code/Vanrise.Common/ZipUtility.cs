using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Text.RegularExpressions;
using Vanrise.Entities;
using System.IO.Compression;

namespace Vanrise.Common
{
    public class ZipUtility
    {
        #region public Methods
        public static byte[] UnZip(byte[] p)
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

        public static byte[] DecompressGZ(Stream gzipStream)
        {
            using (GZipStream stream = new GZipStream(gzipStream, CompressionMode.Decompress))
            {
                const int size = 4096;
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
                    return memory.ToArray();
                }
            }
        }


        public byte[] Zip(ZipFileInfo fileInfo)
        {
            MemoryStream memStream = new MemoryStream();
            using (ZipOutputStream s = new ZipOutputStream(memStream))
            {
                s.SetLevel(9); // highest level of compression
                ZipEntry entry = new ZipEntry(CleanFileName(fileInfo.FileName))
                {
                    DateTime = DateTime.Today
                };

                s.PutNextEntry(entry);
                s.Write(fileInfo.Content, 0, fileInfo.Content.Length);
            }
            return memStream.GetBuffer();
        }
        public MemoryStream ZipFiles(IEnumerable<ZipFileInfo> attachements)
        {
            MemoryStream memStream = new MemoryStream();
            ZipOutputStream s = new ZipOutputStream(memStream);
            {
                s.SetLevel(9);
                foreach (var attach in attachements)
                {
                    if (attach != null)
                    {
                        var buffer = attach.Content;

                        ZipEntry entry = new ZipEntry(CleanFileName(attach.FileName))
                        {
                            DateTime = DateTime.Today
                        };
                        s.PutNextEntry(entry);
                        s.Write(buffer, 0, buffer.Length);
                    }
                }
                s.Finish();
            }
            return memStream;
        }
        #endregion
        #region private Methods

        private string CleanFileName(string fileName)
        {
            string rgPattern = @"[\\\/:\*\?""'<>|]";
            Regex regex = new Regex(rgPattern);
            return regex.Replace(fileName, "");
        }
        #endregion
    }
}
