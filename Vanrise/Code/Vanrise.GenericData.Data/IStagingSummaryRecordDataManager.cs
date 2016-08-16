using System;
using System.Collections.Generic;
using Vanrise.GenericData.Entities;
using Vanrise.Reprocess.Entities;

namespace Vanrise.GenericData.Data
{
    public interface IStagingSummaryRecordDataManager : IDataManager, Vanrise.Data.IBulkApplyDataManager<StagingSummaryRecord>
    {
        void GetStagingSummaryRecords(long processInstanceId, string stageName, DateTime batchStart, Action<StagingSummaryRecord> onItemLoaded);
        void ApplyStreamToDB(object stream);
        void DeleteStagingSummaryRecords(long processInstanceId, string stageName, DateTime batchStart);
        List<StageRecordInfo> GetStageRecordInfo(long processInstanceId, string stageName);
    }
}

