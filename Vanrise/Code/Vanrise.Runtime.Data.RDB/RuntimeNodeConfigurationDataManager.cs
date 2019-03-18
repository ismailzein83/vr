using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Data.RDB;
using Vanrise.Entities;
using Vanrise.Runtime.Entities;

namespace Vanrise.Runtime.Data.RDB
{
    public class RuntimeNodeConfigurationDataManager : IRuntimeNodeConfigurationDataManager
    {
        static string TABLE_NAME = "runtime_RuntimeNodeConfiguration";
        static string TABLE_ALIAS = "NodeConfig";

        const string COL_ID = "ID";
        const string COL_Name = "Name";
        const string COL_Settings = "Settings";
        const string COL_CreatedTime = "CreatedTime";
        const string COL_CreatedBy = "CreatedBy";
        const string COL_LastModifiedTime = "LastModifiedTime";
        const string COL_LastModifiedBy = "LastModifiedBy";

        static RuntimeNodeConfigurationDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
            columns.Add(COL_Settings, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_CreatedBy, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_LastModifiedBy, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "runtime",
                DBTableName = "RuntimeNodeConfiguration",
                Columns = columns,
                IdColumnName = COL_ID,
                CreatedTimeColumnName = COL_CreatedTime,
                ModifiedTimeColumnName = COL_LastModifiedTime

            });
        }
        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("RuntimeConfig", "RuntimeConfigDBConnStringKey", "RuntimeConfigDBConnString");
        }

        public bool AreRuntimeNodeConfigurationUpdated(ref object lastReceivedDataInfo)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref lastReceivedDataInfo);
        }

        public List<RuntimeNodeConfiguration> GetAllNodeConfigurations()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            return queryContext.GetItems(RuntimeNodeConfigurationMapper);
        }

        public bool Insert(RuntimeNodeConfiguration nodeConfig)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            insertQuery.Column(COL_ID).Value(nodeConfig.RuntimeNodeConfigurationId);
            insertQuery.Column(COL_Name).Value(nodeConfig.Name);
            if (nodeConfig.Settings != null)
                insertQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(nodeConfig.Settings));

            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool Update(RuntimeNodeConfiguration nodeConfig)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            updateQuery.Column(COL_Name).Value(nodeConfig.Name);
            if (nodeConfig.Settings != null)
                updateQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(nodeConfig.Settings));
            else
                updateQuery.Column(COL_Settings).Null();

            var notExistContext = updateQuery.IfNotExists(TABLE_ALIAS);
            notExistContext.NotEqualsCondition(COL_ID).Value(nodeConfig.RuntimeNodeConfigurationId);
            notExistContext.EqualsCondition(COL_Name).Value(nodeConfig.Name);

            updateQuery.Where().EqualsCondition(COL_ID).Value(nodeConfig.RuntimeNodeConfigurationId);

            return queryContext.ExecuteNonQuery() > 0;
        }
        #region Mappers
        private RuntimeNodeConfiguration RuntimeNodeConfigurationMapper(IRDBDataReader reader)
        {
            return new RuntimeNodeConfiguration
            {
                Name = reader.GetString(COL_Name),
                RuntimeNodeConfigurationId = reader.GetGuid(COL_ID),
                Settings = Serializer.Deserialize<RuntimeNodeConfigurationSettings>(reader.GetString(COL_Settings))

            };
        }
        #endregion
    }
}
