using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Integration.Entities
{
    public class StreamReaderImportedData : IImportedData
    {

        public DateTime Accessed { get; set; }

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }

        public string Name { get; set; }

        public long Size { get; set; }

        public object Type { get; set; }


        public StreamReader StreamReader { get; set; }

        public string Description
        {
            get { return null; }
        }


        public long? BatchSize
        {
            get { return Size; }
        }


        public void OnDisposed()
        {
            if(this.StreamReader != null)
            {
                this.StreamReader.Close();
            }
        }
    }
}
