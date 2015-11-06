﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Data.StatisticManagement;
using Vanrise.Entities.StatisticManagement;

namespace Vanrise.Common.Data.SQL.StatisticManagement
{
    public class StatisticBatchDataManager : Vanrise.Data.SQL.BaseSQLDataManager, IStatisticBatchDataManager
    {
        public StatisticBatchDataManager()
            : base(GetConnectionStringName("StatisticManagementDBConnStringKey", "StatisticManagementDBConnString"))
        {

        }

        public bool TryLock(int typeId, DateTime batchStart, int currentRuntimeProcessId, IEnumerable<int> runningProcessIds, out StatisticBatchInfo batchInfo, out bool isInfoCorrupted)
        {
            bool isLocked = false;
            string serializedBatchInfo = null;
            bool isInfoCorrupted_Internal = false;

            ExecuteReaderSP("StatisticManagement.sp_StatisticBatch_TryLock", (reader) =>
                {
                    isLocked = true;
                    serializedBatchInfo = reader["BatchInfo"] as string;
                    isInfoCorrupted_Internal = (bool)reader["IsInfoCorrupted"];
                }, typeId, batchStart, currentRuntimeProcessId, runningProcessIds != null ? String.Join(",", runningProcessIds) : null);
            if (serializedBatchInfo != null)
                batchInfo = Serializer.Deserialize<StatisticBatchInfo>(serializedBatchInfo);
            else
                batchInfo = null;
            isInfoCorrupted = isInfoCorrupted_Internal;
            return isLocked;
        }


        public void UnlockBatch(int typeId, DateTime batchStart)
        {
            ExecuteNonQuerySP("[StatisticManagement].[sp_StatisticBatch_UnLock]", typeId, batchStart);
        }

        public void UpdateBatchInfo(int typeId, DateTime batchStart, StatisticBatchInfo batchInfo)
        {
            ExecuteNonQuerySP("[StatisticManagement].[sp_StatisticBatch_UpdateBatchInfo]", typeId, batchStart, Serializer.Serialize(batchInfo));
        }
    }
}
