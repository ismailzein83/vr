using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.Integration.Entities;

namespace Vanrise.Integration.Data.SQL
{
    public class DataSourceRuntimeInstanceDataManager : BaseSQLDataManager, IDataSourceRuntimeInstanceDataManager
    {
        public DataSourceRuntimeInstanceDataManager()
            : base(GetConnectionStringName("IntegrationConnStringKey", "IntegrationDBConnString"))
        {

        }

        public void AddNewInstance(Guid runtimeInstanceId, Guid dataSourceId)
        {
            ExecuteNonQuerySP("[integration].[sp_DataSourceRuntimeInstance_Insert]", runtimeInstanceId, dataSourceId);
        }

        public void TryAddNewInstance(Guid runtimeInstanceId, Guid dataSourceId, int maxNumberOfParallelInstances)
        {
            ExecuteNonQuerySP("[integration].[sp_DataSourceRuntimeInstance_InsertIfNotMaximum]", runtimeInstanceId, dataSourceId, maxNumberOfParallelInstances);
        }

        public Entities.DataSourceRuntimeInstance TryGetOneAndLock(int currentRuntimeProcessId)
        {
            return GetItemSP("[integration].[sp_DataSourceRuntimeInstance_TryGetAndLock]", DataSourceRuntimeInstanceMapper, currentRuntimeProcessId);
        }

        public void SetInstanceCompleted(Guid dsRuntimeInstanceId)
        {
            ExecuteNonQuerySP("[integration].[sp_DataSourceRuntimeInstance_SetCompleted]", dsRuntimeInstanceId);
        }

        public bool IsAnyInstanceRunning(Guid dataSourceId, IEnumerable<int> runningRuntimeProcessesIds)
        {
            string runningProcessIdsAsString = null;
            if (runningRuntimeProcessesIds != null)
                runningProcessIdsAsString = String.Join(",", runningRuntimeProcessesIds);
            return Convert.ToBoolean(ExecuteScalarSP("[integration].[sp_DataSourceRuntimeInstance_IsAnyRunning]", dataSourceId, runningProcessIdsAsString));
        }

        public void DeleteDSInstances(Guid dataSourceId)
        {
            ExecuteNonQuerySP("[integration].[sp_DataSourceRuntimeInstance_DeleteBySource]", dataSourceId);
        }

        #region Private Methods

        DataSourceRuntimeInstance DataSourceRuntimeInstanceMapper(IDataReader reader)
        {
            return new DataSourceRuntimeInstance
            {

                DataSourceRuntimeInstanceId = (Guid)reader["ID"],
                DataSourceId = GetReaderValue<Guid>(reader,"DataSourceID")
            };
        }

        #endregion
    }
}
