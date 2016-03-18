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
    public class ReadCDRsFromFileContext : IReadCDRsFromFileContext, IDisposable
    {
        VRFile _file;

        Action<IEnumerable<CDR>> _onCDRsReceived;

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
        public bool TryReadLine(out string line)
        {
            line = this.StreamReader.ReadLine();
            if (line != null)
                return true;
            else
                return false;
        }

        public ReadCDRsFromFileContext(VRFile file, Action<IEnumerable<CDR>> onCDRsReceived)
        {
            if (file == null)
                throw new ArgumentNullException("file");
            if (onCDRsReceived == null)
                throw new ArgumentNullException("onCDRsReceived");
            this._file = file;
            this._onCDRsReceived = onCDRsReceived;
        }
        public void OnCDRsReceived(IEnumerable<CDR> cdrs)
        {
            this._onCDRsReceived(cdrs);
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
