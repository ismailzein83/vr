using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace Vanrise.Queueing.Data.SQL
{
    public class ExecutionControlDataManager : BaseSQLDataManager, IExecutionControlDataManager
    {
        #region ctor

        public ExecutionControlDataManager()
            : base(GetConnectionStringName("QueueItemDBConnStringKey", "QueueItemDBConnString"))
        {
        }

        #endregion

        public bool IsExecutionPaused()
        {
            var value = ExecuteScalarSP("[queue].[sp_ExecutionControl_IsExecutionPaused]");
            return value != null && value != DBNull.Value ? Convert.ToBoolean(value) : false;
        }

        public bool UpdateExecutionPaused(bool isPaused)
        {
            int affectedRecords = ExecuteNonQuerySP("[queue].[sp_ExecutionControl_UpdateExecutionPaused]", isPaused);            
            return affectedRecords > 0;
        }
    }
}
