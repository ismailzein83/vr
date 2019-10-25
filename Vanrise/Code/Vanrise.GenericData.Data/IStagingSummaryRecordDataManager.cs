﻿using System;
using System.Collections.Generic;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Data
{
    public interface IStagingSummaryRecordDataManager : IDataManager, Vanrise.Data.IBulkApplyDataManager<StagingSummaryRecord>
    {
        void GetStagingSummaryRecords(long processInstanceId, string stageName, DateTime batchStart, DateTime batchEnd, Action<StagingSummaryRecord> onItemLoaded); 
        void ApplyStreamToDB(object stream);
        void DeleteStagingSummaryRecords(long processInstanceId, string stageName, DateTime batchStart, DateTime batchEnd); 
        void DeleteStagingSummaryRecords(long processInstanceId);
        List<StagingSummaryInfo> GetStagingSummaryInfo(long processInstanceId, string stageName);
    }
}