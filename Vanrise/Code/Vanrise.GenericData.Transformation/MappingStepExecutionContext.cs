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

        public DataRecord DefaultRecord { get; set; }

        public Dictionary<string, DataRecord> GetAllDataRecords()
        {
            if (!this.DataRecords.ContainsKey("Default"))
                this.DataRecords.Add("Default", this.DefaultRecord);
            return this.DataRecords;
        }

        public DataRecord GetDataRecord(string recordName)
        {
            if (recordName == null)
                return this.DefaultRecord;
            else
            {
                DataRecord record;
                if (this.DataRecords.TryGetValue(recordName, out record))
                    return record;
                else
                    throw new Exception(string.Format("Record '{0}' not found", recordName));
            }
        }
    }    
}
