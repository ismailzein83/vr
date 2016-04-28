using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public class DataRecordDetail
    {
        public DateTime RecordTime { get; set; }
        public Dictionary<string, DataRecordFieldValue> FieldValues { get; set; }
    }
}
