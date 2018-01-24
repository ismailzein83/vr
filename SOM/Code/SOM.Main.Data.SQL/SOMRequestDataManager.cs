using SOM.Main.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Data.SQL;

namespace SOM.Main.Data.SQL
{
    public class SOMRequestDataManager : BaseSQLDataManager, ISOMRequestDataManager
    {
        #region ctor/Local Variables

        public SOMRequestDataManager()
            : base(GetConnectionStringName("SOMTransaction_DBConnStringKey", "SOMTransactionDBConnString"))
        {

        }

        #endregion

        #region Public Methods

        public void AddRequest(Guid requestId, Guid requestTypeId, string entityId, string title, string serializedSettings)
        {
            ExecuteNonQuerySP("SOM.sp_SOMRequest_Insert", requestId, requestTypeId, entityId, title, serializedSettings);
        }

        public void UpdateRequestProcessInstanceId(Guid requestId, long processInstanceId)
        {
            ExecuteNonQuerySP("SOM.sp_SOMRequest_UpdateProcessInstanceID", requestId, processInstanceId);
        }


        public long? GetRequestProcessInstanceId(Guid somRequestId)
        {
            Object processInstanceIdAsObj = ExecuteScalarSP("SOM.sp_SOMRequest_GetProcessInstanceId ", somRequestId);
            return processInstanceIdAsObj != null ? (long)processInstanceIdAsObj : (long?)null;
        }


        public List<SOMRequestHeader> GetRecentSOMRequestHeaders(string entityId, int nbOfRecords, long? lessThanSequenceNb)
        {
            return GetItemsSP("SOM.sp_SOMRequest_GetRecentHeadersByEntityID", SOMRequestHeaderMapper, entityId, nbOfRecords, lessThanSequenceNb);
        }

        #endregion

        #region Private Methods

        private SOMRequestHeader SOMRequestHeaderMapper(IDataReader reader)
        {
            return new SOMRequestHeader
            {
                SOMRequestId = (Guid)reader["RequestID"],
                SequenceNumber = GetReaderValue<long>(reader, "SequenceNumber"),
                RequestTypeId = (Guid)reader["RequestTypeID"],
                EntityId = reader["EntityID"] as string,
                Title = reader["Title"] as string,
                ProcessInstanceId = GetReaderValue<long?>(reader, "ProcessInstanceID"),
                Status = MapBPInstanceStatus(reader["ExecutionStatus"] != DBNull.Value ? (BPInstanceStatus)reader["ExecutionStatus"] : BPInstanceStatus.New),
                CreatedTime = GetReaderValue<DateTime>(reader, "CreatedTime"),
                LastModifiedTime = GetReaderValue<DateTime>(reader, "StatusUpdatedTime")
            };
        }


        private SOMRequestStatus MapBPInstanceStatus(BPInstanceStatus bpInstanceStatus)
        {
            switch (bpInstanceStatus)
            {
                case BPInstanceStatus.New:
                case BPInstanceStatus.Postponed:
                    return SOMRequestStatus.New;
                case BPInstanceStatus.Waiting:
                    return SOMRequestStatus.Waiting;
                case BPInstanceStatus.Running:
                    return SOMRequestStatus.Running;
                case BPInstanceStatus.Completed:
                    return SOMRequestStatus.Completed;
                case BPInstanceStatus.Aborted:
                case BPInstanceStatus.Suspended:
                case BPInstanceStatus.Terminated:
                    return SOMRequestStatus.Aborted;
                default: throw new NotSupportedException(String.Format("bpInstanceStatus '{0}'", bpInstanceStatus.ToString()));
            }
        }

        #endregion
    }
}
