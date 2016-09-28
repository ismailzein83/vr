using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities.GenericDataRecord
{
    public class DataRecordField
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public Guid DataRecordTypeID { get; set; }
        public DataRecordFieldType Type { get; set; }
    }
}
