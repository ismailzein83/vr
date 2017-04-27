using System;
using System.Collections.Generic;
using System.Data;
using Vanrise.Common;
using Vanrise.Data.SQL;
using Vanrise.Queueing.Entities;

namespace Vanrise.Queueing.Data.SQL
{
    public class HoldRequestDataManager : BaseSQLDataManager, IHoldRequestDataManager
    {
        public HoldRequestDataManager()
            : base(GetConnectionStringName("QueueDBConnStringKey", "QueueDBConnString"))
        {
        }

        public List<HoldRequest> GetAllHoldRequests()
        {
            return GetItemsSP("[queue].[sp_HoldRequest_GetAll]", HoldRequestMapper);
        }

        public void Delete(long holdRequestId)
        {
            ExecuteNonQuerySP("[queue].[sp_HoldRequest_Delete]", holdRequestId);
        }

        public void Insert(long BPInstanceID, Guid executionFlowDefinitionId, DateTime from, DateTime to, List<int> queuesToHold, List<int> queuesToProcess, HoldRequestStatus status)
        {
            string serializeQueuesToHold = queuesToHold != null ? Serializer.Serialize(queuesToHold) : null;
            string serializeQueuesToProcess = queuesToProcess != null ? Serializer.Serialize(queuesToProcess) : null;
            ExecuteNonQuerySP("[queue].[sp_HoldRequest_Insert]", BPInstanceID, executionFlowDefinitionId, from, to, serializeQueuesToHold, serializeQueuesToProcess, (int)status);
        }

        public void UpdateStatus(long holdRequestId, HoldRequestStatus status)
        {
            ExecuteNonQuerySP("[queue].[sp_HoldRequest_UpdateStatus]", holdRequestId, (int)status);
        }

        public bool AreHoldRequestsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("queue.HoldRequest", ref updateHandle);
        }

        #region Mappers

        private HoldRequest HoldRequestMapper(IDataReader reader)
        {
            return new HoldRequest
            {
                HoldRequestId = (long)reader["ID"],
                BPInstanceId = (long)reader["BPInstanceID"],
                ExecutionFlowDefinitionId = (Guid)reader["ExecutionFlowDefinitionId"],
                From = (DateTime)reader["From"],
                To = (DateTime)reader["To"],
                QueuesToHold = reader["QueuesToHold"] != DBNull.Value ? Serializer.Deserialize<List<int>>(reader["QueuesToHold"] as string) : null,
                QueuesToProcess = reader["QueuesToProcess"] != DBNull.Value ? Serializer.Deserialize<List<int>>(reader["QueuesToProcess"] as string) : null,
                Status = GetReaderValue<HoldRequestStatus>(reader, "Status"),
                CreatedTime = (DateTime)reader["Createdtime"]
            };
        }
        #endregion
    }
}