using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.Transformation.Entities;

namespace Vanrise.GenericData.Transformation
{
    public class MappingStepExecutionContext : IMappingStepExecutionContext
    {
        public Dictionary<string, DataRecord> DataRecords { get; set; }

        public DataRecord GetDataRecord(string recordName)
        {
            DataRecord record;
            if (this.DataRecords.TryGetValue(recordName, out record))
                return record;
            else
                throw new Exception(string.Format("Record '{0}' not found", recordName));
        }
    }    
}
