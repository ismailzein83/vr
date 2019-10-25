using System;
using System.Collections.Generic;
using Vanrise.GenericData.Data;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Business
{
    public class StagingSummaryRecordManager
    {
        public void GetStagingSummaryRecords(long processInstanceId, string stageName, DateTime batchStart, DateTime batchEnd, Action<StagingSummaryRecord> onItemLoaded)
        {
            IStagingSummaryRecordDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IStagingSummaryRecordDataManager>();
            dataManager.GetStagingSummaryRecords(processInstanceId, stageName, batchStart, batchEnd, onItemLoaded);
        }

        public void DeleteStagingSummaryRecords(long processInstanceId, string stageName, DateTime batchStart, DateTime batchEnd)
        {
            IStagingSummaryRecordDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IStagingSummaryRecordDataManager>();
            dataManager.DeleteStagingSummaryRecords(processInstanceId, stageName, batchStart, batchEnd);
        }

        public void DeleteStagingSummaryRecords(long processInstanceId)
        {
            IStagingSummaryRecordDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IStagingSummaryRecordDataManager>();
            dataManager.DeleteStagingSummaryRecords(processInstanceId);
        }

        public List<StagingSummaryInfo> GetStagingSummaryInfo(long processInstanceId, string stageName)

        {
            IStagingSummaryRecordDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IStagingSummaryRecordDataManager>();
            return dataManager.GetStagingSummaryInfo(processInstanceId, stageName);
        }
    }
}