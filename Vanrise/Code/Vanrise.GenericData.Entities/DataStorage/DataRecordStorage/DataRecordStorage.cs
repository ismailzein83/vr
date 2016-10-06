using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public class DataRecordStorage
    {
        public Guid DataRecordStorageId { get; set; }

        public string Name { get; set; }

        public Guid DataRecordTypeId { get; set; }

        public Guid DataStoreId { get; set; }

        public DataRecordStorageSettings Settings { get; set; }

        public DataRecordStorageState State { get; set; }
    }
}
