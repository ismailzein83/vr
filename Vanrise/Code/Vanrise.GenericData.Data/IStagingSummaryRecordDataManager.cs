using System;
using System.Collections.Generic;
using Vanrise.GenericData.Entities;
using Vanrise.Reprocess.Entities;

namespace Vanrise.GenericData.Data
{
    public interface IStagingSummaryRecordDataManager :IDataManager,  Vanrise.Data.IBulkApplyDataManager<StagingSummaryRecord>
    {
        void GetStagingSummaryRecords(long processInstanceId, string stageName, Action<StagingSummaryRecord> onItemLoaded);
        void ApplyStreamToDB(object stream);
        void DeleteStagingSummaryRecords(long processInstanceId, string stageName);
        List<StageRecordInfo> GetStageRecordInfo(long processInstanceId, string stageName);
    }
}

