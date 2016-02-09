using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Transformation.Entities
{
    public class DataTransformationExecutionOutput
    {
        public dynamic DataRecords { get; set; }

        public dynamic GetRecordValue(string recordName)
        {
            return this.DataRecords.GetType().GetProperty(recordName).GetValue(this.DataRecords, null);
        }
    }
}
