using CDRComparison.Entities;
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
        public byte[] FileContent
        {
            get
            {
                return _file.Content;
            }
        }

        StreamReader _strReader;
        StreamReader StreamReader
        {
            get
            {
                if (_strReader == null)
                    _strReader = new StreamReader(new MemoryStream(this._file.Content));
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
