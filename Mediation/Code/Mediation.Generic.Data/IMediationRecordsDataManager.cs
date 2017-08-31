using System;
using System.Collections.Generic;
using Mediation.Generic.Entities;
using Vanrise.Data;

namespace Mediation.Generic.Data
{
    public interface IMediationRecordsDataManager : IDataManager, IBulkApplyDataManager<MediationRecord>
    {
        void SaveMediationRecordsToDB(List<MediationRecord> mediationRecords);

        void GetMediationRecordsByStatus(Guid mediationDefinitionId, EventStatus status, Action<string> onSessionIdLoaded);

        IEnumerable<MediationRecord> GetMediationRecordsByIds(Guid mediationDefinitionId, IEnumerable<string> sessionIds);

        bool DeleteMediationRecordsBySessionIds(Guid mediationDefinitionId, IEnumerable<string> sessionIds);

        Guid DataRecordTypeId { set; }
    }

}
