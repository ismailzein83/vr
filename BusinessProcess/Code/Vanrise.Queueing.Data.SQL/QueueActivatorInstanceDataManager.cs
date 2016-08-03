using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.Queueing.Entities;

namespace Vanrise.Queueing.Data.SQL
{
    public class QueueActivatorInstanceDataManager : BaseSQLDataManager, IQueueActivatorInstanceDataManager
    {
        public QueueActivatorInstanceDataManager()
            : base(GetConnectionStringName("QueueDBConnStringKey", "QueueDBConnString"))
        {
        }

        public void InsertActivator(Guid activatorId, int runtimeProcessId, QueueActivatorType activatorType, string serviceURL)
        {
            ExecuteNonQuerySP("[queue].sp_QueueActivatorInstance_Insert", activatorId, runtimeProcessId, (int)activatorType, serviceURL);
        }


        public List<Entities.QueueActivatorInstance> GetAll()
        {
            return GetItemsSP("[queue].[sp_QueueActivatorInstance_GetAll]", QueueActivatorInstanceMapper);
        }

        public void Delete(Guid activatorId)
        {
            ExecuteNonQuerySP("[queue].[sp_QueueActivatorInstance_Delete]", activatorId);
        }

        #region Mappers

        private QueueActivatorInstance QueueActivatorInstanceMapper(IDataReader reader)
        {
            return new QueueActivatorInstance
            {
                ActivatorId = (Guid)reader["ActivatorID"],
                ProcessId = (int)reader["ProcessID"],
                ActivatorType = (QueueActivatorType)reader["ActivatorType"],
                ServiceURL = reader["ServiceURL"] as string
            };
        }

        #endregion
    }
}
