using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Integration.Entities
{
    public class DBReaderImportedData : IImportedData
    {
        public IDataReader Reader { get; set; }

        public string Description
        {
            get { return null; }
        }


        public long? BatchSize
        {
            get { return null ; }
        }
    }
}
