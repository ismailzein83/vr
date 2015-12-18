using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities.GenericDataRecord
{
    public class DataRecordType
    {
        public int DataRecordTypeId { get; set; }

        public string Name { get; set; }

        public int ParentId { get; set; }

        public List<DataRecordField> Fields { get; set; }
    }
}
