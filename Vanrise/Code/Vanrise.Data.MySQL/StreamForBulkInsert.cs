using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.MySQL
{
    public class StreamForBulkInsert
    {
        string _filePath;
        System.IO.StreamWriter _streamWriter;


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
        }

        public void Close()
        {
            _streamWriter.Close();
            _streamWriter.Dispose();
        }
    }
}
