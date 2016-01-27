using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Transformation.Entities
{
    public interface IMappingStepExecutionContext
    {
        Dictionary<string, DataRecord> GetAllDataRecords();

        DataRecord GetDataRecord(string recordName);
    }
}
