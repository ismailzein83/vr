using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public class DataRecordField
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int DataRecordTypeID { get; set; }
        public DataRecordFieldType Type { get; set; }
    }
}
