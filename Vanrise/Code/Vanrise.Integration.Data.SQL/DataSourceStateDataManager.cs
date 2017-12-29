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

        public string GetDataSourceState(Guid dataSourceId)
        {
            return ExecuteScalarSP("[integration].[sp_DataSourceState_GetByID]", dataSourceId) as string;
        }

        public void InsertOrUpdateDataSourceState(Guid dataSourceId, string dataSourceState)
        {
            if (ExecuteNonQuerySP("[integration].[sp_DataSourceState_Update]", dataSourceId, dataSourceState) <= 0)
                ExecuteNonQuerySP("[integration].[sp_DataSourceState_Insert]", dataSourceId, dataSourceState);
        }
    }
}
