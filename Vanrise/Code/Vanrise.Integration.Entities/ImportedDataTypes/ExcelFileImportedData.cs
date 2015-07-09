using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Integration.Entities
{
    public class ExcelFileImportedData : IImportedData
    {
        public byte[] Content { get; set; }

        public string Description
        {
            get { return null; }
        }
    }
}
