using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Entities;
using Vanrise.Runtime.Entities;

namespace Vanrise.Runtime.Data.RDB
{
    public class RuntimeServiceInstanceDataManager : IRuntimeServiceInstanceDataManager
    {
        static string TABLE_NAME = "runtime_RuntimeServiceInstance";
        static string TABLE_ALIAS = "RSInstance";

        const string COL_ID = "ID";
        const string COL_ServiceTypeID = "ServiceTypeID";
        const string COL_ProcessID = "ProcessID";
        const string COL_ServiceInstanceInfo = "ServiceInstanceInfo";
        const string COL_CreatedTime = "CreatedTime";

        static RuntimeServiceInstanceDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_ServiceTypeID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_ProcessID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_ServiceInstanceInfo, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "runtime",
                DBTableName = "RuntimeServiceInstance",
                Columns = columns,
                IdColumnName = COL_ID,
                CreatedTimeColumnName = COL_CreatedTime
            });
        }

        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("Runtime", "RuntimeConnStringKey", "RuntimeDBConnString");
        }

        #region Public Methods

        public void AddService(RuntimeServiceInstance serviceInstance)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            insertQuery.Column(COL_ID).Value(serviceInstance.ServiceInstanceId);
            insertQuery.Column(COL_ServiceTypeID).Value(serviceInstance.ServiceTypeId);
            insertQuery.Column(COL_ProcessID).Value(serviceInstance.ProcessId);
            if (serviceInstance.InstanceInfo != null)
                insertQuery.Column(COL_ServiceInstanceInfo).Value(Common.Serializer.Serialize(serviceInstance.InstanceInfo));

            queryContext.ExecuteNonQuery();
        }

        public void DeleteByProcessId(int runtimeProcessId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var deleteQuery = queryContext.AddDeleteQuery();
            deleteQuery.FromTable(TABLE_NAME);
            deleteQuery.Where().EqualsCondition(COL_ProcessID).Value(runtimeProcessId);

            queryContext.ExecuteNonQuery();
        }

        public List<RuntimeServiceInstance> GetServices()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().Columns(COL_ID, COL_ServiceTypeID, COL_ProcessID, COL_ServiceInstanceInfo);

            return queryContext.GetItems(ServiceInstanceMapper);
        }

        #endregion

        #region Mappers

        private RuntimeServiceInstance ServiceInstanceMapper(IRDBDataReader reader)
        {
            RuntimeServiceInstance instance = new RuntimeServiceInstance
            {
                ServiceInstanceId = reader.GetGuid(COL_ID),
                ServiceTypeId = reader.GetInt(COL_ServiceTypeID),
                ProcessId = reader.GetInt(COL_ProcessID)
            };
            string serializedInfo = reader.GetString(COL_ServiceInstanceInfo);
            if (serializedInfo != null)
                instance.InstanceInfo = Common.Serializer.Deserialize(serializedInfo) as ServiceInstanceInfo;
            return instance;
        }

        #endregion
    }
}
