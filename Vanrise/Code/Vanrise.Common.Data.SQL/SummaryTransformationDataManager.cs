using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Common.Data.SQL
{
    public class SummaryTransformationDataManager : Vanrise.Data.SQL.BaseSQLDataManager, ISummaryTransformationDataManager
    {
        public SummaryTransformationDataManager()
            : base(GetConnectionStringName("SummaryTransformationDBConnStringKey", "SummaryTransformationDBConnString"))
        {

        }
        public bool TryLock(int typeId, DateTime batchStart, int currentRuntimeProcessId, IEnumerable<int> runningRuntimeProcessesIds)
        {
            return Convert.ToBoolean(ExecuteScalarSP("[summarytransformation].[sp_SummaryBatchLock_TryLock]", typeId, batchStart, currentRuntimeProcessId, String.Join(",", runningRuntimeProcessesIds)));
        }

        public void UnLock(int typeId, DateTime batchStart)
        {
            ExecuteScalarSP("[summarytransformation].[sp_SummaryBatchLock_UnLock]", typeId, batchStart);
        }
    }
}
