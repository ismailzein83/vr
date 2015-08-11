using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace Vanrise.Queueing.Data.SQL
{
    public class QueueExecutionFlowDefinitionDataManager : BaseSQLDataManager, IQueueExecutionFlowDefinitionDataManager
    {
        public QueueExecutionFlowDefinitionDataManager()
            : base(GetConnectionStringName("QueueDBConnStringKey", "QueueDBConnString"))
        {
        }

        public List<Entities.QueueExecutionFlowDefinition> GetAll()
        {
            return GetItemsSP("queue.sp_ExecutionFlowDefinition_GetAll", ExecutionFlowDefinitionMapper);
        }

        private Entities.QueueExecutionFlowDefinition ExecutionFlowDefinitionMapper(IDataReader reader)
        {
            return new Entities.QueueExecutionFlowDefinition
            {
                ID = (int)reader["ID"],
                Name = reader["Name"] as string,
                Title = reader["Title"] as string
            };
        }
    }
}
