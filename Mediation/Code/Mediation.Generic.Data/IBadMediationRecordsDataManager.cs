using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mediation.Generic.Entities;
using Vanrise.Data;

namespace Mediation.Generic.Data
{
   public interface IBadMediationRecordsDataManager:IDataManager, IBulkApplyDataManager<BadRecord>
    {
       void SaveBadMediationRecordsToDB(List<BadRecord> badRecords);
    }
}
