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

        public long Size { get; set; }

        public string Description
        {
            get { return FileName; }
        }


        public long? BatchSize
        {
            get { return this.Size; }
        }
    }
}
