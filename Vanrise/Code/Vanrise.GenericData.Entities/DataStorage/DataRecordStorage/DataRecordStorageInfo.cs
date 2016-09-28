using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public class DataRecordStorageInfo
    {
        public int DataRecordStorageId { get; set; }

        public string Name { get; set; }

        public Guid DataRecordTypeId { get; set; }

    }
}
