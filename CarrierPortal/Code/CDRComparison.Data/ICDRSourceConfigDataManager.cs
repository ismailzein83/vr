using CDRComparison.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDRComparison.Data
{
    public interface ICDRSourceConfigDataManager : IDataManager
    {
        IEnumerable<CDRSourceConfig> GetCDRSourceConfigs();
        bool AreCDRSourceConfigsUpdated(ref object updateHandle);
        bool InsertCDRSourceConfig(CDRSourceConfig cdrSourceConfig, out int insertedObjectId);
    }
}
