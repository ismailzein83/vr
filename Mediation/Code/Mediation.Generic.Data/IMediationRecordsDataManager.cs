using System;
using System.Collections.Generic;
using Mediation.Generic.Entities;
using Vanrise.Data;

namespace Mediation.Generic.Data
{
    public interface IMediationRecordsDataManager : IDataManager, IBulkApplyDataManager<MediationRecord>
    {
        void SaveMediationRecordsToDB(List<MediationRecord> mediationRecords);

        IEnumerable<MediationRecord> GetMediationRecords();

        IEnumerable<MediationRecord> GetMediationRecordsByStatus(Guid mediationDefinitionId, EventStatus status);

        IEnumerable<MediationRecord> GetMediationRecordsByIds(Guid mediationDefinitionId, IEnumerable<string> sessionIds);

        bool DeleteMediationRecordsBySessionIds(Guid mediationDefinitionId, IEnumerable<string> sessionIds);

        Guid DataRecordTypeId { set; }
    }

}
