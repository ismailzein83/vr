using CDRComparison.Entities;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace CDRComparison.Business
{
    public abstract class ReadFromFileContext : IReadFromFileContext, IDisposable
    {
        VRFile _file;

        byte[] _content;
        public byte[] FileContent
        {
            get
            {
                if(_content == null)
                {
                    if (_file.Content == null)
                        throw new NullReferenceException("_file.Content");
                    //try
                    //{

                    //    _content = UnZip(_file.Content);
                    //}
                    //catch
                    //{
                        _content = _file.Content;
                    //}
                }
                return _content;
            }
        }

        private byte[] UnZip(byte[] p)
        {
            byte[] unzippedBytes = null;
            using (ZipInputStream s = new ZipInputStream(new MemoryStream(p)))
            {
                ZipEntry theEntry;
                if ((theEntry = s.GetNextEntry()) != null)
                {
                    using (MemoryStream outputStream = new MemoryStream())
                    {

                        int size = 2048;
                        byte[] data = new byte[2048];
                        while (true)
                        {
                            size = s.Read(data, 0, data.Length);
                            if (size > 0)
                            {
                                outputStream.Write(data, 0, size);
                            }
                            else
                            {
                                break;
                            }
                        }
                        unzippedBytes = outputStream.ToArray();
                    }
                }
            }
            return unzippedBytes;
        }

        StreamReader _strReader;
        StreamReader StreamReader
        {
            get
            {
                if (_strReader == null)
                    _strReader = new StreamReader(new MemoryStream(this.FileContent));
                return _strReader;
            }
        }

        public ReadFromFileContext(VRFile file)
        {
            if (file == null)
                throw new ArgumentNullException("file");
            this._file = file;
        }

        public bool TryReadLine(out string line)
        {
            line = this.StreamReader.ReadLine();
            if (line != null)
                return true;
            else
                return false;
        }

        public void Dispose()
        {
            if (_strReader != null)
            {
                _strReader.Close();
                _strReader.Dispose();
            }
        }
    }
}
