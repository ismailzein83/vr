using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Data.SQL;
using Vanrise.Integration.Entities;

namespace Vanrise.Integration.Data.SQL
{
    public class DataSourceStateDataManager : BaseSQLDataManager, IDataSourceStateDataManager
    {
        public DataSourceStateDataManager()
            : base(GetConnectionStringName("IntegrationConnStringKey", "IntegrationDBConnString"))
        {

        }

        public bool TryLockAndGet(Guid dataSourceId, int currentRuntimeProcessId, IEnumerable<int> runningRuntimeProcessesIds, out BaseAdapterState adapterState)
        {
            string runningProcessIdsAsString = null;
            if (runningRuntimeProcessesIds != null)
                runningProcessIdsAsString = String.Join(",", runningRuntimeProcessesIds);
            bool isLocked = false;
            string adapterStateString_Local = null;
            ExecuteReaderSP("integration.sp_DataSourceState_TryLockAndGet",
                (reader) =>
                {
                    if(reader.Read())
                    {
                        isLocked = true;
                        adapterStateString_Local = reader["State"] as string;
                    }
                }, dataSourceId, currentRuntimeProcessId, runningProcessIdsAsString);
            adapterState = adapterStateString_Local != null ? Serializer.Deserialize(adapterStateString_Local) as BaseAdapterState : null;
            return isLocked;
        }

        public void UpdateStateAndUnlock(Guid dataSourceId, Entities.BaseAdapterState state)
        {
            ExecuteNonQuerySP("integration.sp_DataSourceState_UpdateStateAndUnlock", dataSourceId, state != null ? Serializer.Serialize(state) : null);
        }
    }
}
