using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Integration.Entities
{
    public class TextFileImportedData : IImportedData
    {
        public string FileName { get; set; }
        public string Content { get; set; }

        public string Description
        {
            get { return FileName; }
        }
    }

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
    }
}
