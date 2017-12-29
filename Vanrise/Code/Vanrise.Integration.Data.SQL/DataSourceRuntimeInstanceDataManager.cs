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


        public List<DataSourceRuntimeInstance> GetAll()
        {
            return GetItemsSP("[integration].[sp_DataSourceRuntimeInstance_GetAll]", DataSourceRuntimeInstanceMapper);
        }

        public bool IsStillExist(Guid dsRuntimeInstanceId)
        {
            return Convert.ToBoolean(ExecuteScalarSP("[integration].[sp_DataSourceRuntimeInstance_DoesExistByID]", dsRuntimeInstanceId));
        }

        public void DeleteInstance(Guid dsRuntimeInstanceId)
        {
            ExecuteNonQuerySP("[integration].[sp_DataSourceRuntimeInstance_Delete]", dsRuntimeInstanceId);
        }

        public bool DoesAnyDSRuntimeInstanceExist(Guid dataSourceId)
        {
            return Convert.ToBoolean(ExecuteScalarSP("[integration].[sp_DataSourceRuntimeInstance_DoesExistByDataSourceID]", dataSourceId));
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
