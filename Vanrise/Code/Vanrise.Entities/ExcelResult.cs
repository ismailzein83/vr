using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class ExcelResult
    {
        public MemoryStream ExcelFileStream { get; set; }
    }

    public class ExcelResult<T> : ExcelResult, IDataRetrievalResult<T>
    {
        public MemoryStream ExcelFileStream { get; set; }
    }
}
