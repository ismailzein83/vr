using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.SQL
{
    public class StreamForBulkInsert
    {
        string _filePath;
        System.IO.StreamWriter _streamWriter;

        int _recordsCount;
        public int RecordCount
        {
            get
            {
                return _recordsCount;
            }
        }

        internal StreamForBulkInsert(string filePath)
        {
            _filePath = filePath;
            _streamWriter = new System.IO.StreamWriter(filePath);
        }
        public string FilePath
        {
            get
            {
                return _filePath;
            }
        }

        public void WriteRecord(string format, params object[] args)
        {
            this._streamWriter.WriteLine(format, args);
            _recordsCount++;
        }
        public void WriteRecord(string value)
        {
            this._streamWriter.WriteLine(value);
            _recordsCount++;
        }

        bool _isClosed;
        public void Close()
        {
            _streamWriter.Close();
            _streamWriter.Dispose();
            _isClosed = true;
        }

        internal bool IsClosed
        {
            get
            {
                return _isClosed;
            }
        }
    }
}
