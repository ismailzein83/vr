using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public class DataRecordStorage
    {
        public int DataRecordStorageId { get; set; }

        public string Name { get; set; }

        public Guid DataRecordTypeId { get; set; }

        public int DataStoreId { get; set; }

        public DataRecordStorageSettings Settings { get; set; }

        public DataRecordStorageState State { get; set; }
    }
}
