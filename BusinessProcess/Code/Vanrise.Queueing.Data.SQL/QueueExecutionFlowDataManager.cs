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
            : base(GetConnectionStringName("QueueDBConnStringKey", "QueueDBConnString"))
        {
        }

        public QueueExecutionFlow GetExecutionFlow(int executionFlowId)
        {
            return GetItemSP("queue.sp_ExecutionFlow_GetByID", ExecutionFlowMapper, executionFlowId);
        }

        #region Private Methods

        private QueueExecutionFlow ExecutionFlowMapper(IDataReader reader)
        {
            return new QueueExecutionFlow
            {
                ExecutionFlowId = (int)reader["ID"],
                DefinitionId = (int)reader["ExecutionFlowDefinitionID"],
                Name = reader["Name"] as string,
                Tree = Serializer.Deserialize<QueueExecutionFlowTree>(reader["ExecutionTree"] as string)
            };
        }

        #endregion
    }
}
