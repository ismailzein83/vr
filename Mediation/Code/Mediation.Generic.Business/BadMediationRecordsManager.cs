using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mediation.Generic.Data;
using Mediation.Generic.Entities;
using Vanrise.GenericData.Business;

namespace Mediation.Generic.Business
{
    public class BadMediationRecordsManager
    {
        public void SaveMediationRecordsToDB(Guid mediationDefinitionId, List<BadRecord> badRecords)
        {
            long startingId;
            Vanrise.Common.Business.IDManager.Instance.ReserveIDRange(typeof(Mediation.Generic.Entities.BadRecord), badRecords.Count, out startingId);
            foreach (var r in badRecords)
            {
                r.EventId = startingId++;
            }
            IBadMediationRecordsDataManager dataManager = MediationGenericDataManagerFactory.GetDataManager<IBadMediationRecordsDataManager>();
            dataManager.SaveBadMediationRecordsToDB(badRecords);
        }
    }
}
