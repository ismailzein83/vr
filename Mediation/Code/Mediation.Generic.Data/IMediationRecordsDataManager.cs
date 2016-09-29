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

        IEnumerable<MediationRecord> GetMediationRecordsByStatus(int mediationDefinitionId, EventStatus status);

        IEnumerable<MediationRecord> GetMediationRecordsByIds(int mediationDefinitionId, IEnumerable<string> sessionIds);

        bool DeleteMediationRecordsBySessionIds(int mediationDefinitionId, IEnumerable<string> sessionIds);

        Guid DataRecordTypeId { set; }
    }

}
