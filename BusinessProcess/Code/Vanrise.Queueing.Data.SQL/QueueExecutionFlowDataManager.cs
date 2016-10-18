using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Data.SQL;
using Vanrise.Queueing.Entities;

namespace Vanrise.Queueing.Data.SQL
{
    public class QueueExecutionFlowDataManager : BaseSQLDataManager, IQueueExecutionFlowDataManager
    {
        public QueueExecutionFlowDataManager()
            : base(GetConnectionStringName("QueueingConfigDBConnStringKey", "QueueingConfigDBConnString"))
        {
        }



        #region Public Methods

        public List<QueueExecutionFlow> GetExecutionFlows()
        {
            return GetItemsSP("queue.sp_ExecutionFlow_GetAll", ExecutionFlowMapper);
        }

        public bool AddExecutionFlow(QueueExecutionFlow executionFlow)
        {

            int recordesEffected = ExecuteNonQuerySP("queue.sp_ExecutionFlow_Insert", executionFlow.ExecutionFlowId, executionFlow.Name, executionFlow.DefinitionId);

            return (recordesEffected > 0);
        }

        public bool AreExecutionFlowsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("queue.ExecutionFlow", ref updateHandle);
        }


        public bool UpdateExecutionFlow(QueueExecutionFlow executionFlowObject)
        {
            int recordesEffected = ExecuteNonQuerySP("queue.sp_ExecutionFlow_Update", executionFlowObject.ExecutionFlowId, executionFlowObject.Name);
            return (recordesEffected > 0);
        }
        #endregion


        #region Private Methods

        private QueueExecutionFlow ExecutionFlowMapper(IDataReader reader)
        {
            return new QueueExecutionFlow
            {
                ExecutionFlowId = GetReaderValue<Guid>(reader,"ID"),
                DefinitionId =GetReaderValue<Guid>(reader,"ExecutionFlowDefinitionID"),
                Name = reader["Name"] as string,
                Tree = Serializer.Deserialize<QueueExecutionFlowTree>(reader["ExecutionTree"] as string)
            };
        }

        #endregion
    }
}
