using System;
using System.Collections.Generic;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Data
{
    public interface IStagingSummaryRecordDataManager : IDataManager, Vanrise.Data.IBulkApplyDataManager<StagingSummaryRecord>
    {
        void GetStagingSummaryRecords(long processInstanceId, string stageName, DateTime batchStart, Action<StagingSummaryRecord> onItemLoaded);
        void ApplyStreamToDB(object stream);
        void DeleteStagingSummaryRecords(long processInstanceId, string stageName, DateTime batchStart);
        List<StagingSummaryInfo> GetStagingSummaryInfo(long processInstanceId, string stageName);
    }
}

