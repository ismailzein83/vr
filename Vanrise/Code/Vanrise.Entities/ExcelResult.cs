using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class ExcelResult<T> : IDataRetrievalResult<T>
    {
        public MemoryStream ExcelFileStream { get; set; }
    }
}
